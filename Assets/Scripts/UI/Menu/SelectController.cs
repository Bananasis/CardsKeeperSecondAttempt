using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class SelectController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler,IPointerClickHandler
{
    [Inject] private GridMono _gridMono;

    [SerializeField] RectTransform selectionDisplay;
    private Vector2 start;
    private Vector2 startScreen;
    private bool dragged;
    [SerializeField] private Canvas _canvas;
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_gridMono.locked) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        dragged = true;
        _gridMono.DeselectAll();
        
        selectionDisplay.gameObject.SetActive(true);
        start = eventData.pointerCurrentRaycast.worldPosition;
        startScreen = eventData.position / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragged) return;
        dragged = false;
        if (_gridMono.locked) return;
        Vector2 end = eventData.pointerCurrentRaycast.worldPosition;
        Rect rect = new Rect(Mathf.Min(start.x, end.x),
            Mathf.Min(start.y, end.y),
            Mathf.Abs(start.x - end.x),
            Mathf.Abs(start.y - end.y));
        _gridMono.SelectAll(rect);
        selectionDisplay.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragged) return;
        var endScreen = eventData.position / _canvas.scaleFactor;
        Rect rect = new Rect();
        rect.max = new Vector2(Mathf.Max(endScreen.x, startScreen.x), Mathf.Max(endScreen.y, startScreen.y));
        rect.min = new Vector2(Mathf.Min(endScreen.x, startScreen.x), Mathf.Min(endScreen.y, startScreen.y));
        selectionDisplay.anchoredPosition = rect.min;
        selectionDisplay.sizeDelta = rect.size;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _gridMono.DeselectAll();
    }
}