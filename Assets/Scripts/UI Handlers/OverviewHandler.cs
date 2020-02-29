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
    private float m_DisplayTimer;
    private byte m_DisplayStage;
    private float m_BonusScale;
    private uint m_FinalBonusScore;

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
            if (m_DisplayStage == 5) {
                m_SystemManager.AddScore(m_FinalBonusScore);
                m_FinalBonusScore = 0;
                m_DisplayTimer = 0f;
                UpdateFinalBonusScore();
            }
            else if (m_DisplayStage == 6) {
                GoToNextDispalyStage();
                GoToNextStage(); // 5일때 버튼 클릭시 다음 스테이지
            }
            else {
                GoToNextDispalyStage();
            }
        }
    }

    void LateUpdate()
    {
        if (m_DisplayStage < 4) // 최종 점수 화면 이전에는 점수판 자동 진행
            m_DisplayTimer += Time.deltaTime*60f;
            
        if (m_DisplayStage == 0) { // 첫 화면
            if (m_DisplayTimer > 60f) {
                GoToNextDispalyStage();
            }
        }
        else if (m_DisplayStage == 1) { // Gem 점수
            if (m_DisplayTimer > 60f) {
                GoToNextDispalyStage();
                m_GroundGem.SetActive(true);
                m_AirGem.SetActive(true);
                m_RemainsScore.SetActive(true);
                m_TotalStageScore.SetActive(true);
            }
        }
        else if (m_DisplayStage == 2) { // 미스 횟수
            if (m_DisplayTimer > 60f) {
                GoToNextDispalyStage();
                m_Miss.SetActive(true);
            }
        }
        else if (m_DisplayStage == 3) { // 최종 보너스
            if (m_DisplayTimer > 90f) {
                GoToNextDispalyStage();
                m_FinalBonus.SetActive(true);
            }
        }
        else if (m_DisplayStage == 5) { // 최종 보너스 합산
            for (sbyte i = 10; i >= 0; i--) {
                uint target_score = (uint) Mathf.Pow(7f, i);
                if (m_FinalBonusScore >= target_score) {
                    m_FinalBonusScore -= target_score;
                    m_SystemManager.AddScore(target_score);
                    UpdateFinalBonusScore();
                    break;
                }
            }
            if (m_FinalBonusScore == 0) {
                GoToNextDispalyStage();
            }
        }
    }

    private void GoToNextDispalyStage() {
        m_DisplayTimer = 0f;
        m_DisplayStage++;
    }
    
    private void GoToNextStage() {
        DOTween.Kill(m_OverviewBackground);
        m_OverviewBackground.color = Color.white;
        m_SystemManager.StartCoroutine("NextStage");
    }

    private void Init() {
        Text[] ground_gem = m_GroundGem.GetComponentsInChildren<Text>();
        Text[] air_gem = m_AirGem.GetComponentsInChildren<Text>();
        Text[] remains_score = m_RemainsScore.GetComponentsInChildren<Text>();
        Text[] total_stage_score = m_TotalStageScore.GetComponentsInChildren<Text>();
        Text[] miss = m_Miss.GetComponentsInChildren<Text>();
        m_OverviewBackground.DOFade(1f, 1f);
        
        uint ground_gem_score = m_SystemManager.GemsGround * ItemScore.GEM_GROUND;
        uint air_gem_score = m_SystemManager.GemsAir * ItemScore.GEM_AIR;
        byte stage_miss = m_SystemManager.GetStageMiss();
        uint stage_score = m_SystemManager.GetStageScore();
        string bonus_scale;

        if (stage_miss == 0) {
            m_BonusScale = BonusScale.BONUS_0;
            bonus_scale = "[ X 1.5 ]";
        }
        else if (stage_miss == 1) {
            m_BonusScale = BonusScale.BONUS_1;
            bonus_scale = "[ X 1.3 ]";
        }
        else if (stage_miss == 2) {
            m_BonusScale = BonusScale.BONUS_2;
            bonus_scale = "[ X 1.1 ]";
        }
        else {
            m_BonusScale = 0;
            bonus_scale = "[ X 0.0 ]";
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
        m_DisplayTimer = 0f;
        m_DisplayStage = 0;
        m_BonusScale = 0;
        m_FinalBonusScore = 0;
        m_OverviewBackground.color = new Color(1f, 1f, 1f, 0f);
        Init();
    }
}
