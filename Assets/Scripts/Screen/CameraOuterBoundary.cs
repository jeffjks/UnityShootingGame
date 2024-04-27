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
    
    public TriggerBody m_TriggerBody;

    private void Start()
    {
        if (!SystemManager.IsInGame)
            return;
        SetTriggerBodySize();
    }

    private void OnEnable()
    {
        SimulationManager.AddTriggerBody(m_TriggerBody);
        m_TriggerBody.m_OnTriggerBodyExit += OnTriggerBodyExit;
    }

    private void OnDisable()
    {
        SimulationManager.RemoveTriggerBody(m_TriggerBody);
        m_TriggerBody.m_OnTriggerBodyExit -= OnTriggerBodyExit;
    }

    private void SetTriggerBodySize()
    {
        m_TriggerBody.SetBoxSize(new Vector2(Size.MAIN_CAMERA_WIDTH, Size.MAIN_CAMERA_HEIGHT));
    }

    private void OnTriggerBodyExit(TriggerBody other)
    {
        // if (InGameDataManager.Instance == null)
        //     return;
        if (other.m_TriggerBodyType != TriggerBodyType.PlayerWeapon)
            return;
        if (other.CompareTag("PlayerWeapon") == false)
            return;
        
        var otherObject = other.gameObject;
        if (otherObject.activeSelf)
        {
            var playerWeapon = otherObject.GetComponent<PlayerWeapon>();
            PoolingManager.PushToPool(playerWeapon.m_ObjectName, otherObject, PoolingParent.PlayerMissile);
        }
    }
    
    // void OnTriggerExit2D(Collider2D other)
    // {
    //     if (InGameDataManager.Instance == null)
    //         return;
    //     
    //     var otherObject = other.gameObject;
    //     
    //     if (other.CompareTag("PlayerWeapon")) {
    //         if (otherObject.activeSelf)
    //         {
    //             var playerWeapon = otherObject.GetComponent<PlayerWeapon>();
    //             PoolingManager.PushToPool(playerWeapon.m_ObjectName, otherObject, PoolingParent.PlayerMissile);
    //         }
    //     }
    // }
}
