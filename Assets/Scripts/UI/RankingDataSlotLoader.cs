using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class RankingDataSlotLoader : MonoBehaviour
{
    public RankingDataSlot[] m_RankingDataSlots = new RankingDataSlot[6];

    public void LoadRankingData(int rank, LocalRankingData localRankingData)
    {
        m_RankingDataSlots[0].SetRankingData(rank.ToString());
        m_RankingDataSlots[1].SetRankingData(localRankingData.id);
        m_RankingDataSlots[2].SetRankingData(localRankingData.score.ToString());
        m_RankingDataSlots[3].SetRankingData(localRankingData.shipAttributes.GetAttributesCode());
        m_RankingDataSlots[4].SetRankingData(localRankingData.miss + " Miss");
        m_RankingDataSlots[5].SetRankingData(new DateTime(localRankingData.date).ToString("yyyy-MM-dd"));
    }
}

public abstract class RankingDataSlot : MonoBehaviour
{
    public abstract void SetRankingData(string text);
    public abstract void SetRankingData(int data);
}