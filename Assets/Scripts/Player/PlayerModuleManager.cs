using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModule {
	void ModuleShoot(); 
}

public class PlayerModuleAddShot : IModule {

	public void ModuleShoot() {
		// TODO : Addshot 구현
	} 
}

public class PlayerModule {
    private IModule module;

    public void SetModule(IModule module) {
        this.module = module;
    }

    public void ModuleShoot() {
        module.ModuleShoot();
    }
}

public class PlayerModuleManager : MonoBehaviour
{
    PlayerModule m_PlayerModule;

    void Start() {
        m_PlayerModule = new PlayerModule();
        m_PlayerModule.SetModule(null);
    }

	public void ChangeModuleAddShot() {
		m_PlayerModule.SetModule(new PlayerModuleAddShot());
	}

	public void CreateModuleAttack() {
		m_PlayerModule.ModuleShoot();
	}
}