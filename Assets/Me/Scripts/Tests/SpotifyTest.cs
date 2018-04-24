#if UNITY_IPHONE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using SpotifyAPI.Web.Models;

public class SpotifyTest : MonoBehaviour {

    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;

    [UnityTest]
    public IEnumerator Test_Spotify_GetTrack()
    {
        Spotify spotify = new Spotify() ;

        spotify.TestSetup();

        //Opps
        string trackId = "0Pw6Gg8QChw5iSRRSrcWXP";
        FullTrack fullTrack = spotify.GetTrack(trackId);

        yield return null;

        Debug.Log(fullTrack.Name);

        Assert.IsFalse(fullTrack.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetAlbum()
    {
        Spotify spotify = new Spotify();

        spotify.TestSetup();#if UNITY_IPHONE


        //Big Fish Theory 
        string albumId = "1qnEwPuG3l4PqRu8h3ULH7";
        FullAlbum fullAlbum= spotify.GetAlbum(albumId);

        yield return null;

        Debug.Log(fullAlbum.Name);

        Assert.IsFalse(fullAlbum.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetNewAlbumReleases()
    {
        Spotify spotify = new Spotify();

        spotify.TestSetup();

        spotify.privateProfile = new PrivateProfile();
        spotify.privateProfile.Country = "IE";

        NewAlbumReleases newAlbumReleases = spotify.GetNewAlbumReleases();

        yield return null;

        Debug.Log(newAlbumReleases.Albums.Items[0].Name);

        Assert.IsFalse(newAlbumReleases.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetUsersTopTracks()
    {
        Spotify spotify = new Spotify();

        spotify.TestSetup();

        Paging<FullTrack> usersTopTracks = spotify.GetUsersTopTracks();

        yield return null;

        Debug.Log(usersTopTracks.Items[0].Name);

        Assert.IsFalse(usersTopTracks.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetUserRecommendationsWithArtist()
    {
        Spotify spotify = new Spotify();

        spotify.TestSetup();

        //Vince Staples
        string artistId = "68kEuyFKyqrdQQLLsmiatm";

        Recommendations usersRecommendations = spotify.GetUserRecommendations(artistId);

        yield return null;

        Debug.Log(usersRecommendations.Tracks[0].Name);

        Assert.IsFalse(usersRecommendations.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetUserRecommendationsWithTrack()
    {
        Spotify spotify = new Spotify();

        spotify.TestSetup();

        //Opps
        string trackId = "0Pw6Gg8QChw5iSRRSrcWXP";

        List<string> trackIdList = new List<string>();

        trackIdList.Add(trackId);

        Recommendations usersRecommendations = spotify.GetRecommendationsWithTrack(trackIdList);

        yield return null;

        Debug.Log(usersRecommendations.Tracks[0].Name);

        Assert.IsFalse(usersRecommendations.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetUsersTopArtist()
    {
        Spotify spotify = new Spotify();

        spotify.TestSetup();

        Paging<FullArtist> usersTopArtists = spotify.GetUsersTopArtists();

        yield return null;

        Debug.Log(usersTopArtists.Items[0].Name);

        Assert.IsFalse(usersTopArtists.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetFullArtist()
    {
        Spotify spotify = new Spotify();

        spotify.TestSetup();

        //Vince Staples
        string artistId = "68kEuyFKyqrdQQLLsmiatm";

        FullArtist artist = spotify.GetFullArtist(artistId);

        yield return null;

        Debug.Log(artist.Name);

        Assert.IsFalse(artist.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetArtistsTopTracks()
    {
        Spotify spotify = new Spotify();

        spotify.privateProfile = new PrivateProfile();
        spotify.privateProfile.Country = "IE";

        spotify.TestSetup();

        //Vince Staples
        string artistId = "68kEuyFKyqrdQQLLsmiatm";

        SeveralTracks artistTopTracks = spotify.GetArtistsTopTracks(artistId);

        yield return null;

        Debug.Log(artistTopTracks.Tracks[0].Name);

        Assert.IsFalse(artistTopTracks.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetPlaylist()
    {
        Spotify spotify = new Spotify();

        spotify.TestSetup();

        //Piano in the background
        string playlistId = "37i9dQZF1DX7K31D69s4M1";

        string userId = "spotify";

        FullPlaylist fullPlaylist = spotify.GetPlaylist(userId, playlistId);
        //
        yield return null;

        Debug.Log(fullPlaylist.Name);

        Assert.IsFalse(fullPlaylist.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetAudioAnalysis()
    {
        Spotify spotify = new Spotify();

        spotify.TestSetup();

        //Opps
        string trackId = "0Pw6Gg8QChw5iSRRSrcWXP";

        AudioAnalysis audioAnalysis = spotify.GetAudioAnalysis(trackId);
       
        yield return null;

        Debug.Log(audioAnalysis.ToString());

        Assert.IsFalse(audioAnalysis.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetUsersPlayists()
    {
        Spotify spotify = new Spotify();

        spotify.privateProfile = new PrivateProfile();
        spotify.privateProfile.Id = "spotify";
        spotify.TestSetup();   

        Paging<SimplePlaylist> usersPlaylists = spotify.GetUsersPlayists();

        yield return null;

        Debug.Log(usersPlaylists.Items[0].Name);

        Assert.IsFalse(usersPlaylists.HasError());
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetUsersFollowedArtists()
    {
        Spotify spotify = new Spotify();
     
        spotify.TestSetup();

        FollowedArtists followedArtists = spotify.GetUsersFollowedArtists();

        yield return null;

        Debug.Log(followedArtists.Artists.Items[0].Name);

        Assert.IsFalse(followedArtists.HasError());
    }
}
#endif