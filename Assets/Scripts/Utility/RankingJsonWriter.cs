using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class RankingJsonWriter : MonoBehaviour
{
    private readonly List<LocalRankingData> _localRankingData = new();
    private readonly List<string> _fileList = new() { "ranking0.dat", "ranking1.dat", "ranking2.dat" };

    public void GenerateJsonFile()
    {
        foreach (var fileName in _fileList)
        {
            if (!File.Exists($"{Application.dataPath}/{fileName}"))
                Utility.SaveDataFile(Application.dataPath, fileName, _localRankingData);
        }
    }
}
