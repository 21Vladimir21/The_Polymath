using _Main._Scripts.GameFieldLogic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;

    private readonly Camera _camera;

    private RectTransform _draggedObject;
    private GameFieldSell _startDragCell;
    private GameFieldSell _selectedCell;
    private GameFieldSell _lastSelectedCell;

    private bool _isDragged;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out GameFieldSell cell))
            if (cell.IsBusy)
                StartDrag(cell);
    }

    public void OnPointerUp(PointerEventData eventData) => ResetDrag();

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragged == false) return;
        var selectedObject = eventData.pointerCurrentRaycast.gameObject;

        if (selectedObject != null && selectedObject.TryGetComponent(out GameFieldSell cell)) SelectNewCell(cell);
        if (_draggedObject == null) return;

        _draggedObject.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    private void StartDrag(GameFieldSell cell)
    {
        if (_isDragged || cell.IsBusy == false) return;
        _startDragCell = cell;
        _draggedObject = cell.CurrentTile.RectTransform;
        _isDragged = true;
    }

    private void SelectNewCell(GameFieldSell cell)
    {
        if (_startDragCell != cell && cell != _lastSelectedCell)
        {
            _selectedCell = cell;
            _lastSelectedCell = cell;
        }
    }

    private void ResetDrag()
    {
        if (_selectedCell != null && _selectedCell.IsBusy == false)
            RearrangeSoldier();
        else if (_draggedObject != null) _startDragCell.ResetTilePosition();

        ClearDragState();
    }

    private void RearrangeSoldier()
    {
        _selectedCell.AddTile(_startDragCell.CurrentTile);
        _startDragCell.ClearTileData();
        _selectedCell.ResetTilePosition();
    }

    private void ClearDragState()
    {
        _lastSelectedCell = null;
        _startDragCell = null;
        _draggedObject = null;
        _selectedCell = null;
        _isDragged = false;
    }
}