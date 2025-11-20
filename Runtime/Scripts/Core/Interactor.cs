using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Core interaction manager responsible for detecting interactables,
    /// selecting the one currently pointed at, and providing interaction
    /// context data to indicator renderer strategies.
    /// </summary>
    public class Interactor : MonoBehaviour {

        /// <summary>
        /// Transform used as the origin for forward interaction raycasts.
        /// Typically the player camera or an aiming pivot.
        /// </summary>
        public Transform raycastOriginTransform;

        /// <summary>
        /// The interactable currently selected by the forward raycast,
        /// or null if none is valid this frame.
        /// </summary>
        public IInteractive selectedInteractive { get; private set; }

        /// <summary>
        /// Layers considered valid for both raycast selection and
        /// nearby interactable detection.
        /// </summary>
        public LayerMask interactionLayerMask;

        /// <summary>
        /// Maximum distance for interaction raycasts and nearby detection.
        /// </summary>
        [FormerlySerializedAs("maxInteractionDistance")]
        public float interactionRadius;

        private readonly List<AbstractIndicatorRendererStrategy> _indicatorRenderers = new();
        private readonly List<IInteractive> _emptyInteractives = new();

        private void Awake() {
            _indicatorRenderers.AddRange(GetComponents<AbstractIndicatorRendererStrategy>());
        }

        /// <summary>
        /// Performs the full interaction evaluation for this frame.
        /// Determines the selected interactable, gathers all nearby ones,
        /// creates an <see cref="InteractionContext"/>, and invokes
        /// all attached indicator renderer strategies.
        ///
        /// Should be called once per frame by user code.
        /// </summary>
        /// <returns>The currently selected interactable, or null.</returns>
        public IInteractive CheckInteractions() {
            if (raycastOriginTransform == null) return null;
            
            selectedInteractive = GetSelectedInteractive();
            var nearbyInteractives = GetNearbyInteractives();

            var ctx = new InteractionContext(
                raycastOriginTransform,
                selectedInteractive,
                nearbyInteractives,
                interactionRadius
            );

            foreach (var renderer in _indicatorRenderers) {
                renderer.Render(ctx);
            }
            
            return selectedInteractive;
        }
        
        /// <summary>
        /// Collects all interactables within <see cref="interactionRadius"/>
        /// based on the configured <see cref="interactionLayerMask"/>.
        /// Filters out interactables that are currently deactivated.
        /// </summary>
        private List<IInteractive> GetNearbyInteractives() {
            if (raycastOriginTransform == null) return _emptyInteractives;

            var colliders = Physics.OverlapSphere(
                raycastOriginTransform.position,
                interactionRadius,
                interactionLayerMask
            );

            var nearby = new List<IInteractive>();
            foreach (var collider in colliders) {
                var components = collider.GetComponents<IInteractive>();
                foreach (var interactive in components) {
                    if (interactive != null && !interactive.isDeactivated)
                        nearby.Add(interactive);
                }
            }

            return nearby;
        }

        /// <summary>
        /// Performs a forward raycast to determine which interactable,
        /// if any, is currently being pointed at. Returns the closest
        /// valid active interactable hit by the ray.
        /// </summary>
        private IInteractive GetSelectedInteractive() {
            if (raycastOriginTransform == null) return null;
            
            var allHits = Physics.RaycastAll(
                raycastOriginTransform.position,
                raycastOriginTransform.forward,
                interactionRadius,
                interactionLayerMask
            );

            Array.Sort(allHits, (a, b) => a.distance.CompareTo(b.distance));
            
            foreach (var hit in allHits) {
                var interactives = hit.collider?.GetComponents<IInteractive>();
                if (interactives == null) continue;

                foreach (var interactive in interactives) {
                    if (interactive != null && !interactive.isDeactivated)
                        return interactive;
                }
            }
            
            return null;
        }
    }
}