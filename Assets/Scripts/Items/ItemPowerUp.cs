using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPowerUp : ItemBox
{
    protected override void ItemEffect(Collider2D other) {
        PlayerUnit playerUnit = other.GetComponentInParent<PlayerUnit>();
        if (playerUnit != null) {
            AudioService.PlaySound("ItemGet");
            if (playerUnit.PowerUp())
            {
                InGameDataManager.Instance.DisplayTextEffect("POWER UP", 0.8f);
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
