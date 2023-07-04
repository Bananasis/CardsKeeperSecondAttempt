using System;
using DefaultNamespace;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileComponent : MonoPoolable<TileComponent>
{
    protected SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private TileComponentSprite _spriteHolder = default;
    private Rotation _defaultRotation;
    private IDisposable connection;
    private Tile _tile;


    public void UpdateComponent(TileComponentSprite spriteHolder)
    {
        if (_tile == null) throw new Exception();
        _spriteHolder = spriteHolder;
        SetRotation(_tile.position.val.rotation);
    }

    public void UpdateComponent(TileComponentSprite spriteHolder, Tile tile, Rotation rotation = Rotation.Left)
    {
        Clear();
        _tile = tile;
        _spriteHolder = spriteHolder;
        _defaultRotation = rotation;
        connection = tile.position.Subscribe((gd) => SetRotation(gd.rotation));
    }

    public void Clear()
    {
        _spriteRenderer.sprite = null;
        transform.rotation = Quaternion.identity;
        connection?.Dispose();
        _spriteHolder = null;
        _spriteRenderer.color = Color.white;
        _defaultRotation = Rotation.Left;
    }

    public override void Dispose()
    {
        Clear();
        base.Dispose();
    }


    protected void SetRotation(Rotation tileRotation)
    {
        tileRotation = tileRotation.Add(_defaultRotation);
        if (_spriteHolder == null) throw new Exception();
        _spriteRenderer.sortingOrder = _spriteHolder.layer;
        switch (_spriteHolder.rType)
        {
            case RotationType.Multiple:
                _spriteRenderer.sprite = _spriteHolder.rotations[(int) (tileRotation.Add(_spriteHolder.rotation))];
                transform.rotation = Quaternion.identity;
                break;
            case RotationType.None:
                _spriteRenderer.sprite = _spriteHolder.sprite;
                transform.rotation = Quaternion.identity;
                break;
            case RotationType.Single:
                _spriteRenderer.sprite = _spriteHolder.sprite;
                transform.rotation = Quaternion.Euler(0, 0, -90 * (int) (tileRotation.Add(_spriteHolder.rotation)));
                break;
        }
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }
}