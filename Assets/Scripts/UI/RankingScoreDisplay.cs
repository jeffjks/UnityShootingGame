using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class RankingScoreDisplay : MonoBehaviour
{
    public Text m_DisplayTextElement;
    public RankingScoreImageDisplay m_RankingScoreImageDisplay;
    public string m_Unit;

    void Start()
    {
        //Init();
    }

    void OnDisable()
    {
        //Init();
    }

    private void Init()
    {
        //m_RankingScoreInfo.Clear();
        //SetScoreInfo();
    }

    public void UpdateRankingText(string text) {
        m_DisplayTextElement.text = text + m_Unit;
    }

    /*
    public void UpdateScoreInfo(string text) {
        m_RankingScoreInfo.Clear();
        if (!text.Equals(string.Empty)) {
            m_RankingScoreInfo.Append(text).Append(m_Unit);
        }
        SetScoreInfo();
    }

    private void SetScoreInfo() {
        if (m_DisplayTextElement != null) {
            m_DisplayTextElement.text = m_RankingScoreInfo.ToString();
            Debug.Log(m_RankingScoreInfo.ToString());
        }
        else {
            m_RankingScoreImageDisplay.DisplayImages(m_RankingScoreInfo.ToString());
        }
    }*/
}
