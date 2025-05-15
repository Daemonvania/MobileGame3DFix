using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnistrokeGestureRecognition {
    /// <summary>
    /// Job for comparing path with patterns.
    /// </summary>
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    internal struct ParallelCompareJob : IJobFor {
        [ReadOnly] public int pointsNumber;

        /// <summary> Flatten buffer of patterns paths. </summary>
        [ReadOnly] public NativeArray<float2> patternPaths;
        /// <summary> Modifications of patterns </summary>
        [ReadOnly] public NativeArray<GestureModifications> patternsModifications;

        /// <summary> Buffer of gesture path uniformly normalized. </summary>
        [ReadOnly] public NativeArray<float2> uniformGesturePath;
        /// <summary> Buffer of gesture path un uniformly normalized. </summary>
        [ReadOnly] public NativeArray<float2> unUniformGesturePath;
        /// <summary> Buffer of gesture path uniformly normalized. </summary>
        [ReadOnly] public NativeArray<float2> uniformRotatedGesturePath;
        /// <summary> Buffer of gesture path un uniformly normalized. </summary>
        [ReadOnly] public NativeArray<float2> unUniformRotatedGesturePath;

        /// <summary> Compare results buffer. </summary>
        [WriteOnly] public NativeArray<float> distanceBuffer;

        public void Execute(int index) {
            int bufferOffset = pointsNumber * index;

            var currentPatternPath = patternPaths.Slice(bufferOffset, pointsNumber);
            var currentPatternModifications = patternsModifications[index];

            if (GestureModificationsUtils.IsRotationSensitive(currentPatternModifications)) {
                RotationSensitiveCompare(index, currentPatternPath, currentPatternModifications);
            }
            else {
                RotationInvariantCompare(index, currentPatternPath, currentPatternModifications);
            }
        }

        private void RotationInvariantCompare(int index, NativeSlice<float2> patternPath, GestureModifications modifications) {
            var path = GestureModificationsUtils.IsUniformScaled(modifications) ? uniformRotatedGesturePath : unUniformRotatedGesturePath;
            var tmpPathBuffer = new NativeArray<float2>(path.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            var distance = GestureMath.GoldenSectionSearch(path, patternPath, tmpPathBuffer);

            if (GestureModificationsUtils.IsDirectionInvariant(modifications)) {
                var reverseDistance = GestureMath.GoldenSectionSearchReversed(path, patternPath, tmpPathBuffer);

                if (reverseDistance < distance)
                    distance = reverseDistance;
            }

            distanceBuffer[index] = distance;
        }

        private void RotationSensitiveCompare(int index, NativeSlice<float2> patternPath, GestureModifications modifications) {
            var path = GestureModificationsUtils.IsUniformScaled(modifications) ? uniformGesturePath : unUniformGesturePath;

            var distance = GestureMath.CalculateTwoPathDistance(path, patternPath);

            if (GestureModificationsUtils.IsDirectionInvariant(modifications)) {
                var reverseDistance = GestureMath.CalculateTwoPathDistanceReverse(path, patternPath);

                if (reverseDistance < distance)
                    distance = reverseDistance;
            }

            distanceBuffer[index] = distance;
        }
    }
}