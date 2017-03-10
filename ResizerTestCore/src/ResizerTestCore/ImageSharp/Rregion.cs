using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Drawing;

namespace ResizerTestCore.ImageSharp
{
    internal class Rregion : Region
    {
        public Rregion(Rectangle r)
        {
            this.Bounds = r;
        }
        public override int ScanX(int x, float[] buffer, int length, int offset)
        {
            return 0;
        }

        public override int ScanY(int y, float[] buffer, int length, int offset)
        {
            return 0;
        }

        public override int MaxIntersections { get; }
        public override Rectangle Bounds { get; }
    }

}
