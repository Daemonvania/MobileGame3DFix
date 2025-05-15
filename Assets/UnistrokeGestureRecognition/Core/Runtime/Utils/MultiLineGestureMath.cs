using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace UnistrokeGestureRecognition {
    public static class MultiLineGestureMath {
        public static Rect FindGestureUniformRect(ReadOnlyCollection<LinePath> paths) {
            var result = GestureMath.FindGestureUniformRect(paths[0].Path);

            for (int i = 1; i < paths.Count; i++) {
                LinePath line = paths[i];
                var rect = GestureMath.FindGestureUniformRect(line.Path);
                result = result.Combine(rect);
            }

            return result;
        }

        public static Rect FindGestureUnUniformRect(ReadOnlyCollection<LinePath> paths) {
            var result = GestureMath.FindGestureUnUniformRect(paths[0].Path);

            for (int i = 1; i < paths.Count; i++) {
                LinePath line = paths[i];
                var rect = GestureMath.FindGestureUnUniformRect(line.Path);
                result = result.Combine(rect);
            }

            return result;
        }

        public static void NormalizePath(Rect rect, List<LinePath> paths) {
            foreach (var line in paths) {
                GestureMath.NormalizePath(rect, line.InternalPath);
            }
        }
    }
}