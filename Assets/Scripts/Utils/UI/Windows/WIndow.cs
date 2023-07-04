using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using UnityEngine.UI;


[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(CanvasGroup))]
public class Window : MonoBehaviour
{
    public WindowType windowType;
    public bool changingState;
    public Dictionary<KeyCode, Action> shortcuts = new Dictionary<KeyCode, Action>();
    protected GraphicRaycaster _raycaster;
    protected CanvasGroup _canvasGroup;
    protected readonly UnityEvent OnWindowOpen = new UnityEvent();
    protected readonly UnityEvent OnWindowClose = new UnityEvent();
    protected IWindowManager _windowManager;

    public virtual void Init()
    {
        _raycaster = GetComponent<GraphicRaycaster>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ChangeState(WindowState state, bool anim, Action act = default)
    {
        if (changingState) return;
        changingState = true;
        Action action = () =>
        {
            changingState = false;
            act?.Invoke();
        };
        switch (state)
        {
            case WindowState.Show:
                _raycaster.enabled = true;
                gameObject.SetActive(true);
                //OnWindowOpen.Invoke();
                StartCoroutine(OnShow(anim,action));
                break;
            case WindowState.Close:
                OnWindowClose.Invoke();
                StartCoroutine(OnClose(anim,() =>
                {
                    action?.Invoke();
                    _raycaster.enabled = false;
                    gameObject.SetActive(false);
                }));
                break;
            case WindowState.Hide:
                //OnWindowClose.Invoke();
                StartCoroutine(OnHide(anim,() =>
                {
                    action?.Invoke();
                    _raycaster.enabled = false;
                }));
                break;
            case WindowState.Open:
                _raycaster.enabled = true;
                gameObject.SetActive(true);
                OnWindowOpen.Invoke();
                StartCoroutine(OnOpen(anim,action));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    protected virtual IEnumerator OnShow(bool anim,Action act = default)
    {
        yield break;
    }


    protected virtual IEnumerator OnHide(bool anim,Action act = default)
    {
        yield break;
    }


    protected virtual IEnumerator OnClose(bool anim,Action act = default)
    {
        yield break;
    }


    protected virtual IEnumerator OnOpen(bool anim,Action act = default)
    {
        yield break;
    }
}

static class WindowUtils
{
    public static bool Is(this Animations anim, Animations anim2)
    {
        return (anim & anim2) != Animations.None;
    }
}


public enum WindowState
{
    Show,
    Close,
    Hide,
    Open
}