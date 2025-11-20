namespace MoodyLib.Interactions {

    /// <summary>
    /// Interface for objects that can perform a trigger action,
    /// either invoked manually or as part of an interaction sequence.
    /// </summary>
    public interface ITriggerable {

        /// <summary>
        /// Executes the trigger's behavior.
        /// </summary>
        void Trigger();
    }
}