using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
    public interface IPool<P> where P : IPoolable<P>
    {
    }

    public interface IPoolable<out P> : IDisposable where P : IPoolable<P>
    {
    }

    public interface IBindable
    {
        public abstract void Bind(DiContainer container);
    }

    public abstract class MonoBindable : MonoBehaviour, IBindable
    {
        public abstract void Bind(DiContainer container);
    }


    public abstract class MonoPoolable<P> : MonoBindable, IPoolable<P> where P : MonoPoolable<P>
    {
        Action ReturnToPool;
        public virtual int id => 0;

        public virtual void Dispose()
        {
            gameObject.SetActive(false);
            ReturnToPool();
        }

        private MonoPool InstantiatePool(Factory factory)
        {
            return new MonoPool(factory, transform.parent);
        }

        public class Factory : PlaceholderFactory<P>
        {
            // public override P Create()
            // {
            //     var obj = base.Create();
            //     obj.ReturnToPool = () =>
            //     {
            //         obj.gameObject.SetActive(false);
            //         obj.transform.parent = _holder;
            //         _stack.Push(poolable);
            //     };
            // }
        }

        public class MonoPool : IPool<P>
        {
            private Stack<P> _stack = new();
            private Transform _holder;
            private Factory _factory;


            public MonoPool(Factory factory, Transform holder = null)
            {
                _factory = factory;
                _holder = holder;
            }

            public P Get(Transform parent = null, Vector3 position = default, Quaternion rotation = default)
            {
                if (!_stack.TryPop(out var poolable))
                {
                    poolable = _factory.Create();
                    poolable.ReturnToPool = () =>
                    {
                        poolable.gameObject.SetActive(false);
                        poolable.transform.parent = _holder;
                        _stack.Push(poolable);
                    };
                }

                var transform1 = poolable.transform;

                transform1.SetParent(parent??_holder);
                transform1.localScale = Vector3.one;
                transform1.localPosition = position;
                transform1.localRotation = rotation;
                poolable.gameObject.SetActive(true);
                return poolable;
            }
        }

        public override void Bind(DiContainer container)
        {
            container.BindFactory<P, Factory>().FromComponentInNewPrefab(this);
            container.Bind<MonoPool>().To<MonoPool>().FromMethod(() =>
            {
                var pool = InstantiatePool(container.Resolve<Factory>());
                container.QueueForInject(pool);
                return pool;
            }).AsCached();
        }
    }
}