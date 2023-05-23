using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;

public class RankingDataSlotLoader : MonoBehaviour
{
    public RankingDataSlot[] m_RankingDataSlots;

    public void SetRankingSlot(int rank, LocalRankingData localRankingData)
    {
        SetRankingDataRank(rank);
        SetRankingDataID(localRankingData.id);
        SetRankingDataScore(localRankingData.score);
        SetRankingDataAttributes(localRankingData.shipAttributes);
        SetRankingDataMiss(localRankingData.miss);
        SetRankingDataDate(localRankingData.date);
    }

    public void InitRankingSlot()
    {
        foreach (var rankingDataSlot in m_RankingDataSlots)
        {
            rankingDataSlot.InitRankingData();
        }
    }

    private void SetRankingDataRank(int rank)
    {
        m_RankingDataSlots[0].SetRankingData(rank.ToString());
    }

    private void SetRankingDataID(string id)
    {
        m_RankingDataSlots[1].SetRankingData(id);
    }

    private void SetRankingDataScore(long score)
    {
        m_RankingDataSlots[2].SetRankingData(score.ToString());
    }

    private void SetRankingDataAttributes(ShipAttributes shipAttributes)
    {
        //var code = shipAttributes.GetAttributesCode();
        m_RankingDataSlots[3].SetRankingData(shipAttributes);
    }

    private void SetRankingDataMiss(int miss)
    {
        var exceed = (miss > 100);
        var displayMiss = miss;
        if (exceed)
        {
            displayMiss = 100;
        }
        var missText = displayMiss.ToString();
        if (exceed)
        {
            missText += "+";
        }
        m_RankingDataSlots[4].SetRankingData(missText + " Miss");
    }

    private void SetRankingDataDate(long date)
    {
        var dateTime = new DateTime(date).ToString("yyyy-MM-dd");
        m_RankingDataSlots[5].SetRankingData(dateTime);
    }
}

public abstract class RankingDataSlot : MonoBehaviour
{
    public abstract void InitRankingData();
    public abstract void SetRankingData(string text);
    public abstract void SetRankingData(ShipAttributes shipAttributes);
}