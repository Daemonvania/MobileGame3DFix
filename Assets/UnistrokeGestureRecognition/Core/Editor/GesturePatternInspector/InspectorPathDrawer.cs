using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnistrokeGestureRecognition.Editors.Inspector {
    sealed class InspectorPathDrawer : ImmediateModeElement {
        public SerializedObject Gesture { get; set; }

        public InspectorPathDrawer() {
            this.StretchToParentSize();
        }

        protected override void ImmediateRepaint() {
            Gesture.Update();

            if (Gesture.targetObject is IGesturePattern)
                DrawPattern();
            else if (Gesture.targetObject is IMultiLineGesturePattern)
                DrawMultiLinePattern();
        }

        private void DrawMultiLinePattern() {
            Color startColor = new Color32(146, 20, 12, 255);
            Color endColor = new Color32(12, 20, 146, 255);

            var lines = GestureHelper.GetLines(Gesture);

            if (lines.Count < 1)
                return;

            var isDrawingDirection = GestureHelper.GetDirectionSensitivity(Gesture) != GestureDirectionSensitivity.Invariant;

            for (int j = 0; j < lines.Count; j++) {
                LinePath line = lines[j];

                float t = (float)j / (lines.Count - 1);
                var lineColor = Color.Lerp(startColor, endColor, t);

                var path = line.Path;

                if (path.Length < 1)
                    continue;

                var currentPoint = NormalizedToPointYFlip(contentRect, path[0]);
                var labelPoint = currentPoint + new Vector2(0, 10);

                Handles.color = lineColor;
                Handles.DrawSolidDisc(currentPoint, Vector3.forward, 3f);

                for (int i = 1; i < path.Length; i++) {
                    var nextPoint = NormalizedToPointYFlip(contentRect, path[i]);

                    EditorHelper.DrawStrongLine(currentPoint, nextPoint);

                    currentPoint = nextPoint;
                    Handles.DrawSolidDisc(currentPoint, Vector3.forward, 3f);

                }

                if (!isDrawingDirection || path.Length < 2) {
                    Handles.Label(labelPoint, (j + 1).ToString());
                    continue;
                }

                var prevPoint = NormalizedToPointYFlip(contentRect, path[^2]);

                var dir = (currentPoint - prevPoint).normalized;
                var va = EditorHelper.RotateLine(dir, currentPoint, 90 + 70, 20);
                var vb = EditorHelper.RotateLine(dir, currentPoint, -90 - 70, 20);

                EditorHelper.DrawStrongLine(currentPoint, va);
                EditorHelper.DrawStrongLine(currentPoint, vb);

                Handles.Label(labelPoint, (j + 1).ToString());
            }
        }

        private void DrawPattern() {
            Color linesColor = new Color32(146, 20, 12, 255);

            var points = GestureHelper.GetPath(Gesture);

            if (points.Count < 1)
                return;

            var currentPoint = NormalizedToPointYFlip(contentRect, points[0]);
            Handles.color = linesColor;
            Handles.DrawSolidDisc(currentPoint, Vector3.forward, 3f);

            for (int i = 1; i < points.Count; i++) {
                var nextPoint = NormalizedToPointYFlip(contentRect, points[i]);

                EditorHelper.DrawStrongLine(currentPoint, nextPoint);

                currentPoint = nextPoint;
                Handles.DrawSolidDisc(currentPoint, Vector3.forward, 3f);
            }

            if (GestureHelper.GetDirectionSensitivity(Gesture) == GestureDirectionSensitivity.Invariant)
                return;
            if (points.Count < 2)
                return;

            var prevPoint = NormalizedToPointYFlip(contentRect, points[^2]);

            var dir = (currentPoint - prevPoint).normalized;
            var va = EditorHelper.RotateLine(dir, currentPoint, 90 + 70, 20);
            var vb = EditorHelper.RotateLine(dir, currentPoint, -90 - 70, 20);

            EditorHelper.DrawStrongLine(currentPoint, va);
            EditorHelper.DrawStrongLine(currentPoint, vb);
        }

        private Vector2 NormalizedToPointYFlip(Rect rect, Vector2 point) {
            return Rect.NormalizedToPoint(rect, new(point.x, 1 - point.y));
        }
    }
}