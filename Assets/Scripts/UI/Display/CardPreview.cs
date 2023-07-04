using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class CardPreview : MonoBehaviour
{
    [SerializeField] private EnemyCard enemyPreview;
    [SerializeField] private RoomCard roomPreview;
    [Inject] private IScaleController _scaleController;
    private Coroutine waiting;
    private Coroutine switchMobs;
    private bool locked;

    public void Show(Enemy enemy)
    {
        if (locked) return;
        if (waiting != null)
            StopCoroutine(waiting);
        if (!enemyPreview.isActiveAndEnabled)
        {
            waiting = StartCoroutine(WaitAndDo(() => ShowInstant(enemy)));
            return;
        }

        ShowInstant(enemy);
    }

    public void Show(RoomData room)
    {
        if (locked) return;
        if (waiting != null)
            StopCoroutine(waiting);
        if (!roomPreview.isActiveAndEnabled)
        {
            waiting = StartCoroutine(WaitAndDo(() => ShowInstant(room)));
            return;
        }

        ShowInstant(room);
    }

    private void ShowInstant(Enemy enemy)
    {
        enemyPreview.gameObject.SetActive(true);
        roomPreview.gameObject.SetActive(false);
        _scaleController.Scale(1f, 0.3f);
        _scaleController.onFinish = default;
        enemyPreview.UpdateCard(enemy);
    }

    private void ShowInstant(RoomData room)
    {
        enemyPreview.gameObject.SetActive(false);
        roomPreview.gameObject.SetActive(true);
        _scaleController.Scale(1f, 0.3f);
        _scaleController.onFinish = default;
        roomPreview.UpdateCard(room);
        if (room.type == RoomType.Room)
        {
            if (switchMobs != null)
            {
                StopCoroutine(switchMobs);
                switchMobs = null;
            }

            switchMobs = StartCoroutine(DoEverySecond(roomPreview.NextMob));
        }
    }

    public void Hide()
    {
        if (waiting != null)
        {
            StopCoroutine(waiting);
            waiting = null;
        }

        waiting = StartCoroutine(WaitAndDo(HideInstant));
    }

    public void HideInstant()
    {
        if (switchMobs != null)
        {
            StopCoroutine(switchMobs);
            switchMobs = null;
        }

        if (waiting != null)
        {
            StopCoroutine(waiting);
            waiting = null;
        }

        _scaleController.Scale(0.5f, 0.2f);
        _scaleController.onFinish = () =>
        {
            enemyPreview.gameObject.SetActive(false);
            roomPreview.gameObject.SetActive(false);
        };
    }

    private readonly WaitForSeconds halfSecond = new(0.4f);
    private readonly WaitForSeconds second = new(1.4f);

    IEnumerator WaitAndDo(Action action)
    {
        yield return halfSecond;
        action.Invoke();
    }

    IEnumerator DoEverySecond(Action action)
    {
        while (true)
        {
            yield return second;
            action.Invoke();
        }

        yield break;
    }

    public void Unlock()
    {
        locked = false;
    }

    public void Lock()
    {
        locked = true;
        HideInstant();
    }
}