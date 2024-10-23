using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HexCoords))]
public class HexCoordinatesDrawer : PropertyDrawer {
    public override void OnGUI (
		Rect position, SerializedProperty property, GUIContent label
	) {
        HexCoords coords = new HexCoords(
			property.FindPropertyRelative("x_").intValue,
			property.FindPropertyRelative("z_").intValue
		);
		
        EditorGUI.LabelField(position, label);
        position = EditorGUI.PrefixLabel(position, label);
		GUI.Label(position, coords.ToString());
	}
}