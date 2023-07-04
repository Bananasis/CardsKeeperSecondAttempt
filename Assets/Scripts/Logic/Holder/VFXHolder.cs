using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class VFXHolder : MonoBehaviour
{
    [Inject] private VFX.MonoPool pool;

    public void Play(VFXData vfx, Transform root, float time = 0.5f, Quaternion rotation = default,
        Vector3 position = default)
    {
        rotation = rotation == default ? Quaternion.identity : rotation;
        pool.Get(root, position, vfx.defaultRotation * rotation).Play(vfx.sprites, time);
    }
}