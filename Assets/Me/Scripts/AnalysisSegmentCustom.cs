using System.Collections.Generic;
using SpotifyAPI.Web.Models; //Models for the JSON-responses

[System.Serializable]
public class AnalysisSegmentCustom
{
    public double Start { get; set; }
    public double Duration { get; set; }
    public double Confidence { get; set; }
    public double LoudnessStart { get; set; }
    public double LoudnessMaxTime { get; set; }
    public double LoudnessMax { get; set; }
    public double LoudnessEnd { get; set; }
    public List<double> Pitches { get; set; } = new List<double>();
    public List<double> Timbre { get; set; } = new List<double>();

    public AnalysisSegmentCustom(AnalysisSegment analysisSegment)
    {
        Start = analysisSegment.Start;
        Duration = analysisSegment.Duration;
        Confidence = analysisSegment.Confidence;
        LoudnessStart = analysisSegment.LoudnessStart;
        LoudnessMaxTime = analysisSegment.LoudnessMaxTime;
        LoudnessMax = analysisSegment.LoudnessMax;
        LoudnessEnd = analysisSegment.LoudnessEnd;
        Pitches = analysisSegment.Pitches;
        Timbre = analysisSegment.Timbre;
    }
}
