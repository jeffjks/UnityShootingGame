using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1_Explosion : EnemyExplosionCreater
{
    /*
    protected override IEnumerator DyingExplosion()
    {
        CreateExplosionEffect(ExplType.None, ExplAudioType.AirMedium_2);
        StartCoroutine(DeathExplosion1(4900));
        yield return new WaitForMillisecondFrames(1500);

        StartCoroutine(DeathExplosion2(2800));
        StartCoroutine(DeathExplosion3(2800));

        yield return new WaitForMillisecondFrames(3500);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_2); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.2f, 2.2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.2f, 2.2f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(2f, 0f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-2f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.2f, -2.2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.2f, -2.2f));
        
        yield break;
    }

    private IEnumerator DeathExplosion1(int duration) {
        int timer = 0, t_add = 200;
        while (timer < duration) {
            CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0f, 1.225f), new MoveVector(5f, 180f));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(int duration) {
        int timer = 0, t_add = 0;
        Vector2 random_pos;
        while (timer < duration) {
            t_add = Random.Range(450, 600);
            random_pos = Random.insideUnitCircle * 2.5f;
            CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.GroundSmall, random_pos);
            random_pos = Random.insideUnitCircle * 2.5f;
            CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, random_pos);
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion3(int duration) {
        int timer = 0, t_add = 0;
        Vector2 random_pos;
        while (timer < duration) {
            t_add = Random.Range(150, 300);
            random_pos = Random.insideUnitCircle * 2.5f;
            CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.AirSmall, random_pos);
            random_pos = Random.insideUnitCircle * 2.5f;
            CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, random_pos);
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }*/
}
