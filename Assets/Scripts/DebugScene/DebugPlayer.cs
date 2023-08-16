using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DebugPlayer : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    [FormerlySerializedAs("m_PlayerShootHandler")] public PlayerShotHandler m_PlayerShotHandler;
    public PlayerLaserHandler m_PlayerLaserHandler;
    public TextMeshProUGUI m_ShotIndexText;
    public TextMeshProUGUI m_LaserIndexText;
    public TextMeshProUGUI m_ModuleIndexText;
    public TextMeshProUGUI m_PowerText;
    
    private int _attackLevel;
    private int _shotIndex;
    private int _laserIndex;
    private int _moduleIndex;

    private int AttackLevel
    {
        get => _attackLevel;
        set
        {
            _attackLevel = value;
            _attackLevel = Mathf.Clamp(_attackLevel, 0, 4);
            m_PowerText.SetText($"Lv. {_attackLevel + 1}");
            m_PlayerUnit.PlayerAttackLevel = _attackLevel;
        }
    }
    
    private int ShotIndex
    {
        get => _shotIndex;
        set
        {
            _shotIndex = value;
            _shotIndex = Mathf.Clamp(_shotIndex, 0, 2);
            m_ShotIndexText.SetText($"ShotIndex: {_shotIndex}");
            m_PlayerShotHandler.ShotIndex = _shotIndex;
        }
    }
    
    private int LaserIndex
    {
        get => _laserIndex;
        set
        {
            _laserIndex = value;
            _laserIndex = Mathf.Clamp(_laserIndex, 0, 2);
            m_LaserIndexText.SetText($"LaserIndex: {_laserIndex}");
            m_PlayerLaserHandler.LaserIndex = _laserIndex;
        }
    }
    
    private int ModuleIndex
    {
        get => _moduleIndex;
        set
        {
            _moduleIndex = value;
            _moduleIndex = Mathf.Clamp(_moduleIndex, 0, 3);
            m_ModuleIndexText.SetText($"ModuleIndex: {_moduleIndex}");
            m_PlayerShotHandler.ModuleIndex = _moduleIndex;
        }
    }

    private void Start()
    {
        AttackLevel = m_PlayerUnit.PlayerAttackLevel;
        ShotIndex = m_PlayerShotHandler.ShotIndex;
        LaserIndex = m_PlayerLaserHandler.LaserIndex;
        ModuleIndex = m_PlayerShotHandler.ModuleIndex;
    }

    public void OnClickPowerDown()
    {
        AttackLevel--;
    }

    public void OnClickPowerUp()
    {
        AttackLevel++;
    }

    public void OnClickShotIndexDown()
    {
        ShotIndex--;
    }

    public void OnClickShotIndexUp()
    {
        ShotIndex++;
    }

    public void OnClickLaserIndexDown()
    {
        LaserIndex--;
    }

    public void OnClickLaserIndexUp()
    {
        LaserIndex++;
    }

    public void OnClickModuleIndexDown()
    {
        ModuleIndex--;
    }

    public void OnClickModuleIndexUp()
    {
        ModuleIndex++;
    }
}
