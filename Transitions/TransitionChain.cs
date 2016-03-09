using System;
using System.Collections.Generic;
using System.Text;

namespace Transitions
{
    internal class TransitionChain
    {
        #region Public methods

        public TransitionChain(params Transition[] transitions)
        {
            // We store the list of transitions...
            foreach (var transition in transitions)
            {
                this.m_listTransitions.AddLast(transition);
            }

            // We start running them...
            this.runNextTransition();
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Runs the next transition in the list.
        /// </summary>
        private void runNextTransition()
        {
            if (this.m_listTransitions.Count == 0)
            {
                return;
            }

            // We find the next transition and run it. We also register
            // for its completed event, so that we can start the next transition
            // when this one completes...
            var nextTransition = this.m_listTransitions.First.Value;
            nextTransition.TransitionCompletedEvent += this.onTransitionCompleted;
            nextTransition.Run();
        }

        /// <summary>
        /// Called when the transition we have just run has completed.
        /// </summary>
        private void onTransitionCompleted(object sender, Transition.Args e)
        {
            // We unregister from the completed event...
            var transition = (Transition)sender;
            transition.TransitionCompletedEvent -= this.onTransitionCompleted;

            // We remove the completed transition from our collection, and
            // run the next one...
            this.m_listTransitions.RemoveFirst();
            this.runNextTransition();
        }

        #endregion

        #region Private data

        // The list of transitions in the chain...
        private LinkedList<Transition> m_listTransitions = new LinkedList<Transition>();

        #endregion
    }
}
