using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemCreater : MonoBehaviour
{
    [SerializeField] private EnemyDeath m_EnemyDeath;
    [SerializeField] private ItemDatas m_ItemData;
    [SerializeField] private ItemType m_ItemType;
    [SerializeField] private int m_ItemNumber;

    private List<ItemDatas.ItemElement> _itemElementList;
    
    void Start()
    {
        m_EnemyDeath.Action_OnEndDeathAnimation += CreateItems;
        _itemElementList = m_ItemData.itemElementList;
    }

    private void CreateItems()
    {
        if (m_ItemType == ItemType.None)
            return;
        
        Vector3 itemPos;
        if (gameObject.CheckLayer(Layer.AIR)) {
            itemPos = new Vector3(transform.position.x, transform.position.y, Depth.ITEMS);
        }
        else {
            itemPos = transform.position;
        }

        var item = _itemElementList[(int)m_ItemType].Item;

        var gemItem = item as ItemGem;
        if (gemItem)
        {
            if (gameObject.CheckLayer(Layer.AIR)) {
                for (int i = 0; i < m_ItemNumber; i++) {
                    GameObject obj = PoolingManager.PopFromPool(gemItem.m_ObjectName, PoolingParent.GemAir);
                    var pos = (Vector2) transform.position + Random.insideUnitCircle * 0.8f;
                    var gemAir = obj.GetComponent<ItemGemAir>();
                    obj.transform.position = new Vector3(pos.x, pos.y, Depth.ITEMS);
                    obj.SetActive(true);
                    gemAir.Init();
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
                Instantiate(item, itemPos, Quaternion.identity);
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
                    var vec = Random.insideUnitCircle * (Mathf.Sqrt(m_ItemNumber) * 0.7f);
                    obj[i].transform.position = transform.position + new Vector3(vec.x, 0f, vec.y);
                }
                break;
        }
    }
}
