using UnityEngine;
using UnityEditor;
using System.Collections;

public class CameraBoundary : MonoBehaviour
{
    // PlayerMissile, Bullet 파괴
    #if UNITY_EDITOR
        [CustomEditor(typeof(CameraBoundary))]
        [CanEditMultipleObjects]
        public class InfoInspector : Editor {
            public override void OnInspectorGUI()
            {
                EditorGUILayout.HelpBox("PlayerMissile, Bullet 파괴", MessageType.Info);
                base.OnInspectorGUI();
            }
        }
    #endif

    private PoolingManager m_PoolingManager = null;

    void Start()
    {
        m_PoolingManager = PoolingManager.instance_op;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerMissile")) {
            if (other.gameObject.activeSelf) {
                PlayerMissile playerMissile = other.gameObject.GetComponent<PlayerMissile>();
                m_PoolingManager.PushToPool(playerMissile.m_ObjectName, other.gameObject, PoolingParent.PLAYER_MISSILE);
            }
        }
        else if (other.CompareTag("EnemyBullet")) {
            if (other.gameObject.activeSelf) {
                if (other.transform.parent.gameObject.activeSelf) {
                    EnemyBullet enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
                    enemyBullet.Erase();
                }
            }
        }
    }
}
