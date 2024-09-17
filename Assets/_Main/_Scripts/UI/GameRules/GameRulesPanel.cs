using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.UI.GameRules
{
    public class GameRulesPanel : MonoBehaviour
    {
        [SerializeField] private Button nextSlideButton;
        [SerializeField] private Button backSlideButton;
        [SerializeField] private Button hideButton;
        [SerializeField] private List<GameObject> slides;

        private int _currentSlideIndex;

        private void Awake()
        {
            nextSlideButton.onClick.AddListener(NextSlide);
            backSlideButton.onClick.AddListener(BackSlide);
            hideButton.onClick.AddListener(() => gameObject.SetActive(false));
            slides[_currentSlideIndex].SetActive(true);
        }

        private void NextSlide()
        {
            if (_currentSlideIndex >= slides.Count - 1)
                return;

            slides[_currentSlideIndex].SetActive(false);
            _currentSlideIndex++;
            slides[_currentSlideIndex].SetActive(true);
        }

        private void BackSlide()
        {
            if (_currentSlideIndex <= 0)
                return;

            slides[_currentSlideIndex].SetActive(false);
            _currentSlideIndex--;
            slides[_currentSlideIndex].SetActive(true);
        }
    }
}