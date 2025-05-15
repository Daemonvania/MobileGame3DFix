using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnistrokeGestureRecognition.Editors.Window {
    sealed class DirectionDrawer : ImmediateModeElement {
        public List<List<VisualElement>> Points { get; set; }
        public List<EditorLineData> Lines { get; set; }
        public int SelectedLineIndex { get; set; }

        public bool IsDrawing { get; set; }

        public DirectionDrawer(List<List<VisualElement>> points, List<EditorLineData> lines) {
            Points = points;
            Lines = lines;
            this.StretchToParentSize();
        }

        protected override void ImmediateRepaint() {
            const float length = 30;
            const float angle = 70;

            for (int i = 0; i < Lines.Count; i++) {
                var lineData = Lines[i];
                var linesPoints = Points[i];

                if (!IsDrawing || linesPoints.Count < 2)
                    return;

                var color = lineData.LineColor;
                if (SelectedLineIndex != i) {
                    color.a -= 0.8f;
                }

                var v1 = linesPoints[^2].GetCenter();
                var v2 = linesPoints[^1].GetCenter();

                var dir = (v2 - v1).normalized;
                var va = EditorHelper.RotateLine(dir, v2, 90 + angle, length);
                var vb = EditorHelper.RotateLine(dir, v2, -90 - angle, length);

                Handles.color = color;
                DrawLine(v2, va);
                DrawLine(v2, vb);
            }
        }

        private void DrawLine(Vector2 positionA, Vector2 positionB) {
            EditorHelper.DrawStrongLine(positionA, positionB);
        }
    }
}