using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;


public class VFX : MonoPoolable<VFX>
{
    [SerializeField] private SpriteRenderer _renderer;
    private int frame;
    public bool finished;


    public void Play(List<Sprite> anim, float time)
    {
        frame = 0;
        finished = false;
        var frameDur = time / anim.Count;
        StartCoroutine(PlayAndDispose(() =>
        {
            _renderer.sprite = anim[frame];
            return ++frame < anim.Count;
        }, frameDur));
    }

    IEnumerator PlayAndDispose(Func<bool> act, float frameDur)
    {
        var waitObj = new WaitForSeconds(frameDur);
        while (act.Invoke())
            yield return waitObj;
        yield return waitObj;
        Dispose();
    }

    public override void Dispose()
    {
        finished = true;
        base.Dispose();
    }

    private void OnDisable()
    {
        if(!finished) Dispose();
    }
}