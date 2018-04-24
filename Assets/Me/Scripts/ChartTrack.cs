using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ChartTrack
{
    public string position, trackName, artist, streams, url, apiUrl;

    private string pattern = @":\/\/\w+.\w+.\w+\/\w+\/(\w+)";

    private string input = @"https://open.spotify.com/track/1rfofaqEpACxVEHIZBJe6W";

    public ChartTrack(string position, string trackName, string artist, string streams, string url)
    {
        this.position = position;
        this.trackName = trackName;
        this.artist = artist;
        this.streams = streams;
        this.url = url;
    }

    public ChartTrack(List<string> list)
    {
        this.position = list[0];
        this.trackName = list[1];
        this.artist = list[2];
        this.streams = list[3];
        this.url = list[4];

        if (Regex.IsMatch(url, pattern))
        {
            apiUrl = Regex.Matches(url, pattern)[0].Groups[1].ToString();
        }
        else
        {
            Debug.LogError("Error parsing chart track url: " + url);
        }
    }

    public override string ToString()
    {
        return (this.position + " " + this.trackName + " " + this.artist + " " + this.streams + " " + this.url + " " + this.apiUrl);
    }
}