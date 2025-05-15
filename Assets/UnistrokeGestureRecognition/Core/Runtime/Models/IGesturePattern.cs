using System;
using UnityEngine;

namespace UnistrokeGestureRecognition {
    /// <summary>
    /// Stores gesture data.
    /// </summary>
    public interface IGesturePattern {
        /// <summary>
        /// Gesture scaling mode
        /// </summary>
        GestureScalingMode ScalingMode { get; }

        /// <summary>
        /// Gesture rotation sensitivity mode
        /// </summary>
        GestureRotationSensitivity RotationSensitivity { get; }

        /// <summary>
        /// Gesture direction sensitivity mode
        /// </summary>
        GestureDirectionSensitivity DirectionSensitivity { get; }

        /// <summary>
        /// Path of the pattern.
        /// </summary>
        ReadOnlySpan<Vector2> Path { get; }
    }
}