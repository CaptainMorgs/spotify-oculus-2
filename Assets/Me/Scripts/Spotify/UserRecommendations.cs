using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;


public class UserRecommendations : MonoBehaviour
{

    private GameObject thisGameObject;
    private MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    public GameObject topTracksGameObject, topArtistsGameObject;
    private TopTracksScript topTracksScript;
    private TopArtistsScript topArtistsScript;
    private Paging<FullTrack> usersTopTracks;
    private Paging<FullArtist> usersTopArtists;
    private List<PlaylistScriptData> savedTopTracks;

    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = thisGameObject.GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();

        topTracksScript = topTracksGameObject.GetComponent<TopTracksScript>();

        topArtistsScript = topArtistsGameObject.GetComponent<TopArtistsScript>();

        //   StartCoroutine(LoadUserRecommendations());
    }

    public IEnumerator LoadUserRecommendationsWithArtist(List<string> ids)
    {

        Recommendations recommendations = spotifyManagerScript.GetRecommendations(ids);

        if (recommendations == null || recommendations.Tracks.Count == 0)
        {
            Debug.LogError("recommendations is null/empty");
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                FullTrack fullTrack = spotifyManagerScript.GetTrack(recommendations.Tracks[i].Id);

                string recommendationsImageURL = null;

                if (fullTrack.Album.Images.Count != 0)
                {
                    recommendationsImageURL = fullTrack.Album.Images[0].Url;
                }

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                WWW imageURLWWW = null;

                if (recommendationsImageURL != null)
                {
                    imageURLWWW = new WWW(recommendationsImageURL);

                    yield return imageURLWWW;

                    meshRenderers[i].material.mainTexture = imageURLWWW.texture;
                }
                

                playlistScript.setPlaylistName(fullTrack.Name);
                playlistScript.setPlaylistURI(fullTrack.Uri);
                playlistScript.trackId = fullTrack.Id;
                playlistScript.artistId = fullTrack.Artists[0].Id;
                playlistScript.trackType = PlaylistScript.TrackType.track;

                if (imageURLWWW != null)
                {
                    playlistScript.www = imageURLWWW;
                    playlistScript.sprite = Converter.ConvertWWWToSprite(imageURLWWW);
                }           
            }
        }
    }

    public IEnumerator LoadUserRecommendationsWithTrack(List<string> ids)
    {

        Recommendations recommendations = spotifyManagerScript.GetRecommendationsWithTrack(ids);

        if (recommendations == null || recommendations.Tracks.Count == 0)
        {
            Debug.LogError("recommendations is null/empty");
        }
        else
        {

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                FullTrack fullTrack = spotifyManagerScript.GetTrack(recommendations.Tracks[i].Id);

                string recommendationsImageURL = null;

                if (fullTrack.Album.Images.Count != 0)
                {
                    recommendationsImageURL = fullTrack.Album.Images[0].Url;
                }

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                WWW imageURLWWW = null;

                if (recommendationsImageURL != null)
                {
                    imageURLWWW = new WWW(recommendationsImageURL);

                    yield return imageURLWWW;

                    meshRenderers[i].material.mainTexture = imageURLWWW.texture;
                }

                playlistScript.setPlaylistName(fullTrack.Name);
                playlistScript.setPlaylistURI(fullTrack.Uri);

                if (imageURLWWW != null)
                {
                    playlistScript.www = imageURLWWW;
                    playlistScript.sprite = Converter.ConvertWWWToSprite(imageURLWWW);
                }

                playlistScript.trackId = fullTrack.Id;
                playlistScript.artistId = fullTrack.Artists[0].Id;
                playlistScript.trackType = PlaylistScript.TrackType.track;
            }
        }
    }
}
