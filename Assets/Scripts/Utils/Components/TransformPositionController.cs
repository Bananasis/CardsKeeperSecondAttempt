using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class TransformPositionController : PositionController
    {
        public override void Bind(DiContainer container)
        {
            container.Bind<ISelfManagedPosition>().To<TransformPositionController>().FromNewComponentSibling()
                .When((c) => !((MonoBehaviour) c.ObjectInstance).TryGetComponent<RectTransform>(out _));
        }
        
    }
}