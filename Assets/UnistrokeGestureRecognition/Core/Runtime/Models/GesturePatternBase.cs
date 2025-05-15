using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace UnistrokeGestureRecognition {
    /// <summary>
    /// <inheritdoc cref="IGesturePattern"/>
    /// Implement your pattern class from this one to add data and keep editor support.
    /// </summary>
    public abstract class GesturePatternBase : ScriptableObject, IGesturePattern {
        public ReadOnlySpan<Vector2> Path => _path;
        public GestureScalingMode ScalingMode => _scalingMode;
        public GestureRotationSensitivity RotationSensitivity => _rotationSensitivity;
        public GestureDirectionSensitivity DirectionSensitivity => _directionSensitivity;

        [HideInInspector][SerializeField] private Vector2[] _path;
        [SerializeField] private GestureScalingMode _scalingMode = GestureScalingMode.UnUniform;
        [SerializeField] private GestureDirectionSensitivity _directionSensitivity = GestureDirectionSensitivity.Sensitive;
        [SerializeField] private GestureRotationSensitivity _rotationSensitivity = GestureRotationSensitivity.Sensitive;

#if UNITY_EDITOR

        // Editor data

        /// <summary>
        /// Points inside editor window.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private List<Vector2> _editorPath;

        /// <summary>
        /// Colors of lines inside editor window.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private Color _editorLineColor = Color.blue;

        /// <summary>
        /// Process editor path to normalized pattern path.
        /// </summary>
        private void OnValidate() {
            var path = new List<Vector2>(_editorPath);
            var rect = _scalingMode == GestureScalingMode.UnUniform ?
                    GestureMath.FindGestureUnUniformRect(new ReadOnlyCollection<Vector2>(path)) :
                    GestureMath.FindGestureUniformRect(new ReadOnlyCollection<Vector2>(path));

            GestureMath.NormalizePath(rect, path);

            for (int i = 0; i < path.Count; i++) {
                var point = path[i];
                path[i] = new Vector2(point.x, 1 - point.y);
            }

            _path = path.ToArray();
        }
#endif
    }
}