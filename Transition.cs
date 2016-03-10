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
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ComponentModel;

    using UnityEngine;

    /// <summary>
    /// Lets you perform animated transitions of properties on arbitrary objects. These 
    /// will often be transitions of UI properties, for example an animated fade-in of 
    /// a UI object, or an animated move of a UI object from one position to another.
    /// 
    /// Each transition can simulataneously change multiple properties, including properties
    /// across multiple objects.
    /// 
    /// Example transition
    /// ------------------
    /// a.      Transition t = new Transition(new TransitionMethod_Linear(500));
    /// b.      t.add(form1, "Width", 500);
    /// c.      t.add(form1, "BackColor", Color.Red);
    /// d.      t.run();
    ///   
    /// Line a:         Creates a new transition. You specify the transition method.
    ///                 
    /// Lines b. and c: Set the destination values of the properties you are animating.
    /// 
    /// Line d:         Starts the transition.
    /// 
    /// Transition methods
    /// ------------------
    /// TransitionMethod objects specify how the transition is made. Examples include
    /// linear transition, ease-in-ease-out and so on. Different transition methods may
    /// need different parameters.
    /// 
    /// </summary>
    public partial class Transition
    {
        #region Registration

        /// <summary>
        /// You should register all managed-types here.
        /// </summary>
        static Transition()
        {
            RegisterType(new ManagedType_Int());
            RegisterType(new ManagedType_Float());
            RegisterType(new ManagedType_Double());
            RegisterType(new ManagedType_Color());
            RegisterType(new ManagedType_String());
        }

        #endregion

        #region Events

        ///// <summary>
        ///// Args passed with the TransitionCompletedEvent.
        ///// </summary>
        //public class Args : EventArgs
        //{
        //}

        /// <summary>
        /// Event raised when the transition hass completed.
        /// </summary>
        public event EventHandler TransitionCompletedEvent;

        /// <summary>
        /// Gets or sets the type of the loop.
        /// </summary>
        public LoopType LoopType { get; set; }

        #endregion

        #region Public static methods

        /// <summary>
        /// Creates and immediately runs a transition on the property passed in.
        /// </summary>
        public static Transition Run(object target, string strPropertyName, object destinationValue, ITransitionType transitionMethod)
        {
            return Run(target, strPropertyName, destinationValue, transitionMethod, LoopType.None);
        }


        /// <summary>
        /// Creates and immediately runs a transition on the property passed in.
        /// </summary>
        public static Transition Run(object target, string strPropertyName, object destinationValue, ITransitionType transitionMethod, LoopType type)
        {
            var t = new Transition(transitionMethod) { LoopType = type };
            t.Add(target, strPropertyName, destinationValue);
            t.Run();
            return t;
        }

        /// <summary>
        /// Sets the property passed in to the initial value passed in, then creates and 
        /// immediately runs a transition on it.
        /// </summary>
        public static Transition Run(object target, string strPropertyName, object initialValue, object destinationValue, ITransitionType transitionMethod)
        {
            Utility.SetValue(target, strPropertyName, initialValue);
            return Run(target, strPropertyName, destinationValue, transitionMethod);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor. You pass in the object that holds the properties 
        /// that you are performing transitions on.
        /// </summary>
        public Transition(ITransitionType transitionMethod)
        {
            this.transitionMethod = transitionMethod;
        }

        /// <summary>
        /// Adds a property that should be animated as part of this transition.
        /// </summary>
        public void Add(object target, string propertyName, object destinationValue)
        {
            // We get the property info...
            var targetType = target.GetType();
            var propertyInfo = targetType.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new Exception("Object: " + target + " does not have the property: " + propertyName);
            }

            // We check that we support the property type...
            var propertyType = propertyInfo.PropertyType;
            if (managedTypes.ContainsKey(propertyType) == false)
            {
                throw new Exception("Transition does not handle properties of type: " + propertyType);
            }

            // We can only transition properties that are both getable and setable...
            if (propertyInfo.CanRead == false || propertyInfo.CanWrite == false)
            {
                throw new Exception("Property is not both getable and setable: " + propertyName);
            }

            var managedType = managedTypes[propertyType];

            // We can manage this type, so we store the information for the
            // transition of this property...
            var info = new TransitionedPropertyInfo();
            info.endValue = destinationValue;
            info.target = target;
            info.propertyInfo = propertyInfo;
            info.managedType = managedType;

            lock (this.lockObject)
            {
                this.transitionedPropertiesList.Add(info);
            }
        }

        /// <summary>
        /// Starts the transition.
        /// </summary>
        public void Run()
        {
            this.InternalRun(true);
        }

        private void InternalRun(bool regster)
        {
            // We find the current start values for the properties we 
            // are animating...
            foreach (var info in this.transitionedPropertiesList)
            {
                var value = info.propertyInfo.GetValue(info.target, null);
                if (regster)
                {
                    info.originalValue = info.managedType.Copy(value);
                }

                info.startValue = info.originalValue;
            }

            // We start set the start time. We use this when the timer ticks to measure 
            // how long the transition has been runnning for...
            var manager = TransitionManager.Instance;
            this.startTime = manager.GetTime();

            // We register this transition with the transition manager...
            if (regster)
            {
                manager.Register(this);
            }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Property that returns a list of information about each property managed
        /// by this transition.
        /// </summary>
        internal IList<TransitionedPropertyInfo> TransitionedProperties
        {
            get { return this.transitionedPropertiesList; }
        }

        /// <summary>
        /// We remove the property with the info passed in from the transition.
        /// </summary>
        internal void RemoveProperty(TransitionedPropertyInfo info)
        {
            lock (this.lockObject)
            {
                this.transitionedPropertiesList.Remove(info);
            }
        }

        /// <summary>
        /// Called when the transition timer ticks.
        /// </summary>
        /// <param name="currentTime">The current time in milliseconds.</param>
        public void OnTimer(int currentTime)
        {
            // When the timer ticks we:
            // a. Find the elapsed time since the transition started.
            // b. Work out the percentage movement for the properties we're managing.
            // c. Find the actual values of each property, and set them.

            // a.
            var elapsedTime = currentTime - this.startTime;

            // b.
            double transitionPercentage;
            var isCompleted = this.transitionMethod.OnTimer(elapsedTime, out transitionPercentage);

            // We take a copy of the list of properties we are transitioning, as
            // they can be changed by another thread while this method is running...
            var listTransitionedProperties = new List<TransitionedPropertyInfo>();
            lock (this.lockObject)
            {
                foreach (var info in this.transitionedPropertiesList)
                {
                    listTransitionedProperties.Add(info.Copy());
                }
            }

            // c. 
            foreach (var info in listTransitionedProperties)
            {
                // We get the current value for this property...
                var value = info.managedType.GetIntermediateValue(info.startValue, info.endValue, transitionPercentage);

                // We set it...
                var args = new PropertyUpdateArgs(info.target, info.propertyInfo, value);
                this.SetProperty(this, args);
            }

            // Has the transition completed?
            if (isCompleted)
            {
                switch (this.LoopType)
                {
                    case LoopType.None:
                        // We raise an event to notify any observers that the transition has completed...
                        Utility.RaiseEvent(this.TransitionCompletedEvent, this, EventArgs.Empty);
                        break;

                    case LoopType.Loop:
                        Debug.Log("Looped");
                        this.InternalRun(false);
                        break;

                    case LoopType.PingPong:
                        Debug.Log("PingPong");
                        // if we are ping ponging swap start and end values
                        foreach (var info in this.transitionedPropertiesList)
                        {
                            var start = info.startValue;
                            info.startValue = info.endValue;
                            info.endValue = start;
                        }

                        // Reset the start time. We use this when the timer ticks to measure 
                        // how long the transition has been runnning for...
                        var manager = TransitionManager.Instance;
                        this.startTime = manager.GetTime();

                        break;
                }
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Sets a property on the object passed in to the value passed in. This method
        /// invokes itself on the GUI thread if the property is being invoked on a GUI 
        /// object.
        /// </summary>
        private void SetProperty(object sender, PropertyUpdateArgs args)
        {
            try
            {
#if WINDOWS
    // If the target is a control that has been disposed then we don't 
    // try to update its properties. This can happen if the control is
    // on a form that has been closed while the transition is running...
                if (this.isDisposed(args.target))
                {
                    return;
                } 
#endif

                var invokeTarget = args.Target as ISynchronizeInvoke;
                if (invokeTarget != null && invokeTarget.InvokeRequired)
                {
                    // There is some history behind the next two lines, which is worth
                    // going through to understand why they are the way they are.

                    // Initially we used BeginInvoke without the subsequent WaitOne for
                    // the result. A transition could involve a large number of updates
                    // to a property, and as this call was asynchronous it would send a 
                    // large number of updates to the UI thread. These would queue up at
                    // the GUI thread and mean that the UI could be some way behind where
                    // the transition was.

                    // The line was then changed to the blocking Invoke call instead. This 
                    // meant that the transition only proceded at the pace that the GUI 
                    // could process it, and the UI was not overloaded with "old" updates.

                    // However, in some circumstances Invoke could block and lock up the
                    // Transitions background thread. In particular, this can happen if the
                    // control that we are trying to update is in the process of being 
                    // disposed - for example, it is on a form that is being closed. See
                    // here for details: 
                    // http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/7d2c941a-0016-431a-abba-67c5d5dac6a5

                    // To solve this, we use a combination of the two earlier approaches. 
                    // We use BeginInvoke as this does not block and lock up, even if the
                    // underlying object is being disposed. But we do want to wait to give
                    // the UI a chance to process the update. So what we do is to do the
                    // asynchronous BeginInvoke, but then wait (with a short timeout) for
                    // it to complete.
                    var asyncResult = invokeTarget.BeginInvoke(new EventHandler<PropertyUpdateArgs>(this.SetProperty), new object[] { sender, args });
                    asyncResult.AsyncWaitHandle.WaitOne(50);
                }
                else
                {
                    // We are on the correct thread, so we update the property...
                    args.PropertyInfo.SetValue(args.Target, args.Value, null);
                }
            }
            catch (Exception)
            {
                // We silently catch any exceptions. These could be things like 
                // bounds exceptions when setting properties.
            }
        }

        #endregion

        #region Private static functions

        /// <summary>
        /// Registers a transition-type. We hold them in a dictionary.
        /// </summary>
        public static void RegisterType(IManagedType transitionType)
        {
            var type = transitionType.GetManagedType();
            managedTypes[type] = transitionType;
        }

        #endregion

        #region Private static data

        // A map of Type info to IManagedType objects. These are all the types that we
        // know how to perform transitions on...
        private static IDictionary<Type, IManagedType> managedTypes = new Dictionary<Type, IManagedType>();

        #endregion

        #region Private data

        // The transition method used by this transition...
        private ITransitionType transitionMethod;

        // Holds information about one property on one taregt object that we are performing
        // a transition on...
        internal class TransitionedPropertyInfo
        {
            public object startValue;
            public object originalValue;

            public object endValue;

            public object target;

            public PropertyInfo propertyInfo;

            public IManagedType managedType;

            public TransitionedPropertyInfo Copy()
            {
                var info = new TransitionedPropertyInfo();
                info.startValue = this.startValue;
                info.originalValue = this.originalValue;
                info.endValue = this.endValue;
                info.target = this.target;
                info.propertyInfo = this.propertyInfo;
                info.managedType = this.managedType;
                return info;
            }
        }

        // The collection of properties that the current transition is animating...
        private IList<TransitionedPropertyInfo> transitionedPropertiesList = new List<TransitionedPropertyInfo>();

        // Event args used for the event we raise when updating a property...
        private class PropertyUpdateArgs : EventArgs
        {
            public PropertyUpdateArgs(object targetObject, PropertyInfo propertyInfo, object value)
            {
                this.Target = targetObject;
                this.PropertyInfo = propertyInfo;
                this.Value = value;
            }

            public object Target { get; private set; }

            public PropertyInfo PropertyInfo { get; private set; }

            public object Value { get; private set; }
        }

        // An object used to lock the list of transitioned properties, as it can be 
        // accessed by multiple threads...
        private object lockObject = new object();

        /// <summary>
        /// The start time in milliseconds that this transition started running.
        /// </summary>
        private int startTime;

        #endregion
    }
}
