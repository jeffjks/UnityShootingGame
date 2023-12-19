using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class EndingCreditJsonWriter : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private EndingCreditMessageDatas _messageDataEng;
    [SerializeField] private EndingCreditMessageDatas _messageDataKor;
    private readonly Dictionary<Language, string> _endingCreditText = new();
    
    private void WriteJson(Language language, EndingCreditMessageDatas messageData)
    {
        StringBuilder sb = new StringBuilder();
        var credit = messageData.credit;
        var creditDate = messageData.creditDate;
        var unityVersion = messageData.unityVersion;
        
        sb.Append(MakeRichText(messageData.categoryCredit));
        sb.Append("\n");
        sb.Append($"{credit}\n\nMade With Unity {unityVersion}\n\n\n\n");
        sb.Append(MakeRichText(messageData.categoryAssetList));
        sb.Append("\n\n");

        foreach (var assetList in messageData.assetLists)
        {
            sb.Append(MakeAssetList(assetList));
        }
        sb.Append("\n\n\n\n\n\n\n\n\n\n\n");
        sb.Append(MakeRichText("Dead Planet 2"));
        sb.Append($"\nver {Application.version}\n{creditDate}");

        _endingCreditText[language] = sb.ToString();
    }

    public void GenerateJsonFile()
    {
        WriteJson(Language.English, _messageDataEng);
        WriteJson(Language.Korean, _messageDataKor);
        
        Utility.SaveDataFile(GameManager.ResourceFilePath, "resources2.dat", _endingCreditText);
    }

    private StringBuilder MakeAssetList(AssetList assetList)
    {
        StringBuilder tempSb = new StringBuilder(MakeRichText(assetList.assetCategory));
        tempSb.Append("\n");
        tempSb.Append(assetList.assetNames);
        tempSb.Append("\n\n\n");
        return tempSb;
    }

    private string MakeRichText(string text)
    {
        return $"<size=30><b>{text}</b></size>";
    }
#endif
}
