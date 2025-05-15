using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnistrokeGestureRecognition {
    /// <summary>
    /// Performs gesture recognition on a set of patterns.
    /// </summary>
    /// <typeparam name="G">Gesture pattern</typeparam>
    public interface IGestureRecognizer<G> where G : IGesturePattern {
        /// <summary>
        /// Result of the last recognition.
        /// Use this field to get the result after recognition task is completed.
        /// </summary>
        RecognizeResult<G> Result { get; }

        /// <summary>
        /// Recognize gesture by given path.
        /// Copies the path to a temporary native buffer, resamples and normalizes it.
        /// The result path will be compared with patterns.
        /// Blocks the thread until recognition is complete.
        /// The first value is the recorded gesture path.
        /// </summary>
        /// <param name="path">Given path</param>
        /// <returns>Recognition result</returns>
        RecognizeResult<G> Recognize(NativeSlice<float2> path);

        /// <summary>
        /// Recognize gesture by given path.
        /// Returns recognition job handle which define work status.
        /// To get result use <see cref="Result"/> after job is completed.
        /// The first value is the recorded gesture path.
        /// </summary>
        /// <param name="path">Given path</param>
        /// <returns>Recognition job handle</returns>
        JobHandle ScheduleRecognition(NativeSlice<float2> path);

        /// <summary>
        /// Dispose all native buffers.
        /// Call when recognizer is no longer needed.
        /// Do not use recognizer after disposing.
        /// </summary>
        void Dispose();
    }
}