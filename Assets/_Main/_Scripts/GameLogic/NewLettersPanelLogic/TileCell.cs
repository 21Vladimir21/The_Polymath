using System;
using _Main._Scripts.GameLogic.LettersLogic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace _Main._Scripts.GameLogic.NewLettersPanelLogic
{
    public class TileCell : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image shine;
        public bool IsBusy { get; private set; }
        public LetterTile CurrentTile { get; private set; }

        protected Vector2Int Coords;
        public void SetCellCoords(int x, int y) => Coords = new Vector2Int(x, y);


        public virtual void AddTileAndAllowMove(LetterTile tile)
        {
            AddTile(tile);
            tile.ResetTile();
        }

        public void AddTile(LetterTile tile, bool setPosition = true)
        {
            CurrentTile = tile;
            IsBusy = true;
            if (setPosition) ResetTilePosition();
        }

        public virtual void ClearTileData()
        {
            CurrentTile = null;
            IsBusy = false;
        }

        public virtual void ResetTilePosition() => CurrentTile.transform.position = transform.position;


        public void AnimatedMoveTileToCell(Action callback)
        {
            CurrentTile.RectTransform.DOMove(rectTransform.position, 1f).OnComplete(()=>callback?.Invoke());
        }

        public void ActivateShine(Action callback)
        {
            var seq = DOTween.Sequence();
            seq.Append(shine.DOFade(1, 0.5f)).AppendInterval(0.25f).Append(shine.DOFade(0, 0.5f))
                .SetLoops(2).OnComplete(()=>callback?.Invoke());
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        }
#endif
    }
}