using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TriggerBody : MonoBehaviour
{
    public enum BodyType
    {
        Circle,
        Polygon,
    }

    public TriggerBodyType m_TriggerBodyType;
    public BodyType m_BodyType;

    [DrawIf("m_BodyType", BodyType.Circle, ComparisonType.Equals)]
    public BodyCircle m_BodyCircle = new();
    
    [DrawIf("m_BodyType", BodyType.Polygon, ComparisonType.Equals)]
    public BodyPolygon m_BodyPolygon = new();
    
    public UnityAction<TriggerBody> m_OnTriggerBodyEnter;
    public UnityAction<TriggerBody> m_OnTriggerBodyExit;
    public UnityAction<TriggerBody> m_OnTriggerBodyStay;

    public BodyCircle TransformedBodyCircle => GetTransformedBody(m_BodyCircle);
    public BodyPolygon TransformedBodyPolygon => GetTransformedBody(m_BodyPolygon);

    private HashSet<TriggerBody> _triggerBodySet = new();

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        switch (m_BodyType)
        {
            case BodyType.Circle:
                DrawCircleGizmos();
                break;
            case BodyType.Polygon:
                DrawPolygonGizmos();
                break;
        }
    }

    private void DrawCircleGizmos()
    {
        Handles.color = Color.green;
        var gizmoCircle = TransformedBodyCircle;
        Handles.DrawWireDisc(gizmoCircle.m_BodyCenter, Vector3.forward, gizmoCircle.m_BodyRadius);
    }

    private void DrawPolygonGizmos()
    {
        var isConcaveList = new List<bool>();
        var transformedBodyPolygon = TransformedBodyPolygon;
        
        foreach (var bodyPolygonUnit in transformedBodyPolygon.m_BodyPolygonUnits)
        {
            var count = bodyPolygonUnit.m_BodyPoints.Count;

            if (count <= 3)
            {
                isConcaveList.Add(false);
                continue;
            }

            var currentConcave = false;
            var prevCross = 0f;
            
            for (var i = 0; i < count; ++i)
            {
                var prevPoint = bodyPolygonUnit.m_BodyPoints[((i - 1) % count + count) % count];
                var currentPoint = bodyPolygonUnit.m_BodyPoints[i];
                var nextPoint = bodyPolygonUnit.m_BodyPoints[(i + 1) % count];
                var prevEdge = currentPoint - prevPoint;
                var nextEdge = nextPoint - currentPoint;
            
                var currentCross = Vector3.Cross(prevEdge, nextEdge);
                
                if (prevCross != 0f && currentCross.z * prevCross < 0)
                {
                    currentConcave = true;
                    break;
                }

                prevCross = currentCross.z;
            }
            isConcaveList.Add(currentConcave);
        }

        var index = 0;
        foreach (var bodyPolygonUnit in transformedBodyPolygon.m_BodyPolygonUnits)
        {
            Gizmos.color = isConcaveList[index] ? Color.red : Color.green;
            index++;
            
            var count = bodyPolygonUnit.m_BodyPoints.Count;
            for (var i = 0; i < count; ++i)
            {
                var currentPoint = bodyPolygonUnit.m_BodyPoints[i];
                var nextPoint = bodyPolygonUnit.m_BodyPoints[(i + 1) % count];
                Gizmos.DrawLine(currentPoint, nextPoint);
            }
        }
    }
#endif

    private BodyCircle GetTransformedBody(BodyCircle bodyCircle)
    {
        var newBodyCircle = new BodyCircle();
        var point = TransformPoint(bodyCircle.m_BodyCenter);
        newBodyCircle.m_BodyCenter = point;
        
        var radius = m_BodyCircle.m_BodyRadius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        newBodyCircle.m_BodyRadius = radius;
        return newBodyCircle;
    }

    private BodyPolygon GetTransformedBody(BodyPolygon bodyPolygon)
    {
        var newBodyPolygon = new BodyPolygon(new List<BodyPolygonUnit>(bodyPolygon.m_BodyPolygonUnits.Count));
        foreach (var bodyPolygonUnit in bodyPolygon.m_BodyPolygonUnits)
        {
            var newBodyPolygonUnit = new BodyPolygonUnit(new List<Vector2>(bodyPolygonUnit.m_BodyPoints.Count));
            
            foreach (var point in bodyPolygonUnit.m_BodyPoints)
            {
                var transformedPoint = TransformPoint(point);
                newBodyPolygonUnit.m_BodyPoints.Add(transformedPoint);
            }

            newBodyPolygon.m_BodyPolygonUnits.Add(newBodyPolygonUnit);
        }
        
        return newBodyPolygon;
    }

    private Vector2 TransformPoint(Vector2 point)
    {
        point *= transform.lossyScale;
        var rotationMatrix = Matrix4x4.Rotate(transform.rotation); 
        point = rotationMatrix.MultiplyPoint(point);
        point += (Vector2)transform.position;
        return point;
    }

    public void OnTriggerBodyCollision(TriggerBody other, bool result)
    {
        var isContain = _triggerBodySet.Contains(other);
        if (result)
        {
            if (isContain)
                return;
            _triggerBodySet.Add(other);
            m_OnTriggerBodyEnter?.Invoke(other);
            //Debug.Log($"{this} OnTriggerBodyCollisionEnter: {other}");
        }
        else
        {
            if (!isContain)
                return;
            _triggerBodySet.Remove(other);
            m_OnTriggerBodyExit?.Invoke(other);
            //Debug.Log($"{this} OnTriggerBodyCollisionExit: {other}");
        }
    }

    public void OnTriggerBodyCollisionStay()
    {
        foreach (var triggerBody in _triggerBodySet)
        {
            m_OnTriggerBodyStay?.Invoke(triggerBody);
            //Debug.Log($"{this} OnTriggerBodyCollisionStay: {triggerBody}");
        }
    }
}

[Serializable]
public class BodyCircle
{
    public Vector2 m_BodyCenter;
    public float m_BodyRadius;
    
    public BodyCircle() {}

    public BodyCircle(Vector2 bodyCenter, float bodyRadius)
    {
        m_BodyCenter = bodyCenter;
        m_BodyRadius = bodyRadius;
    }

    public BodyCircle(BodyCircle bodyCircle)
    {
        m_BodyCenter = bodyCircle.m_BodyCenter;
        m_BodyRadius = bodyCircle.m_BodyRadius;
    }
}

[Serializable]
public class BodyPolygon
{
    public List<BodyPolygonUnit> m_BodyPolygonUnits = new();
    
    public BodyPolygon() {}

    public BodyPolygon(List<BodyPolygonUnit> bodyPolygonUnits)
    {
        m_BodyPolygonUnits = bodyPolygonUnits;
    }
}

[Serializable]
public class BodyPolygonUnit
{
    public List<Vector2> m_BodyPoints;

    public BodyPolygonUnit(List<Vector2> bodyPoints)
    {
        m_BodyPoints = bodyPoints;
    }
}

