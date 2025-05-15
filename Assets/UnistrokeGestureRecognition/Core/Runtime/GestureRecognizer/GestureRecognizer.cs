using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnistrokeGestureRecognition {
    public sealed class GestureRecognizer<G> : IGestureRecognizer<G> where G : IGesturePattern {
        /// <inheritdoc/>
        public RecognizeResult<G> Result => FindBestDistanceResult();

        private readonly List<G> _patterns;

        private readonly NativeArray<float2> _patternsBuffer;
        private readonly NativeArray<GestureModifications> _patternsMods;

        private readonly NativeArray<float2> _uniformPathBuffer;
        private readonly NativeArray<float2> _unUniformPathBuffer;
        private readonly NativeArray<float2> _uniformRotatedPathBuffer;
        private readonly NativeArray<float2> _unUniformRotatedPathBuffer;

        private readonly NativeArray<float> _distanceBuffer;

        private readonly int _resamplePointsNumber;

        public GestureRecognizer(IEnumerable<G> patterns, int resamplePointsNumber = 128) {
            _resamplePointsNumber = resamplePointsNumber;

            _patterns = patterns.ToList();
            _patterns.Sort((g1, g2) => g1.GetModifications().CompareTo(g2.GetModifications()));

            _patternsBuffer = new NativeArray<float2>(_patterns.Count * resamplePointsNumber, Allocator.Persistent);
            _patternsMods = new NativeArray<GestureModifications>(_patterns.Count, Allocator.Persistent);

            _distanceBuffer = new NativeArray<float>(_patterns.Count, Allocator.Persistent);

            bool hasUniformRotationSensitive = false;
            bool hasUnUniformRotationSensitive = false;
            bool hasUniformRotationInvariant = false;
            bool hasUnUniformRotationInvariant = false;

            foreach (var p in _patterns) {
                if (p is { ScalingMode: GestureScalingMode.Uniform, RotationSensitivity: GestureRotationSensitivity.Sensitive })
                    hasUniformRotationSensitive = true;
                else if (p is { ScalingMode: GestureScalingMode.UnUniform, RotationSensitivity: GestureRotationSensitivity.Sensitive })
                    hasUnUniformRotationSensitive = true;
                else if (p is { ScalingMode: GestureScalingMode.Uniform, RotationSensitivity: GestureRotationSensitivity.Invariant })
                    hasUniformRotationInvariant = true;
                else if (p is { ScalingMode: GestureScalingMode.UnUniform, RotationSensitivity: GestureRotationSensitivity.Invariant })
                    hasUnUniformRotationInvariant = true;
            }

            if (hasUniformRotationSensitive)
                _uniformPathBuffer = new NativeArray<float2>(_resamplePointsNumber, Allocator.Persistent);
            else
                _uniformPathBuffer = new NativeArray<float2>(0, Allocator.Persistent);

            if (hasUnUniformRotationSensitive)
                _unUniformPathBuffer = new NativeArray<float2>(_resamplePointsNumber, Allocator.Persistent);
            else
                _unUniformPathBuffer = new NativeArray<float2>(0, Allocator.Persistent);

            if (hasUniformRotationInvariant)
                _uniformRotatedPathBuffer = new NativeArray<float2>(_resamplePointsNumber, Allocator.Persistent);
            else
                _uniformRotatedPathBuffer = new NativeArray<float2>(0, Allocator.Persistent);

            if (hasUnUniformRotationInvariant)
                _unUniformRotatedPathBuffer = new NativeArray<float2>(_resamplePointsNumber, Allocator.Persistent);
            else
                _unUniformRotatedPathBuffer = new NativeArray<float2>(0, Allocator.Persistent);

            PreparePatterns();
        }

        /// <inheritdoc/>
        public RecognizeResult<G> Recognize(NativeSlice<float2> path) {
            ScheduleRecognition(path).Complete();
            return FindBestDistanceResult();
        }

        /// <inheritdoc/>
        public JobHandle ScheduleRecognition(NativeSlice<float2> path) {
            var prepareJobHandle = PreparePathToRecognition(path);
            JobHandle.ScheduleBatchedJobs();

            var job = new ParallelCompareJob {
                pointsNumber = _resamplePointsNumber,
                distanceBuffer = _distanceBuffer,

                patternPaths = _patternsBuffer,
                patternsModifications = _patternsMods,

                uniformGesturePath = _uniformPathBuffer,
                unUniformGesturePath = _unUniformPathBuffer,
                uniformRotatedGesturePath = _uniformRotatedPathBuffer,
                unUniformRotatedGesturePath = _unUniformRotatedPathBuffer,
            };

            return job.ScheduleParallel(_patterns.Count, 1, prepareJobHandle);
        }

        private JobHandle PreparePathToRecognition(NativeSlice<float2> path) {
            var pathPrepareJobHandler = new JobHandle();

            if (_uniformPathBuffer.Length > 0) {
                var tmpPathBuffer = new NativeArray<float2>(path.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                path.CopyTo(tmpPathBuffer);

                var uniformPathPrepare = new PreparePathJob {
                    isUniformScaling = true,
                    readBuffer = tmpPathBuffer,
                    writeBuffer = _uniformPathBuffer,
                }.Schedule();

                pathPrepareJobHandler = JobHandle.CombineDependencies(pathPrepareJobHandler, uniformPathPrepare);
            }

            if (_unUniformPathBuffer.Length > 0) {
                var tmpPathBuffer = new NativeArray<float2>(path.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                path.CopyTo(tmpPathBuffer);

                var unUniformPathPrepare = new PreparePathJob {
                    isUniformScaling = false,
                    readBuffer = tmpPathBuffer,
                    writeBuffer = _unUniformPathBuffer,
                }.Schedule();

                pathPrepareJobHandler = JobHandle.CombineDependencies(pathPrepareJobHandler, unUniformPathPrepare);
            }

            if (_uniformRotatedPathBuffer.Length > 0) {
                var tmpPathBuffer = new NativeArray<float2>(path.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                path.CopyTo(tmpPathBuffer);

                var uniformRotatedPathPrepare = new PreparePathWithRotationJob {
                    isUniformScaling = true,
                    initialPathBuffer = tmpPathBuffer,
                    writeBuffer = _uniformRotatedPathBuffer,
                }.Schedule();

                pathPrepareJobHandler = JobHandle.CombineDependencies(pathPrepareJobHandler, uniformRotatedPathPrepare);
            }

            if (_unUniformRotatedPathBuffer.Length > 0) {
                var tmpPathBuffer = new NativeArray<float2>(path.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                path.CopyTo(tmpPathBuffer);

                var unUniformRotatedPathPrepare = new PreparePathWithRotationJob {
                    isUniformScaling = false,
                    initialPathBuffer = tmpPathBuffer,
                    writeBuffer = _unUniformRotatedPathBuffer,
                }.Schedule();

                pathPrepareJobHandler = JobHandle.CombineDependencies(pathPrepareJobHandler, unUniformRotatedPathPrepare);
            }

            return pathPrepareJobHandler;
        }

        public void Dispose() {
            _patternsMods.Dispose();
            _patternsBuffer.Dispose();
            _distanceBuffer.Dispose();

            _uniformPathBuffer.Dispose();
            _unUniformPathBuffer.Dispose();
            _uniformRotatedPathBuffer.Dispose();
            _unUniformRotatedPathBuffer.Dispose();
        }

        private RecognizeResult<G> FindBestDistanceResult() {
            var (distance, index) = RecognizerUtils.FindBestDistance(_distanceBuffer);
            return new RecognizeResult<G>(distance, _patterns[index]);
        }

        private void PreparePatterns() {
            for (int i = 0; i < _patterns.Count; i++) {
                var pattern = _patterns[i];
                var bufferSlice = _patternsBuffer.Slice(i * _resamplePointsNumber, _resamplePointsNumber);

                SetModification(_patternsMods, pattern, i);

                RecognizerUtils.PreparePattern(pattern, bufferSlice, pattern.RotationSensitivity == GestureRotationSensitivity.Invariant);
            }

            void SetModification(NativeArray<GestureModifications> modifications, G pattern, int index) {
                modifications[index] = pattern.GetModifications();
            }
        }
    }
}