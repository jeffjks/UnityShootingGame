using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBombHandler : MonoBehaviour
{
    public GameObject m_Bomb, m_Explosion, m_BombDamage;

    public static event UnityAction Action_OnBombUse;
    private static bool _isBombInUse;

    public static bool IsBombInUse
    {
        get => _isBombInUse;
        private set
        {
            _isBombInUse = value;
            Action_OnBombUse?.Invoke();
        }
    }

    private const int TARGET_TIMER = 400;
    private const int REMOVE_TIMER = 2600; // TARGET_TIMER 이후

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void UseBomb(Vector3 playerPosition)
    {
        IsBombInUse = true;
        m_Bomb.SetActive(true);

        playerPosition.z = Depth.PLAYER_MISSILE;
        m_Bomb.transform.position = playerPosition;
        var destPosition_x = Mathf.Clamp(playerPosition.x, -3f, 3f);
        var destPosition_y = - Size.GAME_HEIGHT/2 - 1f;
        Vector3 destPosition = new Vector3(destPosition_x, destPosition_y, Depth.PLAYER_MISSILE);

        m_Bomb.transform.rotation = Quaternion.LookRotation(destPosition - playerPosition);
        
        StartCoroutine(BombExplosion(playerPosition, destPosition));
    }

    private IEnumerator BombExplosion(Vector3 startPos, Vector3 destPos) {
        int frame = TARGET_TIMER * Application.targetFrameRate / 1000;
        AudioService.PlaySound("PlayerBomb1");

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            m_Bomb.transform.position = Vector3.Lerp(startPos, destPos, t_pos);
            yield return new WaitForFrames(1);
        }
        m_Explosion.transform.position = destPos;
        
        BulletManager.SetBulletFreeState(2000);
        m_Bomb.SetActive(false);
        m_Explosion.SetActive(true);
        m_BombDamage.SetActive(true);
        AudioService.PlaySound("PlayerBomb2");

        yield return new WaitForMillisecondFrames(REMOVE_TIMER);
        m_Bomb.SetActive(false);
        m_Explosion.SetActive(false);
        m_BombDamage.SetActive(false);
        IsBombInUse = false;
    }
}