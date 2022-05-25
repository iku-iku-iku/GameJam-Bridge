using System;
using System.Collections.Generic;
using Common;
using Controller;
using Manager;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;
using Random = Unity.Mathematics.Random;



namespace Creator
{
    public class WaterFlowCreator : UnitySingleton<WaterFlowCreator>
    {
        [SerializeField] private List<GameObject> water;

        private readonly List<GameObject> _waterList = new List<GameObject>();

        private Vector2 leftDownBound;


        private Random _random;

        private int _waterCount;

        private void Awake()
        {
            _random = new Random(1);
        }

        private void Start()
        {
            leftDownBound = new Vector2(-MapCreator.Instance.width, -MapCreator.Instance.height);
            for (int x = 1; x < MapCreator.Instance.width - 1; x += 3)
            {
                for (int y = 1; y <= MapCreator.Instance.height - 1; y += 3)
                {
                    var waterPrefab = water[_random.NextInt(water.Count)];

                    var pos = new Vector2(x, y);
                    var go = GridManager.Instance.CreateGrid(waterPrefab, GridType.Water, pos);
                    _waterList.Add(go);
                    MapSynchronizer.Synchronize(off =>
                    {
                        _waterList.Add(GridManager.Instance.Get(GridType.Water, pos + off));
                    });
                }
            }


            _waterCount = _waterList.Count;
        }

        private void Update()
        {
            TransformAccessArray transformAccess = new TransformAccessArray(_waterCount);
            for (int i = 0; i < _waterCount; i++)
            {
                transformAccess.Add(_waterList[i].transform);
            }
        
            float added = Time.deltaTime;
            var job = new FlowWaterJob
            {
                Added = added,
                LeftBound = leftDownBound.x,
                RightBound = leftDownBound.x + 3 * MapCreator.Instance.width
            };
        
            var jh = job.Schedule(transformAccess);
            jh.Complete();
            transformAccess.Dispose();
        }
    }

    [BurstCompile]
    public struct FlowWaterJob : IJobParallelForTransform
    {
        public float Added;
        public float LeftBound;
        public float RightBound;

        public void Execute(int index, TransformAccess transform)
        {
            transform.position += new Vector3(Added, 0, 0);
            if (transform.position.x > RightBound)
            {
                transform.position = new Vector3(LeftBound, transform.position.y, transform.position.z);
            }
        }
    }
}