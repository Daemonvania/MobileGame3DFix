using UnistrokeGestureRecognition.Editors.Window;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnistrokeGestureRecognition.Editors.Inspector {
    abstract class GesturePatternInspectorBase : Editor {
        private const string _pathCanvasName = "pathCanvas";
        private const string _pathCanvasBackgroundName = "canvasBackground";
        private const string _openEditorWindowButtonName = "openEditorButton";

        private VisualElement _pathCanvas;
        private VisualElement _canvasBackground;

        public override VisualElement CreateInspectorGUI() {
            VisualElement inspector = new();

            InspectorElement.FillDefaultInspector(inspector, serializedObject, this);
            VisualTreeAsset uxml = Resources.Load<VisualTreeAsset>("GesturePatternInspector");
            uxml.CloneTree(inspector);

            var drawer = CreateDrawer();

            inspector.Q<Button>(_openEditorWindowButtonName).clicked += () => {
                bool windowIsOpen = EditorWindow.HasOpenInstances<GestureEditorWindow>();
                if (!windowIsOpen) {
                    GestureEditorWindow.OpenWindow();
                }
                else {
                    EditorWindow.FocusWindowIfItsOpen<GestureEditorWindow>();
                }
            };

            _pathCanvas = inspector.Q(_pathCanvasName);
            _pathCanvas.Add(drawer);

            _canvasBackground = inspector.Q(_pathCanvasBackgroundName);

            _canvasBackground.RegisterCallback<GeometryChangedEvent>(
                (e) => {
                    if (e.oldRect.width != e.newRect.height)
                        _canvasBackground.style.height = e.newRect.width;
                }
            );

            return inspector;
        }

        protected ImmediateModeElement CreateDrawer() => new InspectorPathDrawer() {
            Gesture = serializedObject
        };
    }
}