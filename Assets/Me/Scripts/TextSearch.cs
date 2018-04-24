using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TextSearch : MonoBehaviour
{

    private UnityEngine.UI.Text text;
    private TextMeshProUGUI textPro;
    private GameObject spotifyManager;
    private Spotify script;

    // Use this for initialization
    void Start()
    {
        text = transform.root.gameObject.GetComponent<UnityEngine.UI.Text>();
        textPro = transform.root.gameObject.GetComponent<TextMeshProUGUI>();
        spotifyManager = GameObject.Find("SpotifyManager");
        script = spotifyManager.GetComponent<Spotify>();
    }

    public void SearchForText(string searchTerm)
    {
        if (searchTerm != null)
        {
            Debug.Log("Search query: " + searchTerm);
            script.SearchSpotify(searchTerm);
        }
        else
        {
            Debug.LogError("Null search term");
        }
    }
}
