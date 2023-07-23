using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBoss4_Launcher : EnemyUnit
{
    private bool m_Moving = false;
    private int _side;
    private int _moveDirection;
    private float _customDirectionDelta;
    private int _customDirectionSide;

    void Start()
    {
        _side = transform.localPosition.x < 0f ? -1 : 1;
        _moveDirection = _side;
        _customDirectionSide = Random.Range(0, 2) * 2 - 1;
    }

    protected override void Update() {
        base.Update();
        
        // TODO. 최적화 필요
        if (_side == 1) {
            if (transform.localPosition.x > 9f) {
                _moveDirection *= -1;
                transform.localPosition = new Vector3(9f, transform.localPosition.y, transform.localPosition.z);
            }
            else if (transform.localPosition.x < 5f) {
                _moveDirection *= -1;
                transform.localPosition = new Vector3(5f , transform.localPosition.y, transform.localPosition.z);
            }
        }
        else {
            if (transform.localPosition.x < -9f) {
                _moveDirection *= -1;
                transform.localPosition = new Vector3(-9f , transform.localPosition.y, transform.localPosition.z);
            }
            else if (transform.localPosition.x > -5f) {
                _moveDirection *= -1;
                transform.localPosition = new Vector3(-5f , transform.localPosition.y, transform.localPosition.z);
            }
        }

        float dir = _moveDirection*3f / Application.targetFrameRate * Time.timeScale;
        if (m_Moving) {
            transform.localPosition = new Vector3(transform.localPosition.x + dir , transform.localPosition.y, transform.localPosition.z);
        }
        else if (transform.localPosition.x != 5f*_side) {
            transform.localPosition = new Vector3(transform.localPosition.x + dir , transform.localPosition.y, transform.localPosition.z);
        }
        
        CustomDirection += _customDirectionDelta * _customDirectionSide / Application.targetFrameRate * Time.timeScale;
    }

    public void SetCustomDirection(float directionDelta, int side)
    {
        _customDirectionDelta = directionDelta;
        _customDirectionSide = side;
    }

    public void SetMoving(bool boolean) {
        m_Moving = boolean;
    }
}