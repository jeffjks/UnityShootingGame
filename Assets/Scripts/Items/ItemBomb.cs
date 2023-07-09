using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBomb : ItemBox
{
    protected override void ItemEffect(Collider2D other) {
        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController != null) {
            AudioService.PlaySound("ItemGet");
            
            if (InGameDataManager.Instance.AddBomb())
            {
                InGameDataManager.Instance.DisplayTextEffect("BOMB", 0.8f);
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
