using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEditor;

[System.Serializable]
public class RankingAttributeData
{
    public Image attributesImage;
    public Sprite[] attributesSprites;
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

        SetSprite(0, shipAttributes.GetAttributes(AttributeType.Speed));
        SetSprite(1, shipAttributes.GetAttributes(AttributeType.ShotIndex));
        SetSprite(2, shipAttributes.GetAttributes(AttributeType.LaserIndex));
        SetSprite(3, shipAttributes.GetAttributes(AttributeType.ModuleIndex));
    }

    private void SetSprite(int attributeType, int attribute)
    {
        m_AttributesData[attributeType].attributesImage.sprite = m_AttributesData[attributeType].attributesSprites[attribute];
    }
}
