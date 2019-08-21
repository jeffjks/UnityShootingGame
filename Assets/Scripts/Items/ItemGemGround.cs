using UnityEngine;
using System.Collections;

public class ItemGemGround : ItemGem {

    protected override void ItemEffect(Collider2D other) {
        m_SystemManager.AddScore(ItemScore.GEM_GROUND, true);
        m_SystemManager.m_SoundManager.PlayAudio(m_AudioClip);
    }
}
