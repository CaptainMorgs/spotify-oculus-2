using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;


public class FeaturedPlaylistTabScript : MonoBehaviour
{

    private GameObject thisGameObject;
    private MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    public SaveLoad saveLoad;

    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

        //  StartCoroutine(loadFeaturedPlaylists());

        //   LoadFeaturedPlaylistFromFile();

        //   spotifyManagerScript.GetContext();
    }

    public IEnumerator loadFeaturedPlaylists()
    {
        yield return new WaitForSeconds(2);
        FeaturedPlaylists featuredPlaylists = spotifyManagerScript.GetFeaturedPlaylists();

        if (featuredPlaylists == null || featuredPlaylists.Playlists.Items.Count == 0)
        {
            Debug.LogError("featuredPlaylists is null/empty");
        }
        else
        {

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                string featuredPlaylistImageURL = featuredPlaylists.Playlists.Items[i].Images[0].Url;

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                if (meshRendererGameObject.tag != "back")
                {
                    PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                    WWW imageURLWWW = new WWW(featuredPlaylistImageURL);

                    yield return imageURLWWW;

                    meshRenderers[i].material.mainTexture = imageURLWWW.texture;

                    playlistScript.setPlaylistName(featuredPlaylists.Playlists.Items[i].Name);
                    playlistScript.setPlaylistURI(featuredPlaylists.Playlists.Items[i].Uri);
                    playlistScript.setSimplePlaylist(featuredPlaylists.Playlists.Items[i]);
                    playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
                    playlistScript.ownerId = featuredPlaylists.Playlists.Items[i].Owner.Id;
                    playlistScript.playlistId = featuredPlaylists.Playlists.Items[i].Id;

                    saveLoad.SaveTextureToFilePNG(ConvertWWWToTexture(imageURLWWW), "featuredPlaylist" + i + ".png");
                    saveLoad.savedFeaturedPlaylists.Add(new PlaylistScriptData(playlistScript));
                }
            }
        }
    }

    private Texture2D ConvertWWWToTexture(WWW www)
    {
        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        www.LoadImageIntoTexture(texture);

        return texture;
    }

    private Sprite ConvertWWWToSprite(WWW www)
    {

        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        www.LoadImageIntoTexture(texture);

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

        return spriteToUse;
    }

    private Sprite ConvertTextureToSprite(Texture2D texture)
    {
        Rect rec = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 1);
    }

    public void LoadFeaturedPlaylistFromFile()
    {

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            PlaylistScriptData playlistScriptLoadedData = saveLoad.savedFeaturedPlaylists[i];

            PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

            Sprite sprite = saveLoad.QuickLoadSpriteFromFile("featuredPlaylistSprite" + i);

            meshRenderers[i].material.mainTexture = sprite.texture;

            playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
            playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
            playlistScript.artistName = playlistScriptLoaded.artistName;
            playlistScript.sprite = saveLoad.QuickLoadSpriteFromFile("featuredPlaylistSprite" + i);
            playlistScript.playlistId = playlistScriptLoaded.playlistId;
            playlistScript.ownerId = playlistScriptLoaded.ownerId;
        }
    }

    public void LoadFeaturedPlaylistFromFilePNG()
    {
        if (saveLoad.savedFeaturedPlaylists != null && saveLoad.savedFeaturedPlaylists.Count != 0)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                PlaylistScriptData playlistScriptLoadedData = saveLoad.savedFeaturedPlaylists[i];

                PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                Texture2D texture = saveLoad.LoadTextureFromFilePNG("featuredPlaylist" + i + ".png");

                meshRenderers[i].material.mainTexture = texture;

                playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
                playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
                playlistScript.artistName = playlistScriptLoaded.artistName;
                playlistScript.sprite = ConvertTextureToSprite(texture);
                playlistScript.playlistId = playlistScriptLoaded.playlistId;
                playlistScript.ownerId = playlistScriptLoaded.ownerId;
            }
        }
        else
        {
            Debug.LogError("savedFeaturedPlaylists is empty/null");
        }
    }
}
