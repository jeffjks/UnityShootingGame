using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class RankingScoreSlot : MonoBehaviour
{
    public RankingScoreDisplay[] m_RankingScoreDisplays = new RankingScoreDisplay[6];

    public void UpdateScoreInfo(int rank, LocalRankingData localRankingData) {
        m_RankingScoreDisplays[0].UpdateRankingText(rank.ToString());
        m_RankingScoreDisplays[1].UpdateRankingText(localRankingData.id);
        m_RankingScoreDisplays[2].UpdateRankingText(localRankingData.score.ToString());
        m_RankingScoreDisplays[3].m_RankingScoreImageDisplay.DisplayImages(localRankingData.shipAttributes.ToString());
        m_RankingScoreDisplays[4].UpdateRankingText(localRankingData.miss.ToString());
        m_RankingScoreDisplays[5].UpdateRankingText(new DateTime(localRankingData.date).ToString("yyyy-MM-dd"));
    }
}
