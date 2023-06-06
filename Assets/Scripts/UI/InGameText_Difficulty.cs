using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameText_Difficulty : MonoBehaviour
{
    public TextMeshProUGUI m_GameDifficultyText;
    
    private readonly Dictionary<GameDifficulty, string> _gameDifficultyString = new ()
    {
        { GameDifficulty.Normal, "NORMAL" },
        { GameDifficulty.Expert, "EXPERT" },
        { GameDifficulty.Hell, "HELL" }
    };

    private void Start()
    {
        if (_gameDifficultyString.TryGetValue(SystemManager.Difficulty, out var str))
        {
            m_GameDifficultyText.SetText(str);
        }
        else
        {
            Debug.LogError($"Unknown difficulty has detected: {SystemManager.Difficulty}");
            m_GameDifficultyText.SetText("UNKNOWN");
        }
    }
}
