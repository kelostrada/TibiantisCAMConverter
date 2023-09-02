using System;
namespace TibiaThingsReader.Geometry
{
    public class Size
    {
        //--------------------------------------------------------------------------
        // PROPERTIES
        //--------------------------------------------------------------------------

        public double Width { get; set; }
        public double Height { get; set; }

        //--------------------------------------------------------------------------
        // CONSTRUCTOR
        //--------------------------------------------------------------------------

        public Size(double width = double.NaN, double height = double.NaN)
        {
            Width = width;
            Height = height;
        }
    }
}
