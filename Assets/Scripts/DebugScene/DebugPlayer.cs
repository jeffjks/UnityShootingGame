using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DebugPlayer : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    public PlayerShotHandler m_PlayerShotHandler;
    public PlayerLaserHandler m_PlayerLaserHandler;
    public TextMeshProUGUI m_ShotIndexText;
    public TextMeshProUGUI m_LaserIndexText;
    public TextMeshProUGUI m_SubWeaponIndexText;
    public TextMeshProUGUI m_PowerText;
    
    private int _attackLevel;
    private int _shotIndex;
    private int _laserIndex;
    private int _subWeaponIndex;

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
            m_ShotIndexText.SetText($"Shot: {_shotIndex}");
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
            m_LaserIndexText.SetText($"Laser: {_laserIndex}");
            m_PlayerLaserHandler.LaserIndex = _laserIndex;
        }
    }
    
    private int SubWeaponIndex
    {
        get => _subWeaponIndex;
        set
        {
            _subWeaponIndex = value;
            _subWeaponIndex = Mathf.Clamp(_subWeaponIndex, 0, 3);
            m_SubWeaponIndexText.SetText($"SubWeapon: {_subWeaponIndex}");
            m_PlayerShotHandler.SubWeaponIndex = _subWeaponIndex;
        }
    }

    private void Start()
    {
        SystemManager.IsInGame = true;
        
        AttackLevel = m_PlayerUnit.PlayerAttackLevel;
        ShotIndex = m_PlayerShotHandler.ShotIndex;
        LaserIndex = m_PlayerLaserHandler.LaserIndex;
        SubWeaponIndex = m_PlayerShotHandler.SubWeaponIndex;
    }

    public void OnClickScoreText()
    {
        InGameDataManager.Instance.DisplayTextEffect(1000);
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

    public void OnClickSubWeaponIndexDown()
    {
        SubWeaponIndex--;
    }

    public void OnClickSubWeaponIndexUp()
    {
        SubWeaponIndex++;
    }
}
