using System;

namespace UnistrokeGestureRecognition {
    [Flags]
    internal enum GestureModifications : byte {
        None               = 0,
        UniformScaled      = 1,
        RotationSensitive  = 2,
        DirectionSensitive = 4,
    }
}