using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace DefaultNamespace
{
    public class testTest : MonoPoolable<testTest>
    {
        private ISelfManagedPosition _positionController;


        public void Move(Vector3 pos)
        {
            _positionController.preferredPosition = pos;
        }

        [Inject]
        public void Construct(ISelfManagedPosition positionController)
        {
            _positionController = positionController;
        }
    }
}