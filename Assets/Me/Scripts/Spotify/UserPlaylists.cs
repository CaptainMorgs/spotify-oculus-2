using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;


public class UserPlaylists : MonoBehaviour
{

    private GameObject thisGameObject;
    private MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    private SaveLoad saveLoad;

    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = thisGameObject.GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

        //  StartCoroutine(LoadUserPlaylists());
    }

    public IEnumerator LoadUserPlaylists()
    {
        yield return new WaitForSeconds(2);
        Paging<SimplePlaylist> usersPlaylists = spotifyManagerScript.GetUsersPlayists();
        if (usersPlaylists == null || usersPlaylists.Items.Count == 0)
        {
            Debug.LogError("usersPlaylists is null/empty");
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                string userPlaylistImageURL = null;

                if (usersPlaylists.Items[i].Images.Count != 0)
                {
                    userPlaylistImageURL = usersPlaylists.Items[i].Images[0].Url;
                }

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                WWW imageURLWWW = null;

                if (userPlaylistImageURL != null)
                {
                    imageURLWWW = new WWW(userPlaylistImageURL);

                    yield return imageURLWWW;

                    meshRenderers[i].material.mainTexture = imageURLWWW.texture;
                }
              

                playlistScript.setPlaylistName(usersPlaylists.Items[i].Name);
                playlistScript.setPlaylistURI(usersPlaylists.Items[i].Uri);
                playlistScript.ownerId = usersPlaylists.Items[i].Owner.Id;
                playlistScript.playlistId = usersPlaylists.Items[i].Id;

                if (imageURLWWW != null)
                {
                    playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
                    saveLoad.SaveTextureToFilePNG(Converter.ConvertWWWToTexture(imageURLWWW), "userPlaylist" + i + ".png");
                }

                saveLoad.savedUserPlaylists.Add(new PlaylistScriptData(playlistScript));
            }

        }
    }

    public void LoadUserPlaylistsFromFilePNG()
    {
        if (saveLoad.savedUserPlaylists != null && saveLoad.savedUserPlaylists.Count != 0)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                PlaylistScriptData playlistScriptLoadedData = saveLoad.savedUserPlaylists[i];

                PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                Texture2D texture = saveLoad.LoadTextureFromFilePNG("userPlaylist" + i + ".png");

                meshRenderers[i].material.mainTexture = texture;

                playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
                playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
                playlistScript.artistName = playlistScriptLoaded.artistName;
                playlistScript.sprite = Converter.ConvertTextureToSprite(texture);
                playlistScript.playlistId = playlistScriptLoaded.playlistId;
                playlistScript.ownerId = playlistScriptLoaded.ownerId;
            }
        }
        else {
            Debug.LogError("savedUserPlaylists is empty/null");
        }
    }

    private Sprite ConvertWWWToSprite(WWW www)
    {

        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);
        www.LoadImageIntoTexture(texture);

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

        return spriteToUse;
    }
}
