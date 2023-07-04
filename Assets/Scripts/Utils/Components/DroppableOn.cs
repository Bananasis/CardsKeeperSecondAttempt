using System;
using UnityEngine;
using UnityEngine.EventSystems;


public interface IDroppable
{
}

public abstract class DroppableOn<D> : MonoBehaviour, IDroppable, IDragHandler, IBeginDragHandler, IEndDragHandler
    where D : Component, IDropZone
{
    private Transform lastZoneCandidateTransform;
    private D lastZoneCandidate;
    private bool _dragged;
    public bool dragged => _dragged;

    private void OnDisable()
    {
        _dragged = false;
    }

    private void SetLastDropCandidate(D candidate, EventType type, PointerEventData eventData)
    {
        if (candidate == lastZoneCandidate)
        {
            if (candidate == null) return;
            switch (type)
            {
                case EventType.Exit:
                    OnEndDrag(eventData, lastZoneCandidate,true);
                    break;
                case EventType.Enter:
                    OnBeginDrag(eventData, candidate);
                    break;
                case EventType.Move:
                    OnDrag(eventData, candidate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return;
        }

        if (candidate == null)
        {
            if (type == EventType.Move) OnDrag(eventData, lastZoneCandidate);
            OnEndDrag(eventData, lastZoneCandidate,false);

            lastZoneCandidate = candidate;
            return;
        }

        if (lastZoneCandidate == null)
        {
            OnBeginDrag(eventData, candidate);
            if (type == EventType.Move) OnDrag(eventData, candidate);
            lastZoneCandidate = candidate;
            return;
        }

        if (type == EventType.Move) OnDrag(eventData, candidate);
        OnEndDrag(eventData, lastZoneCandidate,false);
        lastZoneCandidate = candidate;
        OnBeginDrag(eventData, lastZoneCandidate);
        if (type == EventType.Move) OnDrag(eventData, candidate);
        if (type == EventType.Exit)
        {
            OnEndDrag(eventData, lastZoneCandidate,true);
    
        }
    }

    private void ProcessCandidate(PointerEventData eventData, EventType type)
    {
        if (lastZoneCandidateTransform == eventData.pointerEnter?.transform)
        {
            SetLastDropCandidate(lastZoneCandidate, type, eventData);
            return;
        }

        lastZoneCandidateTransform = eventData.pointerEnter?.transform;
        if (lastZoneCandidateTransform == null)
        {
            SetLastDropCandidate(null, type, eventData);
            return;
        }

        D candidate = null;
        var parent = eventData.pointerEnter?.transform;
        while (parent != null)
        {
            if (parent.TryGetComponent(out candidate)) break;
            parent = parent.parent;
        }

        SetLastDropCandidate(candidate, type, eventData);
    }


    public void OnDrag(PointerEventData eventData)
    {
        _dragged = true;
        ProcessCandidate(eventData, EventType.Move);
        OnDrag(eventData, lastZoneCandidate != null);
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastZoneCandidate = null;
        lastZoneCandidateTransform = null;
        ProcessCandidate(eventData, EventType.Enter);
        OnBeginDrag(eventData, lastZoneCandidate != null);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        ProcessCandidate(eventData, EventType.Exit);
        OnEndDrag(eventData, lastZoneCandidate != null);
        lastZoneCandidate = null;
        lastZoneCandidateTransform = null;
        _dragged = false;
    }


    protected virtual void OnDrag(PointerEventData eventData, D Zone)
    {
    }


    protected virtual void OnBeginDrag(PointerEventData eventData, D Zone)
    {
    }


    protected virtual void OnEndDrag(PointerEventData eventData, D Zone, bool dropped)
    {
    }


    protected virtual void OnDrag(PointerEventData eventData, bool withDrop)
    {
    }


    protected virtual void OnBeginDrag(PointerEventData eventData, bool withDrop)
    {
    }


    protected virtual void OnEndDrag(PointerEventData eventData, bool withDrop)
    {
    }
}