using System;
using System.Collections.Generic;
using Common;
using Creator;
using DG.Tweening;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    public class WindController : MonoBehaviour
    {
        public float range;

        public List<PlayerController> players = new List<PlayerController>();

        private PlayerController _target;

        [SerializeField] private GameObject prefab;
        public float moveInterval = 0.3f;
        private Vector2 _startPos;
        private bool _isFlying;
        private Sequence _moveAnim;

        private void Awake()
        {
            MapSynchronizer.Synchronize(off =>
            {
                var go = Instantiate(prefab, (Vector3) off + transform.position, prefab.transform.rotation);
                go.transform.SetParent(transform, true);
            });
        }

        private void Start()
        {
            _startPos = transform.position;
            if (!GridManager.Instance.Exist(GridType.Ground, _startPos))
                MapCreator.Instance.Put(_startPos.x, _startPos.y);
        }

        private void Update()
        {
            if (!_isFlying && !GridManager.Instance.Walkable(transform.position))
            {
                Debug.Log(transform.position);
                Destroy(gameObject);
            }

            if (_target == null)
            {
                foreach (var player in players)
                {
                    if (UtilFunc.Dist2d(transform.position, player.transform.position) < range)
                    {
                        if (!player.isFlying) _target = player.GetComponent<PlayerController>();
                    }
                }
            }
            else
            {
                if (UtilFunc.Dist2d(transform.position, _target.transform.position) > range)
                {
                    _target = null;
                }
                else
                {
                    if (UtilFunc.Dist2d(transform.position, _target.transform.position) < 1e-3)
                    {
                        if (!_target.isFlying)
                        {
                            _target.EndMoving();
                            Fly();
                        }
                    }
                    else
                    {
                        Move();
                    }
                }
            }
        }

        private bool IsMoving => _moveAnim != null && _moveAnim.IsPlaying();

        private void Move()
        {
            if (IsMoving) return;

            float dist = 1e9f;
            Vector2? dirChoose = null;
            foreach (var dir in UtilFunc.Dir)
            {
                if (!GridManager.Instance.Walkable((Vector2) transform.position + dir)) continue;
                var dist2d = UtilFunc.Dist2d(transform.position + (Vector3) dir, _target.transform.position);
                if (dist2d < dist)
                {
                    dist = dist2d;
                    dirChoose = dir;
                }
            }

            if (dirChoose != null)
            {
                _moveAnim = DOTween.Sequence();

                _moveAnim.AppendCallback(() => _isFlying = true);

                _moveAnim.Append(transform.DOMove(transform.position + (Vector3) dirChoose.Value, moveInterval));

                _moveAnim.AppendCallback(() => _isFlying = false);
            }
        }

        private void Fly()
        {
            var dir = UtilFunc.Dir[Random.Range(0, 4)];
            Sequence seq = DOTween.Sequence();
            Vector2 tp;
            for (int i = MapCreator.Instance.width / 5;; i++)
            {
                tp = MapSynchronizer.Normalize(_target.logicPos + dir * i);
                if (GridManager.Instance.Walkable(tp)) break;
            }

            seq.AppendCallback(() =>
            {
                _target.isFlying = true;
                _isFlying = true;
                _target.Builder.BridgeCount = 0;
                SoundManager.Instance.PlayEffect(SoundManager.AudioSo.wind);
            });
            
            seq.Append(
                DOTween.To(() => _target.transform.position, p =>
                {
                    _target.SetPosSync(p, false);
                    transform.position = p;
                }, (Vector3) tp, 0.4f)
            );

            seq.AppendCallback(() =>
            {
                _target.SetPosSync(tp);
                _target.isFlying = false;
                transform.position = _startPos;
                _isFlying = false;
            });
        }
    }
}