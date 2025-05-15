namespace UnistrokeGestureRecognition {
    internal static class GestureExtension {
        public static GestureModifications GetModifications(this IGesturePattern gesture) {
            var modifications = GestureModifications.None;

            if (gesture.ScalingMode == GestureScalingMode.Uniform)
                modifications |= GestureModifications.UniformScaled;
            if (gesture.RotationSensitivity == GestureRotationSensitivity.Sensitive)
                modifications |= GestureModifications.RotationSensitive;
            if (gesture.DirectionSensitivity == GestureDirectionSensitivity.Sensitive)
                modifications |= GestureModifications.DirectionSensitive;

            return modifications;
        }
    }
}