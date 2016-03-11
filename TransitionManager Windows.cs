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

#if WINDOWS  //||PORTABLE
namespace Codefarts.Transitions
{
    using System.Threading;
    using System.Timers;

    /// <summary>
    /// This class is responsible for running transitions. It holds the timer that
    /// triggers transition animation. 
    /// </summary><remarks>
    /// This class is a singleton.
    /// 
    /// We manage the transition timer here so that we can have a single timer
    /// across all transitions. If each transition has its own timer, this creates
    /// one thread for each transition, and this can lead to too many threads in
    /// an application.
    /// 
    /// This class essentially just manages the timer for the transitions. It calls 
    /// back into the running transitions, which do the actual work of the transition.
    /// 
    /// </remarks>
    public partial class TransitionManager
    {
        private void InitializeComponent()
        {
            ThreadPool.QueueUserWorkItem(state=> this.OnTimerElapsed());
        }
        
        /// <summary>
        /// Called when the timer ticks.
        /// </summary>
        private void OnTimerElapsed()
        {
            while (true)
            {
                this.UpdateTransitions();
                Thread.Sleep(15);
            }                            
        }                                 
    }
} 
#endif