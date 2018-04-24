using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(TopTracksScript))]
public class TopTracksEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TopTracksScript topTracksScript = (TopTracksScript)target;
       
        if (GUILayout.Button("Load"))
        {
            Debug.Log("Loading Top Tracks");
            topTracksScript.EditorLoadTopTracks();
        }
    }

}
#endif
