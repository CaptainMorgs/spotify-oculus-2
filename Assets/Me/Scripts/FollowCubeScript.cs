using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web.Enums;
using TMPro;
using UnityEngine;

public class FollowCubeScript : MonoBehaviour
{
    public GameObject fragments, Vinyl, uiConfirmation;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    private VinylScript vinylScript;
    public PlaylistScript playlistScript;
    public string artistId;
    public float animationTime = 1.5f;
    public TextMeshProUGUI textMeshPro;

    void Awake()
    {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
    }

    void HandleCollisionWithVinyl(Collision collision)
    {
        Instantiate(fragments, gameObject.transform.position, Quaternion.identity);
        Rigidbody[] fragmentRigidBodies = fragments.GetComponentsInChildren<Rigidbody>();
        fragmentRigidBodies[0].AddExplosionForce(5.0f, fragmentRigidBodies[0].transform.position, 5.0f, 5.0f, ForceMode.Force);

        spotifyManagerScript.Follow(FollowType.Artist, playlistScript.artistId);
        Debug.Log("Followed artist " + playlistScript.playlistName + " with id " + playlistScript.artistId);

        GameObject spawnedUIConfirmation = Instantiate(uiConfirmation, gameObject.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

        spawnedUIConfirmation.transform.Find("Follow Confirmation Text").GetComponent<TextMeshProUGUI>().text = "Followed " + playlistScript.playlistName + "!";
        Destroy(gameObject);
    }

    public void HandleCollisionWithVinyl2(GameObject vinylGameObject)
    {
        Debug.Log("In HandleCollisionWithVinyl2");

        if (playlistScript.trackType == PlaylistScript.TrackType.artist)
        {
            spotifyManagerScript.Follow(FollowType.Artist, playlistScript.artistId);

            GameObject spawnedUIConfirmation = Instantiate(uiConfirmation,
                gameObject.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

            spawnedUIConfirmation.transform.Find("Follow Confirmation Text").GetComponent<TextMeshProUGUI>().text =
                "Followed " + playlistScript.playlistName + "!";
        }

        else if (playlistScript.trackType == PlaylistScript.TrackType.track)
        {
            spotifyManagerScript.SaveTrack(playlistScript.trackId);

            GameObject spawnedUIConfirmation = Instantiate(uiConfirmation,
                gameObject.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

            spawnedUIConfirmation.transform.Find("Follow Confirmation Text").GetComponent<TextMeshProUGUI>().text =
                "Saved " + playlistScript.getPlaylistName() + "!";
        }

        else if (playlistScript.trackType == PlaylistScript.TrackType.playlist)
        {
            spotifyManagerScript.FollowPlaylist(playlistScript.ownerId, playlistScript.playlistId);

            GameObject spawnedUIConfirmation = Instantiate(uiConfirmation,
                gameObject.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

            spawnedUIConfirmation.transform.Find("Follow Confirmation Text").GetComponent<TextMeshProUGUI>().text =
                "Followed  " + playlistScript.getPlaylistName() + "!";
        }

        else if (playlistScript.trackType == PlaylistScript.TrackType.album)
        {
            spotifyManagerScript.SaveAlbum(playlistScript.albumId);

            GameObject spawnedUIConfirmation = Instantiate(uiConfirmation,
                gameObject.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);

            spawnedUIConfirmation.transform.Find("Follow Confirmation Text").GetComponent<TextMeshProUGUI>().text =
                "Saved " + playlistScript.getPlaylistName() + "!";
        }

        Destroy(vinylGameObject);
        Destroy(gameObject);
    }

    public void AnimateToPlayer(Vector3 vector3)
    {
        Hashtable hashtable = new Hashtable();
        hashtable.Add("x", vector3.x);
        hashtable.Add("y", vector3.y);
        hashtable.Add("z", vector3.z);
        hashtable.Add("time", animationTime);
        hashtable.Add("oncomplete", "AnimateOnComplete");

        iTween.MoveTo(gameObject, hashtable);

        iTween.RotateTo(gameObject, new Vector3(0, 0, 0), animationTime);
    }

    public void SetText()
    {
        if (playlistScript.trackType == PlaylistScript.TrackType.artist)
        {
            textMeshPro.text = "Follow Artist";
        }
        else if (playlistScript.trackType == PlaylistScript.TrackType.track)
        {
            textMeshPro.text = "Save Track";
        }
        else if (playlistScript.trackType == PlaylistScript.TrackType.playlist)
        {
            textMeshPro.text = "Follow Playlist";
        }
        else if (playlistScript.trackType == PlaylistScript.TrackType.album)
        {
            textMeshPro.text = "Save Album";
        }
    }
}
