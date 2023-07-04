using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class RectTransformPositionController : PositionController
    {
        private RectTransform _rect;

        protected RectTransform rect => _rect ??= GetComponent<RectTransform>();


        public override void Bind(DiContainer container)
        {
            container.Bind<ISelfManagedPosition>().To<RectTransformPositionController>().FromNewComponentSibling()
                .When((c) =>
                    ((MonoBehaviour) c.ObjectInstance).TryGetComponent<RectTransform>(out _)
                );
        }

        protected override void GoTo(Vector3 pos)
        {
            _positionTween?.Kill();
            if (_preferLocalPosition)
            {
                _positionTween = rect.DOAnchorPos(pos, 0.5f);
             return;   
            }

            _positionTween = rect.DOMove(pos, 0.5f);
        }

        protected override void Set(Vector3 pos, bool local)
        {
            if (local)
            {
                rect.anchoredPosition = pos;
                return;
            }
            base.Set(pos, local);
        }

        protected override Vector3 GetPosition(bool local)
        {
            if (local) return rect.anchoredPosition;
            return base.GetPosition(local);
        }
    }
}