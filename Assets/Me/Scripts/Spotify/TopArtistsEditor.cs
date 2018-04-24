using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(TopArtistsScript))]
public class TopArtistsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TopArtistsScript topArtistsScript = (TopArtistsScript)target;
       
        if (GUILayout.Button("Load"))
        {
            Debug.Log("Loading Top Artists");
            topArtistsScript.EditorLoadTopArists();
        }
    }

}
#endif
