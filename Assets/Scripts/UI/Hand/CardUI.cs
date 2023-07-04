using System.Threading;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Zenject;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public abstract class CardUI<H, T> : MonoPoolable<T>, IPointerEnterHandler, IPointerExitHandler, ICard
    where H : IDynamicHand<T> where T : CardUI<H, T>
{
    [SerializeField] protected Canvas _canvas;
    protected int _order;
    [Inject] protected H hand;
    [SerializeField] private GraphicRaycaster _raycaster;
    [Inject] protected IBezierPosition _position;
    [Inject] protected IScaleController _scale;


    public override void Dispose()
    {
        hovered = false;
        dragged = false;
        draggedCell.Dispose();
        inHand = true;
        hoveredLock = 0;
        position.bezierSize = 1;
        LockInteractable(true);
        hand.TryRemoveFromHand((T) this);
        base.Dispose();
    }

    public void Init()
    {
        inHand = true;
    }

    public void UpdateSortingOrder(int order)
    {
        _order = order;
        if (_position.selfManagedPosition && !hovered)
            _canvas.sortingOrder = _order;
    }

    public bool hovered { get; private set; }
    public bool inHand { get; protected set; }

    private bool lockInteractable;
    public void LockInteractable(bool interactable)
    {
        lockInteractable = !interactable;
        _raycaster.enabled = interactable;
    }
    
    public void SetInteractable(bool interactable)
    {
        if(lockInteractable) return;
        _raycaster.enabled = interactable;
    }

    
   

    public IBezierPosition position => _position;


    private int hoveredLock;

    protected void Hover()
    {
        hoveredLock++;
        if(hovered) return;
        hovered = true;
        _canvas.sortingOrder = 30;
        _scale.Scale(1.5f);
        position.bezierSize = 2f;
        position.preferredRotation = Quaternion.identity;
    }


    protected void UnHover()
    {
        
        if(!hovered) return;
        hoveredLock--;
        if(hoveredLock != 0) return;
        hovered = false;
        _canvas.sortingOrder = _order;
        _scale.Scale(1);
        position.bezierSize = 1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.reentered) return;
        Hover();
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.fullyExited) return;
        UnHover();
    }

    public bool dragged
    {
        get => draggedCell.val;
        protected set => _draggedCell.val = value;
    }

    protected readonly RootCell<bool> _draggedCell = new RootCell<bool>();
    public ICell<bool> draggedCell => _draggedCell;
}

public interface ICard
{
    public void UpdateSortingOrder(int order);
    bool hovered { get; }
    bool dragged { get; }
    bool inHand { get; }
    void LockInteractable(bool interactable);
    public void SetInteractable(bool interactable);
    public IBezierPosition position { get; }
}