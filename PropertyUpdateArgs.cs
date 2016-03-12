namespace Codefarts.Transitions
{
    using System;
    using System.Reflection;

    public class PropertyUpdateArgs : EventArgs
    {
        public PropertyUpdateArgs(object targetObject, PropertyInfo propertyInfo, object value)
        {
            this.Target = targetObject;
            this.PropertyInfo = propertyInfo;
            this.Value = value;
        }

        public object Target { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public object Value { get; private set; }
    }
}