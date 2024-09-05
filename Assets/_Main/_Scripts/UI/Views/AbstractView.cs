using System;
using System.Collections;
using UnityEngine;

namespace _Main._Scripts.UI.Views
{
    public abstract class AbstractView : MonoBehaviour
    {
        public virtual void Init()
        {
           
        }

        public void Open(Action callback = null)
        {
            gameObject.SetActive(true);
            callback?.Invoke();
        }

        public void Close(Action callback = null)
        {
            gameObject.SetActive(false);
            callback?.Invoke();
        }

        protected IEnumerator ShowWithDelay(Action callback, float delay = 0.5f)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }
    }
}