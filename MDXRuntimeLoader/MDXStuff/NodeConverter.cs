using MDXReForged.MDX;
using MDXReForged.Structs;
using Stride.Core.Mathematics;
using Stride.Rendering;
using System.Collections.Generic;

namespace MDXRuntimeLoader.MDXStuff
{
    internal static class NodeConverter
    {
        // increase all ids to 1 for future ROOT armature node
        /*internal static Dictionary<int, int> RemapIds(List<GenObject> allNodes)
        {
            var idMap = new Dictionary<int, int>();
            for (int i = 0; i < allNodes.Count; i++)
            {
                idMap[allNodes[i].ObjectId] = i + 1;
            }

            idMap.Add(-1, 0); // no parent = ROOT parent
            
            return idMap;
        }*/

        internal static MeshSkinningDefinition ConvertSkinning(List<GenObject> allNodes, IReadOnlyList<C34Matrix> matrices, IReadOnlyList<CVector3> pivots)
        {
            var skinning = new MeshSkinningDefinition();
            skinning.Bones = new MeshBoneDefinition[allNodes.Count];
            //Console.WriteLine("MESH BONE DEF");
            for (int i = 0; i < allNodes.Count; i++)
            {
                var obj = allNodes[i];

                var pivot = pivots[obj.ObjectId].ToStrideVector3();
                pivot = new Vector3(-pivot.X, -pivot.Y, -pivot.Z);

                var matrix = Matrix.Identity;
                matrix.TranslationVector = pivot;

                skinning.Bones[i] = new MeshBoneDefinition
                {
                    NodeIndex = i + 1,
                    LinkToMeshMatrix = matrix
                };
            }

            return skinning;
        }
    }
}
