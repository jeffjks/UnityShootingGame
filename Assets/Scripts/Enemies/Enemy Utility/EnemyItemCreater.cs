using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemCreater : MonoBehaviour
{
    [SerializeField] private EnemyDeath m_EnemyDeath;
    [SerializeField] private Item m_Item;
    [SerializeField] private int m_ItemNumber;
    
    void Start()
    {
        m_EnemyDeath.Action_OnDeath += CreateItems;
    }

    private void CreateItems() {
        Vector3 item_pos;
        if (Utility.CheckLayer(gameObject, Layer.AIR)) {
            item_pos = new Vector3(transform.position.x, transform.position.y, Depth.ITEMS);
        }
        else {
            item_pos = transform.position;
        }

        var gemItem = m_Item as ItemGem;
        if (gemItem)
        {
            if (Utility.CheckLayer(gameObject, Layer.AIR)) {
                for (int i = 0; i < m_ItemNumber; i++) {
                    GameObject obj = PoolingManager.PopFromPool(gemItem.m_ObjectName, PoolingParent.GemAir);
                    Vector3 pos = transform.position + (Vector3) Random.insideUnitCircle * 0.8f;
                    obj.transform.position = new Vector3(pos.x, pos.y, Depth.ITEMS);
                    obj.SetActive(true);
                }
            }
            else {
                GameObject[] obj = new GameObject[m_ItemNumber];
                for (int i = 0; i < m_ItemNumber; i++) {
                    obj[i] = PoolingManager.PopFromPool(gemItem.m_ObjectName, PoolingParent.GemGround);
                }
                CreateGems(obj);
                for (int i = 0; i < m_ItemNumber; i++) {
                    obj[i].SetActive(true);
                }
            }
        }
        else
        {
            for (var i = 0; i < m_ItemNumber; ++i)
                Instantiate(m_Item, item_pos, Quaternion.identity);
        }
        
        if (m_ItemNumber == 0)
            Debug.LogWarning($"Item number is zero!");
    }

    private void CreateGems(GameObject[] obj) {
        switch(m_ItemNumber) {
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
                for (int i = 0; i < m_ItemNumber; i++) {
                    Vector3 vec = Random.insideUnitSphere * (Mathf.Sqrt(m_ItemNumber) / 2f);
                    obj[i].transform.position = transform.position + new Vector3(vec.x, 0f, vec.z);
                }
                break;
        }
    }
}
