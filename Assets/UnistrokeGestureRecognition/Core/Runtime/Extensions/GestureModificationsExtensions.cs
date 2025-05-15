namespace UnistrokeGestureRecognition {
    internal static class GestureModificationsExtensions {
        public static bool HasFlag(this GestureModifications modification, GestureModifications flag) => GestureModificationsUtils.HasFlag(modification, flag);

        public static bool IsUniformScaled(this GestureModifications modification) => modification.HasFlag(GestureModifications.UniformScaled);
        public static bool IsUnUniformScaled(this GestureModifications modification) => !modification.HasFlag(GestureModifications.UniformScaled);

        public static bool IsRotationSensitive(this GestureModifications modification) => modification.HasFlag(GestureModifications.RotationSensitive);
        public static bool IsRotationInvariant(this GestureModifications modification) => !modification.HasFlag(GestureModifications.RotationSensitive);

        public static bool IsDirectionSensitive(this GestureModifications modification) => modification.HasFlag(GestureModifications.DirectionSensitive);
        public static bool IsDirectionInvariant(this GestureModifications modification) => !modification.HasFlag(GestureModifications.DirectionSensitive);
    }
}