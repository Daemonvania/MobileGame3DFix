namespace UnistrokeGestureRecognition {
    public enum GestureDirectionSensitivity {
        /// <summary>
        /// The gesture can only be input from one direction
        /// </summary>
        Invariant,
        
        /// <summary>
        /// The gesture can be input starting from either the beginning or the end
        /// </summary>
        Sensitive,
    } 
}