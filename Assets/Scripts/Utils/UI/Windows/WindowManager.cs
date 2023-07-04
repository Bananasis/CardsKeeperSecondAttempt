using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utils;

public partial class WindowManager : MonoBehaviour
{
    [SerializeField] private List<Window> windows;
    private readonly Stack<Window> openWindows = new Stack<Window>();
    
    private void Update()
    {
        var tw = TopWindow();
        if (tw is null) return;
        foreach (var keyAction in tw.shortcuts)
        {
            if (Input.GetKeyDown(keyAction.Key))
                keyAction.Value.Invoke();
        }
    }

 



    private void Start()
    {
        windows.ForEach(w => w.Init());
        OpenWindow(WindowType.MainMenu);
    }



}


