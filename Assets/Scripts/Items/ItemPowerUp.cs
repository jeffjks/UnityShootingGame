using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPowerUp : ItemBox
{
    protected override void ItemEffect(Collider2D other) {
        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController != null) {
            AudioService.PlaySound("ItemGet");
            if (playerController.PowerUp())
            {
                InGameDataManager.Instance.DisplayTextEffect("POWER UP");
            }
            else
            {
                InGameDataManager.Instance.AddScore(m_ItemData.itemScore, true, m_ItemData.itemType);
                InGameDataManager.Instance.DisplayTextEffect(m_ItemData.itemScore);
            }
        }
        else {
            Debug.LogAssertion("Can not find PlayerController Component.");
        }
    }
}
