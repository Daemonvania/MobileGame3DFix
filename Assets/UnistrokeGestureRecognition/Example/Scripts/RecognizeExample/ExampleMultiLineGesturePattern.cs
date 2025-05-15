using UnityEngine;

namespace UnistrokeGestureRecognition.Example {
    // [CreateAssetMenu(fileName = "ExampleGesturePattern", menuName = "Example Gesture Pattern")]
    public class ExampleMultiLineGesturePattern : MultiLineGesturePatternBase {
        // Derivative classes will work with the editor and inspector 
        // Add your data here

        [SerializeField]
        private string _name;

        public string Name => _name;
    }
}