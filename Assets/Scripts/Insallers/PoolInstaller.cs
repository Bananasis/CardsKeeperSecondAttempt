using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class PoolInstaller : MonoInstaller
{
    [SerializeField] private List<MonoBindable> prefs;
    [SerializeField] private List<GameObject> componentHolders;
    public override void InstallBindings()
    {
        foreach (var componentHolder in componentHolders)
        {
            foreach (var component in componentHolder.GetComponents<Component>())
            {
                if (component is not IBindable bindable) continue;
                bindable.Bind(Container);
            }

         
        }
        
        foreach (var monoBindable in prefs)
        {
            monoBindable.Bind(Container);
        }
    }
    
    
}