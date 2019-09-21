using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class PlayerLaserCreater : MonoBehaviour
{
    public GameObject m_FireEffect;
    public GameObject m_StormEffect;
    public GameObject m_RushEffect;
    public GameObject m_HitEffect;

    public ParticleSystem[] m_FireParticles;
    public ParticleSystem[] m_StormParticles;
    public ParticleSystem[] m_RushParticles;
    public ParticleSystem[] m_HitParticles;

    [SerializeField] private BoxCollider2D m_Collider2D = null;
    [SerializeField] private PlayerLaserShooterManager m_LaserShooter = null;
    [SerializeField] private PlayerShooterManager m_PlayerShooter = null;

    [HideInInspector] public float m_MaxLength;
    
    private LineRenderer m_LineRenderer;
    private Vector3 m_LaserHitBoxWidth;
    private float m_HitOffset;
    private float m_EndPointAlpha;
    private bool LaserSaver = false;
    private int m_LayerMask;
    
    private ParticleSystem.MainModule m_ParticleMainModule;
    private float m_StormSpeed;

    void Awake ()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_HitOffset = m_LaserShooter.m_HitOffset;
        m_EndPointAlpha = m_LaserShooter.m_EndPointAlpha;

        if (m_StormParticles.Length > 0) {
            m_ParticleMainModule = m_StormParticles[0].main;
            m_StormSpeed = m_ParticleMainModule.startSpeed.constant;
        }

        m_LayerMask = Layer.LARGE;
    }

    void OnEnable() {
        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, transform.position);

        float fire_local_scale = 1f + (float) m_PlayerShooter.m_ShotLevel*0.25f;
        m_FireEffect.transform.localScale = new Vector3(fire_local_scale, fire_local_scale, 1.5f);

        float rush_local_scale = 1f + (float) m_PlayerShooter.m_ShotLevel*0.25f;
        m_RushEffect.transform.localScale = new Vector3(rush_local_scale, rush_local_scale, rush_local_scale);

        float hit_local_scale = 1f + (float) m_PlayerShooter.m_ShotLevel*0.25f;
        m_HitEffect.transform.localScale = new Vector3(hit_local_scale, hit_local_scale, hit_local_scale);

        float laser_width = 1f + (float) m_PlayerShooter.m_ShotLevel*0.5f;
        m_LineRenderer.startWidth = laser_width;
        m_LineRenderer.endWidth = laser_width;
        float hitbox_width = laser_width*0.8f;
        m_LaserHitBoxWidth = new Vector3(hitbox_width*0.5f, 0f, 0f);
        m_Collider2D.size = new Vector2(hitbox_width, 0f);
        
        StopParticles(m_StormParticles);
        StopParticles(m_HitParticles);
    }

    void FixedUpdate()
    {
        if (m_LineRenderer != null) {
            m_LineRenderer.SetPosition(0, transform.position);
            //Debug.DrawRay(transform.position + width, transform.forward, Color.red, 0.1f);

            RaycastHit2D hit1 = Physics2D.Raycast(transform.position - m_LaserHitBoxWidth, transform.up, m_MaxLength, m_LayerMask);
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, transform.up, m_MaxLength, m_LayerMask);
            RaycastHit2D hit3 = Physics2D.Raycast(transform.position + m_LaserHitBoxWidth, transform.up, m_MaxLength, m_LayerMask);

            if ((hit1.collider != null) || (hit2.collider != null) || (hit3.collider != null)) { // 하나라도 충돌하면

                float min_y = Mathf.Min(hit1.point.y, hit2.point.y, hit3.point.y);
                Vector3 end_point = new Vector3(transform.position.x, min_y, Depth.PLAYER); // 가장 작은 y좌표를 endpoint로
                m_LaserShooter.m_MaxLength = Mathf.Max(min_y - m_LaserShooter.transform.position.y + m_EndPointAlpha, 0.1f);
                
                m_LineRenderer.SetPosition(1, end_point);
                m_HitEffect.transform.position = end_point + Vector3.up * m_HitOffset;
                m_RushEffect.transform.position = end_point + Vector3.up * m_HitOffset;

                PlayParticles(m_FireParticles);
                PlayParticles(m_StormParticles);
                PlayParticles(m_HitParticles);
                PlayParticles(m_RushParticles);
            }
            else { // 아무 충돌이 없으면
                var end_point = transform.position + transform.up * m_MaxLength;
                m_LineRenderer.SetPosition(1, end_point);
                m_HitEffect.transform.position = end_point;
                m_RushEffect.transform.position = end_point;

                PlayParticles(m_FireParticles);
                PlayParticles(m_StormParticles);
                if (m_RushEffect.transform.position.y < 0f) {
                    PlayParticles(m_RushParticles);
                }
                else {
                    StopParticles(m_RushParticles);
                }
                StopParticles(m_HitParticles);
            }

            if (m_StormParticles.Length > 0)
                m_ParticleMainModule.startLifetime = - m_LaserShooter.m_MaxLength / m_StormSpeed * 0.6f;

            m_Collider2D.offset = new Vector2(m_Collider2D.offset.x, m_LaserShooter.m_MaxLength/2);
            m_Collider2D.size = new Vector2(m_Collider2D.size.x, m_LaserShooter.m_MaxLength);

            //Insurance against the appearance of a laser in the center of coordinates!
            if (!m_LineRenderer.enabled && !LaserSaver)
            {
                LaserSaver = true;
                m_LineRenderer.enabled = true;
            }
        }  
    }

    public void DisablePrepare() // Initiate Laser
    {
        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, transform.position);
        
        //m_FireParticles can = null in multiply shooting
        StopParticles(m_FireParticles);
        StopParticles(m_StormParticles);
        StopParticles(m_RushParticles);
        StopParticles(m_HitParticles);
    }

    private void PlayParticles(ParticleSystem[] particle_system) {
        foreach (var AllPs in particle_system) {
            if (!AllPs.isPlaying) AllPs.Play();
        }
    }

    private void StopParticles(ParticleSystem[] particle_system) {
        if (particle_system != null) {
            foreach (var part in particle_system) {
                if (part.isPlaying) part.Stop();
            }
        }
    }
}
