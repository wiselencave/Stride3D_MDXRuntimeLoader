using System.Runtime.InteropServices;

namespace MDXRuntimeLoader.MDXStuff.VertexStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Byte4(byte x, byte y, byte z, byte w)
    {
        public byte X = x, Y = y, Z = z, W = w;
    }
}