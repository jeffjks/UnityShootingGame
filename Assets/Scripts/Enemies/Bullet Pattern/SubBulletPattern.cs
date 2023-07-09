using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBulletPattern : BulletFactory, IBulletPattern
{
    public BulletProperty m_BulletProperty;

    public SubBulletPattern(EnemyObject enemyObject) : base(enemyObject)
    {
    }
    
    public IEnumerator ExecutePattern(int patternIndex = 0)
    {
        CreateBullet(m_BulletProperty);
        yield break;
    }
}
