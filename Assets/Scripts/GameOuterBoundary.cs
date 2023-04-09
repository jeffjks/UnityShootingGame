using UnityEngine;
using UnityEditor;
using System.Collections;


public class GameOuterBoundary : MonoBehaviour
{
    // Enemy, EnemySpawner, ItemGem, Item 파괴
    #if UNITY_EDITOR
        [CustomEditor(typeof(GameOuterBoundary))]
        [CanEditMultipleObjects]
        public class InfoInspector : Editor {
            public override void OnInspectorGUI()
            {
                EditorGUILayout.HelpBox("바깥으로 나간 Enemy, EnemySpawner, 파괴", MessageType.Info);
                base.OnInspectorGUI();
            }
        }
    #endif

    private PoolingManager m_PoolingManager = null;
    private SystemManager m_SystemManager = null;

    void Start()
    {
        m_PoolingManager = PoolingManager.instance_op;
        m_SystemManager = SystemManager.instance_sm;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) {
            EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
            if (enemyObject.m_ParentEnemy == null) {
                if (!enemyObject.m_IsDead) {
                    enemyObject.OutOfBound();
                }
            }
        }

        /*
        else if (other.CompareTag("ItemGem")) {
            if (other.gameObject.activeSelf) {
                ItemGem item_gem = other.gameObject.GetComponentInParent<ItemGem>();
                if (item_gem == null)
                    return;
                item_gem.OnDeath();
            }
        }

        else if (other.CompareTag("ItemBox")) {
            ItemBox item_box = other.gameObject.GetComponentInParent<ItemBox>();
            Destroy(item_box.gameObject);
        }*/

        else if (other.CompareTag("Debris")) {
            if (other.gameObject.activeSelf) {
                DebrisEffect debris = other.gameObject.GetComponentInParent<DebrisEffect>();
                if (debris == null)
                    return;
                debris.ReturnToPool();
            }
        }
    }

    /*
    void OnTriggerStay2D(Collider2D other) // 탄 소거
    {
        if (m_SystemManager.m_BulletsEraseTimer > 0) {
            if (other.CompareTag("EnemyBullet")) {
                if (other.gameObject.activeSelf) {
                    if (other.transform.parent.gameObject.activeSelf) {
                        EnemyBullet enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
                        enemyBullet.OnDeath();
                    }
                }
            }
        }
    }*/
}
