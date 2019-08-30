using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour {
    public GameObject ShieldBody;
    public GameObject Ring1;
    public GameObject Ring2;
    public float RotateSpeed;
    public float ShieldOutShineTimeSpace;

    float curTime;
    bool isShine = false;
    Material shieldMat;
    float offsetY = 1;

	void Start ()
    {
        shieldMat = ShieldBody.GetComponent<MeshRenderer>().material;
    }
    
    void Update()
    {
        transform.localRotation = Quaternion.identity;
    }

	void FixedUpdate ()
    {
        Ring1.transform.Rotate(Vector3.right, Time.deltaTime * RotateSpeed);
        Ring2.transform.Rotate(Vector3.forward, Time.deltaTime * RotateSpeed);

        if (isShine) {
            offsetY += 0.025f;
            shieldMat.SetFloat("_ScanningOffsetY", offsetY);
            if (offsetY > 0.63) {
                offsetY = -0.64f;
                isShine = false;
                curTime = 0;
            }
        }
        else {
            curTime += Time.deltaTime;
            if (curTime >= ShieldOutShineTimeSpace) {
                isShine = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("EnemyBullet")) { // 대상이 총알이면 대상 파괴
            EnemyBullet enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
            enemyBullet.OnDeath();
        }
    }
}
