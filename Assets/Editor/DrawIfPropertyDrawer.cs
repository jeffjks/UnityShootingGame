﻿using UnityEditor;
using UnityEngine;
 
/// <summary>
/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// </summary>
[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfPropertyDrawer : PropertyDrawer
{
    #region Fields
 
    // Reference to the attribute on the property.
    DrawIfAttribute drawIf;
 
    // Field that is being compared.
    SerializedProperty comparedField;
 
    #endregion
 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShowMe(property) && drawIf.disablingType == DisablingType.DontDraw)
            return 0f;
   
        // The height of the property should be defaulted to the default height.
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
 
    /// <summary>
    /// Errors default to showing the property.
    /// </summary>
    private bool ShowMe(SerializedProperty property)
    {
        drawIf = attribute as DrawIfAttribute;
        // Replace propertyname to the value from the parameter
        string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName) : drawIf.comparedPropertyName;
 
        comparedField = property.serializedObject.FindProperty(path);
 
        if (comparedField == null)
        {
            //Debug.LogError("Cannot find property with name: " + path);
            return true;
        }

        // Compare the values to see if the condition is met.
        switch (drawIf.comparisonType)
        {
            case ComparisonType.Equals:
                if (comparedField.type == "bool") {
                    return comparedField.boolValue.Equals(drawIf.comparedValue);
                }
                else if (comparedField.type == "Enum") {
                    return comparedField.enumValueIndex.Equals((int)drawIf.comparedValue);
                }
                else {
                    Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                    return true;
                }
 
            case ComparisonType.NotEqual:
                if (comparedField.type == "bool") {
                    return !comparedField.boolValue.Equals(drawIf.comparedValue);
                }
                else if (comparedField.type == "Enum") {
                    return !comparedField.enumValueIndex.Equals((int)drawIf.comparedValue);
                }
                else {
                    Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                    return true;
                }
        }
        return true;

        /*
        // get the value & compare based on types
        switch (comparedField.type)
        { // Possible extend cases to support your own type
            case "bool":
                return comparedField.boolValue.Equals(drawIf.comparedValue);
            case "Enum":
                return comparedField.enumValueIndex.Equals((int)drawIf.comparedValue);
            default:
                Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                return true;
        }*/
    }
 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // If the condition is met, simply draw the field.
        if (ShowMe(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        } //...check if the disabling type is read only. If it is, draw it disabled
        else if (drawIf.disablingType == DisablingType.ReadOnly)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
 
}