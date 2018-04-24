using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PlaylistScript))]
public class PlaylistScriptEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlaylistScript playlistScript = (PlaylistScript)target;
        if (GUILayout.Button("Play Track"))
        {
            playlistScript.PlaySomething();
        }

        if (GUILayout.Button("Spawn Vinyl"))
        {
            playlistScript.SpawnVinyl();
        }
    }
}
#endif
