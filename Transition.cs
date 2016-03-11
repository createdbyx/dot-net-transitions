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
    using System.ComponentModel;

    /// <summary>
    /// Lets you perform animated transitions on arbitrary objects.
    /// </summary>
    /// <remarks>
    /// <p>Lets you perform animated transitions on arbitrary objects. These 
    /// will often be transitions of UI properties, for example an animated fade-in of 
    /// a UI object, or an animated move of a UI object from one position to another.</p>
    /// 
    /// <p>Each transition can simulataneously change multiple properties, including properties
    /// across multiple objects.</p>
    /// 
    /// <pre>Example transition
    /// ------------------
    /// a.      Transition t = new Transition(new TransitionMethod_Linear(500));
    /// b.      t.add(form1, "Width", 500);
    /// c.      t.add(form1, "BackColor", Color.Red);
    /// d.      t.run();
    /// </pre>
    /// <p>Line a:         Creates a new transition. You specify the transition method.
    ///                 
    /// Lines b. and c: Set the destination values of the properties you are animating.
    /// 
    /// Line d:         Starts the transition.</p>
    /// 
    /// <p>Transition methods
    /// ------------------
    /// TransitionMethod objects specify how the transition is made. Examples include
    /// linear transition, ease-in-ease-out and so on. Different transition methods may
    /// need different parameters.                </p> 
    /// </remarks>
    public partial class Transition<T> : ITransition
    {
        /// <summary>
        /// Gets the type of the generic value used by the <see cref="Transition{T}" /> class.
        /// </summary>
        public Type ValueType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Event raised when the transition has completed.
        /// </summary>
        public event EventHandler TransitionCompletedEvent;

        /// <summary>
        /// Gets or sets the type of the loop.
        /// </summary>
        public LoopType LoopType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Transition{T}"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        #region Public static methods

        /// <summary>
        /// Gets or sets the callback responsible for setting the value.
        /// </summary>
        public Action<T> SetValueCallback { get; set; }

        /// <summary>
        /// Gets or sets the interpolate callback responsible for interpolating across values.
        /// </summary>
        public static Func<T, T, double, T> InterpolateCallback { get; set; }

        /// <summary>
        /// Creates and immediately runs a transition on the property passed in.
        /// </summary>
        public static Transition<T> Run(Action<T> setValue, T startValue, T destinationValue, ITransitionType transitionMethod)
        {
            return Run(setValue, startValue, destinationValue, transitionMethod, LoopType.None);
        }

        /// <summary>
        /// Creates and immediately runs a transition on the property passed in.
        /// </summary>
        public static Transition<T> Run(Action<T> setValue, T startValue, T destinationValue, ITransitionType transitionMethod, LoopType type)
        {
            var t = new Transition<T>(transitionMethod);
            t.LoopType = type;
            t.OriginalValue = startValue;
            t.StartValue = startValue;
            t.CurrentValue = startValue;
            t.SetValueCallback = setValue;
            t.EndValue = destinationValue;
            TransitionManager.Instance.Register(t);
            return t;
        }

        /// <summary>
        /// Creates and immediately runs a transition on the property passed in.
        /// </summary>
        public static Transition<T> Run(Action<T> setValue, T destinationValue, ITransitionType transitionMethod)
        {
            return Run(setValue, destinationValue, transitionMethod, LoopType.None);
        }

        /// <summary>
        /// Creates and immediately runs a transition on the property passed in.
        /// </summary>
        public static Transition<T> Run(Action<T> setValue, T destinationValue, ITransitionType transitionMethod, LoopType type)
        {
            var t = new Transition<T>(transitionMethod) { LoopType = type, SetValueCallback = setValue, EndValue = destinationValue };
            TransitionManager.Instance.Register(t);
            return t;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Transition{T}"/> class.
        /// </summary>
        public Transition()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Constructor. You pass in the object that holds the properties 
        /// that you are performing transitions on.
        /// </summary>
        public Transition(ITransitionType transitionMethod) : this()
        {
            this.TransitionMethod = transitionMethod;
        }

        /// <summary>
        /// Resets the transition.
        /// </summary>
        /// <param name="currentTime">The current time in milliseconds.</param>
        public void Reset(int currentTime)
        {
            // We set the start value to the original value.
            this.StartValue = this.OriginalValue;

            // We start set the start time. We use this when the timer ticks to measure 
            // how long the transition has been runnning for.
            this.startTime = currentTime;
        }

        #endregion

        #region Internal methods

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

            // if we are not enabled add the elapsed time to the start time and exit.
            // we do this to ensure that if and when we are re-enabled the transition continues
            // seemlessly without suddenly changing due to the large time disparity.
            if (!this.Enabled)
            {
                this.startTime += elapsedTime;
                return;
            }

            // b.
            var transitionPercentage = 0d;
            var transitionMethod = this.TransitionMethod;

            // if no transition method available isCompleted will be false
            var isCompleted = transitionMethod == null ? false : transitionMethod.OnTimer(elapsedTime, out transitionPercentage);

            // We interpolate the current value usiong the interpolation callback.     
            var interpolateCallback = InterpolateCallback;
            if (interpolateCallback != null)
            {
                var value = interpolateCallback(this.StartValue, this.EndValue, transitionPercentage);
                this.CurrentValue = value;
            }

            // call the set value callback is present
            var valueCallback = this.SetValueCallback;
            if (valueCallback != null)
            {
                valueCallback(this.CurrentValue);
            }

            // Has the transition completed?
            if (!isCompleted)
            {
                return;
            }

            // if not yet completed check loop type and take appropreate steps
            switch (this.LoopType)
            {
                case LoopType.None:
                    // We raise an event to notify any observers that the transition has completed.
                    this.RaiseEvent(this.TransitionCompletedEvent, this, EventArgs.Empty);
                    break;

                case LoopType.Loop:
                    this.Reset(currentTime);
                    break;

                case LoopType.PingPong:
                    // if we are ping ponging swap start and end values
                    var start = this.StartValue;
                    this.StartValue = this.EndValue;
                    this.EndValue = start;

                    // Reset the start time. We use this when the timer ticks to measure 
                    // how long the transition has been runnning for...
                    this.startTime = currentTime;
                    break;
            }
        }

        /// <summary>
        /// Fires the event passed in in a thread-safe way. 
        /// </summary><remarks>
        /// This method loops through the targets of the event and invokes each in turn. If the
        /// target supports ISychronizeInvoke (such as forms or controls) and is set to run 
        /// on a different thread, then we call BeginInvoke to marshal the event to the target
        /// thread. If the target does not support this interface (such as most non-form classes)
        /// or we are on the same thread as the target, then the event is fired on the same
        /// thread as this is called from.
        /// </remarks>
        private void RaiseEvent(EventHandler theEvent, object sender, EventArgs args)
        {
            // Is the event set up?
            if (theEvent == null)
            {
                return;
            }

            // We loop through each of the delegate handlers for this event. For each of 
            // them we need to decide whether to invoke it on the current thread or to
            // make a cross-thread invocation...
            foreach (EventHandler handler in theEvent.GetInvocationList())
            {
                try
                {
#if PORTABLE
                    // call the handler
                    handler(sender, args);
#else
                    var target = handler.Target as ISynchronizeInvoke;
                    if (target == null || target.InvokeRequired == false)
                    {
                        // Either the target is not a form or control, or we are already
                        // on the right thread for it. Either way we can just fire the
                        // event as normal...
                        handler(sender, args);
                    }
                    else
                    {
                        // The target is most likely a form or control that needs the
                        // handler to be invoked on its own thread...
                        target.BeginInvoke(handler, new object[] { sender, args });
                    }
#endif
                }
                catch (Exception)
                {
                    // The event handler may have been detached while processing the events.
                    // We just ignore this and invoke the remaining handlers.
                }
            }
        }

        #endregion

        /// <summary>
        /// The transition method used by this transition.
        /// </summary>                                               
        public ITransitionType TransitionMethod { get; set; }

        /// <summary>
        /// Gets or sets the targeted start value.
        /// </summary>
        public T StartValue { get; set; }

        /// <summary>
        /// Gets or sets the original value.
        /// </summary>
        public T OriginalValue { get; set; }

        /// <summary>
        /// Gets or sets the current transition value.
        /// </summary>
        public T CurrentValue { get; set; }

        /// <summary>
        /// Gets or sets the targeted end value.
        /// </summary>
        public T EndValue { get; set; }

        /// <summary>
        /// The start time in milliseconds that this transition started running.
        /// </summary>
        private int startTime;
    }
}
