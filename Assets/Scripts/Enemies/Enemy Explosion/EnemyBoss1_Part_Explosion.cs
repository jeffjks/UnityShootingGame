using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1_Part_Explosion : EnemyExplosionCreater
{
    protected override IEnumerator DyingExplosion()
    {
        CreateExplosionEffect(ExplosionEffect.None, ExplosionAudio.AirMedium_1);

        CreateExplosionEffect(ExplosionEffect.General_2, ExplosionAudio.None, new Vector3(-0.66f, 0f, 0f));
        CreateExplosionEffect(ExplosionEffect.General_2, ExplosionAudio.None, new Vector3(0.66f, 0f, 0f));
        CreateExplosionEffect(ExplosionEffect.General_2, ExplosionAudio.None, new Vector3(-0.62f, 0f, 0.33f));
        CreateExplosionEffect(ExplosionEffect.General_2, ExplosionAudio.None, new Vector3(0.62f, 0f, 0.33f));
        CreateExplosionEffect(ExplosionEffect.General_3, ExplosionAudio.None, new Vector3(-0.69f, 0f, -0.4f));
        CreateExplosionEffect(ExplosionEffect.General_3, ExplosionAudio.None, new Vector3(0.69f, 0f, -0.4f));
        
        m_EnemyDeath.OnDeath();
        yield break;
    }
}
