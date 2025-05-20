using Microsoft.Unity.VisualStudio.Editor;
using UnistrokeGestureRecognition.Example;
using UnityEngine;

[CreateAssetMenu(fileName = "CutSO", menuName = "Scriptable Objects/CutSO")]
public class CutSO : ScriptableObject
{
    public ExampleGesturePattern pattern;
    public Sprite gestureImage;
    
}
