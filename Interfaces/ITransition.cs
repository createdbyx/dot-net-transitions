namespace Codefarts.Transitions
{
    using System;

    public interface ITransition
    {
        Type ValueType { get; }
        /// <summary>
        /// Event raised when the transition hass completed.
        /// </summary>
        event EventHandler TransitionCompletedEvent;

        /// <summary>
        /// Starts the transition.
        /// </summary>
        void Run();

        /// <summary>
        /// Called when the transition timer ticks.
        /// </summary>
        /// <param name="currentTime">The current time in milliseconds.</param>
        void OnTimer(int currentTime);
        bool Enabled { get; set; }
    }
}