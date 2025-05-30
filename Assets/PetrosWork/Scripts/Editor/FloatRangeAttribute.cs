using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(FloatRangeAttribute))]
public class FloatRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Vector2)
        {
            EditorGUI.LabelField(position, label.text, "Use FloatRange with Vector2.");
            return;
        }

        FloatRangeAttribute range = attribute as FloatRangeAttribute;

        Vector2 rangeValue = property.vector2Value;

        EditorGUI.BeginProperty(position, label, property);

        // Draw the label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate rects
        float labelWidth = 30;
        float fieldWidth = 50;
        float spacing = 5;
        float sliderWidth = position.width - (fieldWidth + labelWidth + spacing) * 2;

        Rect minFieldRect = new Rect(position.x, position.y, fieldWidth, position.height);
        Rect sliderRect = new Rect(position.x + fieldWidth + spacing, position.y, sliderWidth, position.height);
        Rect maxFieldRect = new Rect(sliderRect.x + sliderWidth + spacing, position.y, fieldWidth, position.height);

        rangeValue.x = EditorGUI.FloatField(minFieldRect, rangeValue.x);
        rangeValue.y = EditorGUI.FloatField(maxFieldRect, rangeValue.y);

        EditorGUI.MinMaxSlider(sliderRect, ref rangeValue.x, ref rangeValue.y, range.Min, range.Max);

        // Clamp to range
        rangeValue.x = Mathf.Clamp(rangeValue.x, range.Min, range.Max);
        rangeValue.y = Mathf.Clamp(rangeValue.y, range.Min, range.Max);

        property.vector2Value = rangeValue;

        EditorGUI.EndProperty();
    }
}
