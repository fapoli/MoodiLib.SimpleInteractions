using System;
using UnityEngine;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Interface for objects that can be interacted with by an <see cref="Interactor"/>.
    /// </summary>
    public interface IInteractive {

        /// <summary>
        /// Indicates whether this interactive is currently inactive and should be ignored
        /// by the interaction system.
        /// </summary>
        bool isDeactivated { get; }

        /// <summary>
        /// Attempts to execute the interaction using the provided caller.
        /// Behavior depends on the concrete implementation.
        /// </summary>
        /// <param name="caller">The GameObject initiating the interaction.</param>
        void TryInteract(GameObject caller);
    }
}