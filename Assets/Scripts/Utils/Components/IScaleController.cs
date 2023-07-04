using System;

namespace DefaultNamespace
{
    public interface IScaleController
    {
        public void Scale(float scale,float time = 0.5f);
        public void Pulsate(float scale);
        public bool locked { get; set; }

        public Action onFinish { set; }
    }
}