using UnityEngine;

namespace UnistrokeGestureRecognition.Example {
    // You can add "CreateAssetMenu" attribute here
    public class ExampleGesturePattern : GesturePatternBase {
        // Derivative classes will work with the editor and inspector 
        // Add your data here

        [SerializeField]
        private string _name;

        public string Name => _name;
    }
}