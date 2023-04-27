using UnityEngine;
using UnityEditor;
using System.Collections;


public class CameraOuterBoundary : MonoBehaviour
{
    // PlayerShot 파괴
    #if UNITY_EDITOR
        [CustomEditor(typeof(CameraOuterBoundary))]
        [CanEditMultipleObjects]
        public class InfoInspector : Editor {
            public override void OnInspectorGUI()
            {
                EditorGUILayout.HelpBox("PlayerShot 파괴", MessageType.Info);
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
        if (other.CompareTag("PlayerWeapon")) {
            if (other.gameObject.activeSelf) {
                PlayerWeapon playerWeapon = other.gameObject.GetComponent<PlayerWeapon>();
                m_PoolingManager.PushToPool(playerWeapon.m_ObjectName, other.gameObject, PoolingParent.PLAYER_MISSILE);
            }
        }
    }
}
