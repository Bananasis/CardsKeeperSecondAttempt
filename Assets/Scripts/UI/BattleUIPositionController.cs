using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class BattleUIPositionController : RectTransformPositionController
{
    [Inject] private FightManager _fightManager;
    private Vector2 showPosition;
    private Vector2 hidePosition;
    [SerializeField] private bool showOnBattle;
    [SerializeField] private Vector2 hideCoefficients;

    void Start()
    {
        showPosition = rect.anchoredPosition;
        var rect1 = rect.rect;
        hidePosition = new Vector2(rect1.width * hideCoefficients.x, rect1.height * hideCoefficients.y) + showPosition;
         currentPosition = showOnBattle == _fightManager.battle.val ? showPosition : hidePosition;
        _fightManager.battle.Subscribe((b) => { preferredPosition = showOnBattle == b ? showPosition : hidePosition; });
    }
}