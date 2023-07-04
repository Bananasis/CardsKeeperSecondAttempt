using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Debug = System.Diagnostics.Debug;
using Random = System.Random;

public static class RandomUtils
{
    static readonly Random random = new();

    public static List<T> Sample<T>(this List<T> list, int n)
    {
        if (list.Count <= n) return new List<T>(list);

        var sample = new List<T>();
        float selected = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (!(random.NextDouble() < (n - selected) / (list.Count - i))) continue;
            sample.Add(list[i]);
            selected++;
        }

        return sample;
    }

    public static T Random<T>(this List<T> list)
    {
        return list[random.Next(list.Count)];
    }

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        for (var i = 0; i < n - 1; i++)
        {
            var r = i + random.Next(n - i);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }

    public static List<T> GetShuffled<T>(this List<T> list)
    {
        var newList = new List<T>(list);
        newList.Shuffle();
        return newList;
    }
}


public class ConnectionManager : IDisposable
{
    private readonly HashSet<IDisposable> connections = new();
    private readonly ConnectionManager manager;
    private bool disposing;

    public ConnectionManager(ConnectionManager manager = default)
    {
        this.manager = manager;
        manager?.Add(this);
    }

    public virtual void DisposeOf(IDisposable connection)
    {
        if (disposing) return;

        if (connections.Remove(connection))
            connection.Dispose();
    }

    public virtual void Add(IDisposable connection)
    {
        if (disposing) throw new Exception();
        connections.Add(connection);
    }

    public virtual void Dispose()
    {
        if (disposing) return;
        disposing = true;
        foreach (var connection in connections)
        {
            connection.Dispose();
        }

        disposing = false;
        connections.Clear();
        manager?.DisposeOf(this);
    }
}

public class Connection : IDisposable
{
    private readonly UnityEvent uEvent;
    private readonly UnityAction uAction;
    private readonly Action onDispose;
    private bool disposed;

    public Connection(UnityEvent uEvent, UnityAction uAction, Action onDispose = default)
    {
        this.uEvent = uEvent;
        this.uAction = uAction;
        this.onDispose = onDispose;
        uEvent.AddListener(uAction);
    }

    public void Dispose()
    {
        if (disposed) return;
        uEvent.RemoveListener(uAction);
        disposed = true;
        onDispose?.Invoke();
    }
}

public class Connection<T> : IDisposable
{
    private readonly UnityEvent<T> uEvent;
    private readonly UnityAction<T> uAction;
    private readonly Action onDispose;
    private bool disposed;

    public Connection(UnityEvent<T> uEvent, UnityAction<T> uAction, Action onDispose = default)
    {
        this.uEvent = uEvent;
        this.uAction = uAction;
        this.onDispose = onDispose;
        uEvent.AddListener(uAction);
    }

    public void Dispose()
    {
        if (disposed) return;
        uEvent.RemoveListener(uAction);
        disposed = true;
        onDispose?.Invoke();
    }
}

public static class Extensions
{
    public static void Dispose(this List<IDisposable> disposables)
    {
        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }

        disposables.Clear();
    }

    public static void AddListenerOnce(this UnityEvent unityEvent, Action action)
    {
        UnityAction call = default;
        call = () =>
        {
            action?.Invoke();
            unityEvent.RemoveListener(call);
        };
        unityEvent.AddListener(call);
    }

    public static IDisposable Subscribe(this UnityEvent unityEvent, Action action, ConnectionManager manager1 = default,
        ConnectionManager manager2 = default)
    {
        IDisposable conn = null;
        conn = new Connection(
            unityEvent,
            () => action?.Invoke(),
            () =>
            {
                manager1?.DisposeOf(conn);
                manager2?.DisposeOf(conn);
            });
        manager1?.Add(conn);
        manager2?.Add(conn);
        return conn;
    }

    public static IDisposable Subscribe<T>(this UnityEvent<T> unityEvent, Action<T> action,
        ConnectionManager manager1 = default, ConnectionManager manager2 = default)
    {
        IDisposable conn = null;
        conn = new Connection<T>(
            unityEvent,
            (arg) => action?.Invoke(arg),
            () =>
            {
                manager1?.DisposeOf(conn);
                manager2?.DisposeOf(conn);
            });
        manager1?.Add(conn);
        manager2?.Add(conn);
        return conn;
    }

    public static IDisposable SubscribeOnce(this UnityEvent unityEvent, Action action,
        ConnectionManager manager1 = default, ConnectionManager manager2 = default)
    {
        IDisposable conn = default;
        UnityAction call = () =>
        {
            action?.Invoke();
            conn.Dispose();
        };
        conn = new Connection(unityEvent, call,
            () =>
            {
                manager1?.DisposeOf(conn);
                manager2?.DisposeOf(conn);
            });
        manager1.Add(conn);
        manager2.Add(conn);
        return conn;
    }

    public static IDisposable SubscribeOnce<T>(this UnityEvent<T> unityEvent, Action<T> action,
        ConnectionManager manager1 = default, ConnectionManager manager2 = default)
    {
        IDisposable conn = default;
        UnityAction<T> call = (arg) =>
        {
            action?.Invoke(arg);
            conn.Dispose();
        };
        conn = new Connection<T>(unityEvent, call, () =>
        {
            manager1?.DisposeOf(conn);
            manager2?.DisposeOf(conn);
        });
        manager1.Add(conn);
        manager2.Add(conn);
        return conn;
    }
}