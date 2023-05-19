using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBomb : ItemBox
{
    protected override void ItemEffect(Collider2D other) {
        PlayerShooter playerShooter = other.GetComponentInParent<PlayerShooter>();
        if (playerShooter != null) {
            AudioService.PlaySound("ItemGet");
            playerShooter.AddBomb();
        }
        else {
            Debug.LogAssertion("Can not find PlayerShooter Component.");
        }
    }
}
