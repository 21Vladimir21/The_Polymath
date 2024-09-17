using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.UI
{
    public class MenuPanel : MonoBehaviour
    {
        [field: SerializeField] public Button MenuButton { get; private set; }
        [SerializeField] private Button openButton;
        [SerializeField] private Button hideButton;
        [SerializeField] private SimpleScaleAnimation panel;


        private void Awake()
        {
            openButton.onClick.AddListener(OpenPanel);
            hideButton.onClick.AddListener(HidePanel);
            MenuButton.onClick.AddListener(HidePanel);
        }

        private void OpenPanel()
        {
            panel.gameObject.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            panel.OpenObject();
        }

        private void HidePanel() => panel.HideObject(() => panel.gameObject.SetActive(false));
    }
}