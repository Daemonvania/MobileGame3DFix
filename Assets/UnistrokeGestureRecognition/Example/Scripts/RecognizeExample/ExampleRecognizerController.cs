using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Lean.Touch;
using Unity.Mathematics;

namespace UnistrokeGestureRecognition.Example {
    public sealed class ExampleRecognizerController : MonoBehaviour {

        public delegate void OnSliceCompleted(ScreenHalf screen, bool isCorrect);

        public enum ScreenHalf
        {
            top,
            bottom,
            none
        }
        
        [SerializeField] private List<ExampleGesturePattern> _patterns;
        
        //can centralize this so its always the same
        [SerializeField, Range(0.6f, 1f)] private float _minimumScore = 0.8f;

        // [SerializeField] private PathDrawerBase _pathDrawer;
        [SerializeField] private NameController _nameController;
        [SerializeField]
         private Camera _camera;

        [Space] [SerializeField] private ScreenHalf screen;
       
        public event OnSliceCompleted onSliceCompleted;
      
        private IGestureRecorder _gestureRecorder;
        private IGestureRecognizer<ExampleGesturePattern> _recognizer;
        private JobHandle? _recognizeJob;

        private LeanFinger _trackingFinger;
        
        private ExampleGesturePattern _currentPattern;

        private void Awake() {
            _gestureRecorder = new GestureRecorder(256, 0.1f);
            _recognizer = new GestureRecognizer<ExampleGesturePattern>(_patterns, 256);
        }

        public void SetPattern(ExampleGesturePattern pattern)
        {
            _currentPattern = pattern;
        }
        private void Start() {
            // _pathDrawer.Show();

            // // Subscribe to LeanTouch events
            // LeanTouch.OnFingerDown += OnFingerDown;
            // LeanTouch.OnFingerUp += OnFingerUp;
            // LeanTouch.OnFingerUpdate += OnFingerSet;
        }

        private void OnDestroy() {
            _recognizer.Dispose();
            _gestureRecorder.Dispose();

            // Unsubscribe from events
            // LeanTouch.OnFingerDown -= OnFingerDown;
            // LeanTouch.OnFingerUp -= OnFingerUp;
            // LeanTouch.OnFingerUpdate -= OnFingerSet;
        }

        private void LateUpdate() {
            if (!_recognizeJob.HasValue)
                return;

            _recognizeJob.Value.Complete();

            RecognizeResult<ExampleGesturePattern> result = _recognizer.Result;

            if (result.Pattern == _currentPattern && result.Score >= _minimumScore) {
                onSliceCompleted?.Invoke(screen, true);
                // _nameController.Set($"{result.Pattern.Name}: {result.Score:0.00}");
            }
            else
            {
                onSliceCompleted?.Invoke(screen, false);
            }

            _recognizeJob = null;
        }

        public void OnExternalFingerDown(LeanFinger finger) {
            if (_trackingFinger == null) {
                _trackingFinger = finger;
                Clear();
            }
        }

        public void OnExternalFingerUp(LeanFinger finger) {
            if (finger == _trackingFinger) {
                if (_gestureRecorder.Length > 1) {
                    RecognizeRecordedGesture();
                }
                _trackingFinger = null;
            }
        }

        public void OnExternalFingerSet(LeanFinger finger) {
            if (finger == _trackingFinger) {
                Vector2 screenPosition = finger.ScreenPosition;

                // Convert screen position to viewport position relative to this camera
                Vector2 normalizedScreenPos = new Vector2(
                    screenPosition.x / Screen.width,
                    screenPosition.y / Screen.height
                );

                // Now convert viewport-relative position to local camera viewport space
                Rect camRect = _camera.rect;

                Vector2 adjustedViewportPos = new Vector2(
                    normalizedScreenPos.x, // No need to adjust X
                    (normalizedScreenPos.y - camRect.y) / camRect.height // Adjust Y only
                );

                Vector3 worldPoint = _camera.ViewportToWorldPoint(new Vector3(adjustedViewportPos.x, adjustedViewportPos.y, _camera.nearClipPlane + 10f)); // adjust Z as needed

                _gestureRecorder.RecordPoint(screenPosition);
                // _pathDrawer.AddPoint(worldPoint);
            }
        }

        private void RecognizeRecordedGesture() {
            // Retrieve the path
            var path = _gestureRecorder.Path;
            
            if (!IsGestureLargeEnough(path, minSize: 500f)) {
                onSliceCompleted?.Invoke(screen, false); // or just return silently
                return;
            }
            if (screen == ScreenHalf.top)
            {
                // Flip the path horizontally and vertically
                for (int i = 0; i < path.Length; i++)
                {
                    path[i] = new Vector2(Screen.width - path[i].x, Screen.height - path[i].y);
                }
            }

            _recognizeJob = _recognizer.ScheduleRecognition(path);
        }
        private bool IsGestureLargeEnough(NativeSlice<float2> path, float minSize) {
            if (path.Length < 2) return false;

            float2 min = path[0];
            float2 max = path[0];

            for (int i = 1; i < path.Length; i++) {
                min = math.min(min, path[i]);
                max = math.max(max, path[i]);
            }

            float2 size = max - min;
            return math.length(size) >= minSize;
        }
        private void Clear() {
            // _pathDrawer.Clear();
            _gestureRecorder.Reset();
        }

        private void OnValidate() {
            if (Application.isPlaying && _recognizer != null) {
                _recognizer.Dispose();
                _recognizer = new GestureRecognizer<ExampleGesturePattern>(_patterns);
            }
        }
    }
}
