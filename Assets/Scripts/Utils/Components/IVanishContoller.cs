using System;

namespace DefaultNamespace
{
    public interface IVanishController
    {
        public bool vanishing { get; }
        public void Vanish(float degree,Action onComplete = null);
        public void VanishInstant(float degree);
    }
}