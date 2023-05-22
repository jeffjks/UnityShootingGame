using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class RegisterLocalRankingMenuHandler : GameUI
{
    public InputField m_InputFieldID;
    public SpriteRenderer m_SpriteRenderer;

    private GameDifficulty m_Difficulty;
    private long m_TotalScore;
    private ShipAttributes m_ShipAttributes;
    private int m_TotalMiss;
    private long m_ClearedTime;
    
    private List<LocalRankingData> m_LocalRankingDataList = new List<LocalRankingData>();
    private bool m_Active = true;

    void Start()
    {
        m_Difficulty = m_SystemManager.GetDifficulty();
        m_TotalScore = m_SystemManager.GetTotalScore();
        m_ShipAttributes = m_PlayerManager.m_CurrentAttributes;
        m_TotalMiss = m_SystemManager.GetTotalMiss();
        m_ClearedTime = m_SystemManager.GetClearedTime();

        m_InputFieldID.text = PlayerPrefs.GetString("LastLoaclRankingID", string.Empty);
        m_InputFieldID.ActivateInputField();

        m_GameManager = GameManager.instance_gm;
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");

        if (m_Active) {
            if (Input.GetButtonDown("Fire1")) {
                switch(m_Selection) {
                    case 1:
                        RegisterLocalRanking(m_InputFieldID.text);
                        break;
                    case 2:
                        Cancel();
                        break;
                    default:
                        break;
                }
            }

            MoveCursorVertical(moveRawVertical);
        }
        // m_Selection = EndToStart(m_Selection, m_Total);
        
        m_IsEnabled[1] = !(m_InputFieldID.text.Length < 3);

        SetColor();
	}

    protected override void MoveCursorVertical(int move) {
        if (move != 0) {
            if (!m_isVerticalAxisInUse) {
                m_Selection -= move;
                m_isVerticalAxisInUse = true;

                if (m_Selection < 0) {
                    m_Selection = 0;
                }
                else if (m_Selection >= m_Total) {
                    m_Selection = m_Total - 1;
                }

                switch(m_Selection) {
                    case 0:
                        m_InputFieldID.ActivateInputField();
                        break;
                    default:
                        m_InputFieldID.DeactivateInputField();
                        break;
                }
            }
        }
        else {
            m_isVerticalAxisInUse = false;
        }
    }

    private void RegisterLocalRanking(string id) {
        m_Active = false;

        LocalRankingData localRankingData = new LocalRankingData(id, m_TotalScore, m_ShipAttributes, m_TotalMiss, m_ClearedTime);

        //ReadLocalRanking(m_Difficulty, localRankingData);
        WriteLocalRanking(m_Difficulty, localRankingData);

        StartCoroutine(ReturnToMainMenu());
    }

    private void Cancel() {
        m_Active = false;
        AudioService.PlaySound("CancelUI");
        m_InputFieldID.DeactivateInputField();
        StartCoroutine(ReturnToMainMenu());
    }

    /*
    public void ReadLocalRanking(int difficulty, LocalRankingData localRankingData) {
        string filePath = $"{m_GameManager.m_RankingDirectory}ranking{difficulty}.bin";

        BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read));
        while (true){
            try {
                string id = br.ReadString();
                long score = br.ReadInt64();
                ShipAttributes shipAttributes = new ShipAttributes(br.ReadInt32());
                int miss = br.ReadInt32();
                long date = br.ReadInt64();

                int attributesCode = shipAttributes.GetAttributesCode();

                //Debug.Log($"{id}, {score}, {attributesCode}, {miss}, {date}");

                LocalRankingData record = new LocalRankingData(id, score, shipAttributes, miss, date);
                m_LocalRankingDataList.Add(record);
                //Console.WriteLine("{0} {1}", var1, var2);
            }
            catch (EndOfStreamException) { // 파일 끝에 도달한 예외 처리
                br.Close();
                break;
            }
        }

        for (int i = m_LocalRankingDataList.Count - 1; i >= 0; i--)
        {
            if (m_LocalRankingDataList[i].id != localRankingData.id) {
                continue;
            }
            if (m_LocalRankingDataList[i].score)
        }

        m_LocalRankingDataList.Sort(new Comparison<LocalRankingData>((n1, n2) => CompareListElement(n1, n2)));

        TryDisplayScoreRanking();
    }*/

    public void WriteLocalRanking(GameDifficulty difficulty, LocalRankingData record) {
        string filePath = $"{m_GameManager.m_RankingDirectory}ranking{(int) difficulty}.bin";

        FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
        try {
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(record.id);
            bw.Write(record.score);
            bw.Write(record.shipAttributes.GetAttributesCode());
            bw.Write(record.miss);
            bw.Write(record.date);
        }
        catch (System.NullReferenceException) {
            Debug.Log("A");
            return;
        }
        fs.Close();
    }
    
    private IEnumerator ReturnToMainMenu()
    {
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, 0f);

        m_SpriteRenderer.DOFade(1f, 2f);
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
        yield break;
    }
}
