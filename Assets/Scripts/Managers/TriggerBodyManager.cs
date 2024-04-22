using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriggerBodyManager
{
    public static bool CheckOverlapTriggerBody(TriggerBody triggerBodyA, TriggerBody triggerBodyB)
    {
        var bodyTypeA = triggerBodyA.m_BodyType;
        var bodyTypeB = triggerBodyB.m_BodyType;
        var result = false;
        
        if (bodyTypeA == TriggerBody.BodyType.Polygon && bodyTypeB == TriggerBody.BodyType.Polygon)
            result = CheckOverlapBody(triggerBodyA.TransformedBodyPolygon, triggerBodyB.TransformedBodyPolygon);
        else if (bodyTypeA == TriggerBody.BodyType.Polygon && bodyTypeB == TriggerBody.BodyType.Circle)
            result = CheckOverlapBody(triggerBodyA.TransformedBodyPolygon, triggerBodyB.TransformedBodyCircle);
        else if (bodyTypeA == TriggerBody.BodyType.Circle && bodyTypeB == TriggerBody.BodyType.Polygon)
            result = CheckOverlapBody(triggerBodyB.TransformedBodyPolygon, triggerBodyA.TransformedBodyCircle);
        else if (bodyTypeA == TriggerBody.BodyType.Circle && bodyTypeB == TriggerBody.BodyType.Circle)
            result = CheckOverlapBody(triggerBodyA.TransformedBodyCircle, triggerBodyB.TransformedBodyCircle);
        else
            Debug.LogError($"There is no matching triggerBodyType ({bodyTypeA}, {bodyTypeB})");

        if (result)
        {
            triggerBodyA.m_CollisionCallback?.Invoke(triggerBodyB);
            triggerBodyB.m_CollisionCallback?.Invoke(triggerBodyA);
        }
        
        return result;
    }
    
    private static bool CheckOverlapBody(BodyPolygon bodyPolygonA, BodyPolygon bodyPolygonB)
    {
        foreach (var bodyPolygonUnitA in bodyPolygonA.m_BodyPolygonUnits)
        {
            foreach (var bodyPolygonUnitB in bodyPolygonB.m_BodyPolygonUnits)
            {
                if (CheckOverlapBodyUnit(bodyPolygonUnitA, bodyPolygonUnitB))
                    return true;
            }
        }
        return false;
    }
    
    private static bool CheckOverlapBody(BodyPolygon bodyPolygon, BodyCircle bodyCircle)
    {
        foreach (var bodyPolygonUnit in bodyPolygon.m_BodyPolygonUnits)
        {
            if (CheckOverlapBodyUnit(bodyPolygonUnit, bodyCircle))
                return true;
        }
        return false;
    }

    private static bool CheckOverlapBody(BodyCircle bodyCircleA, BodyCircle bodyCircleB)
    {
        var distance = Vector2.Distance(bodyCircleA.m_BodyCenter, bodyCircleB.m_BodyCenter);
        return distance < bodyCircleA.m_BodyRadius + bodyCircleA.m_BodyRadius;
    }

    private static bool CheckOverlapBodyUnit(BodyPolygonUnit bodyPolygonUnitA, BodyPolygonUnit bodyPolygonUnitB)
    {
        var countA = bodyPolygonUnitA.m_BodyPoints.Count;
        var countB = bodyPolygonUnitB.m_BodyPoints.Count;
        
        for (var i = 0; i < countA + countB; ++i)
        {
            var currentEdge = i < countA
                ? bodyPolygonUnitA.m_BodyPoints[(i + 1) % countA] - bodyPolygonUnitA.m_BodyPoints[i]
                : bodyPolygonUnitB.m_BodyPoints[(i - countA + 1) % countB] - bodyPolygonUnitB.m_BodyPoints[i - countA];
                
            // Find the axis perpendicular to the current edge
            var axis = new Vector2(-currentEdge.y, currentEdge.x);
            axis.Normalize();

            // Find the projection of the polygon on the current axis
            ProjectBodyUnit(axis, bodyPolygonUnitA, out var minA, out var maxA);
            ProjectBodyUnit(axis, bodyPolygonUnitB, out var minB, out var maxB);

            // Check if the polygon projections are currentlty intersecting
            if (IsIntersectDistance(minA, maxA, minB, maxB) == false)
                return false;
        }

        return true;
    }

    private static bool CheckOverlapBodyUnit(BodyPolygonUnit bodyPolygonUnit, BodyCircle bodyCircle)
    {
        var count = bodyPolygonUnit.m_BodyPoints.Count;
        
        for (var i = 0; i < count; ++i)
        {
            var currentPoint = bodyPolygonUnit.m_BodyPoints[i];
            var nextPoint = bodyPolygonUnit.m_BodyPoints[(i + 1) % count];

            var distance = GetDistanceFromPointToLine(currentPoint, nextPoint, bodyCircle.m_BodyCenter);
            
            if (distance < bodyCircle.m_BodyRadius)
                return true;
        }
    
        return false;
    }
    
    private static float GetDistanceFromPointToLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        //Get heading
        Vector2 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        
        var closestPoint = origin + heading * dotP;
        return Vector2.Distance(closestPoint, point);
    }
        
    private static void ProjectBodyUnit(Vector2 axis, BodyPolygonUnit bodyPolygonUnit, out float minDot, out float maxDot)
    {
        var dotProduct = Vector2.Dot(axis, bodyPolygonUnit.m_BodyPoints[0]);
        minDot = dotProduct;
        maxDot = dotProduct;
        
        foreach (var bodyPoint in bodyPolygonUnit.m_BodyPoints)
        {
            dotProduct = Vector2.Dot(axis, bodyPoint);
            if (dotProduct < minDot)
            {
                minDot = dotProduct;
            }
            else if (dotProduct > maxDot)
            {
                maxDot = dotProduct;
            }
        }
    }
    
    private static bool IsIntersectDistance(float minA, float maxA, float minB, float maxB)
    {
        var distance = minA < minB ? minB - maxA : minA - maxB;
        return distance < 0f;
    }
}