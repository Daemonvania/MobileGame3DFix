using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace UnistrokeGestureRecognition {
    public static class RecognizerUtils {
        public static (float distance, int index) FindBestDistance(NativeArray<float> distanceBuffer) {
            float bestDistance = float.MaxValue;
            int bestDistanceIndex = 0;

            for (int i = 0; i < distanceBuffer.Length; i++) {
                var distance = distanceBuffer[i];
                if (distance < bestDistance) {
                    bestDistance = distance;
                    bestDistanceIndex = i;
                }
            }

            return (bestDistance, bestDistanceIndex);
        }

        public static void PreparePatterns<G>(List<G> patterns, NativeArray<float2> resampleBuffer, int resamplePointsNumber, bool rotate = false) where G : IGesturePattern {
            for (int i = 0; i < patterns.Count; i++) {
                var pattern = patterns[i];
                var slice = resampleBuffer.Slice(i * resamplePointsNumber, resamplePointsNumber);

                PreparePattern(pattern, slice, rotate);
            }
        }

        public static void PreparePattern<G>(G pattern, NativeSlice<float2> pathBuffer, bool rotate = false) where G : IGesturePattern {
            var patternPath = pattern.Path;

            if (patternPath.Length < 2) {
                throw new ArgumentException("Patter must have at least 2 points!");
            }

            var tmpPathBuffer = new NativeArray<float2>(patternPath.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for (int o = 0; o < tmpPathBuffer.Length; o++) {
                tmpPathBuffer[o] = patternPath[o];
            }

            if (rotate) {
                var center = GestureMath.FindCentroid(tmpPathBuffer);
                var rad = GestureMath.CalculateAngleInRad(center, tmpPathBuffer[0]);
                GestureMath.RotatePathByRad(tmpPathBuffer, rad);
            }

            GestureMath.ResampleAndNormalizePath(tmpPathBuffer, pathBuffer, pattern.ScalingMode == GestureScalingMode.Uniform);

            tmpPathBuffer.Dispose();
        }
    }
}