using MDXReForged.Structs;
using Stride.Core.Mathematics;
using System.Collections.Generic;

namespace MDXRuntimeLoader.MDXStuff
{
    public static class CVector3Extensions
    {
        private static readonly float scaleFactor = 50; // The vector will be reduced by the specified amount (if need)
        public static Vector3 ToStrideVector3(this MDXReForged.Structs.CVector3 vector, bool needScaling = true)
        {
            float scale = needScaling ? scaleFactor : 1;

            return new Vector3(
                -vector.Y / scale,    // Y_wc3 -> -X_stride
                vector.Z / scale,    // Z_wc3 -> Y_stride
                -vector.X / scale);   // X_wc3 -> -Z_stride
        }
        public static Vector3 ToStrideScaleVector3(this MDXReForged.Structs.CVector3 vector)
        {
            return new Vector3(
                vector.Y,
                vector.Z,
                vector.X);
        }
    }
    public static class C34MatrixExtensions
    {
        private static readonly float scaleFactor = 50;

        public static Matrix ToStrideMatrix(this C34Matrix m, bool needScaling = true)
        {
            float scale = needScaling ? scaleFactor : 1f;

            var matrix = new Matrix(
                m.M11, m.M12, m.M13, 0f,
                m.M21, m.M22, m.M23, 0f,
                m.M31, m.M32, m.M33, 0f,
                m.M41 / scale, m.M42 / scale, m.M43 / scale, 1f);  // translation

            matrix.Decompose(out Vector3 scaleVec, out Quaternion rotation, out Vector3 translation);

            scaleVec = new Vector3(scaleVec.Y, scaleVec.Z, scaleVec.X);
            rotation = new Quaternion(-rotation.Y, rotation.Z, -rotation.X, rotation.W); rotation.Normalize();
            translation = new Vector3(-translation.Y, translation.Z, -translation.X);

            Matrix.Transformation(ref scaleVec, ref rotation, ref translation, out Matrix result);
            return result;
        }
    }
    public static class CVector4Extensions
    {
        public static Vector4 ToStrideVector4(this CVector4 vector)
        {
            return new Vector4(
                -vector.Y,   // Y_wc3 -> -X_stride
                vector.Z,   // Z_wc3 -> Y_stride
                -vector.X,   // X_wc3 -> -Z_stride
                vector.W);
            //return new Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }
        public static Quaternion ToStrideQuaternion(this CVector4 vector)
        {
            var quat = new Quaternion(
                -vector.Y,   // Y_wc3 -> -X_stride
                vector.Z,   // Z_wc3 -> Y_stride
                -vector.X,   // X_wc3 -> -Z_stride
                vector.W);
            //quat.Normalize();
            return quat;    
            //return new Quaternion(vector.X, vector.Y, vector.Z, vector.W);
        }
    }
    public static class CAnimatorNodeExtensions
    {
        public static IEnumerable<CAnimatorNode<T>> SliceByTime<T>(this IEnumerable<CAnimatorNode<T>> nodes, (int minTime, int maxTime) bounds)
        {
            foreach (var node in nodes)
            {
                if (node.Time >= bounds.minTime && node.Time <= bounds.maxTime)
                    yield return node;
            }
        }
    }
}
