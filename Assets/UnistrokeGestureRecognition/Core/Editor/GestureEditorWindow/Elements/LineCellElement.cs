using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnistrokeGestureRecognition.Editors.Window {
    public sealed class LineCellElement : VisualElement {
        public event Action Pressed;

        public event Action DeletePressed {
            add => _deleteButton.clicked += value;
            remove => _deleteButton.clicked -= value;
        }

        public event Action MoveUpClicked {
            add => _moveUpButton.clicked += value;
            remove => _moveUpButton.clicked -= value;
        }

        public event Action MoveDownClicked {
            add => _moveDownButton.clicked += value;
            remove => _moveDownButton.clicked -= value;
        }

        public event Action ColorChanged;

        public string Text {
            get => _lineColorPicker.label;
            set => _lineColorPicker.label = value;
        }

        public Color LineColor {
            get => _lineColorPicker.value;
            set => _lineColorPicker.value = value;
        }

        public bool IsSelected {
            get => _isSelected;
            set {
                _isSelected = value;

                if (value) {
                    AddToClassList("selected");
                }
                else {
                    RemoveFromClassList("selected");
                }
            }
        }

        private readonly ColorField _lineColorPicker;

        private readonly Button _deleteButton;
        private readonly Button _moveUpButton;
        private readonly Button _moveDownButton;

        private bool _isSelected;

        public LineCellElement() {
            AddToClassList("line-cell");

            _lineColorPicker = new ColorField();
            _lineColorPicker.AddToClassList("line-field");
            _lineColorPicker.RegisterValueChangedCallback(e => ColorChanged?.Invoke());
            Add(_lineColorPicker);

            var moveButtonsContainer = new VisualElement();

            _moveUpButton = new Button() { text = "↑" };
            _moveUpButton.AddToClassList("move-line-button");

            _moveDownButton = new Button() { text = "↓" };
            _moveDownButton.AddToClassList("move-line-button");

            moveButtonsContainer.Add(_moveUpButton);
            moveButtonsContainer.Add(_moveDownButton);
            Add(moveButtonsContainer);

            _deleteButton = new Button() { text = "✕" };
            _deleteButton.AddToClassList("delete-line-button");
            Add(_deleteButton);

            RegisterCallback<MouseDownEvent>(e => Pressed?.Invoke());
        }
    }
}