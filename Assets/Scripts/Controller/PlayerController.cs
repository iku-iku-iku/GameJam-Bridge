using System.Collections.Generic;
using Common;
using Components;
using Creator;
using DG.Tweening;
using Manager;
using UnityEngine;

namespace Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float sensitivity = 0.2f; // 按键灵敏度
        [SerializeField] private float moveInterval = 0.2f; // 移到间隔
        [SerializeField] private KeyCode up;
        [SerializeField] private KeyCode left;
        [SerializeField] private KeyCode right;
        [SerializeField] private KeyCode down;
        [SerializeField] private KeyCode withdraw;
        [SerializeField] private KeyCode give;
        [SerializeField] public Vector2 startPos;
        public Dictionary<Vector2, PlayerController> shadows = new Dictionary<Vector2, PlayerController>();

        public PlayerController another;
        public bool isShadow;


        private Sequence _moveTween; // 移动动画

        public bool isFlying;


        public BridgeBuilder Builder { get; set; } // 造桥能力

        private Animator _animator; // 动画
        private static readonly int Walk = Animator.StringToHash("Walk");

        public Vector2 Pos => transform.position;

        public Vector2 logicPos; // 逻辑位置

        private void Awake()
        {
            Builder = GetComponent<BridgeBuilder>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            if (isShadow) return;
            ResetPos();
        }

        private void Update()
        {
            if (isShadow) return;
            HandleInput();


            if (!GridManager.Instance.Walkable(logicPos) && !isFlying)
            {
                _moveTween.Complete();
                Die();
            }
        }

        private void Die()
        {
            ResetPos();
        }

        public void ResetPos()
        {
            SetPosSync(startPos);
            if (!GridManager.Instance.Exist(GridType.Ground, startPos))
                MapCreator.Instance.Put(startPos.x, startPos.y);
        }

        private void HandleInput()
        {
            if (isFlying) return;

            void MoveOrBridge(Vector2 dir)
            {
                Vector2 newPos = Pos + dir;
                if (GridManager.Instance.Walkable(newPos))
                {
                    Move(dir);
                    MapSynchronizer.Synchronize(off => { shadows[off].Move(dir); });
                }
                else if (!isShadow) Bridge(dir);
            }

            if (Input.GetKeyDown(withdraw))
            {
                // _builder.WithDraw();
                Builder.WithDraw(Pos);
            }

            if (Input.GetKeyDown(give))
            {
                if (Builder.BridgeCount <= 0) return;
                Builder.BridgeCount--;
                var tween = BridgeBuilder.CreateAnim(Pos, another.Pos, Builder.horizontalBridge,
                    UtilFunc.Dist2d(Pos, another.Pos) * 0.05f);
                tween.onComplete += () => { another.GetComponent<BridgeBuilder>().BridgeCount++; };
            }

            if (Input.GetKey(left))
            {
                MoveOrBridge(Vector2.left);
            }
            else if (Input.GetKey(right))
            {
                MoveOrBridge(Vector2.right);
            }
            else if (Input.GetKey(up))
            {
                MoveOrBridge(Vector2.up);
            }
            else if (Input.GetKey(down))
            {
                MoveOrBridge(Vector2.down);
            }
        }

        // 建桥，核心玩法
        private void Bridge(Vector2 dir)
        {
            var pos = MapSynchronizer.Normalize(Pos);
            if (Builder.CanReach(pos + dir, dir, out Vector2 reachedPos))
            {
                Builder.BuildBetween(MapSynchronizer.Normalize(pos), MapSynchronizer.Normalize(reachedPos), dir);
            }
        }

        public Sequence Move(Vector2 dir)
        {
            if (_moveTween != null && _moveTween.IsPlaying()) return null; // 移动动画播放时不能移动
            if (!isShadow && MapSynchronizer.IsOutsize(Pos + dir))
            {
                var p = MapSynchronizer.Normalize(Pos + dir) - dir;
                SetPosSync(p);
            }


            var seq = MoveAnim(dir);
            MapSynchronizer.Synchronize(off => shadows[off].MoveAnim(dir));
            return seq;
        }

        private Sequence MoveAnim(Vector2 dir)
        {
            logicPos += dir;
            _moveTween = DOTween.Sequence();
            _moveTween.AppendCallback(() => _animator.SetBool(Walk, true));
            _moveTween.Append(transform.DOMove(Pos + dir, moveInterval));
            _moveTween.AppendCallback(() => _animator.SetBool(Walk, false));
            return _moveTween;
        }

        public void SetPosSync(Vector2 pos, bool setLogic = true)
        {
            SetPos(pos, setLogic);
            MapSynchronizer.Synchronize(off => shadows[off].SetPos(pos + off, setLogic));
        }

        public void EndMoving() => _moveTween?.Complete();

        private void SetPos(Vector2 pos, bool setLogic = true)
        {
            transform.position = pos;
            logicPos = pos;
        }
    }
}