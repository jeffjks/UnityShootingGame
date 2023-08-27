using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubWeapon {
	public string ObjectName { get; }
	public void Shoot(PlayerShotHandler playerShotHandler, PlayerDamageDatas playerDamage, int damageLevel);
}

public class PlayerSubWeaponNone : ISubWeapon
{
	public string ObjectName => string.Empty;
	public void Shoot(PlayerShotHandler playerShotHandler, PlayerDamageDatas playerDamage, int damageLevel)
	{
		return;
	} 
}

public class PlayerSubWeaponHomingMissile : ISubWeapon
{
	public string ObjectName => "PlayerHomingMissile";
	public void Shoot(PlayerShotHandler playerShotHandler, PlayerDamageDatas playerDamage, int damageLevel) {
		Vector3[] shotPosition = new Vector3[2];
		shotPosition[0] = playerShotHandler.m_PlayerShotPosition[5].position;
		shotPosition[0].z = Depth.PLAYER_MISSILE;
		shotPosition[1] = playerShotHandler.m_PlayerShotPosition[6].position;
		shotPosition[1].z = Depth.PLAYER_MISSILE;
		playerShotHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[0], 180f - 15f, damageLevel);
		playerShotHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[1], 180f + 15f, damageLevel);
	} 
}

public class PlayerSubWeaponRocket : ISubWeapon
{
	public string ObjectName => "PlayerRocket";
	public void Shoot(PlayerShotHandler playerShotHandler, PlayerDamageDatas playerDamage, int damageLevel) {
		Vector3[] shotPosition = new Vector3[2];
		shotPosition[0] = playerShotHandler.m_PlayerShotPosition[5].position;
		shotPosition[0].z = Depth.PLAYER_MISSILE;
		shotPosition[1] = playerShotHandler.m_PlayerShotPosition[6].position;
		shotPosition[1].z = Depth.PLAYER_MISSILE;
		playerShotHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[0], 180f, damageLevel);
		playerShotHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[1], 180f, damageLevel);
	} 
}

public class PlayerSubWeaponAddShot : ISubWeapon
{
	public string ObjectName => "PlayerAddShot";
	public void Shoot(PlayerShotHandler playerShotHandler, PlayerDamageDatas playerDamage, int damageLevel) {
		Vector3[] shotPosition = new Vector3[2];
		var rot = playerShotHandler.m_PlayerBody.eulerAngles.y;
		shotPosition[0] = playerShotHandler.m_PlayerShotPosition[5].position + new Vector3(0f, 1f, 0f);
		shotPosition[0].z = Depth.PLAYER_MISSILE;
		shotPosition[1] = playerShotHandler.m_PlayerShotPosition[6].position + new Vector3(0f, 1f, 0f);;
		shotPosition[1].z = Depth.PLAYER_MISSILE;
		playerShotHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[0], 180f + rot, damageLevel);
		playerShotHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[1], 180f + rot, damageLevel);
	} 
}

public class PlayerSubWeapon {
    private ISubWeapon _subWeapon;

    public void SetSubWeapon(ISubWeapon subWeapon) {
        this._subWeapon = subWeapon;
    }

    public void Shoot(PlayerShotHandler playerShotHandler, PlayerDamageDatas playerDamage, int damageLevel) {
        _subWeapon.Shoot(playerShotHandler, playerDamage, damageLevel);
    }
}

public class PlayerSubWeaponManager
{
    private readonly PlayerSubWeapon _playerSubWeapon;

    public PlayerSubWeaponManager() {
        _playerSubWeapon = new PlayerSubWeapon();
        _playerSubWeapon.SetSubWeapon(null);
    }

	public void ChangeSubWeapon(ISubWeapon subWeapon) {
		_playerSubWeapon.SetSubWeapon(subWeapon);
	}

	public void CreateSubWeaponAttack() {
		//m_PlayerSubWeapon.Shoot();
	}
}