using UnityEngine;

public class RankingDifficultyMenuHandler : MenuHandler
{
    public MenuHandler m_RankingPanel;
    public RankingDataLoader m_RankingDataLoader;
    
    protected override void Init() { }

    public void RankingNormal()
    {
        m_RankingDataLoader.m_GameDifficulty = GameDifficulty.Normal; 
        GoToTargetMenu(m_RankingPanel);
    }

    public void RankingExpert()
    {
        m_RankingDataLoader.m_GameDifficulty = GameDifficulty.Expert;
        GoToTargetMenu(m_RankingPanel);
    }

    public void RankingHell()
    {
        m_RankingDataLoader.m_GameDifficulty = GameDifficulty.Hell;
        GoToTargetMenu(m_RankingPanel);
    }
}
