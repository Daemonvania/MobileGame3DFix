namespace UnistrokeGestureRecognition {
    /// <summary>
    /// Define path scaling strategy.
    /// </summary>
    public enum GestureScalingMode {
        /// <summary>
        // Makes the gesture sensitive to aspect ratio — 
        // the gesture will maintain its aspect ratio when resized 
        // (e.g., if the gesture is drawn as a square, it won't match a rectangular gesture pattern).
        /// </summary>
        Uniform,

        /// <summary>
        // Makes the gesture invariant to aspect ratio — 
        // the gesture will scale to a 1:1 aspect ratio 
        // (a square gesture will match a rectangular gesture pattern)
        /// </summary>
        UnUniform
    }
}