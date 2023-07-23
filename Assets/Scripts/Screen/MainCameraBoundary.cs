using UnityEngine;
using UnityEditor;
using System.Collections;

public class MainCameraBoundary : MonoBehaviour
{
    // PlayerWeapon
    #if UNITY_EDITOR
        [CustomEditor(typeof(MainCameraBoundary))]
        [CanEditMultipleObjects]
        public class InfoInspector : Editor {
            public override void OnInspectorGUI()
            {
                EditorGUILayout.HelpBox("화면 바깥으로 나가는 PlayerWeapon 파괴", MessageType.Info);
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
        var otherObject = other.gameObject;
        
        if (other.CompareTag("PlayerWeapon")) {
            if (otherObject.activeSelf)
            {
                var playerWeapon = otherObject.GetComponent<PlayerWeapon>();
                PoolingManager.PushToPool(playerWeapon.m_ObjectName, otherObject, PoolingParent.PlayerMissile);
            }
        }
    }
}
