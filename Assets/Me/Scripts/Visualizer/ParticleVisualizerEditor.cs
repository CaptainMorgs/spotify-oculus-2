using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ParticleVisualizer))]
public class ParticleVisualizerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ParticleVisualizer particleVisualizer = (ParticleVisualizer)target;

        string[] options = new string[]
        {
         "Spark", "Lightning",
        };

        particleVisualizer.selected = EditorGUILayout.Popup("Label", particleVisualizer.selected, options);

        if (GUILayout.Button("Change Material"))
        {
            particleVisualizer.ChangeMaterial(particleVisualizer.selected);
        }
    }
}
#endif

