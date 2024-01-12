using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombDamage : PlayerObject
{
    public int m_BombDuration;
    [SerializeField] private BoxCollider2D m_BoxCollider2D = null;

    void Start()
    {
        m_BoxCollider2D.size = new Vector2(Size.GAME_WIDTH, Size.GAME_HEIGHT);
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        transform.position = new Vector2(0f, -Size.GAME_HEIGHT/2);
    }

    void OnEnable()
    {
        StartCoroutine(BombDamageEnd());
    }

    void OnTriggerStay2D(Collider2D other) // 충돌 감지
    {
        if (PauseManager.IsGamePaused)
            return;
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
            DealDamage(enemyObject);
        }
    }

    private IEnumerator BombDamageEnd() {
        yield return new WaitForMillisecondFrames(m_BombDuration);
        OnBombRemoved();
    }

    private void OnBombRemoved() {
        gameObject.SetActive(false);
    }
}
