using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnistrokeGestureRecognition.Editors {
    static class GestureHelper {
        private const string _directionSensitivityPropertyName = "_directionSensitivity";
        private const string _rotationSensitivityPropertyName = "_rotationSensitivity";
        private const string _scalingModePropertyName = "_scalingMode";

        private const string _pathPropertyName = "_path";

        private const string _editorLinesColorsPropertyName = "_editorLinesColors";
        private const string _editorLineColorPropertyName = "_editorLineColor";

        private const string _editorMultiLinePathPropertyName = "_editorLinesPath";
        private const string _editorPathPropertyName = "_editorPath";

        public static List<EditorLineData> GetGestureEditorData(SerializedObject so) {
            if (so.targetObject is IGesturePattern) {
                return new List<EditorLineData> {
                    new() { Path = GetEditorPath(so, 0), LineColor = GetLineColor(so, 0)}};
            }
            else if (so.targetObject is IMultiLineGesturePattern) {
                var lines = GetMultiLinePathFromProperty(GetEditorMultiLinPathProperty(so));
                var data = new List<EditorLineData>();

                for (int i = 0; i < lines.Count; i++) {
                    var item = lines[i];
                    var color = GetLineColor(so, i);

                    var linePath = new List<Vector2>();
                    foreach (var point in item.Path) {
                        linePath.Add(point);
                    }

                    data.Add(new EditorLineData() { LineColor = color, Path = linePath });
                }

                if (data.Count == 0) {
                    data.Add(new EditorLineData() { LineColor = Color.white, Path = new() });
                }

                return data;
            }

            return null;
        }

        public static int Size(SerializedObject so, int index) {
            if (so.targetObject is IGesturePattern) {
                return GetEditorPathProperty(so).arraySize;
            }
            else if (so.targetObject is IMultiLineGesturePattern) {
                var lines = GetEditorMultiLinPathProperty(so);

                if (lines.arraySize == 0)
                    return 0;

                if (lines.GetArrayElementAtIndex(index).boxedValue is not LinePath line)
                    return 0;
                return line.Path.Length;
            }

            return 0;
        }

        public static void ClearLine(SerializedObject so, int index) {
            if (so.targetObject is IGesturePattern) {
                GetEditorPathProperty(so).ClearArray();
                return;
            }
            else if (so.targetObject is IMultiLineGesturePattern) {
                var lines = GetEditorMultiLinPathProperty(so);

                if (lines.GetArrayElementAtIndex(index).boxedValue is not LinePath)
                    return;

                lines.GetArrayElementAtIndex(index).boxedValue = new LinePath(new Vector2[0]);
            }
        }

        public static void DeletePointAtLine(SerializedObject so, int lineIndex, int index) {
            if (so.targetObject is IGesturePattern) {
                GetEditorPathProperty(so).DeleteArrayElementAtIndex(index);
                return;
            }
            else if (so.targetObject is IMultiLineGesturePattern) {
                var lines = GetEditorMultiLinPathProperty(so);

                if (lines.GetArrayElementAtIndex(lineIndex).boxedValue is not LinePath line)
                    return;

                var points = new List<Vector2>();
                for (int i = 0; i < line.Path.Length; i++) {
                    if (index == i)
                        continue;
                    points.Add(line.Path[i]);
                }

                lines.GetArrayElementAtIndex(lineIndex).boxedValue = new LinePath(points.ToArray());

                return;
            }
        }

        public static void InsertPointToLine(SerializedObject so, int lineIndex, int index, Vector2 position) {
            if (so.targetObject is IGesturePattern) {
                var sp = GetEditorPathProperty(so);
                sp.InsertArrayElementAtIndex(index);
                sp.GetArrayElementAtIndex(index).vector2Value = position;
                return;
            }
            else if (so.targetObject is IMultiLineGesturePattern) {
                var lines = GetEditorMultiLinPathProperty(so);

                if (lines.arraySize == 0) {
                    lines.InsertArrayElementAtIndex(lineIndex);
                    lines.GetArrayElementAtIndex(index).boxedValue = new LinePath(new Vector2[0]);
                }

                if (lines.GetArrayElementAtIndex(lineIndex).boxedValue is not LinePath line)
                    return;

                var points = new List<Vector2>();

                if (line.Path.Length == 0) {
                    points.Add(position);
                }
                else {
                    for (int i = 0; i < line.Path.Length; i++) {
                        if (index == i)
                            points.Add(position);
                        points.Add(line.Path[i]);
                    }

                    if (line.Path.Length < index + 1) {
                        points.Add(position);
                    }
                }

                lines.GetArrayElementAtIndex(lineIndex).boxedValue = new LinePath(points.ToArray());
            }
        }

        public static void SetPointPositionInLine(SerializedObject so, int lineIndex, int index, Vector2 position) {
            if (so.targetObject is IGesturePattern) {
                GetEditorPathProperty(so).GetArrayElementAtIndex(index).vector2Value = position;
                return;
            }
            else if (so.targetObject is IMultiLineGesturePattern) {
                var lines = GetEditorMultiLinPathProperty(so);

                if (lines.GetArrayElementAtIndex(lineIndex).boxedValue is not LinePath line)
                    return;

                var points = line.Path.ToArray();
                points[index] = position;

                lines.GetArrayElementAtIndex(lineIndex).boxedValue = new LinePath(points);
            }
        }

        public static void AddLine(SerializedObject so) {
            if (so.targetObject is not IMultiLineGesturePattern)
                return;
            var lines = GetEditorMultiLinPathProperty(so);

            var insertIndex = lines.arraySize;

            lines.InsertArrayElementAtIndex(insertIndex);
            lines.GetArrayElementAtIndex(insertIndex).boxedValue = new LinePath(new Vector2[0]);

            var colors = GetEditorLinesColorsProperty(so);

            colors.InsertArrayElementAtIndex(insertIndex);
            colors.GetArrayElementAtIndex(insertIndex).colorValue = Color.white;
        }

        public static void DeleteLine(SerializedObject so, int index) {
            if (so.targetObject is not IMultiLineGesturePattern)
                return;

            var lines = GetEditorMultiLinPathProperty(so);
            if (lines.arraySize <= 1)
                return;

            lines.DeleteArrayElementAtIndex(index);

            var colors = GetEditorLinesColorsProperty(so);
            colors.DeleteArrayElementAtIndex(index);
        }

        public static void MoveLineDown(SerializedObject so, int index) {
            if (so.targetObject is not IMultiLineGesturePattern)
                return;

            var lines = GetEditorMultiLinPathProperty(so);
            if (lines.arraySize <= 1)
                return;

            lines.MoveArrayElement(index, index + 1);

            var colors = GetEditorLinesColorsProperty(so);
            colors.MoveArrayElement(index, index + 1);
        }

        public static void MoveLineUp(SerializedObject so, int index) {
            if (so.targetObject is not IMultiLineGesturePattern)
                return;

            var lines = GetEditorMultiLinPathProperty(so);
            if (lines.arraySize <= 1)
                return;

            lines.MoveArrayElement(index, index - 1);

            var colors = GetEditorLinesColorsProperty(so);
            colors.MoveArrayElement(index, index - 1);
        }

        public static List<Vector2> GetEditorPath(SerializedObject so, int index) {
            if (so.targetObject is IGesturePattern) {
                var sp = GetEditorPathProperty(so);
                return GetPathFromProperty(sp);
            }

            return null;
        }

        public static List<LinePath> GetLines(SerializedObject so) {
            var sp = so.FindProperty(_pathPropertyName);
            return GetMultiLinePathFromProperty(sp);
        }

        public static List<Vector2> GetPath(SerializedObject so) {
            var sp = so.FindProperty(_pathPropertyName);
            return GetPathFromProperty(sp);
        }

        public static GestureRotationSensitivity GetRotationSensitivity(SerializedObject so) {
            var sp = so.FindProperty(_rotationSensitivityPropertyName);
            return (GestureRotationSensitivity)sp.enumValueIndex;
        }

        public static GestureDirectionSensitivity GetDirectionSensitivity(SerializedObject so) {
            var sp = so.FindProperty(_directionSensitivityPropertyName);
            return (GestureDirectionSensitivity)sp.enumValueIndex;
        }

        public static GestureScalingMode GetScalingMode(SerializedObject so) {
            var sp = so.FindProperty(_scalingModePropertyName);
            return (GestureScalingMode)sp.enumValueIndex;
        }

        public static Color GetLineColor(SerializedObject so, int index) {
            if (so.targetObject is IGesturePattern) {
                var sp = so.FindProperty(_editorLineColorPropertyName);
                return sp.colorValue;
            }
            else if (so.targetObject is IMultiLineGesturePattern) {
                var sp = GetEditorLinesColorsProperty(so);

                if (sp.arraySize <= 0)
                    return Color.white;

                try {
                    return sp.GetArrayElementAtIndex(index).colorValue;
                }
                catch {
                    return Color.white;
                }
            }

            return Color.white;
        }

        public static void SetLineColor(SerializedObject so, Color lineColor, int index) {
            if (so.targetObject is IGesturePattern) {
                var sp = so.FindProperty(_editorLineColorPropertyName);
                sp.colorValue = lineColor;
                return;
            }
            else if (so.targetObject is IMultiLineGesturePattern) {
                var colors = GetEditorLinesColorsProperty(so);

                if (colors.arraySize <= 0) {
                    colors.InsertArrayElementAtIndex(index);
                }

                colors.GetArrayElementAtIndex(index).colorValue = lineColor;
            }
        }

        private static SerializedProperty GetEditorLinesColorsProperty(SerializedObject so) {
            return so.FindProperty(_editorLinesColorsPropertyName);
        }

        private static SerializedProperty GetEditorMultiLinPathProperty(SerializedObject so) {
            return so.FindProperty(_editorMultiLinePathPropertyName);
        }

        private static List<LinePath> GetMultiLinePathFromProperty(SerializedProperty sp) {
            var size = sp.arraySize;
            var path = new List<LinePath>(size);

            for (int i = 0; i < size; i++) {
                path.Add((LinePath)sp.GetArrayElementAtIndex(i).boxedValue);
            }

            return path;
        }

        private static SerializedProperty GetEditorPathProperty(SerializedObject so) {
            return so.FindProperty(_editorPathPropertyName);
        }

        private static List<Vector2> GetPathFromProperty(SerializedProperty sp) {
            var size = sp.arraySize;
            var path = new List<Vector2>(size);

            for (int i = 0; i < size; i++) {
                path.Add(sp.GetArrayElementAtIndex(i).vector2Value);
            }

            return path;
        }
    }
}