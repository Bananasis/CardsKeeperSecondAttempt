using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    [SerializeField] private List<Component> bindableComponents = new();
    [SerializeField] private List<MonoBindable> prefs;

    public override void InstallBindings()
    {
       // Container.Bind<RectTransform>().To<RectTransform>().FromNewComponentSibling();
        bindableComponents.ForEach(c =>
        {
            if (c is not IBindable bindable) throw new Exception();
            bindable.Bind(Container);
        });
        foreach (var monoBindable in prefs)
        {
            monoBindable.Bind(Container);
        }
    }
}