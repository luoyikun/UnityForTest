using UnityEngine;
using UnityEngine.UI;

namespace xasset.example
{
    public class LoadingScreen : MonoBehaviour, IProgressBar
    {
        public Slider slider;
        public Text text;
        public CanvasGroup canvasGroup;
        public GraphicRaycaster raycaster;

        public void SetMessage(string message)
        {
            text.text = message;
        }

        public void SetProgress(float progress)
        {
            slider.value = progress;
        }

        public void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1 : 0;
            raycaster.enabled = visible;
        }
    }
}