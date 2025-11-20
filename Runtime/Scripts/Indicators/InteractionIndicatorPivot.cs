using UnityEngine;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Defines a local-space pivot offset used by indicator renderer strategies
    /// (such as <see cref="IconRenderStrategy"/>) to determine where screen-space
    /// indicators should appear above this object.
    /// This affects only visual placement and does not influence physics,
    /// interaction checks, or line-of-sight.
    /// </summary>
    public class InteractionIndicatorPivot : MonoBehaviour {

        /// <summary>
        /// Local offset where the indicator should be visually placed.
        /// </summary>
        public Vector3 offset;
        
        protected virtual void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(offset), 0.01f);
        }
    }
}