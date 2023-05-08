using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextUI_GameMode : MonoBehaviour
{
    public Text m_GameModeText;
    private Sequence m_Sequence;
    
    public void FadeEffect() {
        m_Sequence = DOTween.Sequence()
        .Append(m_GameModeText.DOFade(0f, 0.6f))
        .Append(m_GameModeText.DOFade(1f, 0.2f))
        .SetEase(Ease.Linear)
        .SetLoops(-1);
    }

    public void UpdateGameModeText(GameMode gameType) {
        if (gameType == GameMode.GAMEMODE_NORMAL) {
            m_GameModeText.gameObject.SetActive(false);
        }
        else {
            if (gameType == GameMode.GAMEMODE_TRAINING) {
                m_GameModeText.text = "TRAINING";
            }
            else if (gameType == GameMode.GAMEMODE_REPLAY) {
                m_GameModeText.text = "REPLAY";
            }
            m_GameModeText.gameObject.SetActive(true);
        }
    }
}
