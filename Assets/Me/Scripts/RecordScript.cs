using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models;

public class RecordScript : MonoBehaviour {

	private string songURI;
	private string songName;

	private GameObject spotifyManager;
	private Spotify script;

	// Use this for initialization
	void Start () {
		spotifyManager = GameObject.Find ("SpotifyManager");
		script = spotifyManager.GetComponent<Spotify>();
	}
	
	public RecordScript(string songURI, string songName) {
		this.songURI = songURI;
		this.songName = songName;
	}

	public void setSongURI (string songURI) {
		this.songURI = songURI;
	}

	public void setSongName (string songURI) {
		this.songName = songName;
	}

	public void playSong() {	 
		script.playSong (songURI);
	}

	public void pauseSong() {
	
	}
}
