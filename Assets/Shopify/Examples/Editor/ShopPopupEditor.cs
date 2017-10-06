using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

[CustomEditor(typeof(ShopPopup))]
public class ShopPopupEditor : Editor {
    public override void OnInspectorGUI() {

		if (FindObjectOfType<EventSystem>() == null) {
            EditorGUILayout.Space();
            var boxRect = EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            GUI.Box(boxRect, "");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(" ", (GUIStyle) "CN EntryError", GUILayout.Width(30));
            EditorGUILayout.LabelField("No Event System Found In Scene", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            GUILayout.Space(20f);

            EditorGUILayout.LabelField(
				"No Event System found in scene. The Shop Popup needs an event system to handle input events.", 
				EditorStyles.helpBox
			);

            if (GUILayout.Button("Create Event System")) {
                var eventSystemObject = new GameObject("Event System");
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();


        }

        DrawDefaultInspector();
    }
}
