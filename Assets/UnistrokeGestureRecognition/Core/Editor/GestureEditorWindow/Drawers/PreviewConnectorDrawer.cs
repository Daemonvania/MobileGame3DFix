using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnistrokeGestureRecognition.Editors.Window {
    sealed class PreviewConnectorDrawer : ImmediateModeElement {
        public List<List<VisualElement>> Points { get; set; }
        public List<EditorLineData> Lines { get; set; }
        public int SelectedLineIndex { get; set; }

        public VisualElement Marker { get; private set; }
        public bool IsDrawing { get; set; } = false;

        public PreviewConnectorDrawer(List<List<VisualElement>> points, List<EditorLineData> lines, VisualElement marker) {
            Points = points;
            Lines = lines;
            Marker = marker;
            this.StretchToParentSize();
        }

        protected override void ImmediateRepaint() {
            if (Lines.Count <= 0 || Points.Count <= 0)
                return;

            if (SelectedLineIndex + 1 > Lines.Count)
                return;

            if (!IsDrawing || Points.Count < 1)
                return;
            if (!Marker.visible)
                return;

            var selectedLine = Lines[SelectedLineIndex];
            var selectedLinePoints = Points[SelectedLineIndex];

            if (selectedLinePoints.Count <= 0)
                return;

            var lastPoint = selectedLinePoints[^1];
            DrawLine(lastPoint.GetCenter(), Marker.GetCenter(), selectedLine.LineColor);
        }

        private void DrawLine(Vector2 positionA, Vector2 positionB, Color color) {
            color.a = 0.5f;

            Handles.color = color;
            Handles.DrawDottedLine(positionA, positionB, 1f);
        }
    }
}