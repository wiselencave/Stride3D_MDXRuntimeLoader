using MDXReForged.MDX;
using MDXReForged.Structs;
using Stride.Core.Mathematics;
using Stride.Rendering;
using System.Collections.Generic;

namespace MDXRuntimeLoader.MDXStuff
{
    internal class SkeletonLoader
    {
        internal static Skeleton Load(List<GenObject> nodes, IReadOnlyList<C34Matrix> bindPoses, IReadOnlyList<CVector3> pivots)
        {
            Skeleton skeleton = new Skeleton();
            var mNodeDef = new List<ModelNodeDefinition>(2);


            foreach (var node in nodes)
            {
                var pivot = pivots[node.ObjectId].ToStrideVector3();
                Vector3 pivotParent;

                if (node.ParentId >= 0)
                {
                    pivotParent = pivots[node.ParentId].ToStrideVector3();
                }
                else
                {
                    pivotParent = Vector3.Zero;
                }

                var localTranslation = Vector3.Subtract(pivot, pivotParent);

                mNodeDef.Add(
                    new ModelNodeDefinition
                    {
                        Name = node.Name,
                        Flags = ModelNodeFlags.Default,
                        ParentIndex = node.ParentId + 1, 
                        Transform = new TransformTRS
                        {
                            Position = localTranslation,
                            Rotation = Quaternion.Zero,
                            Scale = Vector3.One,
                        },
                        

                    }
                );
            }
            mNodeDef.Insert(
                0,
                new ModelNodeDefinition
                {
                    Name = "Armature",
                    Flags = ModelNodeFlags.EnableRender,
                    ParentIndex = -1,
                    Transform = new TransformTRS
                    {
                        Position = Vector3.Zero,
                        Rotation = Quaternion.Identity,
                        Scale = Vector3.One
                    },
                }
            );

            skeleton.Nodes = mNodeDef.ToArray();
            return skeleton;
        }
    }
}
