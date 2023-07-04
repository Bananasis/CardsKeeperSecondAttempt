using System;

namespace Utils
{
    public interface ISignal
    {
        public void Invoke();
        public IDisposable Subscribe(Action action, bool once = false, bool preInvoke = false);
    }
}