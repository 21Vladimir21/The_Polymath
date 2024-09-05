using System;
using _Main._Scripts.GameLogic.JackPotLogic;
using _Main._Scripts.GameLogic.LettersLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;

    private readonly Camera _camera;

    private RectTransform _draggedObject;
    private TileCell _startDragCell;
    private TileCell _selectedCell;

    private bool _isDragged;
    public Action<LetterTile> OnSwappedTiles;

    public bool CanDrag { get; set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanDrag == false) return;

        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out TileCell cell))
            if (cell.IsBusy && cell.CurrentTile.CanMove)
                StartDrag(cell);
    }

    public void OnPointerUp(PointerEventData eventData) => ResetDrag();

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragged == false) return;
        var selectedObject = eventData.pointerCurrentRaycast.gameObject;

        if (selectedObject != null && selectedObject.TryGetComponent(out TileCell cell)) SelectNewCell(cell);
        if (_draggedObject == null) return;

        _draggedObject.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    private void StartDrag(TileCell cell)
    {
        if (_isDragged || cell.IsBusy == false) return;
        _startDragCell = cell;
        var cellCurrentTile = cell.CurrentTile;
        cellCurrentTile.ResetTile();
        cellCurrentTile.UpTile();
        _draggedObject = cellCurrentTile.RectTransform;
        _isDragged = true;
    }

    private void SelectNewCell(TileCell cell) => _selectedCell = cell;

    private void ResetDrag()
    {
        if (_selectedCell != null && _selectedCell.IsBusy == false)
            RearrangeTile();
        else if (_selectedCell != null && _startDragCell != _selectedCell && _selectedCell.IsBusy &&
                 _selectedCell.CurrentTile is JackPotTile)
        {
            Debug.Log($"SwapTiles");
            var jackPotTile = _selectedCell.CurrentTile;
            _selectedCell.CurrentTile.OnSwapped?.Invoke(jackPotTile, _startDragCell.CurrentTile);
            _selectedCell.ClearTileData();
            _selectedCell.AddTile(_startDragCell.CurrentTile);
            _startDragCell.ClearTileData();
            OnSwappedTiles.Invoke(jackPotTile);
        }
        else if (_draggedObject != null) _startDragCell.ResetTilePosition();

        ClearDragState();
    }

    private void RearrangeTile()
    {
        _selectedCell.AddTileAndAllowMove(_startDragCell.CurrentTile);
        _startDragCell.ClearTileData();
    }

    private void ClearDragState()
    {
        _startDragCell = null;
        _draggedObject = null;
        _selectedCell = null;
        _isDragged = false;
    }
}