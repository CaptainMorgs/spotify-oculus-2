using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using CI.QuickSave;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SaveLoad : MonoBehaviour
{
    public GameObject topTracksGameObject;
    private TopTracksScript topTracksScript;
    public FeaturedPlaylistTabScript featuredPlaylistTabScript;
    public TopArtistsScript topArtistsScript;
    public NewAlbumReleasesScript newAlbumReleasesScript;
    public UserPlaylists userPlaylists;
    public UsersFollowedArtists usersFollowedArtistsScript;
    public ChartsScript chartScript;
    public RecentlyPlayed recentlyPlayedScript;
    public int saveTime = 120;

    public List<PlaylistScriptData> savedTopTracks = new List<PlaylistScriptData>();
    public List<PlaylistScriptData> savedFeaturedPlaylists = new List<PlaylistScriptData>();
    public List<PlaylistScriptData> savedTopArtists = new List<PlaylistScriptData>();
    public List<PlaylistScriptData> savedNewReleases = new List<PlaylistScriptData>();
    public List<PlaylistScriptData> savedUserPlaylists = new List<PlaylistScriptData>();
    public List<PlaylistScriptData> savedUserFollowedArtists = new List<PlaylistScriptData>();
    public List<PlaylistScriptData> savedChartTracks = new List<PlaylistScriptData>();
    public List<PlaylistScriptData> savedRecentlyPlayed = new List<PlaylistScriptData>();


    private QuickSaveSettings quickSaveSettings = new QuickSaveSettings();

    void Start()
    {
        quickSaveSettings.SecurityMode = SecurityMode.None;
        Stopwatch sw = new Stopwatch();
        topTracksScript = topTracksGameObject.GetComponent<TopTracksScript>();
        ClearData();
        Load();
        sw.Start();
        topTracksScript.LoadTopTracksFromFilePNG();
        featuredPlaylistTabScript.LoadFeaturedPlaylistFromFilePNG();
        topArtistsScript.LoadTopArtistsFromFilePNG();
        newAlbumReleasesScript.LoadNewReleasesFromFilePNG();
        userPlaylists.LoadUserPlaylistsFromFilePNG();
        usersFollowedArtistsScript.LoadUserFollowedArtistsFromFilePNG();
        chartScript.LoadChartTracksFromFilePNG();
        recentlyPlayedScript.LoadRecentlyPlayedFromFilePNG();
        sw.Stop();
        Debug.Log("Time taken to load top tracks: " + sw.Elapsed);
        Debug.Log("Loading from " + Application.persistentDataPath);

    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();

        //Logging the directory where files are being saved
        Debug.Log("Saving at " + Application.persistentDataPath);

        FileStream file = File.Create(Application.persistentDataPath + "/savedTopTracks.gd");
        bf.Serialize(file, savedTopTracks);
        file.Close();

        FileStream file2 = File.Create(Application.persistentDataPath + "/savedFeaturedPlaylists.gd");
        bf.Serialize(file2, savedFeaturedPlaylists);
        file2.Close();

        FileStream file3 = File.Create(Application.persistentDataPath + "/savedTopArtists.gd");
        bf.Serialize(file3, savedTopArtists);
        file3.Close();

        FileStream file4 = File.Create(Application.persistentDataPath + "/savedNewReleases.gd");
        bf.Serialize(file4, savedNewReleases);
        file4.Close();

        FileStream file5 = File.Create(Application.persistentDataPath + "/savedUserPlaylists.gd");
        bf.Serialize(file5, savedUserPlaylists);
        file5.Close();

        FileStream file6 = File.Create(Application.persistentDataPath + "/savedUserFollowedArtists.gd");
        bf.Serialize(file6, savedUserFollowedArtists);
        file6.Close();

        FileStream file7 = File.Create(Application.persistentDataPath + "/savedChartTracks.gd");
        bf.Serialize(file7, savedChartTracks);
        file7.Close();

        FileStream file8 = File.Create(Application.persistentDataPath + "/savedRecentlyPlayed.gd");
        bf.Serialize(file8, savedRecentlyPlayed);
        file8.Close();
    }

    public void Load()
    {
        Debug.Log("Loading from " + Application.persistentDataPath);
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/savedTopTracks.gd"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savedTopTracks.gd", FileMode.Open);
            savedTopTracks = (List<PlaylistScriptData>)bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/savedFeaturedPlaylists.gd"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savedFeaturedPlaylists.gd", FileMode.Open);
            savedFeaturedPlaylists = (List<PlaylistScriptData>)bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/savedTopArtists.gd"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savedTopArtists.gd", FileMode.Open);
            savedTopArtists = (List<PlaylistScriptData>)bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/savedNewReleases.gd"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savedNewReleases.gd", FileMode.Open);
            savedNewReleases = (List<PlaylistScriptData>)bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/savedUserPlaylists.gd"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savedUserPlaylists.gd", FileMode.Open);
            savedUserPlaylists = (List<PlaylistScriptData>)bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/savedUserFollowedArtists.gd"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savedUserFollowedArtists.gd", FileMode.Open);
            savedUserFollowedArtists = (List<PlaylistScriptData>)bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/savedChartTracks.gd"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savedChartTracks.gd", FileMode.Open);
            savedChartTracks = (List<PlaylistScriptData>)bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/savedRecentlyPlayed.gd"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savedRecentlyPlayed.gd", FileMode.Open);
            savedRecentlyPlayed = (List<PlaylistScriptData>)bf.Deserialize(file);
            file.Close();
        }
    }

    public void SaveWWWToFile(WWW www, string fileName)
    {
        byte[] bytes = www.bytes;
        int length = www.bytes.Length;
        Debug.Log("Saving www " + fileName + " at " + Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/" + fileName);
        BinaryWriter br = new BinaryWriter(file);
        br.Write(bytes);
        file.Close();
    }

    public WWW LoadWWWFromFile(string fileName)
    {
        Debug.Log("Loading www " + fileName + " from " + Application.persistentDataPath);
        FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
        MemoryStream ms = new MemoryStream();
        file.CopyTo(ms);
        byte[] bytes = ms.ToArray();
        BinaryFormatter bf = new BinaryFormatter();
        ms.Position = 0;
        WWW www = (WWW)bf.Deserialize(ms);
        file.Close();
        return www;
    }

    public void QuickSaveSpriteToFile(Sprite sprite, string fileName)
    {
        Debug.Log("Saving " + fileName);
        QuickSaveRoot.Save<Sprite>(fileName, sprite, quickSaveSettings);
    }

    public Sprite QuickLoadSpriteFromFile(string fileName)
    {
        Debug.Log("Loading " + fileName);
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Sprite sprite = QuickSaveRoot.Load<Sprite>(fileName, quickSaveSettings);
        sw.Stop();
        Debug.Log("Time to load sprite " + fileName + " :" + sw.Elapsed);
        return sprite;
    }

    /// <summary>
    /// Loads everything by making calls to spotify
    /// </summary>
    public void Reload()
    {
        Debug.Log("Reload called");
        ClearData();
        StartCoroutine(DelayedLoad());

        //StartCoroutine(topTracksScript.loadTopTracks());
        //StartCoroutine(topArtistsScript.loadTopArtists());
        //StartCoroutine(featuredPlaylistTabScript.loadFeaturedPlaylists());
        //StartCoroutine(newAlbumReleasesScript.loadNewAlbumReleases());
        //StartCoroutine(userPlaylists.LoadUserPlaylists());
        //StartCoroutine(usersFollowedArtistsScript.LoadUsersFollowedArtists());
        //StartCoroutine(chartScript.LoadChartTracks());
        //StartCoroutine(recentlyPlayedScript.LoadRecentlyPlayed());
    }

    private IEnumerator DelayedLoad()
    {
        StartCoroutine(topTracksScript.loadTopTracks());
        Debug.Log("Loading Top Tracks");
        yield return new WaitForSeconds(15);
        StartCoroutine(topArtistsScript.loadTopArtists());
        Debug.Log("Loading Top Artists");
        yield return new WaitForSeconds(15);
        StartCoroutine(featuredPlaylistTabScript.loadFeaturedPlaylists());
        Debug.Log("Loading Featured Playlists");
        yield return new WaitForSeconds(15);
        StartCoroutine(newAlbumReleasesScript.loadNewAlbumReleases());
        Debug.Log("Loading New Album Releases");
        yield return new WaitForSeconds(15);
        StartCoroutine(userPlaylists.LoadUserPlaylists());
        Debug.Log("Loading User Playlists");
        yield return new WaitForSeconds(15);
        StartCoroutine(usersFollowedArtistsScript.LoadUsersFollowedArtists());
        Debug.Log("Loading Users Followed Artists");
        yield return new WaitForSeconds(15);
        StartCoroutine(chartScript.LoadChartTracks());
        Debug.Log("Loading Chart Tracks");
        yield return new WaitForSeconds(15);
        StartCoroutine(recentlyPlayedScript.LoadRecentlyPlayed());
        Debug.Log("Loading Recently Played");
        yield return new WaitForSeconds(15);      
    }

    public void ClearData()
    {
        savedTopTracks.Clear();
        savedFeaturedPlaylists.Clear();
        savedTopArtists.Clear();
        savedNewReleases.Clear();
        savedUserPlaylists.Clear();
        savedUserFollowedArtists.Clear();
        savedChartTracks.Clear();
        savedRecentlyPlayed.Clear();
    }

    public void SaveTextureToFilePNG(Texture2D texture, string fileName)
    {
        var bytes = texture.EncodeToPNG();
        FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Create);
        var binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
    }

    public Texture2D LoadTextureFromFilePNG(string fileName)
    {
        FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
        MemoryStream ms = new MemoryStream();
        file.CopyTo(ms);
        byte[] bytes = ms.ToArray();
        Texture2D texture = new Texture2D(640, 640);
        texture.LoadImage(bytes);
        file.Close();
        return texture;
    }
}
