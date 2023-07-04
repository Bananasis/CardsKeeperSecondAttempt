using System;
using System.Collections;
using UnityEngine;


public class AlphaWindow : Window
{
    protected virtual float deltaAlpha => 3;

    public override void Init()
    {
        base.Init();
        _canvasGroup.alpha = 0;
    }


    protected override IEnumerator OnClose(bool anim, Action act = default)
    {
        return OnHide(anim, act);
    }

    protected override IEnumerator OnOpen(bool anim, Action act = default)
    {
        return OnShow(anim, act);
    }

    protected override IEnumerator OnShow(bool anim, Action act = default)
    {
        if (anim)
            for (float alpha = 0; alpha < 1; alpha += Time.unscaledDeltaTime * deltaAlpha)
            {
                _canvasGroup.alpha = alpha;
                yield return null;
            }

        _canvasGroup.alpha = 1;
        act?.Invoke();
    }

    protected override IEnumerator OnHide(bool anim, Action act = default)
    {
        if (anim)
            for (float alpha = 1; alpha > 0; alpha -= Time.unscaledDeltaTime * deltaAlpha)
            {
                _canvasGroup.alpha = alpha;
                yield return null;
            }

        _windowManager.CloseTopWindow();
        _canvasGroup.alpha = 0;
        act?.Invoke();
    }
}