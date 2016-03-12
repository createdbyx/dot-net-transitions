#if !PORTABLE
namespace Codefarts.Transitions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Codefarts.Transitions;
    using Codefarts.Transitions.TransitionTypes;

    public class TransitionHelpers
    {
        private struct SpecialKey
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public SpecialKey(object target, string name)
            {
                this.Target = target;
                this.Name = name;
            }

            object Target;
            string Name;
        }

        private static IDictionary<SpecialKey, ITransition> activeTransitions = new Dictionary<SpecialKey, ITransition>();

        /// <summary>
        /// Adds a property that should be animated as part of this transition.
        /// </summary>
        public static Transition<T> Run<T>(object target, string propertyName, T destinationValue, ITransitionType type, LoopType loopType)
        {
            var key = new SpecialKey(target, propertyName);
            if (activeTransitions.ContainsKey(key))
            {
                return activeTransitions[key] as Transition<T>;
            }

            // We get the property info.
            var targetType = target.GetType();
            var propertyInfo = targetType.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new Exception("Object: " + target + " does not have the property: " + propertyName);
            }

            // We can only transition properties that are both getable and setable.
            if (propertyInfo.CanRead == false || propertyInfo.CanWrite == false)
            {
                throw new Exception("Property is not both getable and setable: " + propertyName);
            }

            // We can only transition properties that are both getable and setable.
            if (propertyInfo.PropertyType != typeof(T))
            {
                throw new Exception("Property type does not match destination value type: " + propertyName);
            }

            var currentValue = (T)propertyInfo.GetValue(target, null);

            var t = Transition<T>.Run(value =>
                {
                    SetProperty(target, new PropertyUpdateArgs(target, propertyInfo, value));
                }, currentValue, destinationValue, new EaseInEaseOut(1000), loopType);

            t.TransitionCompletedEvent += OnTransitionCompletedEvent;
            activeTransitions[key] = t;

            return t;
        }

        private static void OnTransitionCompletedEvent(object s, EventArgs e)
        {
            var item = activeTransitions.FirstOrDefault(x => x.Value == s);
            item.Value.TransitionCompletedEvent -= OnTransitionCompletedEvent;
            activeTransitions.Remove(item.Key);
        }

        /// <summary>
        /// Sets a property on the object passed in to the value passed in. This method
        /// invokes itself on the GUI thread if the property is being invoked on a GUI 
        /// object.
        /// </summary>
        public static void SetProperty(object sender, PropertyUpdateArgs args)
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
                    var asyncResult = invokeTarget.BeginInvoke(new EventHandler<PropertyUpdateArgs>(SetProperty), new object[] { sender, args });
                    asyncResult.AsyncWaitHandle.WaitOne(50);
                }
                else
                {
                    // We are on the correct thread, so we update the property.
                    args.PropertyInfo.SetValue(args.Target, args.Value, null);
                }
            }
            catch (Exception)
            {
                // We silently catch any exceptions. These could be things like 
                // bounds exceptions when setting properties.
            }
        }
    }
}
#endif