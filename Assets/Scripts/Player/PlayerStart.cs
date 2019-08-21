using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public PlayerController m_PlayerController;
    public GameObject m_DronePart; // Shot Spawner
    public GameObject[] m_SpeedPart = new GameObject[3];
    public GameObject m_ModulePart;
    public GameObject m_PlayerShield;

    private float m_Vspeed = 0f;
    private PlayerManager m_PlayerManager = null;

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        StartCoroutine(SpawnEvent());
        m_PlayerController.DisableInvincible();
        SetAttributes();
    }

    void Update() {
        Move();
    }

    private IEnumerator SpawnEvent() {
        yield return new WaitForSeconds(2.5f);
        m_Vspeed = 8.8f;
        while(m_Vspeed > -4f) {
            m_Vspeed -= 0.3f;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        EndOpening();
        yield break;
    }

    private void EndOpening() {
        m_PlayerController.EnableInvincible(m_PlayerController.m_ReviveInvincibleTime);
        m_PlayerManager.PlayerControlable = true;
        m_Vspeed = 0f;
        GetComponent<PlayerStart>().enabled = false;
    }

    private void Move() {
        transform.Translate(Vector3.up * m_Vspeed * Time.deltaTime, Space.World);
    }

    void OnDestroy() {
        StopAllCoroutines();
    }


    private void SetAttributes() {
        m_DronePart.SetActive(false);
        
        m_SpeedPart[m_PlayerManager.m_CurrentAttributes[1]].SetActive(true);

        if (m_PlayerManager.m_CurrentAttributes[5] != 0)
            m_ModulePart.SetActive(true);
        
        SetPlayerColors();
        
        m_DronePart.SetActive(true);
        m_PlayerShield.SetActive(true);
        //transform.GetChild(0).GetChild(11).gameObject.SetActive(true);
    }

    private void SetPlayerColors() {
        MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();
        PlayerColors playerColors = GetComponent<PlayerColors>();
        int max_meshRenderer = meshRenderer.Length;
        
        // Color
        for (int i = 0; i < 3; i++) {
            if (m_PlayerManager.m_CurrentAttributes[0] == i)
                for (int j = 0; j < max_meshRenderer; j++) {
                meshRenderer[j].material = playerColors.m_Materials[i];
            }
        }
    }
}
