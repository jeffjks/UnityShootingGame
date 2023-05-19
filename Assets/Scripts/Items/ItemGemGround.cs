using UnityEngine;
using System.Collections;

public class ItemGemGround : ItemGem {

    protected override void ItemEffect(Collider2D other) {
        m_SystemManager.AddScoreEffect(ItemScore.GEM_GROUND, true);
        AudioService.PlaySound("ItemGem");
    }

    public override void OnItemRemoved() {
        ReturnToPool();
    }

    public override void ReturnToPool() {
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.ITEM_GEM_GROUND);
    }
}
