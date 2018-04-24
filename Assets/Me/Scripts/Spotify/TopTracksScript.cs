using System.Collections;
using System.Collections.Generic;
using System.Threading;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;

public class TopTracksScript : MonoBehaviour
{

    private GameObject thisGameObject;
    public MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    private AudioAnalysis[] audioAnalysisArray;
    public Paging<FullTrack> usersTopTracks;
    public SaveLoad saveLoad;
    public List<PlaylistScriptData> savedTopTracks;

    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

        // StartCoroutine(loadTopTracks());

        //   LoadTopTracksFromFile();
    }

    public void EditorLoadTopTracks()
    {
        StartCoroutine(loadTopTracks());
    }

    public IEnumerator loadTopTracks()
    {
        yield return new WaitForSeconds(2);

        usersTopTracks = spotifyManagerScript.GetUsersTopTracks();
        if (usersTopTracks == null || usersTopTracks.Items.Count == 0)
        {
            Debug.LogError("usersTopTracks is null/empty");
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                string topTrackImageURL = null;

                if (usersTopTracks.Items[i].Album.Images.Count != 0)
                {
                     topTrackImageURL = usersTopTracks.Items[i].Album.Images[0].Url;
                }

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                WWW imageURLWWW = null;

                if (topTrackImageURL != null)
                {
                     imageURLWWW = new WWW(topTrackImageURL);

                    yield return imageURLWWW;

                    meshRenderers[i].material.mainTexture = imageURLWWW.texture;
                }

                AudioAnalysis audioAnalysis = spotifyManagerScript.GetAudioAnalysis(usersTopTracks.Items[i].Id);

                playlistScript.setPlaylistName(usersTopTracks.Items[i].Name);
                playlistScript.setPlaylistURI(usersTopTracks.Items[i].Uri);
                playlistScript.setFullTrack(usersTopTracks.Items[i]);
                playlistScript.audioAnalysis = audioAnalysis;

                if (imageURLWWW != null)
                {
                    playlistScript.www = imageURLWWW;
                    playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
                    saveLoad.SaveTextureToFilePNG(ConvertWWWToTexture(imageURLWWW), "topTrack" + i + ".png");
                }

                playlistScript.trackId = usersTopTracks.Items[i].Id;
                playlistScript.audioAnalysisCustom =
                    new AudioAnalysisCustom(audioAnalysis);
                saveLoad.savedTopTracks.Add(new PlaylistScriptData(playlistScript));
            }
           // yield return new WaitForSeconds(saveLoad.saveTime);
           // saveLoad.Save();
        }
    }

    private Sprite ConvertWWWToSprite(WWW www)
    {
        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        www.LoadImageIntoTexture(texture);

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 1);

        return spriteToUse;
    }

    private Texture2D ConvertWWWToTexture(WWW www)
    {
        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        www.LoadImageIntoTexture(texture);

        return texture;
    }

    private Sprite ConvertTextureToSprite(Texture2D texture)
    {
        Rect rec = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 1);
    }

    public void LoadTopTracksFromFile()
    {
        savedTopTracks = saveLoad.savedTopTracks;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            PlaylistScriptData playlistScriptLoadedData = saveLoad.savedTopTracks[i];

            PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

            Sprite sprite = saveLoad.QuickLoadSpriteFromFile("topTrackSprite" + i);

            meshRenderers[i].material.mainTexture = sprite.texture;

            playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
            playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
            playlistScript.artistName = playlistScriptLoaded.artistName;
            playlistScript.sprite = sprite;
            playlistScript.trackId = playlistScriptLoaded.trackId;
            playlistScript.audioAnalysisCustom = playlistScriptLoaded.audioAnalysisCustom;
        }
    }

    public void LoadTopTracksFromFilePNG()
    {
        if (saveLoad.savedTopTracks != null && saveLoad.savedTopTracks.Count != 0)
        {

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                PlaylistScriptData playlistScriptLoadedData = saveLoad.savedTopTracks[i];

                PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                Texture2D texture = saveLoad.LoadTextureFromFilePNG("topTrack" + i + ".png");

                meshRenderers[i].material.mainTexture = texture;

                playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
                playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
                playlistScript.artistName = playlistScriptLoaded.artistName;
                playlistScript.sprite = ConvertTextureToSprite(texture);
                playlistScript.trackId = playlistScriptLoaded.trackId;
                playlistScript.audioAnalysisCustom = playlistScriptLoaded.audioAnalysisCustom;
            }
        }
        else
        {
            Debug.LogError("savedTopTracks is empty/null");
        }
    }
}
