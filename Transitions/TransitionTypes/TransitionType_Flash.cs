using System;
using System.Collections.Generic;
using System.Text;

namespace Transitions
{
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
        public TransitionType_Flash(int iNumberOfFlashes, int iFlashTime)
        {
            // This class is derived from the user-defined transition type.
            // Here we set up a custom "user-defined" transition for the 
            // number of flashes passed in...
            var dFlashInterval = 100.0 / iNumberOfFlashes;

            // We set up the elements of the user-defined transition...
            IList<TransitionElement> elements = new List<TransitionElement>();
            for(var i=0; i<iNumberOfFlashes; ++i)
            {
                // Each flash consists of two elements: one going to the destination value, 
                // and another going back again...
                var dFlashStartTime = i * dFlashInterval;
                var dFlashEndTime = dFlashStartTime + dFlashInterval;
                var dFlashMidPoint = (dFlashStartTime + dFlashEndTime) / 2.0;
                elements.Add(new TransitionElement(dFlashMidPoint, 100, InterpolationMethod.EaseInEaseOut));
                elements.Add(new TransitionElement(dFlashEndTime, 0, InterpolationMethod.EaseInEaseOut));
            }

            this.setup(elements, iFlashTime * iNumberOfFlashes);
        }

        #endregion

    }
}
