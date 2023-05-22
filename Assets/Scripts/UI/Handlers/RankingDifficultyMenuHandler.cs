using UnityEngine;

public class RankingDifficultyMenuHandler : MenuHandler
{
    public GameObject m_NormalRankingPanel;
    public GameObject m_ExpertRankingPanel;
    public GameObject m_HellRankingPanel;

    public void NormalRanking() {
        GoToTargetMenu(m_NormalRankingPanel);
    }

    public void ExpertRanking() {
        GoToTargetMenu(m_ExpertRankingPanel);
    }

    public void HellRanking() {
        GoToTargetMenu(m_HellRankingPanel);
    }
}
