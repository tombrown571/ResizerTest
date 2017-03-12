using System;
using System.Collections.Generic;
using System.Text;

namespace ResizerServiceTest.Common
{
    public class Preset
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Quality { get; set; } = 90;
        public string Mode { get; set; } = "pad";
        public string BackgroundColour { get; set; } = "white";
        public string Format { get; set; } = "jpg";
    }
}
