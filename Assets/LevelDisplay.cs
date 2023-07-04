using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class LevelDisplay : MonoBehaviour
{
    [Inject] private GameManager _gameManager; 
    // Start is called before the first frame update
    void Start()
    {
        var text = GetComponent<TextMeshProUGUI>();
        _gameManager.level.Subscribe((l) => text.text = $"Level {l+1}");
    }

    
}
