using UnityEngine;
using UnityEditor;
using System.Collections;


public class OuterGameBoundary : MonoBehaviour
{
    #if UNITY_EDITOR
        [CustomEditor(typeof(OuterGameBoundary))]
        [CanEditMultipleObjects]
        public class InfoInspector : Editor {
            public override void OnInspectorGUI()
            {
                EditorGUILayout.HelpBox("바깥으로 나간 Enemy 파괴", MessageType.Info);
                base.OnInspectorGUI();
            }
        }
    #endif

    public TriggerBody m_TriggerBody;
    
    private BoxCollider2D _boxCollider2D;

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        
        _boxCollider2D.size = new Vector2(Size.GAME_WIDTH, Size.GAME_HEIGHT);
        transform.position = new Vector3(0f, -Size.GAME_HEIGHT/2, Depth.CAMERA);
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

    private void OnTriggerBodyExit(TriggerBody other)
    {
        if (InGameDataManager.Instance == null)
            return;
        if (other.m_TriggerBodyType != TriggerBodyType.Enemy)
            return;
        
        var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
        if (enemyUnit.transform != enemyUnit.transform.root) // 본체가 아닐 경우
            return;
        if (!enemyUnit.IsColliderInit)
            return;
        if (enemyUnit.m_EnemyType == EnemyType.Boss)
            return;
        if (enemyUnit.m_EnemyDeath.IsDead)
            return;
        enemyUnit.OutOfBound();
    }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (InGameDataManager.Instance == null)
    //         return;
    //     
    //     if (other.CompareTag("Enemy")) {
    //         var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
    //         if (enemyUnit.transform != enemyUnit.transform.root) // 본체가 아닐 경우
    //             return;
    //         if (!enemyUnit.IsColliderInit)
    //             return;
    //         if (enemyUnit.m_EnemyType == EnemyType.Boss)
    //             return;
    //         if (enemyUnit.m_EnemyDeath.IsDead)
    //             return;
    //         enemyUnit.OutOfBound();
    //     }
    // }
}
