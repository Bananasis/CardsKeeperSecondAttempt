using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BattleSpeedButton : MonoBehaviour
{
    [SerializeField] private FightManager.BattleSpeed speed;
    [Inject] private FightManager _fightManager;

    void Start()
    {
        var toggle = GetComponent<Toggle>();
        GetComponent<Toggle>().onValueChanged.Subscribe((t) =>
        {
            if (t) _fightManager.SetSpeed(speed);
        });
        _fightManager.battle.Subscribe((b) =>
        {
            if (b && speed == FightManager.BattleSpeed.Normal) toggle.isOn = true;
        });
    }
}