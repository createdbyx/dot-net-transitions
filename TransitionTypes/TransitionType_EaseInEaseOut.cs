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
    /// Manages an ease-in-ease-out transition. This accelerates during the first 
    /// half of the transition, and then decelerates during the second half.
    /// </summary>
    public class TransitionType_EaseInEaseOut : ITransitionType
    {
        public bool Loop { get; set; }

        #region Public methods

        /// <summary>
        /// Constructor. You pass in the time that the transition 
        /// will take (in milliseconds).
        /// </summary>
        public TransitionType_EaseInEaseOut(int iTransitionTime)
        {
            if (iTransitionTime <= 0)
            {
                throw new Exception("Transition time must be greater than zero.");
            }

            this.transitionTime = iTransitionTime;
        }

        #endregion

        #region ITransitionMethod Members

        /// <summary>
        /// Gets or sets the type of the loop.
        /// </summary>
        public LoopType LoopType { get; set; }

        /// <summary>
        /// Works out the percentage completed given the time passed in.
        /// This uses the formula:
        ///   s = ut + 1/2at^2
        /// We accelerate as at the rate needed (a=4) to get to 0.5 at t=0.5, and
        /// then decelerate at the same rate to end up at 1.0 at t=1.0.
        /// </summary>
        public void OnTimer(int iTime, out double dPercentage, out bool bCompleted)
        {
            // We find the percentage time elapsed...
            var elapsedTime = iTime / this.transitionTime;
            dPercentage = Utility.ConvertLinearToEaseInEaseOut(elapsedTime);
            bCompleted = false;

            if (elapsedTime >= 1.0)
            {
                dPercentage = 1.0;
                switch (this.LoopType)
                {
                    case LoopType.None:
                        bCompleted = true;
                        break;

                    case LoopType.Loop:
                        this.transitionTime = iTime;
                        break;

                    case LoopType.PingPong:
                        break;
                }
            }
        }

        #endregion

        #region Private data

        private double transitionTime = 0.0;

        #endregion
    }
}
