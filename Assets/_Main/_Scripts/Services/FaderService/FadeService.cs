using System;
using _Main._Scripts.LetterPooLogic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.Services.FaderService
{
    public class FadeService : MonoBehaviour, IService
    {
        [SerializeField] private Image fade;
        [SerializeField] private float fadeDuration;


        public void EnableFade(Action closeCallback = null, Action openCallback = null)
        {
            var seq = DOTween.Sequence();
            seq.Append(fade.DOFade(1, fadeDuration / 2)).AppendCallback(() => closeCallback?.Invoke())
                .Append(fade.DOFade(0, fadeDuration / 2)).AppendCallback(() => openCallback?.Invoke());
        }
    }
}