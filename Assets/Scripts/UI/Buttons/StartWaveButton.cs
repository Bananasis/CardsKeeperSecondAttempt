using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class StartWaveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Inject] private GameManager _gameManager;

    [SerializeField] private Image icon;
    [SerializeField] private Color can;

    [SerializeField] private Color cant;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.Subscribe(_gameManager.StartWave);
        _gameManager.canStartNewWave.Subscribe(UpdateButton);
    }

    void UpdateButton(bool canStartNewWave)
    {
        _color?.Kill();
        _color = icon.DOColor(canStartNewWave ? can : cant, 0.5f);
        if (!hovered) return;
        _pulsate?.Kill();
        if (!_gameManager.canStartNewWave.val)
        {
            if (_rescale.IsPlaying())
                _rescale.onComplete = () => _pulsate = icon.transform.DOScale(1, 0.7f);
            else
                _pulsate = icon.transform.DOScale(1, 0.7f);
            return;
        }

        if (_rescale.IsPlaying())
            _rescale.onComplete = () => _pulsate = icon.transform.DOScale(1.5f, 0.7f).SetLoops(-1, LoopType.Yoyo);
        else
            _pulsate = icon.transform.DOScale(1.5f, 0.7f).SetLoops(-1, LoopType.Yoyo);
    }

    private bool hovered;
    private Tween _color;
    private Tween _pulsate;
    private Tween _rescale;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.reentered) return;
        hovered = true;
        _rescale?.Kill();
        _pulsate.Kill();
        _rescale = transform.DOScale(1.4f, 0.5f);
        if (_gameManager.canStartNewWave.val)
            _rescale.onComplete = () => { _pulsate = icon.transform.DOScale(1.5f, 0.7f).SetLoops(-1, LoopType.Yoyo); };
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.fullyExited) return;
        hovered = false;
        _rescale.Kill();
        _pulsate.Kill();
        _rescale = transform.DOScale(1, 0.5f);
        _rescale.onComplete = () => _pulsate = icon.transform.DOScale(1, 0.7f);
    }
}