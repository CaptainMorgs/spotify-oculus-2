using System.Collections.Generic;
using System.Net;
using UnityEngine;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.IO;
using System.Threading;

public class Spotify : MonoBehaviour
{
    #region Variables
    public static SpotifyWebAPI _spotify;
    public GameObject playlistPrefab, recordPlayer, FeaturedPlaylistTab, CurrentSongGameObject, searchResultsTab, audioVisualizer;
    private static string clientId = "4dd553a707024f8bb4f210bb86d73ee1";
    private static string redirectUriLocal = "http://localhost";
    private FeaturedPlaylistTabScript featuredPlaylistTabScript;
    private CurrentSong currentSongScript;
    private RecordPlayer recordPlayerScript;
    private bool waitingOnRestCall = false;
    private SearchResultsScript searchResultsScript;
    private AudioVisualizer audioVisualizerScript;
    public ParticleVisualizer particleVisualizer;
    private PlaybackContext context;
    public PrivateProfile privateProfile;
    private bool shuffleState, threadRunning, saveTrackThreadRunning;
    private RepeatState repeatState;
    private AudioAnalysis audioAnalysis;
    //  private bool followPlaylistThreadRunning, saveAlbumThreadRunning, resumePlaybackRunning, skipPlaybackToNextRunning, skipPlaybackToPreviousRunning, setShuffleRunning, pausePlaybackRunning, setRepeatRunning;
    private bool followPlaylistThreadRunning, saveAlbumThreadRunning;
    public delegate void ClickAction();
    public static event ClickAction OnClicked;
    #endregion

    // Use this for initialization
    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        ImplicitGrantAuth();

        context = _spotify.GetPlayback();

        if (context != null)
        {
            Debug.Log("Device Id: " + context.Device.Id);

            shuffleState = context.ShuffleState;

            repeatState = context.RepeatState;
        }

        privateProfile = _spotify.GetPrivateProfile();

        if (privateProfile != null)
        {
            Debug.Log(privateProfile.Country);
        }

        audioVisualizer = GameObject.Find("AudioVisualizer");

        audioVisualizerScript = audioVisualizer.GetComponent<AudioVisualizer>();

        featuredPlaylistTabScript = FeaturedPlaylistTab.GetComponent<FeaturedPlaylistTabScript>();

        searchResultsScript = searchResultsTab.GetComponent<SearchResultsScript>();

        currentSongScript = CurrentSongGameObject.GetComponent<CurrentSong>();

        recordPlayerScript = recordPlayer.GetComponent<RecordPlayer>();

        //Ignore collisions between character controller and vinyls
        Physics.IgnoreLayerCollision(8, 9);

        OnClicked += SendAudioAnaylisToParticleVisualizer;
    }

    public void RestCallTest()
    {
        string endPoint = @"https://api.spotify.com/v1/me/top/artists";
        var request = (HttpWebRequest)WebRequest.Create(endPoint);

        request.Method = "GET";

        using (var response = (HttpWebResponse)request.GetResponse())
        {
            var responseValue = string.Empty;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                throw new ApplicationException(message);
            }

            // grab the response
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
            }

            var json = responseValue;
            Debug.Log(responseValue);
        }
    }

    public FeaturedPlaylists GetFeaturedPlaylists()
    {
        FeaturedPlaylists playlists = _spotify.GetFeaturedPlaylists();
        return playlists;
    }

    public FullAlbum GetAlbum(string id)
    {
        FullAlbum fullAlbum = _spotify.GetAlbum(id);
        return fullAlbum;
    }

    public NewAlbumReleases GetNewAlbumReleases()
    {
        privateProfile = _spotify.GetPrivateProfile();

        if (privateProfile == null || privateProfile.Country == null || privateProfile.Country == "")
        {
            Debug.LogError("Private profile is invalid, defaulting to ireland");

            NewAlbumReleases newAlbumReleases = _spotify.GetNewAlbumReleases("IE", 9, 0);
            return newAlbumReleases;
        }
        else
        {
            NewAlbumReleases newAlbumReleases = _spotify.GetNewAlbumReleases(privateProfile.Country, 9, 0);
            return newAlbumReleases;
        }
    }

    public Paging<FullTrack> GetUsersTopTracks()
    {
        Paging<FullTrack> usersTopTracks = _spotify.GetUsersTopTracks(TimeRangeType.ShortTerm, 9, 0);
        return usersTopTracks;
    }

    public CursorPaging<PlayHistory> GetRecentlyPlayed()
    {
        CursorPaging<PlayHistory> recentlyPlayed = _spotify.GetUsersRecentlyPlayedTracks();
        return recentlyPlayed;
    }

    /// <summary>
    /// Get user recommendations based on seeds of their top tracks and artists
    /// </summary>
    /// <param name="usersTopTracks"></param>
    /// <param name="usersTopArtists"></param>
    /// <returns></returns>
    public Recommendations GetUserRecommendations(Paging<FullTrack> usersTopTracks, Paging<FullArtist> usersTopArtists)
    {
        List<String> trackIds = new List<String>();

        List<String> artistIds = new List<String>();

        List<String> genres = new List<String>();

        for (int i = 0; i < 5; i++)
        {
            trackIds.Add(usersTopTracks.Items[i].Id);
        }

        Recommendations usersRecommendations = _spotify.GetRecommendations(trackIds);

        return usersRecommendations;
    }

    public Recommendations GetUserRecommendations(string artistId)
    {
        List<String> trackIds = new List<String>();

        List<String> artistIds = new List<String>();

        List<String> genres = new List<String>();

        artistIds.Add(artistId);

        Recommendations usersRecommendations = _spotify.GetRecommendations(artistIds);

        if (usersRecommendations.HasError())
        {
            Debug.LogError(usersRecommendations.Error.Message);
            Debug.LogError(usersRecommendations.Error.Status);

        }

        return usersRecommendations;
    }

    public Recommendations GetRecommendations(List<string> idList)
    {

        Recommendations usersRecommendations = _spotify.GetRecommendations(idList);

        if (usersRecommendations.HasError())
        {
            Debug.LogError(usersRecommendations.Error.Message);
            Debug.LogError(usersRecommendations.Error.Status);
        }

        return usersRecommendations;
    }

    public Recommendations GetRecommendationsWithTrack(List<string> trackIdList)
    {

        Recommendations usersRecommendations = _spotify.GetRecommendations(null, null, trackIdList);

        if (usersRecommendations.HasError())
        {
            Debug.LogError(usersRecommendations.Error.Message);
            Debug.LogError(usersRecommendations.Error.Status);
        }

        return usersRecommendations;
    }



    public Paging<FullArtist> GetUsersTopArtists()
    {
        Paging<FullArtist> usersTopArtists = _spotify.GetUsersTopArtists(TimeRangeType.ShortTerm, 9, 0);
        return usersTopArtists;
    }

    public FullArtist GetFullArtist(string artistID)
    {
        FullArtist artist = _spotify.GetArtist(artistID);
        return artist;
    }

    public SeveralTracks GetArtistsTopTracks(string artistID)
    {
        privateProfile = _spotify.GetPrivateProfile();

        if (privateProfile == null || privateProfile.Country == null || privateProfile.Country == "")
        {
            Debug.LogError("Invalid Private Profile, Defaulting to Ireland");
            SeveralTracks artistTopTracks = _spotify.GetArtistsTopTracks(artistID, "IE");
            return artistTopTracks;
        }
        else
        {

            SeveralTracks artistTopTracks = _spotify.GetArtistsTopTracks(artistID, privateProfile.Country);
            return artistTopTracks;
        }
    }

    public FullPlaylist GetPlaylist(string userId, string playlistId)
    {

        FullPlaylist fullPlaylist = _spotify.GetPlaylist(userId, playlistId);

        if (fullPlaylist.HasError())
        {
            Debug.LogError(fullPlaylist.Error.Message);
            Debug.LogError(fullPlaylist.Error.Status);
        }

        return fullPlaylist;
    }


    public void playSong(string songID)
    {
        PlaybackContext context = _spotify.GetPlayback();

        if (context.Device == null || context.Device.Id == null || context.Device.Id == "")
        {
            Debug.LogError("Invalid device");
            return;
        }
        else
        {
            Debug.Log("In playSong, have gotten spotify playback, device id is " + context.Device.Id);
        }

        ErrorResponse error = _spotify.ResumePlayback(uris: new List<string> { context.Device.Id, "spotify:track:4iV5W9uYEdYUVa79Axb7Rh" });

        if (error.Error != null)
        {
            Debug.LogError(error.Error.Message);
        }
    }

    public AudioAnalysis GetAudioAnalysis(String id)
    {
        return _spotify.GetAudioAnalysis(id);
    }

    public Paging<SimplePlaylist> GetUsersPlayists()
    {
        privateProfile = _spotify.GetPrivateProfile();

        if (privateProfile != null || privateProfile.Id != null || privateProfile.Id != "")
        {
            return _spotify.GetUserPlaylists(privateProfile.Id, 9, 0);
        }
        else
        {
            Debug.LogError("Private profile id not found");
            return null;
        }
    }

    public FollowedArtists GetUsersFollowedArtists()
    {
        return _spotify.GetFollowedArtists(FollowType.Artist, 9);
    }

    public FullTrack GetTrack(String id)
    {
        return _spotify.GetTrack(id);
    }

    public void PlayUri(string playlistURI)
    {
        Thread myThread = new Thread(() => PlayURIThread(playlistURI));
        myThread.Start();
        Debug.Log("PlayURIThread finished");
    }

    public void PlayURIThread(string playlistURI)
    {

        PlaybackContext context = _spotify.GetPlayback();

        if (context.Device == null || context.Device.Id == null || context.Device.Id == "")
        {
            Debug.LogError("Invalid device");
            return;
        }
        else
        {
            Debug.Log("In PlayURIThread, have gotten spotify playback, device id is " + context.Device.Id);
        }

        //    float before = Time.realtimeSinceStartup;
        ErrorResponse error = _spotify.ResumePlayback(context.Device.Id, playlistURI);
        //     Debug.Log("Time taken to play URI: " + (Time.realtimeSinceStartup - before));

        if (error.Error != null)
        {
            Debug.LogError(error.Error.Message);
            Debug.LogError(error.Error.Status);
            Debug.LogError(playlistURI);
        }
    }

    public void PlaySongUri(string songURI)
    {
        ThreadStart starter = new ThreadStart(() => PlaySongURIThread(songURI));

        Debug.Log("In PlaySongUri");

        Thread myThread = new Thread(starter);

        myThread.Start();
        threadRunning = true;
    }

    public void PlaySongURIThread(string songURI)
    {
        Debug.Log("In PlaySongUriThread");

        PlaybackContext context = _spotify.GetPlayback();


        if (context.Device == null || context.Device.Id == null || context.Device.Id == "")
        {
            Debug.LogError("Invalid device");
            threadRunning = false;
            return;
        }
        else
        {
            Debug.Log("In PlaySongUriThread, have gotten spotify playback, device id is " + context.Device.Id);
        }

        ErrorResponse error = _spotify.ResumePlayback(context.Device.Id, uris: new List<string> { songURI });

        Debug.Log("Spotify Resume Playback has been called");

        if (error.HasError())
        {
            Debug.LogError(error.Error.Message);
            Debug.LogError(error.Error.Status);
        }

        threadRunning = false;
    }

    public void PlaySongUri(string songURI, AudioAnalysisCustom audioAnalysisCustom)
    {
        ThreadStart starter = new ThreadStart(() => PlaySongURIThread(songURI));

        Thread myThread = new Thread(starter);

        myThread.Start();
        threadRunning = true;

        while (threadRunning)
        {
        }

        SendAudioAnaylisToParticleVisualizer(audioAnalysisCustom);
    }

    public void PlaySongURIWithSavedAudioAnalysisThread(string songURI)
    {

        PlaybackContext context = _spotify.GetPlayback();

        ErrorResponse error = _spotify.ResumePlayback(context.Device.Id, uris: new List<string> { songURI });

        if (error.HasError())
        {
            Debug.LogError(error.Error.Message);
            Debug.LogError(songURI);
        }
        context = _spotify.GetPlayback();
        if (context.Item != null)
        {
            Debug.Log("Currently playing song: " + context.Item.Name);
            Debug.Log("Artist: " + context.Item.Artists[0].Name);
        }
        threadRunning = false;
    }

    private void SendAudioAnaylisToParticleVisualizer()
    {
        if (audioAnalysis != null)
        {
            particleVisualizer.SendAnalysis(audioAnalysis);
        }
    }

    private void SendAudioAnaylisToParticleVisualizer(AudioAnalysisCustom audioAnalysisCustom)
    {
        if (audioAnalysisCustom != null)
        {
            particleVisualizer.SendAnalysis(audioAnalysisCustom);
        }
    }

    public void ResumePlayback()
    {
        //  if (!resumePlaybackRunning)
        //  {
        Thread myThread = new Thread(ResumePlaybackThread);
        myThread.Start();
        //    resumePlaybackRunning = true;
        //}
        //else
        //{
        //    Debug.LogError("ResumePlayback thread already running");
        //}
    }

    public void ResumePlaybackThread()
    {
        PlaybackContext context = _spotify.GetPlayback();

        if (context.Device == null || context.Device.Id == null || context.Device.Id == "")
        {
            Debug.LogError("Invalid device");
            threadRunning = false;
            return;
        }

        if (!context.IsPlaying)
        {
            ErrorResponse error = _spotify.ResumePlayback(context.Device.Id);

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
        }
        //     resumePlaybackRunning = false;
    }

    public void SkipPlaybackToNext()
    {
        //if (!skipPlaybackToNextRunning)
        //{
        Thread myThread = new Thread(SkipPlaybackToNextThread);
        myThread.Start();
        //    skipPlaybackToNextRunning = true;
        //}
        //else
        //{
        //    Debug.LogError("SkipPlaybackToNext thread already running");
        //}
    }

    public void SkipPlaybackToNextThread()
    {
        PlaybackContext context = _spotify.GetPlayback();

        if (context.Device == null || context.Device.Id == null || context.Device.Id == "")
        {
            Debug.LogError("Invalid device");
            return;
        }
        else
        {
            Debug.Log("In SkipPlaybackToNext, have gotten spotify playback, device id is " + context.Device.Id);
        }

        ErrorResponse error = _spotify.SkipPlaybackToNext();

        if (error.Error != null)
        {
            Debug.Log(error.Error.Message);
        }
        //  skipPlaybackToNextRunning = false;
    }

    public void SkipPlaybackToPrevious()
    {
        //   if (!skipPlaybackToPreviousRunning)
        //   {
        Thread myThread = new Thread(SkipPlaybackToPreviousThread);
        myThread.Start();
        //         skipPlaybackToPreviousRunning = true;
        //}
        //else
        //{
        //    Debug.LogError("SkipPlaybackToPrevious thread already running");
        //}
    }

    public void SkipPlaybackToPreviousThread()
    {
        PlaybackContext context = _spotify.GetPlayback();

        if (context.Device == null || context.Device.Id == null || context.Device.Id == "")
        {
            Debug.LogError("Invalid device");
            return;
        }
        else
        {
            Debug.Log("In SkipPlaybackToNext, have gotten spotify playback, device id is " + context.Device.Id);
        }

        if (context.IsPlaying)
        {
            ErrorResponse error = _spotify.SkipPlaybackToPrevious();

            if (error.Error != null)
            {
                Debug.LogError(error.Error.Message);
            }
        }
        else
        {
            Debug.Log("Can't skip playback to previous if not currently playing");
        }

        //    skipPlaybackToPreviousRunning = false;
    }

    public void SetShuffle()
    {
        //if (!setShuffleRunning)
        //{
        Thread myThread = new Thread(SetShuffleThread);
        myThread.Start();
        //    setShuffleRunning = true;
        //}
        //else
        //{
        //    Debug.LogError("setShuffleRunning thread already running");
        //}
    }

    public void SetShuffleThread()
    {
        context = _spotify.GetPlayback();

        shuffleState = context.ShuffleState;

        if (shuffleState)
        {
            ErrorResponse error = _spotify.SetShuffle(false);

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                shuffleState = false;
                Debug.Log("Shuffle state set to false");
            }
        }
        else
        {
            ErrorResponse error = _spotify.SetShuffle(true);

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                shuffleState = true;
                Debug.Log("Shuffle state set to true");
            }
        }
        //   setShuffleRunning = false;
    }

    public void SetRepeatMode()
    {
        //    if (!setRepeatRunning)
        //    {
        Thread myThread = new Thread(SetRepeatModeThread);
        myThread.Start();
        //    setRepeatRunning = true;
        //}
        //else
        //{
        //    Debug.LogError("SetRepeatMode thread already running");
        //}
    }

    public void SetRepeatModeThread()
    {
        context = _spotify.GetPlayback();

        repeatState = context.RepeatState;

        if (repeatState == RepeatState.Off)
        {
            ErrorResponse error = _spotify.SetRepeatMode(RepeatState.Context);

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                repeatState = RepeatState.Context;
                Debug.Log("Repeat state is context");
            }
        }
        else if (repeatState == RepeatState.Context)
        {
            ErrorResponse error = _spotify.SetRepeatMode(RepeatState.Track);

            if (error.HasError())
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                repeatState = RepeatState.Track;
                Debug.Log("Repeat state is track");

            }
        }
        else if (repeatState == RepeatState.Track)
        {
            ErrorResponse error = _spotify.SetRepeatMode(RepeatState.Off);

            if (error.HasError())
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                repeatState = RepeatState.Off;
                Debug.Log("Repeat state is off");

            }
        }
        //  setRepeatRunning = false;
    }

    public void PausePlayback()
    {
        //if (!pausePlaybackRunning)
        //{
            Thread myThread = new Thread(PausePlaybackThread);
            myThread.Start();
        //    pausePlaybackRunning = true;
        //}
        //else
        //{
        //    Debug.LogError("PausePlayback thread already running");
        //}

    }

    public void PausePlaybackThread()
    {
        PlaybackContext context = _spotify.GetPlayback();

        if (context.Device == null || context.Device.Id == null || context.Device.Id == "")
        {
            Debug.LogError("Invalid device");
            return;
        }
        else
        {
            Debug.Log("In PausePlaybackThread, have gotten spotify playback, device id is " + context.Device.Id);
        }

        if (context.IsPlaying)
        {
            ErrorResponse error = _spotify.PausePlayback(context.Device.Id);
            audioVisualizerScript.repeat = false;

            if (error.Error != null)
            {
                Debug.LogError(error.Error.Status);
                Debug.LogError(error.Error.Message);
            }
        }
     //   pausePlaybackRunning = false;

    }

    public void SaveAlbum(string id)
    {
        if (!saveAlbumThreadRunning)
        {
            Debug.Log("Starting SaveAlbum  Thread, saving album " + id);
            Thread myThread = new Thread(() => SaveAlbumThread(id));
            myThread.Start();
            saveAlbumThreadRunning = true;
        }
        else
        {
            Debug.LogError("SaveAlbum thread is already running");
        }


    }

    public void SaveAlbumThread(string id)
    {
        Debug.Log("In SaveAlbum  Thread, saving album " + id);
        ErrorResponse errorResponse = _spotify.SaveAlbum(id);

        if (errorResponse.HasError())
        {
            Debug.LogError(errorResponse.Error.Message);
            Debug.LogError(errorResponse.Error.Status);
        }
        else
        {
            Debug.Log("Saved album " + id);
        }
        saveAlbumThreadRunning = false;

    }

    public void FollowPlaylist(string ownerId, string playlistId)
    {
        if (!followPlaylistThreadRunning)
        {
            Debug.Log("Starting FollowPlaylist  Thread, Following playlist " + playlistId);
            Thread myThread = new Thread(() => FollowPlaylistThread(ownerId, playlistId));
            myThread.Start();
            followPlaylistThreadRunning = true;
        }
        else
        {
            Debug.LogError("FollowPlaylist thread is already running");
        }
    }

    public void FollowPlaylistThread(string ownerId, string playlistId)
    {
        ErrorResponse errorResponse = _spotify.FollowPlaylist(ownerId, playlistId);

        if (errorResponse.HasError())
        {
            Debug.LogError(errorResponse.Error.Message);
            Debug.LogError(errorResponse.Error.Status);
        }
        else
        {
            Debug.Log("Followed playlist " + playlistId);
        }
        followPlaylistThreadRunning = false;
    }

    public void SaveTrack(string id)
    {
        if (!saveTrackThreadRunning)
        {

            Debug.Log("Starting Save Track Thread, Saving track " + id);
            Thread myThread = new Thread(() => SaveTrackThread(id));
            myThread.Start();
            saveTrackThreadRunning = true;
        }
        else
        {
            Debug.LogError("Save track thread is already running");
        }

    }

    public void SaveTrackThread(string id)
    {
        ErrorResponse errorResponse = _spotify.SaveTrack(id);

        if (errorResponse.HasError())
        {
            Debug.LogError(errorResponse.Error.Message);
            Debug.LogError(errorResponse.Error.Status);
        }
        else
        {
            Debug.Log("Saved track " + id);
        }
        saveTrackThreadRunning = false;
    }

    public void Follow(FollowType followType, string id)
    {
        Debug.Log("Starting Follow Thread, " + followType);
        Thread myThread = new Thread(() => FollowThread(followType, id));
        myThread.Start();
    }

    public void FollowThread(FollowType followType, string id)
    {
        ErrorResponse errorResponse = _spotify.Follow(followType, id);

        if (errorResponse.HasError())
        {
            Debug.LogError(errorResponse.Error.Message);
            Debug.LogError(errorResponse.Error.Status);
        }
        else
        {
            Debug.Log("Followed artist " + id);
        }
    }

    public void SearchSpotify(string searchQuery)
    {
        if (!searchQuery.Equals(""))
        {
            SearchItem searchItem = _spotify.SearchItems(searchQuery, SearchType.Album);

            if (searchItem != null)
            {
                searchResultsScript = searchResultsTab.GetComponent<SearchResultsScript>();

                StartCoroutine(searchResultsScript.LoadSearchResults(searchItem));
            }
            else
            {
                Debug.LogError("Null search result");
            }
        }
        else
        {
            Debug.LogError("Empty search query");
        }
    }


    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    //Authenticates to Spotify
    public async void ImplicitGrantAuth()
    {
        WebAPIFactory webApiFactory = new WebAPIFactory(
            redirectUriLocal,
            8080,
            clientId,
            Scope.PlaylistModifyPrivate | Scope.PlaylistModifyPublic | Scope.PlaylistReadCollaborative |
            Scope.PlaylistReadPrivate | Scope.Streaming | Scope.UserFollowModify |
            Scope.UserFollowRead | Scope.UserLibraryModify | Scope.UserLibraryRead |
            Scope.UserModifyPlaybackState | Scope.UserReadBirthdate | Scope.UserReadEmail |
            Scope.UserReadPlaybackState | Scope.UserReadPrivate | Scope.UserReadRecentlyPlayed |
            Scope.UserTopRead,
            TimeSpan.FromSeconds(20)
        );

        try
        {
            //This will open the user's browser and returns once
            //the user is authorized
            _spotify = await webApiFactory.GetWebApi();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message.ToString());
            Debug.LogError(ex.StackTrace);
        }

        if (_spotify == null)
            return;
    }

    public async void ImplicitGrantAuthShowDialog()
    {
        WebAPIFactory webApiFactory = new WebAPIFactory(
            redirectUriLocal,
            8080,
            clientId,
            Scope.PlaylistModifyPrivate | Scope.PlaylistModifyPublic | Scope.PlaylistReadCollaborative |
            Scope.PlaylistReadPrivate | Scope.Streaming | Scope.UserFollowModify |
            Scope.UserFollowRead | Scope.UserLibraryModify | Scope.UserLibraryRead |
            Scope.UserModifyPlaybackState | Scope.UserReadBirthdate | Scope.UserReadEmail |
            Scope.UserReadPlaybackState | Scope.UserReadPrivate | Scope.UserReadRecentlyPlayed |
            Scope.UserTopRead,
            TimeSpan.FromSeconds(20),
            "&show_dialog=true"
        );

        try
        {
            //This will open the user's browser and returns once
            //the user is authorized
            _spotify = await webApiFactory.GetWebApi();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message.ToString());
            Debug.LogError(ex.StackTrace);
            ImplicitGrantAuth();
        }

        if (_spotify == null)
            return;
    }

    public void GetContext()
    {
        PlaybackContext context = _spotify.GetPlayback();
        if (context.Item != null)
        {
            Debug.Log("Device: " + context.Device.Name);
        }
        if (context.Item != null)
        {
            Debug.Log("Context: " + context.Item.Name);
        }
        else
        {
            Debug.Log("Context null with error " + context.Error.Message + " with message: " + context.Error.Message);

        }
    }

    public void TestSetup()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        ImplicitGrantAuth();
    }
}

