using System;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Interface for components that define whether an interaction is currently permitted.
    /// Evaluated by <see cref="AbstractInteractive"/> during interaction checks.
    /// </summary>
    public interface ICondition {

        /// <summary>
        /// Indicates whether the condition is currently satisfied.
        /// Interaction is allowed only when this value is true.
        /// </summary>
        bool isConditionMet { get; }

        /// <summary>
        /// Raised whenever the condition's state changes.
        /// Used by dependent systems to react to condition updates.
        /// </summary>
        event Action<ICondition> OnConditionChanged;
    }
}