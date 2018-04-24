using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumePlayback : MonoBehaviour {

	public GameObject SpotifyManager;
	private Spotify script;
	public GameObject recordPlayer;
	private RecordPlayer recordPlayerScript;

	// Use this for initialization
	void Start () {
		 script = SpotifyManager.GetComponent<Spotify>();
		recordPlayerScript = recordPlayer.GetComponent<RecordPlayer> ();
	}

	public void ResumePlaybackFunction() {
		script.ResumePlayback();

	}

    void OnMouseDown()
    {
        ResumePlaybackFunction();
    }

    void OnTriggerEnter(Collider collider) {
        ResumePlaybackFunction();
    }
}
