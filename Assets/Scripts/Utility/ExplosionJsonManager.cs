using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class ExplosionJsonManager : MonoBehaviour
{
    [SerializeField] private ExplosionSoundPlayer m_ExplosionSoundPlayer;
    private string[] m_PoolingString;
    private AudioClip[] m_ExplosionAudio;
    private Dictionary<string, List<ExplosionData>> m_ExplosionJsonData;

    private PoolingManager m_PoolingManager = null;

    public static ExplosionJsonManager instance_jm = null;

    private void Start()
    {
        if (instance_jm != null) {
            Destroy(this.gameObject);
            return;
        }
        instance_jm = this;

        m_PoolingManager = PoolingManager.instance_op;
        
        InitExplosionEffectString();

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        OpenJsonFile();
    }

    private void InitExplosionEffectString()
    {
        m_PoolingString = new string[] {
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
        return m_PoolingString[(int) ExplType];
    }


    private void OpenJsonFile() {
        m_ExplosionJsonData = LoadJsonFile<Dictionary<string, List<ExplosionData>>>(Application.dataPath, "resources1");
    }
    
    private T LoadJsonFile<T>(string filePath, string fileName) {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.dat", filePath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string encryptedStr = Encoding.UTF8.GetString(data);
        string jsonData = AESEncrypter.AESDecrypt128(encryptedStr);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }



    public void StartExplosionEffect(string enemyKey, EnemyDeath enemyDeath) {
        StartCoroutine(ExplosionEffectSequence(enemyKey, enemyDeath));
    }

    private IEnumerator ExplosionEffectSequence(string enemyKey, EnemyDeath enemyDeath) {
        List<ExplosionData> list = new List<ExplosionData>();
        try {
            list = m_ExplosionJsonData[enemyKey];
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
                StartCoroutine(CreateExplosionSequnce(enemyDeath, value.effect, value.coroutine));
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
            GameObject explosionObject = m_PoolingManager.PopFromPool(GetPoolingString(ExplType), PoolingParent.EXPLOSION);
            ExplosionEffecter explosion_effecter = explosionObject.GetComponent<ExplosionEffecter>();

            explosion_effecter.m_MoveVector = moveVector;
            
            Vector3 explosionPosition = enemyDeath.transform.TransformPoint(transformPosition);
            Vector2 randomPos = (radius == 0f) ? Vector2.zero : Random.insideUnitCircle * radius;
            
            if ((1 << enemyDeath.gameObject.layer & Layer.AIR) != 0) { // 공중
                explosionPosition = new Vector3(explosionPosition.x + randomPos.x, explosionPosition.y + randomPos.y, Depth.EXPLOSION);
            }
            else { // 지상
                explosionPosition = new Vector3(explosionPosition.x + randomPos.x, explosionPosition.y, explosionPosition.z + randomPos.y);
            }
            
            explosionObject.transform.position = explosionPosition;
            explosionObject.SetActive(true);
        }
        
        
        if (ExplAudioType != ExplAudioType.None && num == 0) {
            m_ExplosionSoundPlayer.PlayAudio(ExplAudioType);
        }
    }

    private IEnumerator CreateExplosionSequnce(EnemyDeath enemyDeath, Effect effect, Coroutine coroutine) {
        int duration = coroutine.duration;
        int timer = 0;
        int timer_add = Random.Range(coroutine.timer_add[0], coroutine.timer_add[1]);
        int number = coroutine.number;

        while (timer < duration || duration == -1) {
            if (enemyDeath == null) {
                yield break;
            }
            for (int i = 0; i < number; ++i) {
                CreateExplosionEffect(enemyDeath, effect, i);
            }
            timer += timer_add;
            yield return new WaitForMillisecondFrames(timer_add);
        }
        yield break;
    }

    private void OnExplosionEnd(EnemyDeath enemyDeath) {
        if (enemyDeath.m_IsDead) {
            enemyDeath.OnDeath();
        }
    }
}
