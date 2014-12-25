using UnityEngine;

public class MathUtils
{
    public const float epsilon = 0.1f;

    static public bool AreFloatsEqual(float floatA, float floatB)
    {
        return Mathf.Abs(floatA - floatB) < epsilon;
    }

    static public bool ArePointsEqual(Vector2 pointA, Vector2 pointB)
    {
        return (pointB - pointA).sqrMagnitude < epsilon;
    }

    static public bool ArePointsEqual(Vector3 pointA, Vector3 pointB)
    {
        return (pointB - pointA).sqrMagnitude < epsilon;
    }

    static public void TwoSegmentsIntersection(Vector2 segment1Point1, Vector2 segment1Point2, Vector2 segment2Point1, Vector2 segment2Point2, out bool intersects, out Vector2 intersection)
    {
        //order points by ascending x
        if (segment1Point1.x > segment1Point2.x)
        {
            Vector2 tmpPoint = segment1Point2;
            segment1Point2 = segment1Point1;
            segment1Point1 = tmpPoint;
        }
        if (segment2Point1.x > segment2Point2.x)
        {
            Vector2 tmpPoint = segment2Point2;
            segment2Point2 = segment2Point1;
            segment2Point1 = tmpPoint;
        }

        //Both lines equation
        float x, y;
        if (!AreFloatsEqual(segment1Point1.x, segment1Point2.x) && !AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //y = a1x + b1 && y = a2x + b2
        {
            float a1 = (segment1Point2.y - segment1Point1.y) / (segment1Point2.x - segment1Point1.x);
            float b1 = segment1Point1.y - a1 * segment1Point1.x;
            float a2 = (segment2Point2.y - segment2Point1.y) / (segment2Point2.x - segment2Point1.x);
            float b2 = segment2Point1.y - a2 * segment2Point1.x;

            if (a1 == a2) //parallel lines
            {
                intersects = false;
                intersection = Vector2.zero;
                return;
            }
            else
            {
                x = (b2 - b1) / (a1 - a2);
                y = a1 * x + b1;
            }
        }
        else if (!AreFloatsEqual(segment1Point1.x, segment1Point2.x) && AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //y = a1x + b1 && x = a2
        {
            float a1 = (segment1Point2.y - segment1Point1.y) / (segment1Point2.x - segment1Point1.x);
            float b1 = segment1Point1.y - a1 * segment1Point1.x;
            float a2 = segment2Point1.x;

            x = a2;
            y = a1 * a2 + b1;
        }
        else if (AreFloatsEqual(segment1Point1.x, segment1Point2.x) && !AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //x = a1 && y = a2x + b2
        {
            float a1 = segment1Point1.x;
            float a2 = (segment2Point2.y - segment2Point1.y) / (segment2Point2.x - segment2Point1.x);
            float b2 = segment2Point1.y - a2 * segment2Point1.x;

            x = a1;
            y = a2 * a1 + b2;
        }
        else //parallel vertical lines, no intersection or infinite intersections. In both cases return no intersection
        {
            intersects = false;
            intersection = Vector2.zero;
            return;
        }


        //Check if (x, y) point is contained in both segments
        if (x >= segment1Point1.x && x <= segment1Point2.x
            &&
            x >= segment2Point1.x && x <= segment2Point2.x)
        {
            intersects = true;
            intersection = new Vector2(x, y);
        }
        else
        {
            intersects = false;
            intersection = Vector2.zero;
        }
    }
}
