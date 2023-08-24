using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class RankingJsonWriter : MonoBehaviour
{
#if UNITY_EDITOR
    private readonly List<LocalRankingData> _localRankingData = new();
    private readonly List<string> _fileList = new() { "ranking0", "ranking1", "ranking2" };

    public void GenerateJsonFile()
    {
        foreach (var fileName in _fileList)
        {
            if (!File.Exists($"{Application.dataPath}/{fileName}.dat"))
                Utility.SaveDataFile(Application.dataPath, fileName, _localRankingData);
        }
    }
#endif
}
