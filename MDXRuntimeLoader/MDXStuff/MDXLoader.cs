using MDXReForged.MDX;
using MDXReForged.Structs;
using Microsoft.VisualBasic.Logging;
using Stride.Animations;
using Stride.Core.Mathematics;
using Stride.Core.Serialization.Contents;
using Stride.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MDXRuntimeLoader.MDXStuff
{
    internal class MDXLoader
    {
        public static (Stride.Rendering.Model, Dictionary<string, AnimationClip>) LoadMDX(GraphicsDevice graphicsDevice, ContentManager content, Model mdx)
        {
            // Merge bones and helpers
            var bones = mdx.GetBones();
            var helpers = mdx.GetHelpers();
            var allNodes = new List<GenObject>();

            allNodes.AddRange(bones);
            allNodes.AddRange(helpers);

            var pivots = mdx.GetPivots();
            var bindPoses = mdx.GetBindPoses();

            //var nodeIDs = NodeConverter.RemapIds(allNodes);
            var model = new Stride.Rendering.Model();

            MaterialProcessor.Process(content, graphicsDevice, model, mdx);

            var skinning = NodeConverter.ConvertSkinning(allNodes, bindPoses, pivots);
            model.Skeleton = SkeletonLoader.Load(allNodes, bindPoses, pivots);
            MeshLoader.Load(graphicsDevice, content, mdx, model, skinning);


            var animations = Animator.GetClips(mdx.GetSequences(), mdx.GetGlobalSequences(), allNodes, model.Skeleton.Nodes);

            //model.BoundingBox = new BoundingBox(mdx.Bounds.Extent.Min.ToStrideVector3(), mdx.Bounds.Extent.Max.ToStrideVector3());
            //model.BoundingSphere = new BoundingSphere(Vector3.Zero, mdx.GetSequences()[0].Bounds.Radius);
            return (model, animations);
        }
    }
}
