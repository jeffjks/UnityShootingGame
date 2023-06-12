using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteEffectController : MonoBehaviour // 흰색 점멸 이펙트
{
    public GameObject m_EffectImage;
    public Animator m_Animator;
    
    public void PlayWhiteEffect(bool isLarge)
    {
        m_Animator.SetBool("isLarge", isLarge);
        m_EffectImage.SetActive(true);
    }
}