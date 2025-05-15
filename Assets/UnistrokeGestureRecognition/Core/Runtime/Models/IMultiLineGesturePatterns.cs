using System;
using UnityEngine;

namespace UnistrokeGestureRecognition {
    public interface IMultiLineGesturePattern {
        /// <summary>
        /// Gesture scaling mode
        /// </summary>
        GestureScalingMode ScalingMode { get; }


        /// <summary>
        /// Gesture direction sensitivity mode
        /// </summary>
        GestureDirectionSensitivity DirectionSensitivity { get; }

        /// <summary>
        /// Path of the pattern.
        /// </summary>
        ReadOnlySpan<LinePath> Path { get; }
    }
}