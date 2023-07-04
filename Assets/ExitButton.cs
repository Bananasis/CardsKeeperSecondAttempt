using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ExitButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [Inject] private GameManager _gameManager;

    private void Start()
    {
        button.onClick.AddListener(  _gameManager.QuitToMenu);
    }
}
