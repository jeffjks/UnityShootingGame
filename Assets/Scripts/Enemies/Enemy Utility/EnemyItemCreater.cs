using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemCreater : MonoBehaviour
{
    [SerializeField] private EnemyDeath m_EnemyDeath;
    [SerializeField] private GameObject m_ItemBox = null;
    [SerializeField] private byte m_GemNumber = 0;
    private PoolingManager m_PoolingManager = null;
    
    void Start()
    {
        m_PoolingManager = PoolingManager.instance_op;
        m_EnemyDeath.Action_OnDeath += CreateItems;
    }

    private void CreateItems() {
        if (m_ItemBox != null) { // 아이템 드랍
            Vector3 item_pos;
            if ((1 << gameObject.layer & Layer.AIR) != 0) {
                item_pos = new Vector3(transform.position.x, transform.position.y, Depth.ITEMS);
            }
            else {
                item_pos = transform.position;
            }
            Instantiate(m_ItemBox, item_pos, Quaternion.identity);
        }

        if ((1 << gameObject.layer & Layer.AIR) != 0) {
            for (int i = 0; i < m_GemNumber; i++) {
                GameObject obj = m_PoolingManager.PopFromPool("ItemGemAir", PoolingParent.ITEM_GEM_AIR);
                Vector3 pos = transform.position + (Vector3) UnityEngine.Random.insideUnitCircle * 0.8f;
                obj.transform.position = new Vector3(pos.x, pos.y, Depth.ITEMS);
                obj.SetActive(true);
            }
        }
        else {
            GameObject[] obj = new GameObject[m_GemNumber];
            for (int i = 0; i < m_GemNumber; i++) {
                obj[i] = m_PoolingManager.PopFromPool("ItemGemGround", 3);
            }
            CreateGems(obj);
            for (int i = 0; i < m_GemNumber; i++) {
                obj[i].SetActive(true);
            }
        }
    }

    private void CreateGems(GameObject[] obj) {
        switch(m_GemNumber) {
            case 1:
                obj[0].transform.position = transform.position;
                break;
            case 2:
                obj[0].transform.position = transform.position + new Vector3(0f, 0f, 0.25f);
                obj[1].transform.position = transform.position + new Vector3(0f, 0f, -0.25f);
                break;
            case 3:
                obj[0].transform.position = transform.position + new Vector3(0f, 0f, 0.25f);
                obj[1].transform.position = transform.position + new Vector3(-0.29f, 0f, -0.25f);
                obj[2].transform.position = transform.position + new Vector3(0.29f, 0f, -0.25f);
                break;
            case 4:
                obj[0].transform.position = transform.position + new Vector3(-0.25f, 0f, 0.25f);
                obj[1].transform.position = transform.position + new Vector3(-0.25f, 0f, -0.25f);
                obj[2].transform.position = transform.position + new Vector3(0.25f, 0f, 0.25f);
                obj[3].transform.position = transform.position + new Vector3(0.25f, 0f, -0.25f);
                break;
            case 5:
                obj[0].transform.position = transform.position + new Vector3(-0.3f, 0f, 0.3f);
                obj[1].transform.position = transform.position + new Vector3(-0.3f, 0f, -0.3f);
                obj[2].transform.position = transform.position;
                obj[3].transform.position = transform.position + new Vector3(0.3f, 0f, 0.3f);
                obj[4].transform.position = transform.position + new Vector3(0.3f, 0f, -0.3f);
                break;
            default:
                for (int i = 0; i < m_GemNumber; i++) {
                    Vector3 vec = UnityEngine.Random.insideUnitSphere * Mathf.Sqrt(m_GemNumber) * 0.5f;
                    obj[i].transform.position = transform.position + new Vector3(vec.x, 0f, vec.z);
                }
                break;
        }
    }
}
