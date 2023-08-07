using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SubBulletPattern : BulletFactory, IBulletPattern
{
    public BulletProperty m_BulletProperty;

    public SubBulletPattern(EnemyObject enemyObject) : base(enemyObject)
    {
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted = null)
    {
        CreateBullet(m_BulletProperty);
        onCompleted?.Invoke();
        yield break;
    }
}
