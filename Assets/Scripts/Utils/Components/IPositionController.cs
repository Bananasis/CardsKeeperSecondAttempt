using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace DefaultNamespace
{
    public interface IBindableComponent : IBindable
    {
    }

    public interface IPositionController
    {
        Vector3 currentPosition { get; set; }
        Quaternion currentRotation { set; }

        Vector3 currentWorldPosition { set; }
        Quaternion currentWorldRotation { set; }
    }

    public interface ISelfManagedPosition : IPositionController
    {
        Vector3 preferredPosition { set; }
        Vector3 preferredWorldPosition { set; }

        Quaternion preferredRotation { set; }
        Quaternion preferredWorldRotation { set; }

        bool lockedRotation { set; }
        bool lockedPosition { set; }
        bool selfManaged { set; }
        bool selfManagedRotation { set; get; }
        bool selfManagedPosition { set; get; }
    }

    public interface IBezierPosition : ISelfManagedPosition
    {
        public float bezierSize { get; set; }
        public UnityEvent OnNeedPositionUpdate { get; }
    }
}