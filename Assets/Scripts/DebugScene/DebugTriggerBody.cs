using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTriggerBody : MonoBehaviour
{
    private bool _isCreatingBullets;

    private void Update()
    {
        CreateBulelt();
    }

    public void OnClickStart()
    {
        _isCreatingBullets = !_isCreatingBullets;
    }

    private void CreateBulelt()
    {
        if (_isCreatingBullets == false)
            return;

        BulletProperty property = new BulletProperty(new Vector3(0f, -2f, Depth.ENEMY_BULLET), BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, Random.Range(0f, 360f));
        GameObject bulletObject = PoolingManager.PopFromPool("EnemyBullet", PoolingParent.EnemyBullet);
        var enemyBullet = bulletObject.GetComponent<EnemyBullet>();
        enemyBullet.transform.position = property.startPos;
        bulletObject.SetActive(true);
        enemyBullet.OnStart(property);
    }
}