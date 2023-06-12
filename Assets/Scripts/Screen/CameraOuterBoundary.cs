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
    
    private BoxCollider2D _boxCollider2D;

    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        
        SetColliderSize();
    }

    private void SetColliderSize()
    {
        _boxCollider2D.size = new Vector2(Size.MAIN_CAMERA_WIDTH, Size.MAIN_CAMERA_HEIGHT);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerWeapon")) {
            if (other.gameObject.activeSelf) {
                PlayerWeapon playerWeapon = other.gameObject.GetComponent<PlayerWeapon>();
                PoolingManager.PushToPool(playerWeapon.m_ObjectName, other.gameObject, PoolingParent.PlayerMissile);
            }
        }
    }
}
