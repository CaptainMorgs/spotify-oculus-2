using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Mono.Csv;
using TMPro;

public class CSVReader : MonoBehaviour
{
    public ArrayList chartTrackList = new ArrayList();

    public TextMeshProUGUI textMeshPro;

    // Use this for initialization
    void Start()
    {
        ReadCSV();
    }

    private void ReadCSV()
    {
        List<List<string>> dataGrid = null;
        try
        {
           dataGrid = CsvFileReader.ReadAll(Application.persistentDataPath + "/regional-global-daily-latest.csv", Encoding.GetEncoding("utf-8"));
        }
        catch (Exception e)
        {
            textMeshPro.text = e.Message.ToString();
        }
        if (dataGrid != null)
        {

            foreach (var row in dataGrid)
            {
                ChartTrack chartTrack = new ChartTrack(row);
                chartTrackList.Add(chartTrack);
            }
        }
    }
}

