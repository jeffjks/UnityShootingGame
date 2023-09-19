using UnityEngine;
using UnityEditor;
using System.Collections;


public class InnerGameBoundary : MonoBehaviour
{
    #if UNITY_EDITOR
        [CustomEditor(typeof(InnerGameBoundary))]
        [CanEditMultipleObjects]
        public class InfoInspector : Editor {
            public override void OnInspectorGUI()
            {
                EditorGUILayout.HelpBox("안으로 들어온 Enemy의 IsColliderInit 플래그 체크,\n바깥으로 나간 Debris 파괴", MessageType.Info);
                base.OnInspectorGUI();
            }
        }
    #endif

    private BoxCollider2D _boxCollider2D;

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        
        _boxCollider2D.size = new Vector2(Size.GAME_WIDTH, Size.GAME_HEIGHT);
        transform.position = new Vector3(0f, -Size.GAME_HEIGHT/2, Depth.CAMERA);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (InGameDataManager.Instance == null)
            return;
        
        if (other.CompareTag("Enemy")) {
            var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
            if (enemyUnit.transform != enemyUnit.transform.root) // 본체가 아닐 경우
                return;
            enemyUnit.IsColliderInit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (InGameDataManager.Instance == null)
            return;
        
        if (other.CompareTag("Debris")) {
            if (other.gameObject.activeSelf) {
                var debris = other.gameObject.GetComponentInParent<DebrisEffect>();
                if (debris == null)
                    return;
                debris.ReturnToPool();
            }
        }
    }
}
