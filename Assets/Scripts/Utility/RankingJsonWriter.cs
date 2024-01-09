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
            var directoryInfo = new DirectoryInfo(GameManager.RankingFilePath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            
            if (!File.Exists($"{GameManager.RankingFilePath}{fileName}"))
                Utility.SaveDataFile(GameManager.RankingFilePath, fileName, _localRankingData);
        }
    }
}
