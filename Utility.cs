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
    /// <summary>
    /// A class holding static utility functions.
    /// </summary>
    public class Utility
    {
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
    }
}
