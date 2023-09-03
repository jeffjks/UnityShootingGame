using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

public class ExplosionJsonManager : MonoBehaviour
{
    private string[] _poolingString;
    private Dictionary<string, List<ExplosionData>> _explosionJsonData;

    public static ExplosionJsonManager Instance { get; private set; }

    private void Start()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitExplosionEffectString();
        SystemManager.Action_OnQuitInGame += StopAllCoroutines;

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        SystemManager.Action_OnQuitInGame -= StopAllCoroutines;
    }

    private void OnEnable()
    {
        _explosionJsonData = Utility.LoadDataFile<Dictionary<string, List<ExplosionData>>>(Application.streamingAssetsPath, "resources1.dat").jsonData;
    }

    private void InitExplosionEffectString()
    {
        _poolingString = new[] {
            "ExplosionGround_1",
            "ExplosionGround_2",
            "ExplosionGround_3",
            "ExplosionNormal_1",
            "ExplosionNormal_2",
            "ExplosionNormal_3",
            "ExplosionSimple_1",
            "ExplosionSimple_2",
            "ExplosionStarShape",
            "ExplosionMineShape"
        };
    }

    private string GetPoolingString(ExplType ExplType) {
        return _poolingString[(int) ExplType];
    }



    public void StartExplosionEffect(string enemyKey, EnemyDeath enemyDeath) {
        StartCoroutine(ExplosionEffectSequence(enemyKey, enemyDeath));
    }

    private IEnumerator ExplosionEffectSequence(string enemyKey, EnemyDeath enemyDeath) {
        List<ExplosionData> list = new ();
        try {
            list = _explosionJsonData[enemyKey];
        }
        catch (System.Exception e) {
            Debug.Log(e);
            OnExplosionEnd(enemyDeath);
            yield break;
        }

        foreach (var value in list) {
            if (value.effect == null) {
                yield return new WaitForMillisecondFrames(value.waitAfter);
                continue;
            }
            if (value.coroutine == null) {
                CreateExplosionEffect(enemyDeath, value.effect);
            }
            else {
                StartCoroutine(CreateExplosionSequence(enemyDeath, value.effect, value.coroutine));
            }
            if (value.waitAfter > 0) {
                yield return new WaitForMillisecondFrames(value.waitAfter);
            }
        }
        
        OnExplosionEnd(enemyDeath);
    }

    public void CreateExplosionEffect(EnemyDeath enemyDeath, Effect effect, int num = 0) {
        if (effect == null) {
            return;
        }

        ExplType ExplType = effect.explType;
        ExplAudioType ExplAudioType = effect.explAudioType;
        Vector3 transformPosition = effect.position;
        float radius = effect.radius;
        MoveVector moveVector = new MoveVector(Random.Range(effect.speed[0], effect.speed[1]), Random.Range(effect.direction[0], effect.direction[1]));

        if (ExplType != ExplType.None) {
            GameObject explosionObject = PoolingManager.PopFromPool(GetPoolingString(ExplType), PoolingParent.Explosion);
            ExplosionEffecter explosionEffecter = explosionObject.GetComponent<ExplosionEffecter>();

            explosionEffecter.m_MoveVector = moveVector;
            
            Vector3 explosionPosition = enemyDeath.transform.TransformPoint(transformPosition);
            Vector2 randomPos = (radius == 0f) ? Vector2.zero : Random.insideUnitCircle * radius;
            
            if (Utility.CheckLayer(enemyDeath.gameObject, Layer.AIR)) {
                explosionPosition = new Vector3(explosionPosition.x + randomPos.x, explosionPosition.y + randomPos.y, Depth.EXPLOSION);
            }
            else {
                explosionPosition = new Vector3(explosionPosition.x + randomPos.x, explosionPosition.y, explosionPosition.z + randomPos.y);
            }
            
            explosionObject.transform.position = explosionPosition;
            explosionObject.SetActive(true);
        }
        
        
        if (ExplAudioType != ExplAudioType.None && num == 0) {
            AudioService.PlaySound(ExplAudioType);
        }
    }

    private IEnumerator CreateExplosionSequence(EnemyDeath enemyDeath, Effect effect, ExplosionCoroutine coroutine) {
        int duration = coroutine.duration;
        int timer = 0;
        int number = coroutine.number;

        while (timer < duration || duration == -1) {
            if (enemyDeath == null) {
                yield break;
            }
            for (int i = 0; i < number; ++i) {
                CreateExplosionEffect(enemyDeath, effect, i);
            }
            var timer_add = Random.Range(coroutine.timer_add[0], coroutine.timer_add[1]);
            timer += timer_add;
            yield return new WaitForMillisecondFrames(timer_add);
        }
    }

    private void OnExplosionEnd(EnemyDeath enemyDeath) {
        if (enemyDeath.IsDead) {
            enemyDeath.OnEndDeathAnimation();
        }
    }
}
