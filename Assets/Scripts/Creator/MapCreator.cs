using System.Collections.Generic;
using System.Linq;
using Common;
using Controller;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using GridType = Manager.GridType;
using Random = UnityEngine.Random;

namespace Creator
{
    public class MapCreator : UnitySingleton<MapCreator>
    {
        [SerializeField] public int width;
        [SerializeField] public int height;
        [SerializeField] public int portalCount;
        [SerializeField] public int windCount;
        [SerializeField] public int bridgeCount;
        [SerializeField] public Text num1;
        [SerializeField] public Text num2;

        public GameObject p1;
        public GameObject p2;
        public GameObject ground;
        public GameObject portal;
        public GameObject wind;
        public GameObject horizontalBridge;
        public GameObject verticalBridge;

        private int _count;

        private const int MaxCount = 100000;

        private List<Vector2> _groundPoses = new List<Vector2>();

        private void Awake()
        {
            CreateGround(0, 0, width - 1, height - 1);
            CreatePortal();
            CreatePlayer();
            CreateWind();
            ChangeToBridge();
        }

        private void ChangeToBridge()
        {
            int count = 0;

            int idx = portalCount + windCount + 1;
            while (count < bridgeCount && idx < _groundPoses.Count)
            {
                var pos = _groundPoses[idx++];
                if (Random.Range(0, 2) == 1)
                {
                    if (GridManager.Instance.Walkable(pos + Vector2.up) &&
                        GridManager.Instance.Walkable(pos + Vector2.down))
                    {
                        GridManager.Instance.Destroy(GridType.Ground, pos);
                        GridManager.Instance.CreateGrid(verticalBridge, GridType.Bridge, pos);
                        count++;
                    }
                }
                else
                {
                    if (GridManager.Instance.Walkable(pos + Vector2.left) &&
                        GridManager.Instance.Walkable(pos + Vector2.right))
                    {
                        GridManager.Instance.Destroy(GridType.Ground, pos);
                        GridManager.Instance.CreateGrid(horizontalBridge, GridType.Bridge, pos);
                        count++;
                    }
                }
            }
        }


        public void Put(float x, float y)
        {
            _groundPoses.Add(new Vector2(x, y));
            _groundPoses = _groundPoses.OrderBy(i => Random.Range(-1f, 1f)).ToList();
            GridManager.Instance.CreateGrid(ground, GridType.Ground, new Vector2(x, y));
        }

        private void CreatePlayer()
        {
            var p1Pos = _groundPoses[portalCount + windCount];
            var p2Pos = MapSynchronizer.Normalize(p1Pos + new Vector2(width / 2, height / 2));
            var player1 = Instantiate(p1).GetComponent<PlayerController>();
            player1.startPos = p1Pos;
            Global.P1 = player1;
            var player2 = Instantiate(p2).GetComponent<PlayerController>();
            player2.startPos = p2Pos;
            Global.P2 = player2;
            player1.another = Global.P2;
            player2.another = Global.P1;
            player1.Builder.countText = num1;
            player2.Builder.countText = num2;

            MapSynchronizer.Synchronize(off =>
            {
                var p1Shadow = Instantiate(p1).GetComponent<PlayerController>();
                p1Shadow.isShadow = true;
                p1Shadow.startPos = p1Pos + off;
                player1.shadows.Add(off, p1Shadow);

                var p2Shadow = Instantiate(p2).GetComponent<PlayerController>();
                p2Shadow.isShadow = true;
                p2Shadow.startPos = p2Pos + off;
                player2.shadows.Add(off, p2Shadow);
            });
        }

        private void CreateWind()
        {
            var winds = _groundPoses.GetRange(portalCount, windCount);
            foreach (var pos in winds)
            {
                var windController = Instantiate(wind, pos, wind.transform.rotation).GetComponent<WindController>();
                windController.players.Add(Global.P1);
                windController.players.Add(Global.P2);
            }
        }

        private void CreatePortal()
        {
            Global.PortalControllers = new List<PortalController>();
            var portals = _groundPoses.GetRange(0, portalCount);
            foreach (var pos in portals)
            {
                var go = GridManager.Instance.CreateGrid(portal, GridType.Portal, pos);
                Global.PortalControllers.Add(go.AddComponent<PortalController>());
            }
        }

        // 分形随机生成
        private void CreateGround(int x1, int y1, int x2, int y2)
        {
            if (_count >= MaxCount) return;
            int n = x2 - x1 + 1;
            int m = y2 - y1 + 1;

            int bound = Random.Range(2, 14);
            if (n <= bound || m <= bound) return;

            int hMid = x1 + n / 2;
            int vMid = y1 + m / 2;

            int hSplit1 = Random.Range(x1, hMid);
            int hSplit2 = Random.Range(hMid + 1, x2 + 1);

            int vSplit1 = Random.Range(y1, vMid);
            int vSplit2 = Random.Range(vMid + 1, y2 + 1);

            for (int x = x1; x <= x2; x++)
            {
                if (x == hSplit1 || x == hSplit2 || x == hMid) continue;
                Put(x, vMid);
                _count++;
                if (_count >= MaxCount) return;
            }

            for (int y = y1; y <= y2; y++)
            {
                if (y == vSplit1 || y == vSplit2 || y == vMid) continue;
                Put(hMid, y);
                _count++;
                if (_count >= MaxCount) return;
            }


            CreateGround(x1, y1, hMid - 1, vMid - 1);
            CreateGround(hMid + 1, vMid + 1, x2, y2);
            CreateGround(x1, vMid + 1, hMid - 1, y2);
            CreateGround(hMid + 1, y1, x2, vMid - 1);
        }
    }
}