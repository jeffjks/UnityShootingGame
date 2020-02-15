using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBomb : ItemBox
{
    protected override void ItemEffect(Collider2D other) {
        PlayerShooter playerShooter = other.GetComponentInParent<PlayerShooter>();
        if (playerShooter != null) {
            m_SystemManager.m_SoundManager.PlayAudio(m_AudioClip);
            playerShooter.AddBomb();
        }
        else {
            Debug.LogAssertion("Can not find PlayerShooter Component.");
        }
    }
}
