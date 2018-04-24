using SpotifyAPI.Web.Models; //Models for the JSON-responses
using System.Collections.Generic;

[System.Serializable]
public class AudioAnalysisCustom {

    public double trackLength, tempo, beatsPerSecond;
    public int timeSignature, key;
    public List<AnalysisSegmentCustom> Segments = new List<AnalysisSegmentCustom>();

    public AudioAnalysisCustom(AudioAnalysis audioAnalysis)
    {
        trackLength = audioAnalysis.Track.Duration;
        tempo = audioAnalysis.Track.Tempo;
        beatsPerSecond = tempo / (double)60;
        key = audioAnalysis.Track.Key;

        for (int i = 0; i < audioAnalysis.Segments.Count; i++)
        {
            Segments.Add(new AnalysisSegmentCustom(audioAnalysis.Segments[i]));
        }
    }
}
