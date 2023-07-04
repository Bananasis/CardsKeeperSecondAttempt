#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomPropertyDrawer(typeof(CallbackColor))]
public class TEEEST : PropertyDrawer
{
    public override void OnGUI(Rect a_Rect, SerializedProperty a_Property, GUIContent a_Label)
    {
        var prop = a_Property.FindPropertyRelative("Color");
        var color = EditorGUI.ColorField(a_Rect, a_Label, prop.colorValue);
        ((CallbackColor) a_Property.managedReferenceValue).color = color;
    }
}

[Serializable]
public class CallbackColor
{
    public readonly UnityEvent<Color> OnColorChange = new();
    [SerializeField] private Color Color;
    [SerializeField, HideInInspector] private Color _color;

    public Color color
    {
        get => _color;
        set
        {
            if (Color != value) Color = value;
            if (_color == value)
                return;
            _color = value;
            OnColorChange.Invoke(_color);
            Debug.Log("aaaaaaaaaaaaa");
        }
    }
}
#endif