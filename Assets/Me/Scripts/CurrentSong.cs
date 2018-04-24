using SpotifyAPI.Web.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentSong : MonoBehaviour
{

    private GameObject spotifyManager, songNameObject;
    private Spotify spotifyScript;
    private string artistID, songID;
    private MeshRenderer meshRenderer;
    private GameObject text, audioAnalysis, artistNamePro, audioAnalysisPro;
    private UnityEngine.UI.Text artistNameText, audioAnalysisText;
    private TextMeshPro artistNameProText, audioAnalysisProText;

    // Use this for initialization
    void Start()
    {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyScript = spotifyManager.GetComponent<Spotify>();

        meshRenderer = GetComponent<MeshRenderer>();

        text = GameObject.Find("ArtistName");
        audioAnalysis = GameObject.Find("AudioAnalysis");
        artistNameText = text.GetComponent<UnityEngine.UI.Text>();
        audioAnalysisText = audioAnalysis.GetComponent<UnityEngine.UI.Text>();
    }

    public void updateCurrentlyPlaying(string artistID, string artistName, AudioAnalysis audioAnalysis)
    {
        this.artistID = artistID;
        artistNameText.text = artistName;

        audioAnalysisText.text = (" TimeSignature: " + audioAnalysis.Track.TimeSignature + "\n") + ("Tempo: " + audioAnalysis.Track.Tempo + "\n") + ("Mode: " + audioAnalysis.Track.Mode + "\n") + ("Key: " + audioAnalysis.Track.Key + "\n") + ("Duration: " + audioAnalysis.Track.Duration + "\n");

        StartCoroutine(DisplayArtist());
    }

    public void updateCurrentlyPlaying(string artistID, string artistName)
    {
        this.artistID = artistID;
        artistNameText.text = artistName;
        StartCoroutine(DisplayArtist());
    }

    private IEnumerator DisplayArtist()
    {
        FullArtist artist = spotifyScript.GetFullArtist(artistID);
        string artistImageURL = artist.Images[0].Url;
        WWW imageURLWWW = new WWW(artistImageURL);
        yield return imageURLWWW;
        meshRenderer.material.mainTexture = imageURLWWW.texture;
    }
}
