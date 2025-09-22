using MDXReForged.MDX;
using MDXReForged.Structs;
using Stride.Animations;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDXRuntimeLoader.MDXStuff
{
    internal class Animator
    {
        const string baseString = "[ModelComponent.Key].Skeleton.NodeTransformations[index].Transform.type";
        internal static Dictionary<string, AnimationClip> GetClips(IReadOnlyList<Sequence> sequences, IReadOnlyList<int> globalSequences, List<GenObject> mdxNodes, ModelNodeDefinition[] nodes)
        {
            var clips = new Dictionary<string, AnimationClip>(sequences.Count);
            foreach (var seq in sequences)
            {
                var range = seq.MaxTime - seq.MinTime + 1;
                var clip = new AnimationClip {
                    Duration = TimeSpan.FromMilliseconds(range),
                    RepeatMode = AnimationRepeatMode.LoopInfinite
                };

                foreach (var node in mdxNodes)
                {
                    var bindPos = nodes[node.ObjectId + 1].Transform.Position;

                    var posKeys = node.TranslationKeys.Nodes.SliceByTime(GetGlobalBounds<CVector3>(node.TranslationKeys, seq, globalSequences)).ToList();
                    var rotKeys = node.RotationKeys.Nodes.SliceByTime(GetGlobalBounds<CVector4>(node.RotationKeys, seq, globalSequences)).ToList();
                    var scaleKeys = node.ScaleKeys.Nodes.SliceByTime(GetGlobalBounds<CVector3>(node.ScaleKeys, seq, globalSequences)).ToList();

                    if (posKeys.Count == 0 && rotKeys.Count == 0 && scaleKeys.Count == 0) continue;

                    var pathBase = baseString.Replace("index", $"{node.ObjectId + 1}");

                    // Position
                    if (posKeys.Count > 0)
                    {
                        var curve = ConvertToStrideCurve(posKeys, AnimationCurveInterpolationType.Linear, seq.MinTime, bindPos);
                        clip.AddCurve(pathBase.Replace("type", "Position"), curve);
                    }
                    // Rotation
                    if (rotKeys.Count > 0)
                    {
                        var curve = ConvertToStrideCurve(rotKeys, AnimationCurveInterpolationType.Linear, seq.MinTime);
                        clip.AddCurve(pathBase.Replace("type", "Rotation"), curve);
                    }
                    // Scale
                    if (scaleKeys.Count > 0)
                    {
                        var curve = ConvertToStrideCurve(scaleKeys, AnimationCurveInterpolationType.Linear, seq.MinTime, false);
                        clip.AddCurve(pathBase.Replace("type", "Scale"), curve);
                    }
                }
                clips.Add(seq.Name, clip);
            }

            return clips;
        }

        private static (int, int) GetGlobalBounds<T>(Track<T> track, Sequence sequence, IReadOnlyList<int> globalSequences) where T : new()
        {
            if (globalSequences.Count > 0 && track.GlobalSequenceId > -1)
            {
                return (0, globalSequences[track.GlobalSequenceId]);
            }
            return (sequence.MinTime, sequence.MaxTime);
        }

        private static AnimationCurve<Vector3> ConvertToStrideCurve(List<CAnimatorNode<CVector3>> nodes, AnimationCurveInterpolationType interp, int minTime, Vector3 bindPosition, bool needScaling = true)
        {
            return new AnimationCurve<Vector3>
            {
                InterpolationType = interp,
                KeyFrames = new FastList<KeyFrameData<Vector3>>(nodes.Select(n =>
                    new KeyFrameData<Vector3>(
                        (CompressedTimeSpan)TimeSpan.FromMilliseconds(n.Time - minTime),
                        n.Value.ToStrideVector3(needScaling) + bindPosition
                    )))
            };
        }

        private static AnimationCurve<Vector3> ConvertToStrideCurve(List<CAnimatorNode<CVector3>> nodes, AnimationCurveInterpolationType interp, int minTime, bool needScaling = true)
        {
            return new AnimationCurve<Vector3>
            {
                InterpolationType = interp,
                KeyFrames = new FastList<KeyFrameData<Vector3>>(nodes.Select(n =>
                    new KeyFrameData<Vector3>(
                        (CompressedTimeSpan)TimeSpan.FromMilliseconds(n.Time - minTime),
                        n.Value.ToStrideScaleVector3()
                    )))
            };
        }

        private static AnimationCurve<Quaternion> ConvertToStrideCurve(List<CAnimatorNode<CVector4>> nodes, AnimationCurveInterpolationType interp, int minTime)
        {
            return new AnimationCurve<Quaternion>
            {
                InterpolationType = interp,
                KeyFrames = new FastList<KeyFrameData<Quaternion>>(nodes.Select(n =>
                    new KeyFrameData<Quaternion>(
                        (CompressedTimeSpan)TimeSpan.FromMilliseconds(n.Time - minTime),
                        n.Value.ToStrideQuaternion()
                    )))
            };
        }
    }
}
