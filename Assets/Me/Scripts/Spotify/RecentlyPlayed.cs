using SpotifyAPI.Web.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecentlyPlayed : MonoBehaviour
{

    public MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    public CursorPaging<PlayHistory> recentlyPlayed;
    public SaveLoad saveLoad;

    // Use this for initialization
    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

        //StartCoroutine(LoadRecentlyPlayed());
    }

    public IEnumerator LoadRecentlyPlayed()
    {
        yield return new WaitForSeconds(2);

        recentlyPlayed = spotifyManagerScript.GetRecentlyPlayed();

        if (recentlyPlayed == null || recentlyPlayed.Items.Count == 0)
        {
            Debug.LogError("recentlyPlayed is null/empty");
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                FullTrack fullTrack = spotifyManagerScript.GetTrack(recentlyPlayed.Items[i].Track.Id);

                string recentlyPlayedImageURL = null;

                if (fullTrack.Album.Images.Count != 0)
                {
                    recentlyPlayedImageURL = fullTrack.Album.Images[0].Url;
                }

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                WWW imageURLWWW = null;

                if (recentlyPlayedImageURL != null)
                {
                    imageURLWWW = new WWW(recentlyPlayedImageURL);

                    yield return imageURLWWW;

                    meshRenderers[i].material.mainTexture = imageURLWWW.texture;
                }
                 

                AudioAnalysis audioAnalysis = spotifyManagerScript.GetAudioAnalysis(recentlyPlayed.Items[i].Track.Id);

                playlistScript.setPlaylistName(recentlyPlayed.Items[i].Track.Name);
                playlistScript.trackType = PlaylistScript.TrackType.track;
                playlistScript.setPlaylistURI(recentlyPlayed.Items[i].Track.Uri);
                playlistScript.artistId = recentlyPlayed.Items[i].Track.Artists[0].Id;
                playlistScript.trackId = recentlyPlayed.Items[i].Track.Id;
                playlistScript.audioAnalysis = audioAnalysis;

                if (imageURLWWW != null)
                {
                    playlistScript.www = imageURLWWW;
                    playlistScript.sprite = Converter.ConvertWWWToSprite(imageURLWWW);
                    saveLoad.SaveTextureToFilePNG(Converter.ConvertWWWToTexture(imageURLWWW), "recentlyPlayed" + i + ".png");
                }

                playlistScript.audioAnalysisCustom = new AudioAnalysisCustom(audioAnalysis);
                saveLoad.savedRecentlyPlayed.Add(new PlaylistScriptData(playlistScript));
            }
        }
    }

    public void LoadRecentlyPlayedFromFilePNG()
    {
        if (saveLoad.savedRecentlyPlayed != null && saveLoad.savedRecentlyPlayed.Count != 0)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                PlaylistScriptData playlistScriptLoadedData = saveLoad.savedRecentlyPlayed[i];

                PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                Texture2D texture = saveLoad.LoadTextureFromFilePNG("recentlyPlayed" + i + ".png");

                meshRenderers[i].material.mainTexture = texture;

                playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
                playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
                playlistScript.artistName = playlistScriptLoaded.artistName;
                playlistScript.trackType = PlaylistScript.TrackType.track;
                playlistScript.sprite = Converter.ConvertTextureToSprite(texture);
                playlistScript.trackId = playlistScriptLoaded.trackId;
                playlistScript.audioAnalysisCustom = playlistScriptLoaded.audioAnalysisCustom;
            }
        }
        else
        {
            Debug.LogError("savedRecentlyPlayed is null/empty");
        }
    }
}
