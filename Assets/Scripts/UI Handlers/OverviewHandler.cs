using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class OverviewHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_GroundGem = null;
    [SerializeField] private GameObject m_AirGem = null;
    [SerializeField] private GameObject m_RemainsScore = null;
    [SerializeField] private GameObject m_TotalStageScore = null;
    [SerializeField] private GameObject m_Miss = null;
    [SerializeField] private GameObject m_FinalBonus = null;
    [SerializeField] private SpriteRenderer m_OverviewBackground = null;
    
    private Text m_FinalBonusScoreText;
    private int m_DisplayTimer = 0;
    private byte m_DisplayStage = 0;
    private float m_BonusScale = 0;
    private uint m_FinalBonusScore = 0;

    private SystemManager m_SystemManager = null;

    void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_FinalBonusScoreText = m_FinalBonus.GetComponentsInChildren<Text>()[1];
        transform.position = new Vector3(transform.position.x, transform.position.y, Depth.OVERVIEW);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            if (m_DisplayStage == 4) {
                m_SystemManager.AddScore(m_FinalBonusScore);
                m_FinalBonusScore = 0;
                UpdateFinalBonusScore();
            }
            else if (m_DisplayStage == 5) {
                m_SystemManager.StartCoroutine("NextStage");
                GoToNextDispalyStage();
            }
            m_DisplayTimer = 100;
        }
    }

    void LateUpdate()
    {
        if (m_DisplayStage <= 4)
            m_DisplayTimer++;
            
        if (m_DisplayStage == 0) { // 0
            if (m_DisplayTimer > 60) {
                GoToNextDispalyStage();
            }
        }
        else if (m_DisplayStage == 1) { // 1
            if (m_DisplayTimer > 60) {
                GoToNextDispalyStage();
                m_GroundGem.SetActive(true);
                m_AirGem.SetActive(true);
                m_RemainsScore.SetActive(true);
                m_TotalStageScore.SetActive(true);
            }
        }
        else if (m_DisplayStage == 2) { // 2
            if (m_DisplayTimer > 60) {
                GoToNextDispalyStage();
                m_Miss.SetActive(true);
            }
        }
        else if (m_DisplayStage == 3) { // 3
            if (m_DisplayTimer > 90) {
                GoToNextDispalyStage();
                m_FinalBonus.SetActive(true);
            }
        }
        else if (m_DisplayStage == 4) { // 4
            if (m_DisplayTimer > 90) {
                for (sbyte i = 10; i >= 0; i--) {
                    uint target_score = (uint) Mathf.Pow(7f, i);
                    if (m_FinalBonusScore >= target_score) {
                        m_FinalBonusScore -= target_score;
                        m_SystemManager.AddScore(target_score);
                        UpdateFinalBonusScore();
                        break;
                    }
                }
            }
            if (m_FinalBonusScore == 0) {
                GoToNextDispalyStage(); // 5일때 버튼 클릭시 다음 스테이지
            }
        }
    }

    private void GoToNextDispalyStage() {
        DOTween.Kill(m_OverviewBackground);
        m_DisplayTimer = 0;
        m_DisplayStage++;
    }

    private void Init() {
        Text[] ground_gem = m_GroundGem.GetComponentsInChildren<Text>();
        Text[] air_gem = m_AirGem.GetComponentsInChildren<Text>();
        Text[] remains_score = m_RemainsScore.GetComponentsInChildren<Text>();
        Text[] total_stage_score = m_TotalStageScore.GetComponentsInChildren<Text>();
        Text[] miss = m_Miss.GetComponentsInChildren<Text>();
        m_OverviewBackground.DOFade(0f, 1f);
        
        uint ground_gem_score = m_SystemManager.GemsGround * ItemScore.GEM_GROUND;
        uint air_gem_score = m_SystemManager.GemsAir * ItemScore.GEM_AIR;
        byte stage_miss = m_SystemManager.GetStageMiss();
        uint stage_score = m_SystemManager.GetStageScore();
        string bonus_scale;

        if (stage_miss == 0) {
            m_BonusScale = BonusScale.BONUS_0;
            bonus_scale = "[ 50%! ]";
        }
        else if (stage_miss == 1) {
            m_BonusScale = BonusScale.BONUS_1;
            bonus_scale = "[ 30%! ]";
        }
        else if (stage_miss == 2) {
            m_BonusScale = BonusScale.BONUS_2;
            bonus_scale = "[ 10%! ]";
        }
        else {
            m_BonusScale = 0;
            bonus_scale = "[ 0% ]";
        }

        m_FinalBonusScore = (uint) (stage_score * m_BonusScale);

        ground_gem[0].text = "X " + m_SystemManager.GemsGround;
        ground_gem[1].text = "" + ground_gem_score;
        air_gem[0].text = "X " + m_SystemManager.GemsAir;
        air_gem[1].text = "" + air_gem_score;
        remains_score[1].text = "" + (stage_score - ground_gem_score - air_gem_score);
        total_stage_score[1].text = "" + stage_score;
        miss[0].text = stage_miss + " Miss";
        miss[1].text = bonus_scale;
        UpdateFinalBonusScore();
    }

    private void UpdateFinalBonusScore() {
        m_FinalBonusScoreText.text = "" + m_FinalBonusScore;
    }

    public void DisplayOverview() {
        m_GroundGem.SetActive(false);
        m_AirGem.SetActive(false);
        m_RemainsScore.SetActive(false);
        m_TotalStageScore.SetActive(false);
        m_Miss.SetActive(false);
        m_FinalBonus.SetActive(false);
        m_DisplayTimer = 0;
        m_DisplayStage = 0;
        m_BonusScale = 0;
        m_FinalBonusScore = 0;
        m_OverviewBackground.color = new Color(1f, 1f, 1f, 0f);
        Init();
    }
}
