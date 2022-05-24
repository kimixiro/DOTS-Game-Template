using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DOTSTemplate.Ui
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField]
        private Slider progressSlider;

        [SerializeField]
        private Animator animator;

        private bool isVisible;
        
        public IEnumerator Show()
        {
            while (!isVisible) yield return null;
        }

        public void Stop()
        {
            animator.SetTrigger("Hide");
        }

        public void UpdateProgress(float progress)
        {
            if (progressSlider == null) return;
            progressSlider.value = progress;
        }

        public void ShowCompleted()
        {
            isVisible = true;
        }

        public void HideCompleted()
        {
            Destroy(gameObject);
        }
    }
}