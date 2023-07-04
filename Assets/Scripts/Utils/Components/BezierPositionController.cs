using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Display.UI.Card.Components
{
    public class BezierPositionController : RectTransformPositionController, IBezierPosition
    {
        public override void Bind(DiContainer container)
        {
            container.Bind<IBezierPosition>().To<BezierPositionController>().FromNewComponentSibling()
                .When((c) =>
                    ((MonoBehaviour) c.ObjectInstance).TryGetComponent<RectTransform>(out _)
                );
        }

        private float _bezierSize = 1;

        public float bezierSize
        {
            get => _bezierSize;
            set
            {
                if (_bezierSize == value) return;
                _bezierSize = value;
                OnNeedPositionUpdate.Invoke();
            }
        }

        public UnityEvent OnNeedPositionUpdate { get; } = new();
    }
}