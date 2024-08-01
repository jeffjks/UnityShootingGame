using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriggerBodyManager
{
    public static bool CheckOverlapTriggerBody(TriggerBody triggerBodyA, TriggerBody triggerBodyB)
    {
        var result = CheckOverlap(triggerBodyA, triggerBodyB);

        triggerBodyA.OnTriggerBodyCollision(triggerBodyB, result);
        triggerBodyB.OnTriggerBodyCollision(triggerBodyA, result);
        
        return result;
    }

    private static bool CheckOverlap(TriggerBody triggerBodyA, TriggerBody triggerBodyB)
    {
        var bodyTypeA = triggerBodyA.BodyTypeForComparison;
        var bodyTypeB = triggerBodyB.BodyTypeForComparison;
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
        var distanceVector = bodyCircleB.m_BodyCenter - bodyCircleA.m_BodyCenter;
        var squaredDistance = distanceVector.sqrMagnitude;
        var radiusSum = bodyCircleA.m_BodyRadius + bodyCircleB.m_BodyRadius;

        return squaredDistance < radiusSum * radiusSum;
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
            
            var axis = new Vector2(-currentEdge.y, currentEdge.x);
            axis.Normalize();

            // Axis에 대해 사영한 그림자의 범위 (min ~ max)
            ProjectBodyUnit(axis, bodyPolygonUnitA.m_BodyPoints, out var minA, out var maxA);
            ProjectBodyUnit(axis, bodyPolygonUnitB.m_BodyPoints, out var minB, out var maxB);
            
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
            var currentEdge = nextPoint - currentPoint;
            
            var axis = new Vector2(-currentEdge.y, currentEdge.x);
            axis.Normalize();

            ProjectBodyUnit(axis, bodyPolygonUnit.m_BodyPoints, out var minA, out var maxA);
            var projectedCircleCenter = Vector2.Dot(axis, bodyCircle.m_BodyCenter);
            var minB = projectedCircleCenter - bodyCircle.m_BodyRadius;
            var maxB = projectedCircleCenter + bodyCircle.m_BodyRadius;
            
            if (IsIntersectDistance(minA, maxA, minB, maxB) == false)
                return false;
        }
    
        return true;
    }
    
    private static float GetDistanceFromPointToLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        var heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();
        
        var lhs = point - origin;
        var dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        
        var closestPoint = origin + heading * dotP;
        return Vector2.Distance(closestPoint, point);
    }
        
    private static void ProjectBodyUnit(Vector2 axis, List<Vector2> points, out float minDot, out float maxDot)
    {
        var dotProduct = Vector2.Dot(axis, points[0]);
        minDot = dotProduct;
        maxDot = dotProduct;

        if (points.Count == 0)
            return;
        
        foreach (var bodyPoint in points)
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