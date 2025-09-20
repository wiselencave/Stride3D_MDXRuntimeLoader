using SharpDX;
using Stride.Core.Mathematics;
using Stride.Graphics;
using System;
using System.Runtime.InteropServices;
using MDXRuntimeLoader.MDXStuff.VertexStructs;

namespace MDXRuntimeLoader.MDXRuntimeLoader.GPUstructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosNormTanTexSkin : IEquatable<VertexPosNormTanTexSkin>, IVertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPosNormTanTexSkin"/> struct.
        /// </summary>
        /// <param name="position">The position of this vertex.</param>
        /// <param name="normal">The vertex normal.</param>
        /// <param name="tangent">The vertex tangent.</param>
        /// <param name="textureCoordinate">UV texture coordinates.</param>
        public VertexPosNormTanTexSkin(Vector3 position, Vector3 normal, Vector4 tangent, Vector2 textureCoordinate, Byte4 joints, Vector4 weights) : this()
        {
            BlendIndices = joints;
            BlendWeight = weights;
            Position = position;
            Normal = normal;
            Tangent = tangent;
            TextureCoordinate = textureCoordinate;
        }

        /// <summary>
        /// XYZ position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The vertex normal.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The vertex tagent.
        /// </summary>
        public Vector4 Tangent;

        /// <summary>
        /// UV texture coordinates.
        /// </summary>
        public Vector2 TextureCoordinate;

        /// <summary>
        /// BlendIndices vector.
        /// </summary>
        public Byte4 BlendIndices;

        /// <summary>
        /// BlendWeight vector.
        /// </summary>
        public Vector4 BlendWeight;


        /// <summary>
        /// Defines structure byte size.
        /// </summary>
        public static readonly int Size = Utilities.SizeOf<VertexPosNormTanTexSkin>(); // 12 + 12 + 16 + 8 + 4 + 16

        /// <summary>
        /// The vertex layout of this struct.
        /// </summary>
        public static readonly VertexDeclaration Layout = new VertexDeclaration(
            VertexElement.Position<Vector3>(0, 0),
            VertexElement.Normal<Vector3>(0, 12),
            VertexElement.Tangent<Vector4>(0, 24),
            VertexElement.TextureCoordinate<Vector2>(0, 40),
            new VertexElement(VertexElementUsage.BlendIndices, 0, PixelFormat.R8G8B8A8_UInt, 48),
            new VertexElement(VertexElementUsage.BlendWeight, 0, PixelFormat.R32G32B32A32_Float, 52)
        );


        public bool Equals(VertexPosNormTanTexSkin other)
        {
            return Position.Equals(other.Position) &&
                Normal.Equals(other.Normal) &&
                Tangent.Equals(other.Tangent) &&
                TextureCoordinate.Equals(other.TextureCoordinate) &&
                BlendIndices.Equals(other.BlendIndices) &&
                BlendWeight.Equals(other.BlendWeight);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is VertexPosNormTanTexSkin && Equals((VertexPosNormTanTexSkin)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ Tangent.GetHashCode();
                hashCode = (hashCode * 397) ^ TextureCoordinate.GetHashCode();
                hashCode = (hashCode * 397) ^ BlendIndices.GetHashCode();
                hashCode = (hashCode * 397) ^ BlendWeight.GetHashCode();
                return hashCode;
            }
        }

        public VertexDeclaration GetLayout()
        {
            return Layout;
        }

        public void FlipWinding()
        {
            TextureCoordinate.X = (1.0f - TextureCoordinate.X);
        }

        public static bool operator ==(VertexPosNormTanTexSkin left, VertexPosNormTanTexSkin right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPosNormTanTexSkin left, VertexPosNormTanTexSkin right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"Position: {Position},Normal: {Normal}, Texcoord: {TextureCoordinate}, BlendIndices: {BlendIndices}, BlendWeight: {BlendWeight}, Tangent: {Tangent}";
        }
    }
}
