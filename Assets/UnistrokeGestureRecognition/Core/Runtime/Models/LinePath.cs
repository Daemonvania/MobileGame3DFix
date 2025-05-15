using System;
using UnityEngine;

namespace UnistrokeGestureRecognition {
    [Serializable]
    public sealed class LinePath {
        public ReadOnlySpan<Vector2> Path => _path;
        internal Vector2[] InternalPath => _path;

        [SerializeField]
        private Vector2[] _path;

        public LinePath(Vector2[] path) {
            _path = path;
        }
    }
}