using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBomb : ItemBox
{
    protected override void ItemEffect(Collider2D other) {
        PlayerShooter playerShooter = other.GetComponent<PlayerShooter>();
        m_SystemManager.m_SoundManager.PlayAudio(m_AudioClip);
        playerShooter.AddBomb();
    }
}
