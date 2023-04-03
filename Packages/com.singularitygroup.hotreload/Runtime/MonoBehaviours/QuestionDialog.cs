using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SingularityGroup.HotReload {
    class QuestionDialog : MonoBehaviour {

        [Header("Information")]
        public Text textSummary;
        public Text textSuggestion;

        [Header("UI controls")]
        public Button buttonContinue;
        public Button buttonCancel;
        
        public TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();

        public void UpdateView(Config config) {
            textSummary.text = config.summary;
            textSuggestion.text = config.suggestion;

            if (string.IsNullOrEmpty(config.continueButtonText)) {
                buttonContinue.enabled = false;
            } else {
                buttonContinue.GetComponentInChildren<Text>().text = config.continueButtonText;
                buttonContinue.onClick.AddListener(() => {
                    completion.TrySetResult(true);
                    Hide();
                });
            }

            if (string.IsNullOrEmpty(config.cancelButtonText)) {
                buttonCancel.enabled = false;
            } else {
                buttonCancel.GetComponentInChildren<Text>().text = config.cancelButtonText;
                buttonCancel.onClick.AddListener(() => {
                    completion.TrySetResult(false);
                    Hide();
                });
            }
        }

        internal class Config {
            public string summary;
            public string suggestion;
            public string continueButtonText = "Continue";
            public string cancelButtonText = "Cancel";
        }

        /// hide this dialog
        void Hide() {
            gameObject.SetActive(false); // this should disable the Update loop?
        }
    }
}