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
    using UnityEngine;

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
    public partial class TransitionManager : MonoBehaviour
    {
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public void Awake()
        {
            this.GetTimeCallback = () => (int)(Time.time * 1000);
            Transition<float>.InterpolateCallback = (start, end, percent) => Utility.Interpolate(start, end, percent);
            Transition<int>.InterpolateCallback = (start, end, percent) => Utility.Interpolate(start, end, percent);
            Transition<double>.InterpolateCallback = (start, end, percent) => Utility.Interpolate(start, end, percent);
            Transition<Vector3>.InterpolateCallback = (start, end, percent) => Vector3.LerpUnclamped(start, end, (float)percent);
            Transition<Vector2>.InterpolateCallback = (start, end, percent) => Vector2.LerpUnclamped(start, end, (float)percent);
            Transition<Vector4>.InterpolateCallback = (start, end, percent) => Vector4.LerpUnclamped(start, end, (float)percent);
            Transition<Color>.InterpolateCallback = (start, end, percent) => Color.Lerp(start, end, (float)percent);
            Transition<Color32>.InterpolateCallback = (start, end, percent) => Color32.Lerp(start, end, (float)percent);
            Transition<Quaternion>.InterpolateCallback = (start, end, percent) => Quaternion.Lerp(start, end, (float)percent);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        public void Update()
        {
            this.UpdateTransitions();
        }
    }
}
#endif