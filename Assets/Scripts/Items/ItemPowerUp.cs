using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPowerUp : ItemBox
{
    protected override void ItemEffect(Collider2D other) {
        PlayerShooter playerShooter = other.GetComponentInParent<PlayerShooter>();
        if (playerShooter != null) {
            m_SystemManager.m_SoundManager.PlayAudio(m_AudioClip);
            playerShooter.PowerUp();
        }
    }
}
