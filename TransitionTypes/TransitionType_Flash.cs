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

    /// <summary>
    /// This transition type 'flashes' the properties a specified number of times, ending
    /// up by reverting them to their initial values. You specify the number of bounces and
    /// the length of each bounce. 
    /// </summary>
    public class TransitionType_Flash : TransitionType_UserDefined
    {
        #region Public methods

        /// <summary>
        /// You specify the number of bounces and the time taken for each bounce.
        /// </summary>
        public TransitionType_Flash(int numberOfFlashes, int flashTime)
        {
            // This class is derived from the user-defined transition type.
            // Here we set up a custom "user-defined" transition for the 
            // number of flashes passed in...
            var flashInterval = 100.0 / numberOfFlashes;

            // We set up the elements of the user-defined transition...
            var elements = new List<TransitionElement>();
            for (var i = 0; i < numberOfFlashes; ++i)
            {
                // Each flash consists of two elements: one going to the destination value, 
                // and another going back again...
                var flashStartTime = i * flashInterval;
                var flashEndTime = flashStartTime + flashInterval;
                var flashMidPoint = (flashStartTime + flashEndTime) / 2.0;
                elements.Add(new TransitionElement(flashMidPoint, 100, InterpolationMethod.EaseInEaseOut));
                elements.Add(new TransitionElement(flashEndTime, 0, InterpolationMethod.EaseInEaseOut));
            }

            this.Setup(elements, flashTime * numberOfFlashes);
        }

        #endregion

    }
}
