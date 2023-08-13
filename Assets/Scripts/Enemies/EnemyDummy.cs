using UnityEngine;

public class EnemyDummy : EnemyUnit {

	public GameObject m_FanL, m_FanR, m_FanB;
	public float m_FanRotationSpeed;

    protected override void Update()
    {
        base.Update();
        
        RotateFan();
    }

    private void RotateFan() {
		m_FanL.transform.Rotate(0, m_FanRotationSpeed * Time.deltaTime, 0);
		m_FanR.transform.Rotate(0, m_FanRotationSpeed * Time.deltaTime, 0);
		m_FanB.transform.Rotate(- m_FanRotationSpeed * Time.deltaTime, 0 , 0);
    }
}