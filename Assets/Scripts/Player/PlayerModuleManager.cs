using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModule {
	public string ObjectName { get; }
	public void Shoot(PlayerShootHandler playerShootHandler, PlayerDamageDatas playerDamage, int damageLevel);
}

public class PlayerModuleNone : IModule
{
	public string ObjectName => string.Empty;
	public void Shoot(PlayerShootHandler playerShootHandler, PlayerDamageDatas playerDamage, int damageLevel)
	{
		return;
	} 
}

public class PlayerModuleHomingMissile : IModule
{
	public string ObjectName => "PlayerHomingMissile";
	public void Shoot(PlayerShootHandler playerShootHandler, PlayerDamageDatas playerDamage, int damageLevel) {
		Vector3[] shotPosition = new Vector3[2];
		shotPosition[0] = playerShootHandler.m_PlayerShotPosition[5].position;
		shotPosition[0].z = Depth.PLAYER_MISSILE;
		shotPosition[1] = playerShootHandler.m_PlayerShotPosition[6].position;
		shotPosition[1].z = Depth.PLAYER_MISSILE;
		playerShootHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[0], 180f - 15f, damageLevel);
		playerShootHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[1], 180f + 15f, damageLevel);
	} 
}

public class PlayerModuleRocket : IModule
{
	public string ObjectName => "PlayerRocket";
	public void Shoot(PlayerShootHandler playerShootHandler, PlayerDamageDatas playerDamage, int damageLevel) {
		Vector3[] shotPosition = new Vector3[2];
		shotPosition[0] = playerShootHandler.m_PlayerShotPosition[5].position;
		shotPosition[0].z = Depth.PLAYER_MISSILE;
		shotPosition[1] = playerShootHandler.m_PlayerShotPosition[6].position;
		shotPosition[1].z = Depth.PLAYER_MISSILE;
		playerShootHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[0], 180f, damageLevel);
		playerShootHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[1], 180f, damageLevel);
	} 
}

public class PlayerModuleAddShot : IModule
{
	public string ObjectName => "PlayerAddShot";
	public void Shoot(PlayerShootHandler playerShootHandler, PlayerDamageDatas playerDamage, int damageLevel) {
		Vector3[] shotPosition = new Vector3[2];
		var rot = playerShootHandler.m_PlayerBody.eulerAngles.y;
		shotPosition[0] = playerShootHandler.m_PlayerShotPosition[5].position;
		shotPosition[0].z = Depth.PLAYER_MISSILE;
		shotPosition[1] = playerShootHandler.m_PlayerShotPosition[6].position;
		shotPosition[1].z = Depth.PLAYER_MISSILE;
		playerShootHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[0], 180f + rot, damageLevel);
		playerShootHandler.CreatePlayerAttack(ObjectName, playerDamage, shotPosition[1], 180f + rot, damageLevel);
	} 
}

public class PlayerModule {
    private IModule module;

    public void SetModule(IModule module) {
        this.module = module;
    }

    public void Shoot(PlayerShootHandler playerShootHandler, PlayerDamageDatas playerDamage, int damageLevel) {
        module.Shoot(playerShootHandler, playerDamage, damageLevel);
    }
}

public class PlayerModuleManager
{
    PlayerModule m_PlayerModule;

    PlayerModuleManager() {
        m_PlayerModule = new PlayerModule();
        m_PlayerModule.SetModule(null);
    }

	public void ChangeModule(IModule module) {
		m_PlayerModule.SetModule(module);
	}

	public void CreateModuleAttack() {
		//m_PlayerModule.Shoot();
	}
}