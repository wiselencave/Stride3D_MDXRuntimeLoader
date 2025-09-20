using System.Runtime.InteropServices;

namespace MDXRuntimeLoader.MDXStuff.VertexStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UShort4(ushort x, ushort y, ushort z, ushort w)
    {
        public ushort X = x, Y = y, Z = z, W = w;
    }
}