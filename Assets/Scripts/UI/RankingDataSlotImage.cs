using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class RankingDataSlotImage : RankingDataSlot
{
    public Image[] m_AttributeSpeed = new Image[3];
    public Image[] m_AttributeShot = new Image[3];
    public Image[] m_AttributeLaser = new Image[3];
    public Image[] m_AttributeModule = new Image[4];

    private readonly List<Image[]> _attributesImageList = new();
    private readonly GameObject[] _attributeSlots = new GameObject[5];

    private RankingDataSlotImage() {
        _attributesImageList.Add(m_AttributeSpeed);
        _attributesImageList.Add(m_AttributeShot);
        _attributesImageList.Add(m_AttributeLaser);
        _attributesImageList.Add(m_AttributeModule);
    }

    void OnDisable()
    {
        DestroyAllAttributeSlots();
    }

    public override void SetRankingData(string text) {
        Debug.LogWarning("Ranking slot warning: Type unmatched.");
    }

    public override void SetRankingData(int shipAttributesCode) {
        DestroyAllAttributeSlots();

        bool[] display = { false, true, true, true, true, false };
        int num = -1;
        for (int i = 0; i < display.Length; ++i) {
            if (display[i]) {
                num++;
            }
        }
        int check = 0;

        while(num >= 0) {
            try {
                if (!display[check++]) {
                    shipAttributesCode /= 10;
                    continue;
                }
                _attributeSlots[num] = new GameObject("Slot ("+(num+1)+")"); // Color, "Speed", "Shot", "Laser", "Module", Bomb
                
                RectTransform rectTransform = _attributeSlots[num].AddComponent<RectTransform>();
                rectTransform.position = new Vector3(num*40f, 0f, 0f);
                rectTransform.sizeDelta = new Vector2(40f, 40f);
                rectTransform.anchorMin = new Vector2(0f, 0.5f);
                rectTransform.anchorMax = new Vector2(0f, 0.5f);
                rectTransform.pivot = new Vector2(0f, 0.5f);
                rectTransform.localScale = new Vector2(1f, 1f);

                Image image = _attributeSlots[num].AddComponent<Image>();
                image.sprite = _attributesImageList[num][shipAttributesCode % 10].sprite;
                image.color = Color.white;

                _attributeSlots[num].transform.SetParent(this.transform, false);
                shipAttributesCode /= 10;
                num--;
            }
            catch (System.IndexOutOfRangeException) {
                num--;
                continue;
            }
        }
    }

    private void DestroyAllAttributeSlots() {
        for (int i = 0; i < _attributeSlots.Length; i++) {
            Destroy(_attributeSlots[i]);
        }
    }
}
