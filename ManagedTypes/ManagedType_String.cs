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

using System;
using System.Collections.Generic;
using System.Text;

namespace Codefarts.Transitions
{
    /// <summary>
    /// Manages transitions for strings. This doesn't make as much sense as transitions
    /// on other types, but I like the way it looks!
    /// </summary>
    internal class ManagedType_String : IManagedType
    {
        #region IManagedType Members

        /// <summary>
        /// Returns the type we're managing.
        /// </summary>
        public Type GetManagedType()
        {
            return typeof(string);
        }

        /// <summary>
        /// Returns a copy of the string passed in.
        /// </summary>
        public object Copy(object o)
        {
            var s = (string)o;
            return new string(s.ToCharArray());
        }

        /// <summary>
        /// Returns an "interpolated" string.
        /// </summary>
        public object GetIntermediateValue(object start, object end, double dPercentage)
        {
            var strStart = (string)start;
            var strEnd = (string)end;

            // We find the length of the interpolated string...
            var iStartLength = strStart.Length;
            var iEndLength = strEnd.Length;
            var iLength = Utility.Interpolate(iStartLength, iEndLength, dPercentage);
            var result = new char[iLength];

            // Now we assign the letters of the results by interpolating the
            // letters from the start and end strings...
            for (var i = 0; i < iLength; ++i)
            {
                // We get the start and end chars at this position...
                var cStart = 'a';
                if(i < iStartLength)
                {
                    cStart = strStart[i];
                }
                var cEnd = 'a';
                if(i < iEndLength)
                {
                    cEnd = strEnd[i];
                }

                // We interpolate them...
                char cInterpolated;
                if (cEnd == ' ')
                {
                    // If the end character is a space we just show a space 
                    // regardless of interpolation. It looks better this way...
                    cInterpolated = ' ';
                }
                else
                {
                    // The end character is not a space, so we interpolate...
                    var iStart = Convert.ToInt32(cStart);
                    var iEnd = Convert.ToInt32(cEnd);
                    var iInterpolated = Utility.Interpolate(iStart, iEnd, dPercentage);
                    cInterpolated = Convert.ToChar(iInterpolated);
                }

                result[i] = cInterpolated;
            }

            return new string(result);
        }

        #endregion
    }
}
