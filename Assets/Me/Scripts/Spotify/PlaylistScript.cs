using UnityEngine;
using SpotifyAPI.Web.Models;


public class PlaylistScript : MonoBehaviour
{
    #region Variables
    public string playlistName, playlistURI, artistName, artistId, ownerId, playlistId, albumId, trackId, streams;
    public int popularity;
    public GameObject spotifyManager, particleVisualizerGameObject;
    private Spotify script;
    public GameObject playlistNameObject, recordPlayer;
    public Raycast raycast;
    private RecordPlayer recordPlayerScript;
    private GameObject audioVisualizer;
    private AudioVisualizer audioVisualizerScript;
    private MeshRenderer meshRenderer;
    public GameObject spriteGameObject;
    private SpriteRenderer spriteRenderer;
    public SimplePlaylist simplePlaylist;
    public FullTrack fullTrack;
    public SimpleAlbum simpleAlbum;
    public FullArtist fullArtist;
    public AudioAnalysis audioAnalysis;
    public Sprite sprite;
    public FullAlbum fullAlbum;
    public WWW www;
    public TrackType trackType;
    public AudioAnalysisCustom audioAnalysisCustom;
    private ParticleVisualizer particleVisualizer;
    public bool inRecommender;

    public enum TrackType
    {
        playlist, artist, track, album, searchResult
    }
    #endregion 


    // Use this for initialization
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        spotifyManager = GameObject.Find("SpotifyManager");
        script = spotifyManager.GetComponent<Spotify>();
        recordPlayerScript = recordPlayer.GetComponent<RecordPlayer>();
        audioVisualizer = GameObject.Find("AudioVisualizer");
        particleVisualizerGameObject = GameObject.Find("Visualizer Room/ParticleVisualizer");
        particleVisualizer = particleVisualizerGameObject.GetComponent<ParticleVisualizer>();
        audioVisualizerScript = audioVisualizer.GetComponent<AudioVisualizer>();
        raycast = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/LocalAvatar/controller_right").GetComponent<Raycast>();

    }

    public PlaylistScript(PlaylistScriptData playlistScriptData)
    {
        playlistName = playlistScriptData.playlistName;
        playlistURI = playlistScriptData.playlistURI;
        artistName = playlistScriptData.artistName;
        popularity = playlistScriptData.popularity;
        artistId = playlistScriptData.artistId;
        playlistId = playlistScriptData.playlistId;
        ownerId = playlistScriptData.ownerId;
        trackId = playlistScriptData.trackId;
        albumId = playlistScriptData.albumId;
        audioAnalysisCustom = playlistScriptData.audioAnalysisCustom;
        streams = playlistScriptData.streams;
    }

    public PlaylistScript()
    {
    }

    public void setPlaylistURI(string playlistURI)
    {
        this.playlistURI = playlistURI;
    }

    public void setSimplePlaylist(SimplePlaylist simplePlaylist)
    {
        this.simplePlaylist = simplePlaylist;
    }

    public void setPlaylistName(string playlistName)
    {
        this.playlistName = playlistName;
    }

    public void setFullTrack(FullTrack fullTrack)
    {
        this.fullTrack = fullTrack;
    }

    public FullTrack getFullTrack()
    {
        return fullTrack;

    }

    public SimplePlaylist getSimplePlaylist()
    {
        return simplePlaylist;

    }
    public string getPlaylistURI()
    {
        return playlistURI;
    }

    public string getPlaylistName()
    {
        return playlistName;
    }

    public void PlaySomething()
    {
        if (transform.tag == "song")
        {
            playSong();

            if (audioAnalysisCustom != null)
            {
                if (audioAnalysisCustom.beatsPerSecond != 0)
                {
                    Debug.Log("Using saved audio analysis!!");
                    particleVisualizer.SendAnalysis(audioAnalysisCustom);
                }
            }
            else
            {
                Debug.Log("Null audio analysis, stopping coroutines");
                particleVisualizer.StopAllCoroutines();
            }
        }
        else if (transform.tag == "artist")
        {
            playArtist();

            if (audioAnalysisCustom != null)
            {
                if (audioAnalysisCustom.beatsPerSecond != 0)
                {
                    Debug.Log("Using saved audio analysis!!");
                    particleVisualizer.SendAnalysis(audioAnalysisCustom);
                }
            }
            else
            {
                Debug.Log("Null audio analysis, stopping coroutines");
                particleVisualizer.StopAllCoroutines();
            }
        }
        else
        {
            playPlaylist();

            Debug.Log("No audio analysis, stopping coroutines");
            particleVisualizer.StopAllCoroutines();
        }
    }

    private void playArtist()
    {
        //just plays the artists top song
        if (artistId != "")
        {
            script = spotifyManager.GetComponent<Spotify>();
            SeveralTracks artistTopTracks = script.GetArtistsTopTracks(artistId);

            if (artistTopTracks != null)
                script.PlaySongUri(artistTopTracks.Tracks[0].Uri);
        }
        else
        {
            Debug.LogError("artistId is empty");
        }
    }

    private void playPlaylist()
    {
        script.PlayUri(playlistURI);
    }

    private void playSong()
    {
        script.PlaySongUri(playlistURI);
    }

    public void TogglePlayButton()
    {
        if (spriteRenderer.enabled == true)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
        }
    }

    void OnMouseDown()
    {
        PlaySomething();
    }

    public void SpawnVinyl()
    {
        raycast.InstansiateVinylWorking(gameObject.GetComponent<PlaylistScript>());
    }
}
