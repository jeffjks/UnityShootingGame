using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankMedium2 : EnemyUnit
{
    public Animator m_ArmorAnimator;

    private readonly int _armorOpenAnimationBool = Animator.StringToHash("Opened");

    private bool _isStartShooing;

    private void Start()
    {
        SetRotatePattern(new RotatePattern_MoveDirection());
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;
        
        if (!_isStartShooing) {
            if (m_IsInteractable && m_Position2D.y < - 1.2f) {
                m_ArmorAnimator.SetBool(_armorOpenAnimationBool, true);
                StartPattern("A", new EnemyTankMedium2_BulletPattern(this));
                _isStartShooing = true;
            }
        }
    }
}

public class EnemyTankMedium2_BulletPattern : BulletFactory, IBulletPattern
{
    public EnemyTankMedium2_BulletPattern(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => _enemyObject.IsInteractable());
        yield return new WaitForMillisecondFrames(Random.Range(800, 1800));
        
        while(true)
        {
            Vector3 pos;
            float dir;
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                pos = GetFirePos(0);
                dir = Random.Range(0f, 360f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.6f, BulletPivot.Fixed, dir, 18, 20f));
                yield return new WaitForMillisecondFrames(1000);
                
                pos = GetFirePos(0);
                dir = Random.Range(0f, 360f);
                var property = new BulletProperty(pos, BulletImage.PinkSmall, 6.6f, BulletPivot.Fixed, dir, 10, 36f);
                var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 200);
                var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkNeedle, 5.8f, BulletPivot.Player, Random.Range(-2f, 2f));
                CreateBullet(property, spawnTiming, subProperty);
                yield return new WaitForMillisecondFrames(1000);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                for (int i = 0; i < 5; i++) {
                    pos = GetFirePos(0);
                    dir = Random.Range(0f, 360f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.6f, BulletPivot.Fixed, dir, 30, 12f));
                    yield return new WaitForMillisecondFrames(380);
                }
                yield return new WaitForMillisecondFrames(700);
                for (int i = 0; i < 3; i++) {
                    pos = GetFirePos(0);
                    dir = Random.Range(0f, 360f);
                    var property = new BulletProperty(pos, BulletImage.PinkSmall, 6.6f, BulletPivot.Fixed, dir, 24, 15f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 200);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkNeedle, 6f, BulletPivot.Player, Random.Range(-2f, 2f));
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(380);
                }
                yield return new WaitForMillisecondFrames(700);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Hell)
            {
                for (int i = 0; i < 5; i++) {
                    pos = GetFirePos(0);
                    dir = Random.Range(0f, 360f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.6f, BulletPivot.Fixed, dir, 36, 10f));
                    yield return new WaitForMillisecondFrames(380);
                }
                yield return new WaitForMillisecondFrames(700);
                for (int i = 0; i < 3; i++) {
                    pos = GetFirePos(0);
                    dir = Random.Range(0f, 360f);
                    var property = new BulletProperty(pos, BulletImage.PinkSmall, 6.6f, BulletPivot.Fixed, dir, 30, 12f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 200);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkNeedle, 6.5f, BulletPivot.Player, Random.Range(-2f, 2f));
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(380);
                }
                yield return new WaitForMillisecondFrames(700);
            }
        }
        //onCompleted?.Invoke();
    }
}