using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnistrokeGestureRecognition {
    /// <summary>
    /// Job for performing path preparation 
    /// (resampling and normalizing). 
    /// </summary>
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    internal struct PreparePathWithRotationJob : IJob {
        /// <summary>
        /// Scaling strategy.
        /// </summary>
        [ReadOnly] public bool isUniformScaling;

        /// <summary>
        /// Initial path buffer.
        /// </summary>
        [DeallocateOnJobCompletion]
        public NativeArray<float2> initialPathBuffer;

        public NativeArray<float2> writeBuffer;

        public void Execute() {
            var center = GestureMath.FindCentroid(initialPathBuffer);
            var rad = GestureMath.CalculateAngleInRad(center, initialPathBuffer[0]);

            GestureMath.RotatePathByRad(initialPathBuffer, rad);
            GestureMath.ResampleAndNormalizePath(initialPathBuffer, writeBuffer, isUniformScaling);
        }
    }
}