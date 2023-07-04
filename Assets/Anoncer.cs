using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class Anoncer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Tween _show;
    private bool showing;
    [Inject] private GameManager _gameManager;
    [Inject] private FightManager _fightManager;
    void Start()
    {
        text.color = new Color(1, 1, 1, 0);
        text.enabled = false;
        _gameManager.OnDefeat.Subscribe(() => Show("Dungeon was plundered", 10));
        _gameManager.OnVictory.Subscribe(() => Show("Victory!", 10));
        _fightManager.OnLevelPassed.Subscribe(() => Show("Wave Complete!", 2));
    }

    private void Show(string aText, float time)
    {
        if(showing) return;
        showing = true;
        text.enabled = true;
        text.text = aText;
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append( text.DOFade(1, 1));
        sequence.AppendInterval(time);
        sequence.Append(text.DOFade(0, 1));
        sequence.AppendCallback(() =>
        {
            text.enabled = false;
            showing = false;
        });


    }

}
