using System;

namespace AIR.Sketch
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