using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleScaleAnimation : MonoBehaviour
{
    [SerializeField] private Ease openEase;
    [SerializeField] private Ease hideEase;
    [SerializeField] private float duration;


    public void OpenObject(Action callback = null)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, duration).SetEase(openEase).OnComplete(() => callback?.Invoke());
    }
    
    public void HideObject(Action callback = null)
    {
        transform.DOScale(0, duration).SetEase(hideEase).OnComplete(() => callback?.Invoke());
    }
}