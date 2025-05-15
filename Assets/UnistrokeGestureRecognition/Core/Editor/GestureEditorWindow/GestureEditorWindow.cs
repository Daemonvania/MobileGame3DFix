using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnistrokeGestureRecognition.Editors.Window {
    sealed class GestureEditorWindow : EditorWindow {
        private static readonly List<string> _dropDownChoices = new() { "8", "16", "32" };
        private static readonly List<int> _dropDownValues = new() { 8, 16, 32 };
        private static int _snapFactor = 16;
        private const float _trackAreaSize = 400f;

        private const string _addButtonName = "addLineButton";

        public VisualElement TrackArea { get; private set; }
        public VisualElement Marker { get; private set; }

        public List<List<VisualElement>> Points { get; private set; } = new();
        public List<EditorLineData> GestureLines = new();

        public int SelectedLineIndex { get; private set; }

        public PointConnectorDrawer ConnectionDrawer { get; private set; }
        public PreviewConnectorDrawer PreviewDrawer { get; private set; }
        public DirectionDrawer DirectionDrawer { get; private set; }
        public CutMarkerDrawer CutMarkerDrawer { get; private set; }

        private SerializedObject _gesture;

        private float _pointRadius = 20f;
        private Color _pointsColor = Color.white;

        private List<LineCellElement> _linesCells = new();
        private ScrollView _lineListView;
        private bool _isSnap = false;

        private IGestureEditorState _currentState;

        #region - Events -
        // Track area events
        // Pointer events 
        public event Action SelectedLineChanged;

        public event Action<PointerEnterEvent> PointerEnterTrackArea;
        public event Action<PointerLeaveEvent> PointerLeaveTrackArea;
        public event Action<PointerMoveEvent> PointerMoveAtTrackArea;

        // Mouse events
        // LB
        public event Action<MouseDownEvent> MouseLBDownAtTrackArea;
        public event Action<MouseUpEvent> MouseLBUpAtTrackArea;

        // RB
        public event Action<MouseDownEvent> MouseRBDownAtTrackArea;
        public event Action<MouseUpEvent> MouseRBUpAtTrackArea;

        // MB
        public event Action<MouseDownEvent> MouseMBDownAtTrackArea;
        public event Action<MouseUpEvent> MouseMBUpAtTrackArea;

        // Point events
        // Pointer events
        public event Action<PointerEnterEvent, VisualElement> PointerEnterPoint;
        public event Action<PointerLeaveEvent, VisualElement> PointerLeavePoint;

        // Mouse events
        // LB
        public event Action<MouseDownEvent, VisualElement> MouseLBDownAtPoint;
        public event Action<MouseUpEvent, VisualElement> MouseLBUpAtPoint;

        // RB
        public event Action<MouseDownEvent, VisualElement> MouseRBDownAtPoint;
        public event Action<MouseUpEvent, VisualElement> MouseRBUpAtPoint;
        #endregion

        public void CreateGUI() {
            InitRoot();

            InitTrackArea();
            InitMarker();

            InitDrawers();

            InitDirectionToggle();
            InitSnapDropDown();
            IntiSnapToggle();

            InitReverseButton();
            InitFlipButtons();
            InitClearPathButton();

            InitPointsSizeSlider();
            InitPointColorPicker();

            InitLineAddButton();

            _lineListView = rootVisualElement.Q<ScrollView>("linesScrollView");

            ChangeCutLabelVisibility(false);
            ChangeDragLabelVisibility(false);
            ChangeEditLabelVisibility(false);

            ChangeEditHintsVisibility(false);
            ChangeDragHintsVisibility(false);
            ChangeCutHintsVisibility(false);

            TrackArea.Add(Marker);

            InitEvents();

            ChangeState(new PointsEditState(this));

            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            OnSelectionChange();
        }

        public void ChangeState(IGestureEditorState state) {
            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        public void ShowSelectedLinePoints() {
            foreach (var point in Points[SelectedLineIndex]) {
                point.visible = true;
            }
        }

        public void HidePoints() {
            foreach (var lines in Points) {
                foreach (var point in lines) {
                    point.visible = false;
                }
            }
        }

        # region - Inits - 
        private void InitRoot() {
            var root = rootVisualElement;

            var uxml = Resources.Load<VisualTreeAsset>("GestureEditorVisualTree");
            var uss = Resources.Load<StyleSheet>("GestureEditorStyleSheet");

            uxml.CloneTree(root);
            root.styleSheets.Add(uss);
        }

        private void InitDrawers() {
            CutMarkerDrawer = new CutMarkerDrawer();
            DirectionDrawer = new DirectionDrawer(Points, GestureLines);
            ConnectionDrawer = new PointConnectorDrawer(Points, GestureLines);
            PreviewDrawer = new PreviewConnectorDrawer(Points, GestureLines, Marker);

            TrackArea.Add(ConnectionDrawer);
            TrackArea.Add(PreviewDrawer);
            TrackArea.Add(DirectionDrawer);
            TrackArea.Add(CutMarkerDrawer);
        }

        private void IntiSnapToggle() {
            var toggle = rootVisualElement.Q<Toggle>("snapToggle");
            _isSnap = toggle.value;
            toggle.RegisterValueChangedCallback((e) => _isSnap = e.newValue);
        }

        private void InitDirectionToggle() {
            var toggle = rootVisualElement.Q<Toggle>("directionToggle");
            DirectionDrawer.IsDrawing = toggle.value;
            toggle.RegisterValueChangedCallback((e) => DirectionDrawer.IsDrawing = e.newValue);
        }

        private void InitTrackArea() {
            TrackArea = rootVisualElement.Q("trackArea");
            TrackArea.Add(new GridBackgroundDrawer());
        }

        private void InitSnapDropDown() {
            var dropDown = rootVisualElement.Q<DropdownField>("snapDropdown");
            dropDown.choices = _dropDownChoices;
            dropDown.index = _dropDownValues.FindIndex((i) => i == _snapFactor);

            dropDown.RegisterValueChangedCallback((e) => _snapFactor = _dropDownValues[dropDown.index]);
        }

        private void InitPointsSizeSlider() {
            var slider = rootVisualElement.Q<Slider>("pointsSizeSlider");
            slider.RegisterValueChangedCallback(e => UpdateSize(slider.value));

            UpdateSize(slider.value);

            void UpdateSize(float size) {
                size = Mathf.Floor(Mathf.Lerp(10, 100, size / 100));

                _pointRadius = size;

                Marker.style.height = size;
                Marker.style.width = size;

                if (_gesture != null) {
                    DeleteAllPoints();
                    GetGestureEditorData();
                }
            }
        }

        private void InitPointColorPicker() {
            var picker = rootVisualElement.Q<ColorField>("pointsColorPicker");
            picker.RegisterValueChangedCallback(e => UpdateColor(picker.value));

            UpdateColor(picker.value);

            void UpdateColor(Color color) {
                _pointsColor = color;

                var markerColor = color;
                markerColor.a -= 0.5f;
                Marker.style.backgroundColor = markerColor;

                foreach (var line in Points) {
                    foreach (var item in line) {
                        item.style.backgroundColor = color;
                    }
                }
            }
        }

        private void InitClearPathButton() {
            var button = rootVisualElement.Q<Button>("clearButton");
            button.clicked += () => {
                foreach (var point in Points[SelectedLineIndex]) {
                    point.RemoveFromHierarchy();
                }

                Points[SelectedLineIndex].Clear();
                GestureLines[SelectedLineIndex].Path.Clear();

                _gesture.Update();
                GestureHelper.ClearLine(_gesture, SelectedLineIndex);
                _gesture.ApplyModifiedProperties();
            };
        }

        private void InitMarker() {
            if (Marker is not null)
                return;

            VisualElement marker = new() {
                name = "marker",
                visible = false
            };
            marker.style.position = Position.Absolute;
            Marker = marker;
        }

        private void InitReverseButton() {
            var button = rootVisualElement.Q<Button>("reverseButton");
            button.clicked += () => {
                var linePoints = Points[SelectedLineIndex];

                linePoints.Reverse();
                _gesture.Update();

                GestureHelper.ClearLine(_gesture, SelectedLineIndex);


                foreach (var point in linePoints) {
                    AddPointToPath(CalculateOriginPosition(point.transform.position, point));
                }

                _gesture.ApplyModifiedProperties();
            };
        }

        private void InitFlipButtons() {
            var flipYButton = rootVisualElement.Q<Button>("flipYButton");
            flipYButton.clicked += () => {
                _gesture.Update();

                GestureHelper.ClearLine(_gesture, SelectedLineIndex);

                var linePoints = Points[SelectedLineIndex];

                foreach (var point in linePoints) {
                    var position = FlipY(CalculateOriginPosition(point.transform.position, point));
                    AddPointToPath(position);
                    point.transform.position = CalculateCenterPosition(position, point);
                }

                _gesture.ApplyModifiedProperties();
            };

            var flipXButton = rootVisualElement.Q<Button>("flipXButton");
            flipXButton.clicked += () => {
                _gesture.Update();

                GestureHelper.ClearLine(_gesture, SelectedLineIndex);

                var linePoints = Points[SelectedLineIndex];
                foreach (var point in linePoints) {
                    var position = FlipX(CalculateOriginPosition(point.transform.position, point));
                    AddPointToPath(position);
                    point.transform.position = CalculateCenterPosition(position, point);
                }
                _gesture.ApplyModifiedProperties();
            };
        }

        private void InitLineAddButton() {
            rootVisualElement.Q<Button>(_addButtonName).clicked += () => {
                if (_gesture.targetObject is IGesturePattern)
                    return;

                _gesture.Update();
                GestureHelper.AddLine(_gesture);
                _gesture.ApplyModifiedProperties();

                OnSelectionChange();
            };
        }

        private void InitEvents() {
            TrackArea.RegisterCallback<MouseUpEvent>(OnMouseUpAtTrackArea);
            TrackArea.RegisterCallback<MouseDownEvent>(OnMouseDownAtTrackArea);
            TrackArea.RegisterCallback<PointerMoveEvent>(OnPointerMoveAtTrackArea);
            TrackArea.RegisterCallback<PointerEnterEvent>(OnPointerEnterTrackArea);
            TrackArea.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveTrackArea);
        }

        #endregion

        private void ChangeElementVisibility(VisualElement element, bool isVisible) => element.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        public void ChangeLinesRightPanelVisibility(bool isVisible) => ChangeElementVisibility(rootVisualElement.Q("linesRightPanel"), isVisible);
        public void ChangeEditLabelVisibility(bool isVisible) => ChangeElementVisibility(rootVisualElement.Q<Label>("editModeLabel"), isVisible);
        public void ChangeCutLabelVisibility(bool isVisible) => ChangeElementVisibility(rootVisualElement.Q<Label>("cutModeLabel"), isVisible);
        public void ChangeDragLabelVisibility(bool isVisible) => ChangeElementVisibility(rootVisualElement.Q<Label>("dragModeLabel"), isVisible);
        public void ChangeEditHintsVisibility(bool isVisible) => ChangeElementVisibility(rootVisualElement.Q("editModeHints"), isVisible);
        public void ChangeCutHintsVisibility(bool isVisible) => ChangeElementVisibility(rootVisualElement.Q("cutModeHints"), isVisible);
        public void ChangeDragHintsVisibility(bool isVisible) => ChangeElementVisibility(rootVisualElement.Q("dragModeHints"), isVisible);


        #region - Points -
        public VisualElement AddPoint(Vector2 position) {
            var pointPosition = CalculateCenterPosition(position, _pointRadius);
            var point = CreatePoint(pointPosition);

            Points[SelectedLineIndex].Add(point);

            AddPointToPath(position);

            return point;
        }

        public VisualElement AddPoint(Vector2 position, int index) {
            var pointPosition = CalculateCenterPosition(position, _pointRadius);
            var point = CreatePoint(pointPosition);
            Points[SelectedLineIndex].Insert(index, point);

            AddPointToPath(position, index);
            return point;
        }

        public void RemovePoint(VisualElement point) {
            var index = Points[SelectedLineIndex].IndexOf(point);
            if (index == -1)
                return;

            Points[SelectedLineIndex].Remove(point);
            point.RemoveFromHierarchy();
            GestureHelper.DeletePointAtLine(_gesture, SelectedLineIndex, index);
            _gesture.ApplyModifiedProperties();
        }

        private void AddPointToPath(Vector2 position) {
            int pathIndex = GestureHelper.Size(_gesture, SelectedLineIndex);
            GestureHelper.InsertPointToLine(_gesture, SelectedLineIndex, pathIndex, position);
            _gesture.ApplyModifiedProperties();
        }

        private void AddPointToPath(Vector2 position, int index) {
            GestureHelper.InsertPointToLine(_gesture, SelectedLineIndex, index, position);
            _gesture.ApplyModifiedProperties();
        }

        private void SetPathPointPosition(Vector2 position, int index) {
            GestureHelper.SetPointPositionInLine(_gesture, SelectedLineIndex, index, position);
        }

        public void MovePoint(VisualElement point, Vector2 position) {
            int pointIndex = Points[SelectedLineIndex].FindIndex((p) => point == p);

            if (pointIndex == -1)
                return;

            var pointPosition = CalculateCenterPosition(position, point);

            _gesture.Update();
            SetPathPointPosition(position, pointIndex);
            point.transform.position = pointPosition;
            _gesture.ApplyModifiedProperties();
        }

        private void InitPoint(Vector2 position, int lineIndex) {
            var point = CreatePoint(CalculateCenterPosition(position, _pointRadius));
            Points[lineIndex].Add(point);
        }

        private VisualElement CreatePoint(Vector2 position) {
            VisualElement point = new();
            point.AddToClassList("point");
            point.style.width = _pointRadius;
            point.style.height = _pointRadius;
            point.style.backgroundColor = _pointsColor;
            point.style.position = Position.Absolute;

            point.transform.position = position;

            point.RegisterCallback<MouseUpEvent, VisualElement>(OnMouseUpAtPoint, point);
            point.RegisterCallback<MouseDownEvent, VisualElement>(OnMouseDownAtPoint, point);
            point.RegisterCallback<PointerEnterEvent, VisualElement>(OnPointerEnterPoint, point);
            point.RegisterCallback<PointerLeaveEvent, VisualElement>(OnPointerLeavePoint, point);

            TrackArea.Add(point);

            return point;
        }

        #endregion

        #region - Event callbacks -
        private void OnPointerEnterTrackArea(PointerEnterEvent e) {
            PointerEnterTrackArea?.Invoke(e);
        }

        private void OnPointerLeaveTrackArea(PointerLeaveEvent e) {
            PointerLeaveTrackArea?.Invoke(e);
        }

        private void OnPointerMoveAtTrackArea(PointerMoveEvent e) {
            PointerMoveAtTrackArea?.Invoke(e);
        }

        private void OnMouseUpAtTrackArea(MouseUpEvent e) {
            if (e.button == 0) {
                MouseLBUpAtTrackArea?.Invoke(e);
            }
            else if (e.button == 1) {
                MouseRBUpAtTrackArea?.Invoke(e);
            }
            else if (e.button == 2) {
                MouseMBUpAtTrackArea?.Invoke(e);
            }
        }

        private void OnMouseDownAtTrackArea(MouseDownEvent e) {
            if (e.pressedButtons == 1) {

                MouseLBDownAtTrackArea?.Invoke(e);
            }
            else if (e.pressedButtons == 2) {
                MouseRBDownAtTrackArea?.Invoke(e);
            }
            else if (e.pressedButtons == 4) {
                MouseMBDownAtTrackArea?.Invoke(e);
            }
        }

        private void OnMouseDownAtPoint(MouseDownEvent e, VisualElement point) {
            if (e.pressedButtons == 1) {
                MouseLBDownAtPoint?.Invoke(e, point);
            }
            else if (e.pressedButtons == 2) {
                MouseRBDownAtPoint?.Invoke(e, point);
            }
            e.StopPropagation();
        }
        private void OnMouseUpAtPoint(MouseUpEvent e, VisualElement point) {
            if (e.pressedButtons == 1) {
                MouseLBUpAtPoint?.Invoke(e, point);
            }
            else if (e.pressedButtons == 2) {
                MouseRBUpAtPoint?.Invoke(e, point);
            }
        }
        private void OnPointerEnterPoint(PointerEnterEvent e, VisualElement point) {
            var hoverColor = _pointsColor;
            hoverColor += new Color(0.2f, 0.2f, 0.2f);
            hoverColor.a -= 0.3f;

            point.style.backgroundColor = hoverColor;
            PointerEnterPoint?.Invoke(e, point);
        }
        private void OnPointerLeavePoint(PointerLeaveEvent e, VisualElement point) {
            point.style.backgroundColor = _pointsColor;
            PointerLeavePoint?.Invoke(e, point);
        }

        #endregion

        #region - Marker -
        public void HideMarker() => Marker.visible = false;
        public void ShowMarker() => Marker.visible = true;
        public void SetMarkerPosition(Vector2 newPosition) => Marker.transform.position = CalculateCenterPosition(newPosition, Marker);
        #endregion

        #region  - Position Math -
        public Vector2 CalculateCenterPosition(Vector2 position, VisualElement element) {
            var rect = element.contentRect;
            return new Vector2(position.x - (rect.xMax * .5f), position.y - (rect.yMax * .5f));
        }

        public Vector2 CalculateCenterPosition(Vector2 newPosition, float radius) {
            return new Vector2(newPosition.x - (radius * .5f), newPosition.y - (radius * .5f));
        }

        public Vector2 CalculateOriginPosition(Vector2 position, VisualElement element) {
            var rect = element.contentRect;
            return new Vector2(position.x + (rect.xMax * .5f), position.y + (rect.yMax * .5f));
        }

        public Vector2 CalculateOriginPosition(Vector2 position, float radius) {
            return new Vector2(position.x + (radius * .5f), position.y + (radius * .5f));
        }

        public Vector2 FlipY(Vector2 position) => new(position.x, _trackAreaSize - position.y);
        public Vector2 FlipX(Vector2 position) => new(_trackAreaSize - position.x, position.y);


        public Vector2 CalculatePosition(Vector2 position) {
            if (!_isSnap)
                return position;

            Rect rect = TrackArea.contentRect;

            float xSnap = rect.xMax / _snapFactor;
            float ySnap = rect.yMax / _snapFactor;

            return new Vector2(SnapValue(position.x, xSnap), SnapValue(position.y, ySnap));
            static float SnapValue(float value, float snap) => Mathf.Round(value / snap) * snap;
        }

        #endregion

        private void EnableEditor() {
            if (_gesture.targetObject is IMultiLineGesturePattern) {
                ChangeElementVisibility(rootVisualElement.Q(_addButtonName), true);
            }
            else if (_gesture.targetObject is IGesturePattern) {
                ChangeElementVisibility(rootVisualElement.Q(_addButtonName), false);
            }

            rootVisualElement.SetEnabled(true);
            GetGestureEditorData();
            CreateLineList();
            UpdateSelectedCell(0);
        }

        private void UpdateSelectedCell(int index) {
            HidePoints();

            SelectedLineIndex = index;

            ShowSelectedLinePoints();

            for (int i = 0; i < _linesCells.Count; i++) {
                _linesCells[i].IsSelected = i == index;
            }

            ConnectionDrawer.SelectedLineIndex = SelectedLineIndex;
            PreviewDrawer.SelectedLineIndex = SelectedLineIndex;
            DirectionDrawer.SelectedLineIndex = SelectedLineIndex;
        }

        private void GetGestureEditorData() {
            var data = GestureHelper.GetGestureEditorData(_gesture);

            GestureLines.AddRange(data);

            if (Points.Capacity < data.Count)
                Points.Capacity = data.Count;

            for (int i = 0; i < data.Count; i++) {
                var currentLine = data[i];

                Points.Add(new());

                for (int j = 0; j < currentLine.Path.Count; j++) {
                    InitPoint(currentLine.Path[j], i);
                }
            }
        }

        private void DisableEditor() {
            rootVisualElement.SetEnabled(false);
            SelectedLineIndex = 0;
            DeleteAllPoints();
        }

        private void DeleteAllPoints() {
            foreach (var line in Points) {
                foreach (var point in line) {
                    point.RemoveFromHierarchy();
                }
            }

            GestureLines.Clear();
            Points.Clear();
        }

        private void OnUndoRedoPerformed() {
            if (_gesture is null)
                return;

            _gesture.Update();
            OnSelectionChange();
        }

        private void CreateLineList() {
            _lineListView.Clear();
            _linesCells.Clear();

            int index = 0;
            foreach (var line in GestureLines) {
                var cell = CreateCell(index, line);
                _lineListView.Add(cell);
                _linesCells.Add(cell);

                index++;
            }

            LineCellElement CreateCell(int index, EditorLineData line) {
                var cell = new LineCellElement() {
                    Text = (index + 1).ToString(),
                    LineColor = line.LineColor,
                };

                cell.ColorChanged += () => OnLineColorChanged(index, cell);
                cell.DeletePressed += () => OnLineDeletePressed(index);
                cell.MoveDownClicked += () => MoveLineDown(index);
                cell.MoveUpClicked += () => MoveLineUp(index);

                cell.Pressed += () => {
                    UpdateSelectedCell(index);
                    SelectedLineChanged?.Invoke();
                };

                return cell;
            }
        }

        private void MoveLineDown(int index) {
            var currentIndex = SelectedLineIndex;

            _gesture.Update();
            GestureHelper.MoveLineDown(_gesture, index);
            _gesture.ApplyModifiedProperties();
            OnSelectionChange();

            UpdateSelectedCell(currentIndex);
        }

        private void MoveLineUp(int index) {
            var currentIndex = SelectedLineIndex;

            _gesture.Update();
            GestureHelper.MoveLineUp(_gesture, index);
            _gesture.ApplyModifiedProperties();
            OnSelectionChange();

            UpdateSelectedCell(currentIndex);
        }

        private void OnLineDeletePressed(int index) {
            _gesture.Update();
            GestureHelper.DeleteLine(_gesture, index);
            _gesture.ApplyModifiedProperties();
            OnSelectionChange();
        }

        private void OnLineColorChanged(int index, LineCellElement cell) {
            var color = cell.LineColor;
            GestureLines[index].LineColor = cell.LineColor;

            _gesture.Update();
            GestureHelper.SetLineColor(_gesture, color, index);
            _gesture.ApplyModifiedProperties();
        }

        [MenuItem("Tools/Gesture Editor")]
        public static void OpenWindow() {
            GestureEditorWindow wnd = GetWindow<GestureEditorWindow>();
            wnd.titleContent = new GUIContent("Gesture Editor");
            wnd.minSize = new(920f, 580f);
        }

        public void OnSelectionChange() {
            if (Selection.activeObject is not IGesturePattern and not IMultiLineGesturePattern) {
                rootVisualElement.Unbind();

                _gesture = null;

                _lineListView.Clear();
                _linesCells.Clear();
                DisableEditor();
                return;
            }

            var so = new SerializedObject(Selection.activeObject);

            SelectedLineIndex = 0;
            DeleteAllPoints();

            rootVisualElement.Bind(so);
            _gesture = so;

            EnableEditor();
        }
    }
}