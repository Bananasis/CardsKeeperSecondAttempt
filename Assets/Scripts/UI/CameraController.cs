using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class CameraController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private bool dragged;
    private Vector2 pos;
    [SerializeField] private float maxCamSize = 10;
    [SerializeField] private float minCamSIze = 2;
    [SerializeField] private Vector2 camAllowedTravel;
    [Inject] private GridMono _gridMono;

    private void Update()
    {
        var cam = Camera.main;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.mouseScrollDelta.y, minCamSIze, maxCamSize);
        if (!dragged) return;
        Vector2 newPos = cam.ScreenToWorldPoint(Input.mousePosition);
        var newCamPos = cam.transform.position + (Vector3) (pos - newPos);
        newCamPos = new Vector3(Mathf.Clamp(newCamPos.x, -camAllowedTravel.x, camAllowedTravel.x),
            Mathf.Clamp(newCamPos.y, -camAllowedTravel.y, camAllowedTravel.y), newCamPos.z);
        cam.transform.position = newCamPos;
    }


    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_gridMono.locked) return;
        if (eventData.button != PointerEventData.InputButton.Right) return;
        dragged = true;
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragged = false;
    }
}