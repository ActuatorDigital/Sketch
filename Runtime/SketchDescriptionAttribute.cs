using System;

namespace Actuator.Sketch
{
    public class SketchDescriptionAttribute : Attribute
    {
        public string Description;

        public SketchDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}