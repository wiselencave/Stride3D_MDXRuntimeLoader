using MDXReForged;
using Stride.Core.Serialization.Contents;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;
using System;
using System.Collections.Generic;
using System.IO;


namespace MDXRuntimeLoader.MDXStuff
{
    internal class MaterialProcessor
    {
        private static string assetsDirectory = "F:\\refUnpackedV203\\war3.w3mod\\Assets\\";

        internal static void Process(ContentManager content, GraphicsDevice graphicsDevice, Model model, MDXReForged.MDX.Model mdx)
        {
            var textures = mdx.GetTextures();

            foreach (var mat in mdx.GetMaterials())
            {
                MDXReForged.MDX.Layer layer = mat.Layers[0]; // HD 1100+
                var flags = layer.Flags;

                MaterialDescriptor descriptor;
                if (layer.ShaderId == LayerShader.HD)
                {
                    // reforged HD shader
                    Texture diffuse = LoadTexture(graphicsDevice, textures, layer, TextureSemantic.Diffuse, true);
                    Texture normal = LoadTexture(graphicsDevice, textures, layer, TextureSemantic.Normal, false);
                    Texture orm = LoadTexture(graphicsDevice, textures, layer, TextureSemantic.ORM, true);
                    Texture emissive = LoadTexture(graphicsDevice, textures, layer, TextureSemantic.Emissive, true);

                    descriptor = new MaterialDescriptor
                    {
                        Attributes = new MaterialAttributes
                        {
                            CullMode = ((flags & ShadingFlags.TwoSided) != 0) ? CullMode.None : CullMode.Back,
                            Diffuse = new MaterialDiffuseMapFeature
                            {
                                DiffuseMap = new ComputeTextureColor
                                {
                                    Texture = diffuse,
                                    TexcoordIndex = TextureCoordinate.Texcoord0
                                }
                            },
                            DiffuseModel = new MaterialDiffuseLambertModelFeature(),
                            Surface = new MaterialNormalMapFeature(new ComputeTextureColor(normal)),
                            Transparency = new MaterialTransparencyCutoffFeature(),
                            Occlusion = new MaterialOcclusionMapFeature
                            {
                                AmbientOcclusionMap = new ComputeTextureScalar
                                {
                                    Texture = orm,
                                    Channel = ColorChannel.R
                                }
                            },
                            MicroSurface = new MaterialGlossinessMapFeature
                            {

                                GlossinessMap = new ComputeBinaryScalar
                                {
                                    Operator = BinaryOperator.Multiply,
                                    LeftChild = new ComputeTextureScalar
                                    {
                                        Texture = orm,
                                        Channel = ColorChannel.G,
                                        TexcoordIndex = TextureCoordinate.Texcoord0
                                    },
                                    RightChild = new ComputeFloat(2.1f)
                                },
                                Invert = true,
                            },
                            Specular = new MaterialMetalnessMapFeature
                            {
                                MetalnessMap = new ComputeTextureScalar
                                {
                                    Texture = orm,
                                    Channel = ColorChannel.B,
                                    TexcoordIndex = TextureCoordinate.Texcoord0
                                }
                            },
                            SpecularModel = new MaterialSpecularMicrofacetModelFeature
                            {
                                Enabled = true,
                            },
                            Emissive = new MaterialEmissiveMapFeature
                            {
                                EmissiveMap = new ComputeTextureColor
                                {
                                    Texture = emissive,
                                },
                                Intensity = new ComputeFloat(7f),
                            }
                        }
                    };
                }
                else
                {
                    // reforged SD shader 1100+
                    Texture diffuse = LoadTexture(graphicsDevice, textures, layer, TextureSemantic.Diffuse, true);

                    descriptor = new MaterialDescriptor
                    {
                        Attributes = new MaterialAttributes
                        {
                            CullMode = ((flags & ShadingFlags.TwoSided) != 0) ? CullMode.None : CullMode.Back,
                            Diffuse = new MaterialDiffuseMapFeature
                            {
                                DiffuseMap = new ComputeTextureColor
                                {
                                    Texture = diffuse,
                                    TexcoordIndex = TextureCoordinate.Texcoord0
                                }
                            },
                            DiffuseModel = new MaterialDiffuseLambertModelFeature(),
                            Transparency = layer.BlendMode == BlendMode.Add ? new MaterialTransparencyAdditiveFeature() : new MaterialTransparencyCutoffFeature(),
                        },
                    };
                }

                Material material = Material.New(graphicsDevice, descriptor);
                model.Materials.Add(material);
            }
        }

        private static string GetPath(IReadOnlyList<MDXReForged.MDX.Texture> textures, MDXReForged.MDX.Layer layer, TextureSemantic semantic)
        {
            string texturePath = textures[(int)layer.GetTextureId(semantic)].Image;
            if (texturePath == String.Empty)
                return Path.Combine(assetsDirectory, "textures\\white.dds");
            return Path.Combine(assetsDirectory, Path.ChangeExtension(texturePath, "dds"));
        }

        private static Texture LoadTexture(GraphicsDevice graphicsDevice, IReadOnlyList<MDXReForged.MDX.Texture> textures, MDXReForged.MDX.Layer layer, TextureSemantic semantic, bool isSRGB)
        {
            Texture texture;
            using (var fs = File.OpenRead(GetPath(textures, layer, semantic)))
            {
                texture = isSRGB
                    ? Texture.Load(graphicsDevice, fs, Stride.Graphics.TextureFlags.ShaderResource, GraphicsResourceUsage.Immutable, true)
                    : Texture.Load(graphicsDevice, fs);
            }
            return texture;
        }
    }
}
