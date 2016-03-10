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
    using System.ComponentModel;

    /// <summary>
    /// A class holding static utility functions.
    /// </summary>
    internal class Utility
    {
        /// <summary>
        /// Returns the value of the property passed in.
        /// </summary>
        public static object GetValue(object target, string strPropertyName)
        {
            var targetType = target.GetType();
            var propertyInfo = targetType.GetProperty(strPropertyName);
            if (propertyInfo == null)
            {
                throw new Exception("Object: " + target + " does not have the property: " + strPropertyName);
            }

            return propertyInfo.GetValue(target, null);
        }

        /// <summary>
        /// Sets the value of the property passed in.
        /// </summary>
        public static void SetValue(object target, string strPropertyName, object value)
        {
            var targetType = target.GetType();
            var propertyInfo = targetType.GetProperty(strPropertyName);
            if (propertyInfo == null)
            {
                throw new Exception("Object: " + target + " does not have the property: " + strPropertyName);
            }

            propertyInfo.SetValue(target, value, null);
        }

        /// <summary>
        /// Returns a value between valueA and valueB for the percentage passed in.
        /// </summary>
        public static double Interpolate(double valueA, double valueB, double percentage)
        {
            var dDifference = valueB - valueA;
            var dDistance = dDifference * percentage;
            var dResult = valueA + dDistance;
            return dResult;
        }

        /// <summary>
        /// Returns a value betweeen valueA and valueB for the percentage passed in.
        /// </summary>
        public static int Interpolate(int valueA, int valueB, double percentage)
        {
            return (int)Interpolate((double)valueA, (double)valueB, percentage);
        }

        /// <summary>
        /// Returns a value betweeen valueA and valueB for the percentage passed in.
        /// </summary>
        public static float Interpolate(float valueA, float valueB, double percentage)
        {
            return (float)Interpolate((double)valueA, (double)valueB, percentage);
        }

        /// <summary>
        /// Converts a fraction representing linear time to a fraction representing
        /// the distance traveled under an ease-in-ease-out transition.
        /// </summary>
        public static double ConvertLinearToEaseInEaseOut(double elapsed)
        {
            // The distance traveled is made up of two parts: the initial acceleration,
            // and then the subsequent deceleration...
            var dFirstHalfTime = (elapsed > 0.5) ? 0.5 : elapsed;
            var dSecondHalfTime = (elapsed > 0.5) ? elapsed - 0.5 : 0.0;
            var dResult = 2 * dFirstHalfTime * dFirstHalfTime + 2 * dSecondHalfTime * (1.0 - dSecondHalfTime);
            return dResult;
        }

        /// <summary>
        /// Converts a fraction representing linear time to a fraction representing
        /// the distance traveled under a constant acceleration transition.
        /// </summary>
        public static double ConvertLinearToAcceleration(double elapsed)
        {
            return elapsed * elapsed;
        }

        /// <summary>
        /// Converts a fraction representing linear time to a fraction representing
        /// the distance traveled under a constant deceleration transition.
        /// </summary>
        public static double ConvertLinearToDeceleration(double elapsed)
        {
            return elapsed * (2.0 - elapsed);
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
        public static void RaiseEvent(EventHandler theEvent, object sender, EventArgs args)
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
                }
                catch (Exception)
                {
                    // The event handler may have been detached while processing the events.
                    // We just ignore this and invoke the remaining handlers.
                }
            }
        }

    }
}
