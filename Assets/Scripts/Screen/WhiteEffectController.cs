using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteEffectController : MonoBehaviour // 흰색 점멸 이펙트
{
    public GameObject m_EffectImage;
    public Animator m_Animator;

    private readonly int _whiteEffectSmall = Animator.StringToHash("WhiteEffectSmall");
    private readonly int _largeEffectSmall = Animator.StringToHash("LargeEffectSmall");
    
    public void PlayWhiteEffect(bool isLarge)
    {
        m_Animator.SetTrigger(isLarge ? _largeEffectSmall : _whiteEffectSmall);
        m_EffectImage.SetActive(true);
    }
}