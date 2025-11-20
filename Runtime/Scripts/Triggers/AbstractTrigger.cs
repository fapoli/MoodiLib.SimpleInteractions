using UnityEngine;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Base class for all triggerable actions used by the interaction system.
    /// A trigger may be invoked manually, fired automatically when the player
    /// enters a trigger collider, or executed as a post-interaction action
    /// by an <see cref="AbstractInteractive"/>.
    /// </summary>
    public abstract class AbstractTrigger : MonoBehaviour, ITriggerable {

        /// <summary>
        /// If true, the trigger activates automatically when the player
        /// enters this object's trigger collider.
        /// </summary>
        public bool isAreaTrigger;

        /// <summary>
        /// If true, the trigger executes only once and ignores further calls.
        /// </summary>
        public bool isSingleUse;
        
        private bool _wasUsed;
        
        private void OnTriggerEnter(Collider other) {
            if (!isAreaTrigger || !other.CompareTag("Player")) return;
            Trigger();
        }

        /// <summary>
        /// Executes the trigger if allowed by single-use rules.
        /// May be called manually or by an interactive's post-action system.
        /// </summary>
        public void Trigger() {
            if (_wasUsed && isSingleUse) return;
            _wasUsed = true;
            OnTriggered();
        }

        /// <summary>
        /// Resets internal usage state, allowing the trigger to fire again.
        /// </summary>
        public void ResetUsage() {
            _wasUsed = false;
        }
        
        /// <summary>
        /// Implement the trigger's behavior in derived classes.
        /// </summary>
        protected abstract void OnTriggered();
    }
}