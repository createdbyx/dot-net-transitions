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
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    public partial class TransitionManager
    {
        public Func<int> GetTimeCallback { get; set; }

        private long startTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionManager"/> class.
        /// </summary>
        public TransitionManager()
        {
            this.startTime = DateTime.Now.Ticks;
        }

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
                    // setup game object that will be used for updating transitions
                    var container = CodefartsContainerObject.Instance;
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
        public void Register(ITransition transition)
        {
            lock (this.lockObject)
            {
                 // We add the transition to the collection we manage, and 
                // observe it so that we know when it has completed.
                var type = transition.ValueType;
                IList<ITransition> list;
                if (!this.transitionsCache.ContainsKey(type))
                {
                    list = new List<ITransition>();
                    this.transitionsCache[type] = list;
                }
                else
                {
                    list = this.transitionsCache[type];
                }

                if (list.Contains(transition))
                {
                    return;
                }

                list.Add(transition);
                transition.Reset(this.GetTime());
                transition.TransitionCompletedEvent += this.OnTransitionCompleted;
            }
        }

        #endregion

        #region Private functions
           
        /// <summary>
        /// Updates the transitions in the transitions cache by calling there OnTimer method.
        /// </summary>
        public void UpdateTransitions()
        {
            ITransition[] listTransitions;
            lock (this.lockObject)
            {
                // We get all enabled tansitions
                listTransitions = this.transitionsCache.SelectMany(x => x.Value.Where(y => y.Enabled)).ToArray();
            }

            // We tick the timer for each transition we're managing.
            var currentTime = this.GetTime();
            foreach (var transition in listTransitions)
            {
                transition.OnTimer(currentTime);
            }
        }

        /// <summary>
        /// Gets the time in milliseconds.
        /// </summary>
        /// <returns>Returns the time in milliseconds.</returns>
        public int GetTime()
        {
            var timeCallback = this.GetTimeCallback;
            if (timeCallback != null)
            {
                return timeCallback();
            }

            return (int)TimeSpan.FromTicks(DateTime.Now.Ticks - this.startTime).TotalMilliseconds;
        }

        /// <summary>
        /// Called when a transition has completed. 
        /// </summary>
        private void OnTransitionCompleted(object sender, EventArgs e)
        {
            // We stop observing the transition...
            var transition = (ITransition)sender;
            transition.TransitionCompletedEvent -= this.OnTransitionCompleted;
            var type = transition.ValueType;

            // We remove the transition from the collection we're managing...
            lock (this.lockObject)
            {
                if (this.transitionsCache.ContainsKey(type))
                {
                    var list = this.transitionsCache[type];
                    list.Remove(transition);
                }
            }
        }

        #endregion

        #region Private data

        // The singleton instance...                                                                                                             
        private static TransitionManager singletonInstance;

        //// The collection of transitions we're managing. (This should really be a set.)
        //private IDictionary<Transition, bool> transitionsCache = new Dictionary<Transition, bool>();
        // The collection of transitions we're managing. (This should really be a set.)
        //private IList<Transition> transitionsCache = new List<Transition>();
        private IDictionary<Type, IList<ITransition>> transitionsCache = new Dictionary<Type, IList<ITransition>>();

        // An object to lock on. This class can be accessed by multiple threads: the 
        // user thread can add new transitions; and the timerr thread can be animating 
        // them. As they access the same collections, the methods need to be protected 
        // by a lock...
        private object lockObject = new object();

        #endregion
    }
}


