using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextUI_GameType : MonoBehaviour
{
    public Text m_GameTypeText;
    private Sequence m_Sequence;
    
    public void FadeEffect() {
        m_Sequence = DOTween.Sequence()
        .Append(m_GameTypeText.DOFade(0f, 0.6f))
        .Append(m_GameTypeText.DOFade(1f, 0.2f))
        .SetEase(Ease.Linear)
        .SetLoops(-1);
    }

    public void UpdateGameTypeText(GameType gameType) {
        if (gameType == GameType.GAMETYPE_NORMAL) {
            m_GameTypeText.gameObject.SetActive(false);
        }
        else {
            if (gameType == GameType.GAMETYPE_TRAINING) {
                m_GameTypeText.text = "TRAINING";
            }
            else if (gameType == GameType.GAMETYPE_REPLAY) {
                m_GameTypeText.text = "REPLAY";
            }
            m_GameTypeText.gameObject.SetActive(true);
        }
    }
}
