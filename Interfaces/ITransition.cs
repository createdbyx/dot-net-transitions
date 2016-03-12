namespace Codefarts.Transitions
{
    using System;

    /// <summary>
    /// Provides a interface for interacting with the generic <see cref="Transition{T}"/> class.
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// Gets the type of the generic value used by the <see cref="Transition{T}"/> class.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Event raised when the transition has completed.
        /// </summary>
        event EventHandler TransitionCompletedEvent;

        /// <summary>
        /// Resets the transition.
        /// </summary>
        /// <param name="currentTime">The current time in milliseconds.</param>
        void Reset(int currentTime);

        /// <summary>
        /// Called when the transition timer ticks.
        /// </summary>
        /// <param name="currentTime">The current time in milliseconds.</param>
        void OnTimer(int currentTime);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Transition{T}"/> is enabled.
        /// </summary>
        bool Enabled { get; set; }
    }
}