using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Controller;
using Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    public enum GridType
    {
        Bridge,
        Ground,
        Water,
        Portal
    }

    public class GridManager : UnitySingleton<GridManager>
    {
        private readonly Dictionary<GridType, GridModel> _typeGrid =
            new Dictionary<GridType, GridModel>
            {
                {GridType.Bridge, new GridModel()},
                {GridType.Ground, new GridModel()},
                {GridType.Water, new GridModel()},
                {GridType.Portal, new GridModel()}
            };


        public void Put(GridType gridType, Vector2 pos, GameObject go)
        {
            _typeGrid[gridType].Put(pos, go);
        }

        public GameObject Get(GridType gridType, Vector2 pos)
        {
            return _typeGrid[gridType].Get(pos);
        }

        public GameObject CreateGrid(GameObject grid, GridType gridType, Vector2 pos, bool hideGrid = true)
        {
            pos = MapSynchronizer.Normalize(pos);
            var go = PutGrid(grid, gridType, pos, hideGrid);

            MapSynchronizer.Synchronize(off => PutGrid(grid, gridType, pos + off, hideGrid));

            return go;
        }

        private GameObject PutGrid(GameObject grid, GridType gridType, Vector2 pos, bool hideGrid)
        {
            var go = Instantiate(grid, pos, grid.transform.rotation);
            if (hideGrid)
            {
                go.hideFlags = HideFlags.HideInHierarchy; // 不在hierarchy中显示
            }

            Put(gridType, pos, go);
            return go;
        }


        public Action<GridType, Vector2> DestroyEvent;


        private void DestroyGrid(GridType gridType, Vector2 pos)
        {
            Destroy(Get(gridType, pos));
            _typeGrid[gridType].Remove(pos);
            DestroyEvent?.Invoke(gridType, pos);
        }

        public void Destroy(GridType gridType, Vector2 pos)
        {
            pos = MapSynchronizer.Normalize(pos);
            DestroyGrid(gridType, pos);
            MapSynchronizer.Synchronize(off => DestroyGrid(gridType, pos + off));
        }

        public void DestroyAll(GridType gridType)
        {
            foreach (var pos in _typeGrid[gridType].Grid.Keys.ToList())
            {
                Destroy(gridType, pos);
            }
        }

        public bool Walkable(Vector2 pos)
        {
            pos = MapSynchronizer.Normalize(pos);
            return Exist(GridType.Ground, pos) || Exist(GridType.Bridge, pos);
        }



        public bool Exist(GridType type, Vector2 pos)
        {
            pos = MapSynchronizer.Normalize(pos);
            return _typeGrid[type].Contains(pos);
        }
    }
}