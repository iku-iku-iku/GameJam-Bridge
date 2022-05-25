using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class ToggleShowUI : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        public bool isShowing;

        private Tween _tween;

        private void Awake()
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            Hide();
        }

        private float GetAlpha() => _canvasGroup.alpha;
        private void SetAlpha(float alpha) => _canvasGroup.alpha = alpha;
        
        

        public void Show(bool transit = false)
        {
            isShowing = true;
            _tween?.Complete();
            if (transit)
            {
                _tween = DOTween.To(GetAlpha, SetAlpha, 1f, 1f).SetUpdate(true);
            }
            else
            {
                _canvasGroup.alpha = 1;
            }

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide(bool transit = false)
        {
            isShowing = false;
            _tween?.Complete();
            if (transit)
            {
                DOTween.To(GetAlpha, SetAlpha, 0, 1f).SetUpdate(true);
            }
            else
            {
                _canvasGroup.alpha = 0;
            }

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}