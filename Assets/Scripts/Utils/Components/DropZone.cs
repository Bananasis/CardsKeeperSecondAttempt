using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public enum EventType
{
    Exit,
    Enter,
    Move
}

public interface IDropZone
{
}

public abstract class DropZone<C> : MonoBehaviour, IDropZone, IPointerEnterHandler, IPointerExitHandler,IDropHandler,
    IPointerMoveHandler where C : Component
{
    private GameObject lastDropCandidateObject;
    private C lastDropCandidate;

    private void SetLastDropCandidate(C candidate, EventType type, PointerEventData eventData)
    {
        if (candidate == lastDropCandidate)
        {
            if (candidate == null) return;
            switch (type)
            {
                case EventType.Exit:
                    OnPointerExit(eventData, candidate, false);
                    break;
                case EventType.Enter:
                    OnPointerEnter(eventData, candidate);
                    break;
                case EventType.Move:
                    OnPointerMove(eventData, candidate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return;
        }

        if (candidate == null)
        {
            if (type == EventType.Move) OnPointerMove(eventData, lastDropCandidate);
            OnPointerExit(eventData, lastDropCandidate, true);
            lastDropCandidate = candidate;
            return;
        }

        if (lastDropCandidate == null)
        {
            OnPointerEnter(eventData, candidate);
            if (type == EventType.Move) OnPointerMove(eventData, candidate);
            lastDropCandidate = candidate;
            return;
        }

        if (type == EventType.Move) OnPointerMove(eventData, lastDropCandidate);
        OnPointerExit(eventData, lastDropCandidate, true);
        lastDropCandidate = candidate;
        OnPointerEnter(eventData, lastDropCandidate);
        if (type == EventType.Move) OnPointerMove(eventData, lastDropCandidate);
        if (type == EventType.Exit) OnPointerExit(eventData, lastDropCandidate, true);
    }

    private void ProcessCandidate(PointerEventData eventData, EventType type)
    {
        var newDropCandidateObject = eventData.dragging ? eventData.pointerDrag : null;
        if (lastDropCandidateObject == newDropCandidateObject)
        {
            SetLastDropCandidate(lastDropCandidate, type, eventData);
            return;
        }

        lastDropCandidateObject = newDropCandidateObject;
        if (lastDropCandidateObject == null)
        {
            SetLastDropCandidate(null, type, eventData);
            return;
        }

        lastDropCandidateObject.TryGetComponent(out C candidate);
        SetLastDropCandidate(candidate, type, eventData);
    }

    public virtual void OnPointerMove(PointerEventData eventData)
    {
        ProcessCandidate(eventData, EventType.Move);
        OnPointerMove(eventData, lastDropCandidate != null);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventData.reentered)
        {
            ProcessCandidate(eventData, EventType.Enter);
        }

        OnPointerEnter(eventData, lastDropCandidate != null);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.fullyExited)
            ProcessCandidate(eventData, EventType.Exit);
        OnPointerExit(eventData, lastDropCandidate != null);
        if (!eventData.fullyExited) return;
        lastDropCandidate = null;
        lastDropCandidateObject = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ProcessCandidate(eventData, EventType.Move);
        if(lastDropCandidate != null)
            OnPointerExit(eventData, lastDropCandidate, true);
        lastDropCandidate = null;
        lastDropCandidateObject = null;
    }
    
    protected virtual void OnPointerEnter(PointerEventData data, C drop)
    {
    }

    protected virtual void OnPointerEnter(PointerEventData data, bool withDrop)
    {
    }

    protected virtual void OnPointerMove(PointerEventData data, C drop)
    {
    }

    protected virtual void OnPointerMove(PointerEventData data, bool withDrop)
    {
    }

    protected virtual void OnPointerExit(PointerEventData data, C drop, bool dropped)
    {
    }

    protected virtual void OnPointerExit(PointerEventData data, bool withDrop)
    {
    }

 
}