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

#if UNITY_5
namespace Codefarts.Transitions
{
    using System;

    using UnityEngine;

    /// <summary>
    /// Class that manages transitions for Color properties. For these we
    /// need to transition the R, G, B and A sub-properties independently.
    /// </summary>
    internal class ManagedType_Color : IManagedType
    {
        #region IManagedType Members

        /// <summary>
        /// Returns the type we are managing.
        /// </summary>
        public Type GetManagedType()
        {
            return typeof(Color);
        }

        /// <summary>
        /// Returns a copy of the color object passed in.
        /// </summary>
        public object Copy(object o)
        {
            var c = (Color)o;
            var result = new Color(c.r, c.g, c.b, c.a);
            return result;
        }

        /// <summary>
        /// Creates an intermediate value for the colors depending on the percentage passed in.
        /// </summary>
        public object GetIntermediateValue(object start, object end, double dPercentage)
        {
            var startColor = (Color)start;
            var endColor = (Color)end;

            // We interpolate the R, G, B and A components separately...
            var iStart_R = startColor.r;
            var iStart_G = startColor.g;
            var iStart_B = startColor.b;
            var iStart_A = startColor.a;

            var iEnd_R = endColor.r;
            var iEnd_G = endColor.g;
            var iEnd_B = endColor.b;
            var iEnd_A = endColor.a;

            var new_R = Utility.Interpolate(iStart_R, iEnd_R, dPercentage);
            var new_G = Utility.Interpolate(iStart_G, iEnd_G, dPercentage);
            var new_B = Utility.Interpolate(iStart_B, iEnd_B, dPercentage);
            var new_A = Utility.Interpolate(iStart_A, iEnd_A, dPercentage);

            return new Color(new_R, new_G, new_B, new_A);
        }

        #endregion
    }
}

#endif