using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEditor;

[System.Serializable]
public struct RankingAttributeData
{
    public Image attributesImage;
    public Sprite[] attributesSprites;
}

[CustomEditor(typeof(AttributesDataList))]
public class AttributesDataEditor : Editor
{
    private SerializedProperty _attributesDataList;

    private void OnEnable()
    {
        _attributesDataList = serializedObject.FindProperty("m_AttributesData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_attributesDataList, true);
        serializedObject.ApplyModifiedProperties();
    }
}

public class AttributesDataList : MonoBehaviour
{
    public List<RankingAttributeData> m_AttributesData = new List<RankingAttributeData>();
}

public class RankingDataSlotImage : RankingDataSlot
{
    public GameObject m_AttributesSlot;
    public List<RankingAttributeData> m_AttributesData = new();

    public override void InitRankingData()
    {
        m_AttributesSlot.SetActive(false);
    }

    public override void SetRankingData(string text) {
        Debug.LogWarning("Ranking slot warning: Type unmatched.");
    }

    public override void SetRankingData(ShipAttributes shipAttributes) {
        if (!m_AttributesSlot.activeSelf)
        {
            m_AttributesSlot.SetActive(true);
        }

        int[] attributes = { shipAttributes.m_Speed, shipAttributes.m_ShotLevel, shipAttributes.m_LaserLevel, shipAttributes.m_Module };
        
        for (int i = 0; i < m_AttributesData.Count; ++i)
        {
            m_AttributesData[i].attributesImage.sprite = m_AttributesData[i].attributesSprites[attributes[i]];
        }
    }
}
