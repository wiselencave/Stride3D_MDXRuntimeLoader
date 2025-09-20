using MDXReForged.Structs;
using MDXRuntimeLoader.MDXRuntimeLoader.GPUstructs;
using MDXRuntimeLoader.MDXStuff.VertexStructs;
using Stride.Core.Mathematics;
using Stride.Core.Serialization.Contents;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Materials;
using System.Collections.Generic;

namespace MDXRuntimeLoader.MDXStuff
{
    internal class MeshLoader
    {
        internal static void Load(GraphicsDevice graphicsDevice, ContentManager content, MDXReForged.MDX.Model mdx, Stride.Rendering.Model model, MeshSkinningDefinition skinning)
        {
            var geosets = mdx.GetGeosets();
            var geosetAnims = mdx.GetGeosetAnimations();
            List<uint> skipIt = new();
            foreach (var anim in geosetAnims)
            {
                if (anim.Alpha == 0 && anim.AlphaKeys.Nodes.Length == 0)
                {
                    skipIt.Add((uint)anim.GeosetId);
                }
            }

            foreach (var geo in geosets)
            {
                if (geo.Level != 0) // skip lods
                    continue;
                if (geo.Name.Contains("organs") || geo.Name.Contains("skeleton")) // skip flesh
                    continue;
                if (skipIt.Contains(geo.GeosetId)) // fix useless doubles like detheroc's body
                    continue;

                // Vertex buffer
                var strideVertices = new VertexPosNormTanTexSkin[geo.Vertices.Count];

                for (int i = 0; i < geo.Vertices.Count; i++)
                {
                    var pos = geo.Vertices[i].ToStrideVector3();
                    var normal = geo.Normals[i].ToStrideVector3(false);
                    var uv = new Vector2(geo.TexCoords[0][i].X, geo.TexCoords[0][i].Y);
                    var tangent = geo.Tangents[i].ToStrideVector4();
                    ConvertSkinData(geo.Skin[i], out Byte4 joints, out Vector4 weights);

                    strideVertices[i] = new VertexPosNormTanTexSkin(pos, normal, tangent, uv, joints, weights);
                }

                var vertexBuffer = Stride.Graphics.Buffer.Vertex.New(graphicsDevice, strideVertices, GraphicsResourceUsage.Dynamic);

                // Index buffer
                var strideIndices = ConvertIndices(geo.GetFlatTriangleIndices());
                var indexBuffer = Stride.Graphics.Buffer.Index.New(graphicsDevice, strideIndices);

                var newMesh = new Mesh
                {
                    Draw = new MeshDraw
                    {
                        PrimitiveType = Stride.Graphics.PrimitiveType.TriangleList,
                        DrawCount = strideIndices.Length,
                        IndexBuffer = new IndexBufferBinding(indexBuffer, true, strideIndices.Length),
                        VertexBuffers = [
                            new VertexBufferBinding(vertexBuffer,
                                  VertexPosNormTanTexSkin.Layout, vertexBuffer.ElementCount)
                        ],
                    },
                    Skinning = skinning,
                    MaterialIndex = (int)geo.MaterialId,
                    //BoundingBox = new BoundingBox(mdx.Bounds.Extent.Min.ToStrideVector3() * 3f, mdx.Bounds.Extent.Max.ToStrideVector3() * 3f),
                    //BoundingSphere = new BoundingSphere(new Vector3(0, 1, 0), 8)
                   
                };

                //indexBuffer.Dispose();
                //vertexBuffer.Dispose();

                newMesh.Parameters.Set(MaterialKeys.HasSkinningPosition, true);
                newMesh.Parameters.Set(MaterialKeys.HasSkinningNormal, true);
                newMesh.Parameters.Set(MaterialKeys.HasSkinningTangent, true);

                model.Meshes.Add(newMesh);
            }
        }

        private static int[] ConvertIndices(ushort[] source)
        {
            int[] indices = new int[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                // a, b, c = a, c, b
                if (i % 3 == 1 && i + 1 < source.Length)
                    indices[i] = source[i + 1];
                else if (i % 3 == 2)
                    indices[i] = source[i - 1];
                else
                    indices[i] = source[i];
            }
            return indices;
        }

        private static void ConvertSkinData(CSkinData skin, out Byte4 packedIndices, out Vector4 normalizedWeights)
        {
            packedIndices = new Byte4(
                skin.BoneIndices[0],
                skin.BoneIndices[1],
                skin.BoneIndices[2],
                skin.BoneIndices[3]);

            // byte to float
            float w0 = skin.BoneWeights[0];
            float w1 = skin.BoneWeights[1];
            float w2 = skin.BoneWeights[2];
            float w3 = skin.BoneWeights[3];

            float total = w0 + w1 + w2 + w3;
            float scale = 1.0f / total;
            normalizedWeights = new Vector4(w0 * scale, w1 * scale, w2 * scale, w3 * scale);
        }
    }
}
