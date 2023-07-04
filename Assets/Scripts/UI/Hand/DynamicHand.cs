using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public abstract class DynamicHand<C> : MonoBehaviour, IDynamicHand<C> where C : ICard
{
    protected readonly List<C> cards = new();
    protected readonly Dictionary<C, IDisposable> _connections = new();
    [SerializeField] protected float bezierPadding = 0.1f;
    [SerializeField] protected float weightedPadding = 5f;
    protected RectTransform rect;
    private MyCubicBezier bezier;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rect.GetLocalCorners(corners);
        bezier = new MyCubicBezier(corners);
    }


    void UpdateHandPositioning()
    {
        float absBezierSizeSum = cards.Where(c => c.inHand || c.dragged).Sum((c) => c.position.bezierSize);
        float bSize = 0;
        if (weightedPadding > absBezierSizeSum)
        {
            bSize = (weightedPadding - absBezierSizeSum) / 2;
            absBezierSizeSum = weightedPadding;
        }

        var interactable = !cards.Any(c => c.dragged);
        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            card.SetInteractable(interactable);
            if(!card.inHand && !card.dragged) continue;
            bSize += card.position.bezierSize / 2;
            var relativeDistancePosition =
                bezierPadding + bSize / absBezierSizeSum * (1 - bezierPadding * 2);
            if (card.inHand)
            {
                
                card.UpdateSortingOrder(i + 12);
                var pos = bezier.GetDistPoint(relativeDistancePosition);
                if (cards[i].hovered) pos.y = 30;
                card.position.preferredPosition = pos;
                card.position.preferredRotation =
                    Quaternion.FromToRotation(Vector3.right, bezier.GetDistTangent(relativeDistancePosition));
            }

            bSize += card.position.bezierSize / 2;
        }
    }

    protected virtual void OnAddToHand(C card)
    {
    }

    protected virtual void OnRemoveFromHand(C card)
    {
    }

    private void RemoveFromHand(C card)
    {
        _connections.TryGetValue(card, out var con);
        con?.Dispose();
        _connections.Remove(card);
        OnRemoveFromHand(card);
        UpdateHandPositioning();
    }

    private void AddToHand(C card, int i)
    {
        cards.Insert(i, card);
        card.position.selfManaged = true;
        _connections[card] = card.position.OnNeedPositionUpdate.Subscribe(UpdateHandPositioning);
        OnAddToHand(card);
        UpdateHandPositioning();
    }

    public void TryAddToHand(C card)
    {
        int index = cards.IndexOf(card);
        if (index == -1)
        {
            var i = 0;
            for (; i < cards.Count; i++)
            {
                if (cards[i].position.currentPosition.x >= card.position.currentPosition.x)
                    break;
            }

            AddToHand(card, i);
            return;
        }

        if (index != 0 && cards[index - 1].position.currentPosition.x > card.position.currentPosition.x)
        {
            var temp = cards[index - 1];
            cards[index] = temp;
            cards[index - 1] = card;
            UpdateHandPositioning();
            return;
        }

        if (index != cards.Count - 1 && cards[index + 1].position.currentPosition.x < card.position.currentPosition.x)
        {
            var temp = cards[index + 1];
            cards[index] = temp;
            cards[index + 1] = card;
            UpdateHandPositioning();
        }
    }

    public void TryRemoveFromHand(C card)
    {
        if (!cards.Remove(card)) return;
        RemoveFromHand(card);
    }
}

public interface IDynamicHand<C> where C : ICard
{
    public void TryAddToHand(C card);
    public void TryRemoveFromHand(C card);
}

class MyCubicBezier
{
    private float[] _lookUpSpacing;
    private int _lookUpSize;
    private Vector3[] _points = new Vector3[4];

    public MyCubicBezier(Vector3[] points, int lookUpSize = 50, int sampleNumber = 200)
    {
        if (lookUpSize < 2) throw new Exception("Minimal lookup size is 2");
        if (points.Length != 4) throw new Exception("Cubic curve requires 4 points");
        _lookUpSize = lookUpSize;
        Array.Copy(points, _points, 4);
        _lookUpSpacing = new float[lookUpSize];


        float absMaxDist = 0;
        var nextPoint = points[0];
        for (int i = 0; i < sampleNumber; i++)
        {
            var point = nextPoint;
            nextPoint = GetArcPoint((float) i / (sampleNumber - 1));
            absMaxDist += (point - nextPoint).magnitude;
        }


        float spacing = 1.0f / (lookUpSize - 1);
        int index = 0;
        float relDist = 0;
        nextPoint = points[0];
        for (int i = 0; i < sampleNumber && index < _lookUpSize; i++)
        {
            if (relDist >= index * spacing)
            {
                _lookUpSpacing[index] = (float) i / (sampleNumber - 1);
                index++;
            }

            var point = nextPoint;
            nextPoint = GetArcPoint((float) i / (sampleNumber - 1));
            relDist += (point - nextPoint).magnitude / absMaxDist;
        }
    }

    public Vector2 GetDistPoint(float d)
    {
        return GetArcPoint(DistToArc(d));
    }

    Vector2 GetArcPoint(float t)
    {
        float tSquare = t * t;
        float tCube = t * tSquare;

        return _points[0] * (-tCube + 3 * tSquare - 3 * t + 1) +
               _points[1] * (3 * tCube - 6 * tSquare + 3 * t) +
               _points[2] * (-3 * tCube + 3 * tSquare) +
               _points[3] * (tCube);
    }

    Vector2 GetArcSpeed(float t)
    {
        float tSquare = t * t;

        return _points[0] * (-3 * tSquare + 6 * t - 3) +
               _points[1] * (9 * tSquare - 12 * t + 3) +
               _points[2] * (-9 * tSquare + 6 * t) +
               _points[3] * (3 * tSquare);
    }

    public Vector2 GetDistTangent(float d)
    {
        return GetArcSpeed(DistToArc(d)).normalized;
    }

    float DistToArc(float d)
    {
        if (Mathf.Abs(d - 1) < 0.001f) return d;
        float scaledDistance = d * (_lookUpSize - 1);
        int lookUpIndex = Mathf.FloorToInt(scaledDistance);
        float arc = Mathf.Lerp(_lookUpSpacing[lookUpIndex], _lookUpSpacing[lookUpIndex + 1],
            scaledDistance - lookUpIndex);
        return arc;
    }
}