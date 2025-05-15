using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnistrokeGestureRecognition.Editors.Window {
    sealed class PointConnectorDrawer : ImmediateModeElement {
        public List<List<VisualElement>> Points { get; set; }
        public List<EditorLineData> Lines { get; set; }
        public int SelectedLineIndex { get; set; }

        public PointConnectorDrawer(List<List<VisualElement>> points, List<EditorLineData> lines) {
            Points = points;
            Lines = lines;

            this.StretchToParentSize();
        }

        protected override void ImmediateRepaint() {
            if (Lines.Count <= 0)
                return;

            if (SelectedLineIndex + 1 > Lines.Count)
                return;

            for (int j = 0; j < Lines.Count; j++) {
                var line = Lines[j];
                var linePoints = Points[j];

                if (linePoints.Count < 2)
                    continue;

                var color = line.LineColor;
                if (SelectedLineIndex != j) {
                    color.a -= 0.8f;
                }

                Handles.color = color;
                var currentPoint = linePoints[0];
                for (int i = 1; i < linePoints.Count; i++) {
                    var nextPoint = linePoints[i];
                    Handles.DrawSolidDisc(currentPoint.GetCenter(), Vector3.forward, 3f);
                    DrawLine(currentPoint.GetCenter(), nextPoint.GetCenter());
                    currentPoint = nextPoint;
                }

                Handles.DrawSolidDisc(currentPoint.GetCenter(), Vector3.forward, 3f);
            }
        }

        private void DrawLine(Vector2 positionA, Vector2 positionB) {
            EditorHelper.DrawStrongLine(positionA, positionB);
        }
    }
}