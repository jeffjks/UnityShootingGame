using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;

public class InGameText_GameMode : MonoBehaviour
{
    public TextMeshProUGUI m_GameModeText;
    
    private readonly Dictionary<GameMode, string> _gameModeString = new ()
    {
        { GameMode.Normal, string.Empty },
        { GameMode.Training, "TRAINING" }
    };

    private void Start()
    {
        if (_gameModeString.TryGetValue(SystemManager.GameMode, out var str))
        {
            if (SystemManager.IsReplayMode)
            {
                str += "\n(REPLAY MODE)";
            }
            m_GameModeText.SetText(str);
        }
        else
        {
            Debug.LogError($"Unknown mode has detected: {SystemManager.GameMode}");
            m_GameModeText.SetText("UNKNOWN");
        }
        FadeEffect();
    }

    private void FadeEffect() {
        DOTween.Sequence()
        .Append(m_GameModeText.DOFade(0f, 0.6f))
        .Append(m_GameModeText.DOFade(1f, 0.2f))
        .SetEase(Ease.Linear)
        .SetLoops(-1);
    }
}
