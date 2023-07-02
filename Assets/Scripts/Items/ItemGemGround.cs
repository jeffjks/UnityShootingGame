using UnityEngine;
using System.Collections;

public class ItemGemGround : ItemGem {

    protected override void ItemEffect(Collider2D other) {
        InGameDataManager.Instance.AddScore(m_ItemData.itemScore, true, m_ItemData.itemType);
        AudioService.PlaySound("ItemGem");
    }

    protected override void OnItemRemoved() {
        ReturnToPool();
    }

    public override void ReturnToPool() {
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.GemGround);
    }
}
