using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPowerUp : ItemBox
{
    protected override void ItemEffect(Collider2D other) {
        PlayerShooter playerShooter = other.GetComponentInParent<PlayerShooter>();
        if (playerShooter != null) {
            AudioService.PlaySound("ItemGet");
            playerShooter.PowerUp();
        }
        else {
            Debug.LogAssertion("Can not find PlayerShooter Component.");
        }
    }
}
