using System;
using System.Collections.Generic;
using UnityEngine;

public interface IRecycler
{
    public void Recycle(IDisposable disposable);
}

public class Recycler : MonoBehaviour, IRecycler
{
    private List<IDisposable> _disposables = new List<IDisposable>();
    private bool wait;
    private void FixedUpdate()
    {
        if (wait)
        {
            wait = false;
            return;
        }

        _disposables.Dispose();
    }
    
    public void Recycle(IDisposable disposable)
    {
        wait = true;
        _disposables.Add(disposable);
    }
}