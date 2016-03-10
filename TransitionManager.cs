/*
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

#if UNITY_5
    using Codefarts.Core;
    using UnityEngine;
#endif

    /// <summary>
    /// This class is responsible for running transitions. It holds the timer that
    /// triggers transition animation. 
    /// </summary><remarks>
    /// This class is a singleton.
    /// 
    /// We manage the transition timer here so that we can have a single timer
    /// across all transitions. If each transition has its own timer, this creates
    /// one thread for each transition, and this can lead to too many threads in
    /// an application.
    /// 
    /// This class essentially just manages the timer for the transitions. It calls 
    /// back into the running transitions, which do the actual work of the transition.
    /// 
    /// </remarks>
    internal partial class TransitionManager
    {
        #region Public methods

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static TransitionManager Instance
        {
            get
            {
                if (singletonInstance == null)
                {
#if UNITY_5
                    var container = CodefartsContainerObject.Instance;
                    // setup game object that will be used for updating input
                    var newGameObj = new GameObject("Codefarts.TransitionManager");
                    newGameObj.transform.parent = container.GameObject.transform;
                    singletonInstance = newGameObj.AddComponent<TransitionManager>();
#else
                    singletonInstance = new TransitionManager();
#endif
                }


                return singletonInstance;
            }
        }

        /// <summary>
        /// You register a transition with the manager here. This will start to run
        /// the transition as the manager's timer ticks.
        /// </summary>
        public void Register(Transition transition)
        {
            lock (this.lockObject)
            {
                // We check to see if the properties of this transition
                // are already being animated by any existing transitions...
                this.RemoveDuplicates(transition);

                // We add the transition to the collection we manage, and 
                // observe it so that we know when it has completed...
                this.transitionsCache[transition] = true;
                transition.TransitionCompletedEvent += this.OnTransitionCompleted;
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Checks if any existing transitions are acting on the same properties as the
        /// transition passed in. If so, we remove the duplicated properties from the 
        /// older transitions.
        /// </summary>
        private void RemoveDuplicates(Transition transition)
        {
            // We look through the set of transitions we're currently managing...
            foreach (var pair in this.transitionsCache)
            {
                this.RemoveDuplicates(transition, pair.Key);
            }
        }

        /// <summary>
        /// Finds any properties in the old-transition that are also in the new one,
        /// and removes them from the old one.
        /// </summary>
        private void RemoveDuplicates(Transition newTransition, Transition oldTransition)
        {
            // Note: This checking might be a bit more efficient if it did the checking
            //       with a set rather than looking through lists. That said, it is only done 
            //       when transitions are added (which isn't very often) rather than on the
            //       timer, so I don't think this matters too much.

            // We get the list of properties for the old and new transitions...
            var newProperties = newTransition.TransitionedProperties;
            var oldProperties = oldTransition.TransitionedProperties;

            // We loop through the old properties backwards (as we may be removing 
            // items from the list if we find a match)...
            for (var i = oldProperties.Count - 1; i >= 0; i--)
            {
                // We get one of the properties from the old transition...
                var oldProperty = oldProperties[i];

                // Is this property part of the new transition?
                foreach (var newProperty in newProperties)
                {
                    if (oldProperty.target == newProperty.target && oldProperty.propertyInfo == newProperty.propertyInfo)
                    {
                        // The old transition contains the same property as the new one,
                        // so we remove it from the old transition...
                        oldTransition.RemoveProperty(oldProperty);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the transitions in the transitions cache by calling there OnTimer method.
        /// </summary>
        private void UpdateTransitions()
        {
            IList<Transition> listTransitions;
            lock (this.lockObject)
            {
                // We take a copy of the collection of transitions as elements 
                // might be removed as we iterate through it...
                listTransitions = new List<Transition>();
                foreach (var pair in this.transitionsCache)
                {
                    listTransitions.Add(pair.Key);
                }
            }

            // We tick the timer for each transition we're managing...
            foreach (var transition in listTransitions)
            {
                transition.OnTimer();
            }
        }

        /// <summary>
        /// Called when a transition has completed. 
        /// </summary>
        private void OnTransitionCompleted(object sender, Transition.Args e)
        {
            // We stop observing the transition...
            var transition = (Transition)sender;
            transition.TransitionCompletedEvent -= this.OnTransitionCompleted;

            // We remove the transition from the collection we're managing...
            lock (this.lockObject)
            {
                this.transitionsCache.Remove(transition);
            }
        }

        #endregion

        #region Private data

        // The singleton instance...
        private static TransitionManager singletonInstance;

        // The collection of transitions we're managing. (This should really be a set.)
        private IDictionary<Transition, bool> transitionsCache = new Dictionary<Transition, bool>();

        // An object to lock on. This class can be accessed by multiple threads: the 
        // user thread can add new transitions; and the timerr thread can be animating 
        // them. As they access the same collections, the methods need to be protected 
        // by a lock...
        private object lockObject = new object();

        #endregion
    }
}


