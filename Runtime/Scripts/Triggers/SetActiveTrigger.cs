using UnityEngine;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Trigger that sets a target GameObject's active state when activated.
    /// Can be used manually, via area activation, or as a post-interaction action.
    /// </summary>
    public class SetActiveTrigger : AbstractTrigger {

        /// <summary>
        /// The GameObject to activate or deactivate.
        /// </summary>
        public GameObject target;

        /// <summary>
        /// The state the target should be set to when triggered.
        /// </summary>
        public bool activeState = true;

        protected override void OnTriggered() {
            if (target == null) return;
            target.SetActive(activeState);
        }
    }
}