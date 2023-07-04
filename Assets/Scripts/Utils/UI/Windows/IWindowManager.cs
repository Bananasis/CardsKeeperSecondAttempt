using System;
using System.Linq;
using UnityEngine.Events;
using Utils;


public interface IWindowManager
{
    UnityEvent<(WindowType, WindowType)> OnWindowChanged { get; }

    void CloseTopWindow(Action onClose = default, Action onOpen = default,
        Animations animations = Animations.Both);

    Window OpenWindow(WindowType type, bool closeTop = false, Action onClose = default, Action onOpen = default,
        Animations animations = Animations.Both);

    void CloseWindowsUntil(WindowType type, Action onOpen = default, bool skipMiddle = false);
    Window TopWindow();
    WindowType NextWindowInOrder();
    WindowType TopWindowType();
}

public partial class WindowManager : IWindowManager
{
    public UnityEvent<(WindowType, WindowType)> OnWindowChanged { get; } = new UnityEvent<(WindowType, WindowType)>();

    public void CloseTopWindow(Action onClose = default, Action onOpen = default,
        Animations animations = Animations.Both)
    {
        if (openWindows.Count == 0) throw new GameException("No open windows");
        var wd = openWindows.Peek();
        wd.ChangeState(WindowState.Close, animations.Is(Animations.Close), () =>
        {
            openWindows.Pop();
            onClose?.Invoke();
            if (openWindows.Count == 0)
            {
                OnWindowChanged.Invoke((wd.windowType, WindowType.None));
                onOpen?.Invoke();
                return;
            }

            var newTopWindow = openWindows.Peek();
            OnWindowChanged.Invoke((wd.windowType, newTopWindow.windowType));
            newTopWindow.ChangeState(WindowState.Show, animations.Is(Animations.Open), onOpen);
        });
    }

    public Window OpenWindow(WindowType type, bool closeTop = false, Action onClose = default, Action onOpen = default,
        Animations animations = Animations.Both)
    {
        var wd = windows.FirstOrDefault((s) => s.windowType == type);
        if (wd is null) throw new GameException($"No window with name {type}");
        if (openWindows.Count > 0)
        {
            Window curTopWindow = openWindows.Peek();
            curTopWindow.ChangeState(closeTop ? WindowState.Close : WindowState.Hide, animations.Is(Animations.Close),
                () =>
                {
                    if (closeTop) openWindows.Pop();
                    wd.ChangeState(WindowState.Open, animations.Is(Animations.Open), onOpen);
                    OnWindowChanged.Invoke((curTopWindow.windowType, wd.windowType));
                    openWindows.Push(wd);
                    onClose?.Invoke();
                });
            return wd;
        }

        onClose?.Invoke();
        openWindows.Push(wd);
        OnWindowChanged.Invoke((WindowType.None, wd.windowType));
        wd.ChangeState(WindowState.Open, animations.Is(Animations.Open), onOpen);

        return wd;
    }

    public void CloseWindowsUntil(WindowType type, Action onOpen = default, bool skipMiddle = false)
    {
        var twt = NextWindowInOrder();
        if (twt == type)
        {
            CloseTopWindow(onOpen: onOpen, animations: skipMiddle ? Animations.Open : Animations.Both);
            return;
        }

        if (TopWindowType() == WindowType.None)
        {
            OpenWindow(type, onOpen: onOpen);
            return;
        }

        CloseTopWindow(onOpen: () => { CloseWindowsUntil(type, onOpen, skipMiddle); },
            animations: skipMiddle ? Animations.None : Animations.Both);
    }

    public WindowType TopWindowType()
    {
        if (openWindows.Count == 0) return WindowType.None;
        return openWindows.Peek().windowType;
    }

    public WindowType NextWindowInOrder()
    {
        if (openWindows.Count == 0 || openWindows.Count == 1) return WindowType.None;
        var top = openWindows.Pop();
        var type = openWindows.Peek().windowType;
        openWindows.Push(top);
        return type;
    }

    public Window TopWindow()
    {
        return openWindows.Count > 0 ? openWindows.Peek() : null;
    }
}

[Flags]
public enum Animations
{
    None = 0,
    Open = 1,
    Close = 2,
    Both = 3
}

public enum WindowType
{
    None,
    Achievements,
    Levels,
    MainMenu,
    Pause,
    Settings,
    Cell,
    GameplayUI,
    Loading,
    Credits
}