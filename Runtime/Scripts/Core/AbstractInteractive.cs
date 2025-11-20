using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Base class for interactable objects used by <see cref="Interactor"/>.
    /// Handles condition checks, single-use restrictions, interaction execution,
    /// and post-interaction triggers.
    /// </summary>
    public abstract class AbstractInteractive : MonoBehaviour, IInteractive {

        /// <summary>
        /// Indicates whether this interactive is currently inactive.
        /// True when marked as used while <see cref="isSingleUse"/> is enabled,
        /// or when any <see cref="ICondition"/> fails.
        /// </summary>
        public bool isDeactivated =>
            (_hasBeenUsed && isSingleUse) || !CheckInteractConditions();
        
        [Header("Interaction")]
        /// <summary>
        /// Conditions that must be satisfied for interaction.
        /// Each element must implement <see cref="ICondition"/>.
        /// </summary>
        public List<MonoBehaviour> interactConditions = new();

        /// <summary>
        /// When enabled, this interactive may be used only once.
        /// After a successful interaction, it becomes permanently inactive.
        /// </summary>
        public bool isSingleUse;

        /// <summary>
        /// Optional sound played at this object's position after interaction.
        /// </summary>
        public AudioClip interactSound;
        
        [Header("Post Actions")]
        /// <summary>
        /// Triggers executed after interaction; see <see cref="AbstractTrigger"/>.
        /// </summary>
        public List<AbstractTrigger> postInteractionTriggers = new();

        /// <summary>
        /// Delay in seconds before executing post-interaction triggers.
        /// </summary>
        public float postInteractionDelay = 0.5f;
        
        private bool _hasBeenUsed;

        private bool _conditionsEvaluatedThisFrame;
        private bool _conditionsResultThisFrame;

        private void LateUpdate() {
            _conditionsEvaluatedThisFrame = false;
        }

        /// <summary>
        /// Attempts to interact with this object.
        /// Executes only when all conditions are met and either the object is reusable
        /// or has not been used before.
        /// </summary>
        /// <param name="caller">The GameObject initiating the interaction.</param>
        public void TryInteract(GameObject caller) {
            if (isSingleUse && _hasBeenUsed) return;
            if (!CheckInteractConditions()) return;

            _hasBeenUsed = true;
            StartCoroutine(Interact(caller));
        }

        /// <summary>
        /// Restores usability when <see cref="isSingleUse"/> is disabled.
        /// </summary>
        protected void ResetUsage() {
            _hasBeenUsed = false;
        }

        /// <summary>
        /// Permanently disables interaction by marking this object as used.
        /// </summary>
        protected void Deactivate() {
            isSingleUse = true;
            _hasBeenUsed = true;
        }

        private IEnumerator ExecutePostTriggers() {
            yield return new WaitForSeconds(postInteractionDelay);
            foreach (var trigger in postInteractionTriggers)
                trigger.Trigger();
        }

        /// <summary>
        /// Executes the full interaction sequence:
        /// waits one frame, runs <see cref="OnInteract"/>, plays the optional sound,
        /// and executes post-interaction triggers.
        /// </summary>
        private IEnumerator Interact(GameObject caller) {
            yield return new WaitForEndOfFrame();
            
            OnInteract(caller);

            if (interactSound)
                AudioSource.PlayClipAtPoint(interactSound, transform.position);

            yield return ExecutePostTriggers();
        }

        /// <summary>
        /// Evaluates all configured interaction conditions once per frame.
        /// Subsequent calls in the same frame return the cached result.
        /// </summary>
        private bool CheckInteractConditions() {
            if (_conditionsEvaluatedThisFrame)
                return _conditionsResultThisFrame;

            _conditionsEvaluatedThisFrame = true;
            _conditionsResultThisFrame = EvaluateInteractConditions();
            return _conditionsResultThisFrame;
        }

        /// <summary>
        /// Evaluates all <see cref="ICondition"/> components in <see cref="interactConditions"/>.
        /// Returns true only if every condition is met.
        /// </summary>
        private bool EvaluateInteractConditions() {
            foreach (var entry in interactConditions)
                if (entry is ICondition cond && !cond.isConditionMet)
                    return false;
            return true;
        }

        /// <summary>
        /// Defines the custom behavior of this interaction.
        /// Called once when interaction succeeds.
        /// </summary>
        protected abstract void OnInteract(GameObject caller);
    }
}