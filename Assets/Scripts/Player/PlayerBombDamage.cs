using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombDamage : PlayerObject
{
    public int m_BombDuration;
    [SerializeField] private BoxCollider2D m_BoxCollider2D;

    private readonly HashSet<EnemyUnit> _enemySet = new();
    private IEnumerator _bombDamageCoroutine;

    void Start()
    {
        m_BoxCollider2D.size = new Vector2(Size.GAME_WIDTH, Size.GAME_HEIGHT);
    }

    private void Update()
    {
        if (PauseManager.IsGamePaused)
            return;
        
        transform.position = new Vector2(0f, -Size.GAME_HEIGHT/2);
    }

    private void LateUpdate()
    {
        if (PauseManager.IsGamePaused)
            return;
        
        DealBombDamage();
    }

    private void DealBombDamage()
    {
        if (_enemySet.Count == 0)
            return;

        foreach (var enemyUnit in _enemySet)
        {
            DealDamage(enemyUnit);
        }
    }

    public void Activate()
    {
        m_BoxCollider2D.enabled = true;
        _bombDamageCoroutine = BombDamageEnd();
        StartCoroutine(_bombDamageCoroutine);
    }

    public void Deactivate()
    {
        if (_bombDamageCoroutine != null)
            StopCoroutine(_bombDamageCoroutine);
        m_BoxCollider2D.enabled = false;
        
        _enemySet.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (PauseManager.IsGamePaused)
            return;
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();

            TriggerEnter(enemyUnit);
        }
    }

    private void OnTriggerExit2D(Collider2D other) // 충돌 감지
    {
        if (PauseManager.IsGamePaused)
            return;
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();

            TriggerExit(enemyUnit);
        }
    }

    public override void ExecuteCollisionEnter(int id)
    {
        var enemyUnit = ObjectIdList[id] as EnemyUnit;

        if (enemyUnit == null)
        {
            Debug.LogError($"{ObjectIdList[id].GetType()} (id: {id}) can not cast to EnemyUnit!");
            return;
        }

        TriggerEnter(enemyUnit);
    }

    public override void ExecuteCollisionExit(int id)
    {
        var enemyUnit = ObjectIdList[id] as EnemyUnit;

        if (enemyUnit == null)
        {
            Debug.LogError($"{ObjectIdList[id].GetType()} (id: {id}) can not cast to EnemyUnit!");
            return;
        }

        TriggerExit(enemyUnit);
    }

    private void TriggerEnter(EnemyUnit enemyUnit)
    {
        if (PauseManager.IsGamePaused)
            return;
        _enemySet.Add(enemyUnit);
    }

    private void TriggerExit(EnemyUnit enemyUnit)
    {
        if (PauseManager.IsGamePaused)
            return;
        _enemySet.Remove(enemyUnit);
    }

    private IEnumerator BombDamageEnd() {
        yield return new WaitForMillisecondFrames(m_BombDuration);
        OnBombRemoved();
    }

    private void OnBombRemoved() {
        gameObject.SetActive(false);
    }
}
