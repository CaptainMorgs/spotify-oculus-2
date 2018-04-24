using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;


public class TopArtistsScript : MonoBehaviour
{

    private GameObject thisGameObject;
    public MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    public Paging<FullArtist> usersTopArtists;
    public SaveLoad saveLoad;
    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

        //    StartCoroutine(loadTopArtists());

        //    LoadTopArtistsFromFile();
    }

    public void EditorLoadTopArists()
    {
        StartCoroutine(loadTopArtists());
    }

    public IEnumerator loadTopArtists()
    {
        yield return new WaitForSeconds(2);
        usersTopArtists = spotifyManagerScript.GetUsersTopArtists();
        if (usersTopArtists == null || usersTopArtists.Items.Count == 0)
        {
            Debug.LogError("usersTopArtists is null/empty");
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                string featuredPlaylistImageURL = null;

                if (usersTopArtists.Items[i].Images.Count != 0)
                {
                     featuredPlaylistImageURL = usersTopArtists.Items[i].Images[0].Url;
                }

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                WWW imageURLWWW = null;

                if (featuredPlaylistImageURL != null)
                {
                    imageURLWWW = new WWW(featuredPlaylistImageURL);

                    yield return imageURLWWW;

                    meshRenderers[i].material.mainTexture = imageURLWWW.texture;
                }

                playlistScript.setPlaylistName(usersTopArtists.Items[i].Name);
                playlistScript.setPlaylistURI(usersTopArtists.Items[i].Uri);
                playlistScript.fullArtist = usersTopArtists.Items[i];
                playlistScript.artistId = usersTopArtists.Items[i].Id;

                if (imageURLWWW != null)
                {
                    playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
                    saveLoad.SaveTextureToFilePNG(ConvertWWWToTexture(imageURLWWW), "topArtist" + i + ".png");
                }

                SeveralTracks artistTopTracks = spotifyManagerScript.GetArtistsTopTracks(usersTopArtists.Items[i].Id);
                playlistScript.audioAnalysisCustom = new AudioAnalysisCustom(spotifyManagerScript.GetAudioAnalysis(artistTopTracks.Tracks[0].Id));
                saveLoad.savedTopArtists.Add(new PlaylistScriptData(playlistScript));
            }
        }
    }

    private Sprite ConvertWWWToSprite(WWW www)
    {

        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        www.LoadImageIntoTexture(texture);

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

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

    public void LoadTopArtistsFromFile()
    {
        //   saveLoad.Load();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            PlaylistScriptData playlistScriptLoadedData = saveLoad.savedTopArtists[i];

            PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

            Sprite sprite = saveLoad.QuickLoadSpriteFromFile("topArtistSprite" + i);

            meshRenderers[i].material.mainTexture = sprite.texture;

            playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
            playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
            playlistScript.artistName = playlistScriptLoaded.artistName;
            playlistScript.sprite = saveLoad.QuickLoadSpriteFromFile("topArtistSprite" + i);
        }
    }

    public void LoadTopArtistsFromFilePNG()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        spotifyManager = GameObject.Find("SpotifyManager");
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

        if (saveLoad.savedTopArtists != null && saveLoad.savedTopArtists.Count != 0)
        {

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                PlaylistScriptData playlistScriptLoadedData = saveLoad.savedTopArtists[i];

                PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                Texture2D texture = saveLoad.LoadTextureFromFilePNG("topArtist" + i + ".png");

                meshRenderers[i].material.mainTexture = texture;

                playlistScript.artistId = playlistScriptLoaded.artistId;
                playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
                playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
                playlistScript.artistName = playlistScriptLoaded.artistName;
                playlistScript.sprite = ConvertTextureToSprite(texture);
                playlistScript.audioAnalysisCustom = playlistScriptLoaded.audioAnalysisCustom;
            }
        }
        else
        {
            Debug.LogError("savedTopArtists is empty/null");
        }
    }
}
