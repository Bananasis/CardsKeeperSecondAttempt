using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using Zenject;


[RequireComponent(typeof(Button))]

public class TurnButton : MonoBehaviour
{
    [Inject] private FightManager _fightManager;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.Subscribe(_fightManager.MakeMove);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
