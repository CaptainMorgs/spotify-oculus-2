using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SpotifyAPI.Web.Models;
using System;

public class RecommenderDeck : MonoBehaviour
{

    public List<GameObject> recommendationSeeds;

    public List<GameObject> activeSeeds;

    public UserRecommendations userRecommendations;

    private GameObject spotifyManager;

    private Spotify spotifyManagerScript;

    public List<string> artistIds = new List<string>();

    public List<string> trackIds = new List<string>();

    // Use this for initialization
    void Start()
    {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
    }

    public void GetRecommendations()
    {
        Debug.Log("GetRecommendations called");
        if (activeSeeds.Count > 0)
        {

             artistIds = new List<string>();
             trackIds = new List<string>();

            foreach (var seed in activeSeeds)
            {
                if (seed != null)
                {
                    PlaylistScript playlistScript = seed.GetComponent<VinylScript>().playlistScript;
                    if (playlistScript.trackType == PlaylistScript.TrackType.artist)
                    {
                        if (playlistScript.artistId != null && playlistScript.artistId != "")
                        {
                            artistIds.Add(playlistScript.artistId);
                        }
                        else
                        {
                            Debug.LogError("artistId null/empty");
                        }
                    }
                    else if (playlistScript.trackType == PlaylistScript.TrackType.playlist)
                    {
                        if (playlistScript.ownerId != null && playlistScript.ownerId != "" && playlistScript.playlistId != null && playlistScript.playlistId != "")
                        {
                            FullPlaylist fullPlaylist = spotifyManagerScript.GetPlaylist(playlistScript.ownerId, playlistScript.playlistId);
                            artistIds.Add(fullPlaylist.Tracks.Items[0].Track.Artists[0].Id);
                        }
                        else
                        {
                            Debug.LogError("ownerId or playlistId null/empty");
                        }

                    }
                    else if (playlistScript.trackType == PlaylistScript.TrackType.track)
                    {
                        if (playlistScript.trackId != null && playlistScript.trackId != "")
                        {
                            trackIds.Add(playlistScript.trackId);
                        }
                        else
                        {
                            Debug.LogError("trackId null/empty");
                        }
                    }
                    else if (playlistScript.trackType == PlaylistScript.TrackType.album)
                    {
                        if (playlistScript.albumId != null && playlistScript.albumId != "")
                        {
                            FullAlbum fullAlbum = spotifyManagerScript.GetAlbum(playlistScript.albumId);
                            artistIds.Add(fullAlbum.Artists[0].Id);
                        }
                        else
                        {
                            Debug.LogError("albumId null/empty");
                        }
                    }
                    else
                    {
                        Debug.LogError("Unsupported track type");
                    }
                }

                if (artistIds.Count != 0)
                {
                    StartCoroutine(userRecommendations.LoadUserRecommendationsWithArtist(artistIds));
                }
                else if (trackIds.Count != 0)
                {
                    StartCoroutine(userRecommendations.LoadUserRecommendationsWithTrack(trackIds));
                }
                else
                {
                    Debug.LogError("Seed Id list is empty");
                }
            }
        }
    }

    public void Reset()
    {
        recommendationSeeds[0].GetComponent<RecommendationSeed>().ResetText();
        recommendationSeeds[1].GetComponent<RecommendationSeed>().ResetText();

        activeSeeds.Clear();       
    }
}
