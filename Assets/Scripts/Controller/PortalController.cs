using System;
using System.Linq;
using Common;
using DG.Tweening;
using Manager;
using UnityEngine;

namespace Controller
{
    public class PortalController : MonoBehaviour
    {
        private SpriteRenderer SpriteRenderer { get; set; }

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            UtilFunc.SetAlpha(SpriteRenderer, 0);
            MapSynchronizer.Synchronize(off =>
            {
                var render = GridManager.Instance.Get(GridType.Portal, off + (Vector2) transform.position)
                    .GetComponent<SpriteRenderer>();
                UtilFunc.SetAlpha(render, 0);
            });
        }

        private void Update()
        {
            if (UtilFunc.SamePos(transform.position, Global.P1.Pos))
            {
                Transport(Global.P1);
            }

            if (UtilFunc.SamePos(transform.position, Global.P2.Pos))
            {
                Transport(Global.P2);
            }
        }

        private void Transport(PlayerController player)
        {
            var distList =
                Global.PortalControllers.ConvertAll(p => UtilFunc.Dist2d(p.transform.position, player.another.Pos));
            var maxDist = distList.Max();
            PortalController targetPortal = null;
            for (int i = 0; i < distList.Count; i++)
            {
                if (Math.Abs(distList[i] - maxDist) < 1e-3)
                {
                    targetPortal = Global.PortalControllers[i];
                }
            }

            if (targetPortal != null && targetPortal != this)
            {
                Sequence seq = DOTween.Sequence();
                seq.AppendCallback(() =>
                {
                    player.isFlying = true;
                    SoundManager.Instance.PlayEffect(SoundManager.AudioSo.transport);
                });
                // if (SpriteRenderer.color.a < 1e-3f)
                // {
                seq.Append(SpriteRenderer.DOFade(1f, 1f));
                MapSynchronizer.Synchronize(off =>
                {
                    seq.Join(GridManager.Instance.Get(GridType.Portal, off + (Vector2) transform.position)
                        .GetComponent<SpriteRenderer>().DOFade(1f, 1f));
                });
                // }
                // else
                // {
                //     seq.AppendInterval(1f);
                // }

                seq.AppendCallback(() => { player.SetPosSync(targetPortal.transform.position); });
                seq.Append(SpriteRenderer.DOFade(0, 0.1f));
                MapSynchronizer.Synchronize(off =>
                {
                    seq.Append(GridManager.Instance.Get(GridType.Portal, off + (Vector2) transform.position)
                        .GetComponent<SpriteRenderer>().DOFade(0, 0.1f));
                });
                seq.AppendCallback(() => player.isFlying = false);
            }
        }
    }
}