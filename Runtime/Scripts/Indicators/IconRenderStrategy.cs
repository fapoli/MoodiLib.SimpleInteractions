using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Renders 2D IMGUI indicators for interactable objects.
    /// Uses <see cref="InteractionContext"/> for selected/nearby data and
    /// <see cref="InteractionIndicatorPivot"/> for visual placement.
    /// <para>
    /// <see cref="Render"/> gathers and filters interactables for the current frame.
    /// <see cref="OnGUI"/> then draws the indicators using that cached state.
    /// </para>
    /// </summary>
    public class IconRenderStrategy : AbstractIndicatorRendererStrategy {

        private const float ReferenceScreenWidth = 1920f;

        /// <summary>Base icon size in pixels (scaled by screen width).</summary>
        public float indicatorSize = 24f;

        /// <summary>Icon used for visible, non-selected interactables.</summary>
        public Texture2D indicatorIcon;

        /// <summary>Icon used for the currently selected interactable.</summary>
        public Texture2D indicatorIconSelected;

        /// <summary>Color applied to all icons.</summary>
        public Color indicatorColor = Color.white;

        private readonly List<IInteractive> _otherInteractives = new();

        private InteractionContext _context;
        private bool _hasContext;

        /// <summary>
        /// Filters nearby interactables using line-of-sight and stores them
        /// for rendering. Called once per frame by the <see cref="Interactor"/>.
        /// <para>
        /// The data prepared here is consumed later by <see cref="OnGUI"/>.
        /// </para>
        /// </summary>
        /// <param name="ctx">The interaction state for this frame.</param>
        public override void Render(InteractionContext ctx) {
            _context = ctx;
            _hasContext = true;

            var origin = ctx.raycastOriginTransform;
            var nearby = ctx.nearbyInteractives;
            var maxDist = ctx.interactionRadius;

            _otherInteractives.Clear();

            if (origin == null) return;

            foreach (var interactive in nearby) {
                if (interactive == null) continue;
                if (interactive == ctx.selectedInteractive) continue;
                if (!HasLineOfSight(origin, interactive, maxDist)) continue;
                _otherInteractives.Add(interactive);
            }
        }

        private void OnGUI() {
            if (!_hasContext) return;

            var mainCamera = Camera.main;
            if (mainCamera == null) return;

            foreach (var interactive in _otherInteractives) {
                DrawIndicator(mainCamera, indicatorIcon, interactive);
            }

            var selected = _context.selectedInteractive;
            if (selected != null && _context.raycastOriginTransform != null) {
                if (HasLineOfSight(_context.raycastOriginTransform, selected, _context.interactionRadius)) {
                    DrawIndicator(mainCamera, indicatorIconSelected, selected);
                }
            }
        }

        private bool HasLineOfSight(Transform origin, IInteractive interactive, float maxDistance) {
            var mb = interactive as MonoBehaviour;
            if (!mb) return false;

            var interactiveTransform = mb.transform;

            var col = mb.GetComponent<Collider>();
            var losPoint = col ? col.bounds.center : interactiveTransform.position;

            var originPos = origin.position;
            var dir = (losPoint - originPos).normalized;
            var dist = Mathf.Min(maxDistance, Vector3.Distance(originPos, losPoint));

            var hits = Physics.RaycastAll(originPos, dir, dist);
            if (hits.Length == 0) return true;

            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits) {
                var t = hit.transform;

                if (t == origin || t.IsChildOf(origin) || origin.IsChildOf(t))
                    continue;

                if (t == interactiveTransform ||
                    t.IsChildOf(interactiveTransform) ||
                    interactiveTransform.IsChildOf(t))
                    return true;

                return false;
            }

            return true;
        }

        private void DrawIndicator(Camera mainCamera, Texture2D icon, IInteractive interactive) {
            var mb = interactive as MonoBehaviour;
            if (!mb) return;

            var pivot = mb.GetComponent<InteractionIndicatorPivot>();
            var offset = pivot ? pivot.offset : Vector3.zero;

            var worldPos = mb.transform.TransformPoint(offset);
            var screenPos = mainCamera.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0f) return;

            var scaledSize = Screen.width * indicatorSize / ReferenceScreenWidth;

            GUI.color = indicatorColor;
            GUI.depth = 0;

            GUI.DrawTexture(
                new Rect(
                    screenPos.x - scaledSize * 0.5f,
                    Screen.height - screenPos.y - scaledSize * 0.5f,
                    scaledSize,
                    scaledSize
                ),
                icon
            );
        }
    }
}