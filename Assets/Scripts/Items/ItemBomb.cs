using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBomb : ItemBox
{
    protected override void ItemEffect(Collider2D other) {
        PlayerShooter playerShooter = other.GetComponentInParent<PlayerShooter>();
        if (playerShooter != null) {
            SoundService.PlaySFX("ItemGet");
            playerShooter.AddBomb();
        }
        else {
            Debug.LogAssertion("Can not find PlayerShooter Component.");
        }
    }
}
