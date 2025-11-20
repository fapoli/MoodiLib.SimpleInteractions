using UnityEngine;
using System.Collections.Generic;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Immutable snapshot of interaction data for the current frame.
    /// Produced by <see cref="Interactor"/> and consumed by renderer strategies.
    /// </summary>
    public readonly struct InteractionContext {

        /// <summary>
        /// The transform from which the interaction ray is cast.
        /// </summary>
        public Transform raycastOriginTransform { get; }

        /// <summary>
        /// The interactable currently selected by the interactor, or null if none.
        /// </summary>
        public IInteractive selectedInteractive { get; }

        /// <summary>
        /// The interactables detected within interaction range for this frame.
        /// </summary>
        public IReadOnlyList<IInteractive> nearbyInteractives { get; }

        /// <summary>
        /// Maximum radius used to search for nearby interactables.
        /// </summary>
        public float interactionRadius { get; }

        public InteractionContext(
            Transform raycastOriginTransform,
            IInteractive selectedInteractive,
            List<IInteractive> nearbyInteractives,
            float interactionRadius
        ) {
            this.raycastOriginTransform = raycastOriginTransform;
            this.selectedInteractive = selectedInteractive;
            this.nearbyInteractives = nearbyInteractives;
            this.interactionRadius = interactionRadius;
        }
    }
}