using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace UnistrokeGestureRecognition {
    public abstract class MultiLineGesturePatternBase : ScriptableObject, IMultiLineGesturePattern {
        public ReadOnlySpan<LinePath> Path => _path;

        public GestureScalingMode ScalingMode => _scalingMode;
        public GestureDirectionSensitivity DirectionSensitivity => _directionSensitivity;

        [HideInInspector][SerializeField] private LinePath[] _path;

        [SerializeField] private GestureScalingMode _scalingMode = GestureScalingMode.UnUniform;
        [SerializeField] private GestureDirectionSensitivity _directionSensitivity = GestureDirectionSensitivity.Sensitive;

#if UNITY_EDITOR
        // Editor data

        /// <summary> Points inside editor window. </summary>
        [SerializeField]
        [HideInInspector]
        private List<LinePath> _editorLinesPath;

        [SerializeField]
        [HideInInspector]
        private List<Color> _editorLinesColors;

        private void OnValidate() {
            var lines = _editorLinesPath
                .Where(l => l.Path.Length > 0)
                .Select(l => new LinePath((Vector2[])l.InternalPath.Clone()))
                .ToList();

            if (lines.Count <= 0)
                return;

            // var path = new List<Vector2>(_editorPath);
            var rect = _scalingMode == GestureScalingMode.UnUniform ?
                    MultiLineGestureMath.FindGestureUnUniformRect(new ReadOnlyCollection<LinePath>(lines)) :
                    MultiLineGestureMath.FindGestureUniformRect(new ReadOnlyCollection<LinePath>(lines));

            MultiLineGestureMath.NormalizePath(rect, lines);

            for (int i = 0; i < lines.Count; i++) {
                var line = lines[i];

                for (int j = 0; j < line.InternalPath.Length; j++) {
                    var point = line.InternalPath[j];
                    line.InternalPath[j] = new Vector2(point.x, 1 - point.y);
                }
            }

            _path = lines.ToArray();
        }
#endif
    }
}