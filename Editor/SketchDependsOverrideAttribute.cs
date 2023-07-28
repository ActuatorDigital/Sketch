using System;

namespace Actuator.Sketch
{
    public class SketchDependsOverrideAttribute : SketchDependsOnAttribute
    {
        public SketchDependsOverrideAttribute(Type serviceType, Type serviceImplementation)
            : base(serviceType, serviceImplementation) { }
    }
}