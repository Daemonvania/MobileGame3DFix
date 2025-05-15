namespace UnistrokeGestureRecognition {
    public enum GestureRotationSensitivity {
        /// <summary>
        /// The input gesture will match the pattern only if it is drawn without rotation.
        /// </summary>
        Invariant,

        /// <summary>
        /// The input gesture will match the pattern even if it is drawn rotated.
        /// </summary>
        Sensitive
    }
}