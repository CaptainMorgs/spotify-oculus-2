using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SpotifyAPI.Web.Models;
using System;

public class HoverUI : MonoBehaviour
{

    private PlaylistScript playlistScript;

    private GameObject text;
    private TextMeshProUGUI textPro;

    // Use this for initialization
    void Start()
    {
        text = transform.Find("TextMeshPro Text").gameObject;
        textPro = text.GetComponent<TextMeshProUGUI>();
    }

    public void updateHoverUI(PlaylistScript playlistScript)
    {
        this.playlistScript = playlistScript;

        FullTrack track = playlistScript.getFullTrack();

        if (track != null)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(playlistScript.getFullTrack().DurationMs);

            FullTrack fullTrack = playlistScript.getFullTrack();

            string answer = string.Format("{0:D2}m:{1:D2}s",
                        t.Minutes,
                        t.Seconds);

            textPro.SetText(playlistScript.getPlaylistName()
            + "\n" + "Length: " + answer + "\n" + fullTrack.Artists[0].Name
            );
        }
        else
        {
            textPro.SetText(playlistScript.getPlaylistName());
        }
    }
    public void updateHoverUI()
    {
        textPro.SetText("");
    }
}
