using DG.Tweening;
using UnityEngine;

namespace _Main._Scripts.GameLogic.NewLettersPanelLogic
{
    public class NewLetterPanelCell : TileCell
    {
        [SerializeField] private float plusYValue = 20;

        private Sequence _shakeSequence;
        private bool _isShaking;


        public override void ResetTilePosition()
        {
            base.ResetTilePosition();
            if (_isShaking) UpAndShakeTile();
        }

        public override void ClearTileData()
        {
            StopShake();
            base.ClearTileData();
        }

        public void UpAndShakeTile()
        {
            _isShaking = true;
            _shakeSequence.Kill();
            _shakeSequence = DOTween.Sequence();
            var tilePosition = CurrentTile.RectTransform.localPosition;
            CurrentTile.RectTransform.localPosition =
                new Vector3(tilePosition.x, tilePosition.y + plusYValue, tilePosition.z);
            _shakeSequence.Append(CurrentTile.RectTransform.DOShakeRotation(Random.Range(0.3f, 1),
                    new Vector3(0, 0, 20), 3, randomnessMode: ShakeRandomnessMode.Harmonic))
                .SetLoops(-1);
        }

        public void DownAndStopShake()
        {
            _isShaking =false;
            ResetTilePosition();
            StopShake();
        }
        private void StopShake()
        {
            _shakeSequence.Kill();
            CurrentTile.RectTransform.localRotation = Quaternion.identity;
        }
    }
}