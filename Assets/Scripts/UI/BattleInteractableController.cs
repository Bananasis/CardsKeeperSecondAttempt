using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;


public class BattleInteractableController : MonoBehaviour
{
    [Inject] private FightManager _fightManager;
    [SerializeField] private List<MonoBehaviour> deactivateOnBattle;
    [SerializeField] private List<MonoBehaviour> activateOnBattle;
    [SerializeField] private List<GameObject> deactivateOnBattleObjects;
    [SerializeField] private List<GameObject>  activateOnBattleObjects;
    void Start()
    {
        
        _fightManager.battle.Subscribe((b) =>
        {
            deactivateOnBattle.ForEach((mb => mb.enabled = !b));
            deactivateOnBattleObjects.ForEach((gm) =>gm.SetActive(!b));
    
        },preInvoke:false);
        _fightManager.battle.Subscribe((b) =>
        {
            activateOnBattle.ForEach((mb => mb.enabled = b));
            activateOnBattleObjects.ForEach((gm) =>gm.SetActive(b));
        },preInvoke:true);
    }

    // Update is called once per frame
}