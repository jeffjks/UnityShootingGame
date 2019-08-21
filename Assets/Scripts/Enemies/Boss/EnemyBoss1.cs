using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1 : EnemyUnit
{
    void Start()
    {
        m_MoveVector = new MoveVector(7f, 0f);
        StartCoroutine(StartDecelerate());
        UpdateTransform();
    }

    private IEnumerator StartDecelerate() {
        yield return new WaitForSeconds(0.6f);
        while(m_MoveVector.speed > 0) {
            m_MoveVector.speed = Mathf.MoveTowards(m_MoveVector.speed, 0f, 5.6f*Time.deltaTime);
            yield return null;
        }
        yield break;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
        yield break;
    }
}
