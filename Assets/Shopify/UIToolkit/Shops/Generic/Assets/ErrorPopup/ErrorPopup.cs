namespace Shopify.UIToolkit.Themes {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ErrorPopup : MonoBehaviour {
        public Animator Animator;
        public Text Text;

        private Queue<string> _errors = new Queue<string>();
        private HashSet<string> _errorsInQueue = new HashSet<string>();


        /// <summary>
        /// Adds an error to the queue of errors to be displayed if it is not already in the queue.
        /// </summary>
        /// <param name="error">The error to display</param>
        /// <returns>true if the error was added to the queue, false if it is a duplicate of a message already in the queue.</returns>
        public bool AddError(string error) {
            if (_errorsInQueue.Contains(error)) return false;
            _errors.Enqueue(error);
            _errorsInQueue.Add(error);

            if (_showErrorRoutine == null) {
                _showErrorRoutine = StartCoroutine(ShowNextError());
            }

            return true;
        }

        private Coroutine _showErrorRoutine;
        public IEnumerator ShowNextError() {
            string error = _errors.Dequeue();
            Text.text = error;
            Animator.Play("ShowError");

            yield return new WaitUntil(() => {
                return Animator.GetCurrentAnimatorStateInfo(0).IsName("Ready");
            });

            _errorsInQueue.Remove(error);

            if (_errors.Count == 0) yield break;
            _showErrorRoutine = StartCoroutine(ShowNextError());
        }
    }
}
