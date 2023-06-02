using System;

namespace AIR.Sketch
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SketchDependsOnAttribute : Attribute
    {
        public Type ServiceImplementation;
        public Type ServiceType;

        public SketchDependsOnAttribute(Type serviceType, Type serviceImplementation)
        {
            ServiceType = serviceType;
            ServiceImplementation = serviceImplementation;
        }
    }
}