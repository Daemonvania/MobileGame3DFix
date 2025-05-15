using UnityEngine;

namespace UnistrokeGestureRecognition {
    /// <summary>
    /// Stores recognition result data.
    /// </summary>
    public readonly struct RecognizeResult<G> where G : IGesturePattern {
        private static readonly float _sqrt2 = Mathf.Sqrt(2);

        /// <summary>
        /// Similarity score of pattern and gesture.
        /// A score is a value in the range of 0 to 1.
        /// Where 1 means that the recorded path is exactly the same as the pattern path.
        /// You can set the required precision value for the results.
        /// 0.8 is a good choice, but you can choose another value.
        /// </summary>
        public double Score { get; }

        /// <summary>
        /// Recognized pattern.
        /// </summary>
        public G Pattern { get; }

        public RecognizeResult(float distance, G pattern) {
            Score = 1 - distance / (0.5 * _sqrt2);
            Pattern = pattern;
        }
    }
}