using System;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class TTTT : MonoBehaviour


    {
        [Inject(Id = "Triangle")] testTest.MonoPool _pool;
        [Inject(Id = "Image")] testTest.MonoPool _poolimg;
        private void Start()
        {
            _pool.Get().Move(new Vector3(10, 10));
            _pool.Get().Move(new Vector3(-10, -10));
            _poolimg.Get().Move(new Vector3(-10, -10));
            _poolimg.Get().Move(new Vector3(-1000, -1000));
        }
    }
}