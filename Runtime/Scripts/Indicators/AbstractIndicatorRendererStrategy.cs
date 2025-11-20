using UnityEngine;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Base class for all indicator rendering strategies used by an <see cref="Interactor"/>.
    /// Each strategy is invoked once per frame during the interaction pass and receives
    /// the current <see cref="InteractionContext"/>.
    /// </summary>
    public abstract class AbstractIndicatorRendererStrategy : MonoBehaviour {

        /// <summary>
        /// Performs the rendering logic for this strategy using the supplied
        /// <see cref="InteractionContext"/>. Called once per frame after the
        /// interactor has evaluated selection and nearby interactables.
        /// </summary>
        /// <param name="ctx">The interaction context for the current frame.</param>
        public abstract void Render(InteractionContext ctx);
    }
}