using System;
using System.Collections.Generic;
using System.Text;

namespace Transitions
{
    internal class ManagedType_Float : IManagedType
    {
        #region IManagedType Members

        /// <summary>
        /// Returns the type we're managing.
        /// </summary>
        public Type getManagedType()
        {
            return typeof(float);
        }

        /// <summary>
        /// Returns a copy of the float passed in.
        /// </summary>
        public object copy(object o)
        {
            var f = (float)o;
            return f;
        }

        /// <summary>
        /// Returns the interpolated value for the percentage passed in.
        /// </summary>
        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            var fStart = (float)start;
            var fEnd = (float)end;
            return Utility.interpolate(fStart, fEnd, dPercentage);
        }

        #endregion
    }
}
