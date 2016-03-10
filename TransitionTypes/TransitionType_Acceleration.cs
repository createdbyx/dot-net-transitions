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

    /// <summary>
    /// Manages transitions under constant acceleration from a standing start.
    /// </summary>
    public class TransitionType_Acceleration : ITransitionType
    {
        #region Public methods

        /// <summary>
        /// Constructor. You pass in the time that the transition 
        /// will take (in milliseconds).
        /// </summary>
        public TransitionType_Acceleration(int timeSpan)
        {
            if (timeSpan <= 0)
            {
                throw new Exception("Transition time must be greater than zero.");
            }

            this.transitionTime = timeSpan;
        }

        #endregion

        #region ITransitionMethod Members

        /// <summary>
        /// Works out the percentage completed given the time passed in.
        /// This uses the formula:
        ///   s = ut + 1/2at^2
        /// The initial velocity is 0, and the acceleration to get to 1.0
        /// at t=1.0 is 2, so the formula just becomes:
        ///   s = t^2
        /// </summary>
        public bool OnTimer(int iTime, out double completionPercentage)
        {
            // We find the percentage time elapsed...
            var dElapsed = iTime / this.transitionTime;
            completionPercentage = dElapsed * dElapsed;
            if (dElapsed >= 1.0)
            {
                completionPercentage = 1.0;
                return true;
            }

            return false;
        }

        #endregion

        #region Private data

        private double transitionTime = 0.0;

        #endregion
    }
}
