namespace UnistrokeGestureRecognition {
    internal static class GestureModificationsUtils {
        public static bool HasFlag(GestureModifications modification, GestureModifications flag) => (modification & flag) == flag;

        public static bool IsUniformScaled(GestureModifications modification) => HasFlag(modification, GestureModifications.UniformScaled);
        public static bool IsUnUniformScaled(GestureModifications modification) => !HasFlag(modification, GestureModifications.UniformScaled);

        public static bool IsRotationSensitive(GestureModifications modification) => HasFlag(modification, GestureModifications.RotationSensitive);
        public static bool IsRotationInvariant(GestureModifications modification) => !HasFlag(modification, GestureModifications.RotationSensitive);

        public static bool IsDirectionSensitive(GestureModifications modification) => HasFlag(modification, GestureModifications.DirectionSensitive);
        public static bool IsDirectionInvariant(GestureModifications modification) => !HasFlag(modification, GestureModifications.DirectionSensitive);
    }
}