using Controller;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class BridgeBuilder : MonoBehaviour
    {
        private PlayerController _player;
        public GameObject horizontalBridge;

        [SerializeField] private GameObject verticalBridge;


        public int startBridgeCount = 5;


        private int _bridgeCount;

        public Text countText;

        private readonly Vector2[] _dir = {Vector2.left, Vector2.up, Vector2.right, Vector2.down,};


        public int BridgeCount
        {
            get => _bridgeCount;
            set
            {
                SetNumText(value);
                _bridgeCount = value;
            }
        }

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        private void Start()
        {
            BridgeCount = startBridgeCount;
            SetNumText(BridgeCount);
        }


        private void SetNumText(int num)
        {
            if (countText == null) return;
            countText.text = $"x{num.ToString()}";
        }


        // 能否在一定步数内到底另一格子
        public bool CanReach(Vector2 start, Vector2 dir, out Vector2 reachedPos)
        {
            start = MapSynchronizer.Normalize(start);
            Vector2 p = start;
            int step = 0;
            for (int i = 0; i < BridgeCount; i++)
            {
                p += dir;
                p = MapSynchronizer.Normalize(p);

                step++;
                if (step > BridgeCount) break;

                if (GridManager.Instance.Walkable(p))
                {
                    reachedPos = p;
                    return true;
                }
            }

            reachedPos = default;

            return false;
        }


        public void WithDraw(Vector2 pos)
        {
            if (GridManager.Instance.Exist(GridType.Bridge, pos)) return;
            pos = MapSynchronizer.Normalize(pos);
            Dfs(pos);
            for (int i = 0; i < 4; i++)
            {
                Dfs(pos + _dir[i]);
            }
        }

        private void Dfs(Vector2 pos)
        {
            pos = MapSynchronizer.Normalize(pos);
            if (GridManager.Instance.Exist(GridType.Bridge, pos))
            {
                CreateAnim(pos, _player.transform.position, GridManager.Instance.Get(GridType.Bridge, pos));
                GridManager.Instance.Destroy(GridType.Bridge, pos);
                SoundManager.Instance.PlayEffect(SoundManager.AudioSo.withdraw);
                BridgeCount++;

                for (int i = 0; i < 4; i++)
                {
                    Dfs(pos + _dir[i]);
                }
            }
        }

        public static Tween CreateAnim(Vector2 p1, Vector2 p2, GameObject prefab, float moveTime = 0.1f)
        {
            var go = Instantiate(prefab, p1, prefab.transform.rotation);
            var tween = go.transform.DOMove(p2, moveTime);
            tween.onComplete += () => Destroy(go);
            return tween;
        }


        // 用桥连接p1和p2
        public void BuildBetween(Vector2 p1, Vector2 p2, Vector2 dir)
        {
            int count = 0;
            for (Vector2 p = MapSynchronizer.Normalize(p1 + dir); p != p2; p = MapSynchronizer.Normalize(p + dir))
            {
                count++;

                GameObject bridgePrefab = dir.x == 0 ? verticalBridge : horizontalBridge;
                GridManager.Instance.CreateGrid(bridgePrefab, GridType.Bridge, p, true);
                SoundManager.Instance.PlayEffect(SoundManager.AudioSo.put);
            }

            BridgeCount -= count;
        }
    }
}