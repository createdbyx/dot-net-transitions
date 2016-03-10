﻿/*
Copyright (c) 2016 Codefarts
contact@codefarts.com
http://www.codefarts.com
Now hosted here: https://github.com/UweKeim/dot-net-transitions
Originally located here: https://code.google.com/archive/p/dot-net-transitions/
---------------------------

The MIT License (MIT)

Copyright (c) 2009 Richard S. Shepherd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace Codefarts.Transitions
{
    using System.Collections.Generic;

    internal class TransitionChain
    {
        #region Public methods

        public TransitionChain(params Transition[] transitions)
        {
            // We store the list of transitions...
            foreach (var transition in transitions)
            {
                this.transitionsList.AddLast(transition);
            }

            // We start running them...
            this.RunNextTransition();
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Runs the next transition in the list.
        /// </summary>
        private void RunNextTransition()
        {
            if (this.transitionsList.Count == 0)
            {
                return;
            }

            // We find the next transition and run it. We also register
            // for its completed event, so that we can start the next transition
            // when this one completes...
            var nextTransition = this.transitionsList.First.Value;
            nextTransition.TransitionCompletedEvent += this.OnTransitionCompleted;
            nextTransition.Run();
        }

        /// <summary>
        /// Called when the transition we have just run has completed.
        /// </summary>
        private void OnTransitionCompleted(object sender, Transition.Args e)
        {
            // We unregister from the completed event...
            var transition = (Transition)sender;
            transition.TransitionCompletedEvent -= this.OnTransitionCompleted;

            // We remove the completed transition from our collection, and
            // run the next one...
            this.transitionsList.RemoveFirst();
            this.RunNextTransition();
        }

        #endregion

        #region Private data

        // The list of transitions in the chain...
        private LinkedList<Transition> transitionsList = new LinkedList<Transition>();

        #endregion
    }
}
