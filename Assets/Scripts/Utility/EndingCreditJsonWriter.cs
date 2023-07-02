using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class EndingCreditJsonWriter : MonoBehaviour
{
    [TextArea(0, 10)]
    public string credit;
    [TextArea(0, 10)]
    public string[] assetLists;

    public string creditDate;
    
    private readonly Dictionary<Language, string> _endingCreditText = new();
    private const string unityVersion = "2021.3.24f1";

    private struct CreditText
    {
        public string creditWord;
        public string usedAssetsWord;
        public List<string> categories;

        public CreditText(string creditWord, string usedAssetsWord, List<string> categories)
        {
            this.creditWord = creditWord;
            this.usedAssetsWord = usedAssetsWord;
            this.categories = categories;
        }
    }
    
    private void WriteJson(Language language, CreditText creditText)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(MakeRichText(creditText.creditWord));
        sb.Append($"{credit}\n\nMade With Unity {unityVersion}\n\n\n\n");
        sb.Append(MakeRichText(creditText.usedAssetsWord));
        sb.Append("\n\n");

        for (var i = 0; i < creditText.categories.Count; ++i)
        {
            sb.Append(MakeAssetList(creditText.categories[i], i));
        }
        sb.Append("\n\n\n\n\n\n\n\n");
        sb.Append(MakeRichText("Dead Planet 2"));
        sb.Append($"\nver {Application.version}\n{creditDate}");

        _endingCreditText[language] = sb.ToString();
    }

    public void GenerateJsonFile()
    {
        WriteJson(Language.English, new CreditText("Credit", "UsedAsset",
            new List<string> { "Models", "Images / Textures", "Effects", "Sounds", "Terrain", "Others" }));
        WriteJson(Language.Korean, new CreditText("제작", "사용한 에셋",
            new List<string> { "모델링", "이미지 / 텍스쳐", "이펙트", "사운드", "지형", "기타" }));
        
        Utility.SaveDataFile(Application.dataPath, "resources2", _endingCreditText);
    }

    private StringBuilder MakeAssetList(string category, int index)
    {
        StringBuilder tempSb = new StringBuilder(MakeRichText(category));
        tempSb.Append("\n");
        tempSb.Append(assetLists[index]);
        tempSb.Append("\n\n");
        return tempSb;
    }

    private string MakeRichText(string text)
    {
        return $"<size=56><b>{text}</b></size>";
    }
}
