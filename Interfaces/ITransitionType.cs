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
    public interface ITransitionType
    {
        /// <summary>
        /// Called by the Transition framework when its timer ticks to pass in the
        /// time (in ms) since the transition started. 
        /// 
        /// You should return (in an out parameter) the percentage movement towards 
        /// the destination value for the time passed in. Note: this does not need to
        /// be a smooth transition from 0% to 100%. You can overshoot with values
        /// greater than 100% or undershoot if you need to (for example, to have some
        /// form of "elasticity").
        /// 
        /// The percentage should be returned as (for example) 0.1 for 10%.
        /// 
        /// You should return a bool indicating whether the transition has completed.
        /// (This may not be at the same time as the percentage has moved to 100%.)
        /// </summary>
        bool OnTimer(int iTime, out double completionPercentage);
    }
}
