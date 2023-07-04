using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class UiEnemyIcon : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Image weapon;
    [SerializeField] private Image portrait;
    [Inject] private CardPreview _cardPreview;
    private Enemy _enemy;
    
    public void UpdateIcon(Enemy enemy)
    {
        _enemy = enemy;
        portrait.sprite = enemy.sprite;
        weapon.sprite = enemy.weaponSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cardPreview.Show(_enemy);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cardPreview.Hide();
    }
}