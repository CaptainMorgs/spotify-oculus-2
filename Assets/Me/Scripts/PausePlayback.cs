using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePlayback : MonoBehaviour {

	public GameObject SpotifyManager;
	private Spotify script;
	public GameObject recordPlayer;
	private RecordPlayer recordPlayerScript;

	// Use this for initialization
	void Start () {
		script = SpotifyManager.GetComponent<Spotify>();
		recordPlayerScript = recordPlayer.GetComponent<RecordPlayer> ();
	}

	public void PausePlaybackFunction() {
		script.PausePlayback();
	}

    void OnMouseDown()
    {
        PausePlaybackFunction();
    }

    void OnTriggerEnter(Collider collider)
    {
        PausePlaybackFunction();
    }
}
