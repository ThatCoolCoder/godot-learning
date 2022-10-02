using Godot;
using System;
using System.Collections.Generic;

namespace Physics
{
    public static class Geometry
    {
        // Class containing methods for geometrical math

        public static (float, float) LineIntersectRotatedRectangleAreas(Vector2 p1, Vector2 p2, Rect2 rect, float rectRotation)
        {
            var center = rect.Position + rect.Size / 2;
            var localP1 = (p1 - center).Rotated(rectRotation) + center;
            var localP2 = (p2 - center).Rotated(rectRotation) + center;

            return LineIntersectRectangleAreas(localP1, localP2, rect);
        }

        public static (float, float) LineIntersectRectangleAreas(Vector2 p1, Vector2 p2, Rect2 rect)
        {
            (float, float) AdjacentSidesResult(Vector2 corner, Vector2 intersect1, Vector2 intersect2)
            {
                var area1 = AreaOfTriangle(corner, intersect1, intersect2);
                return (area1, rect.Area - area1);
            }

            var (top, bottom, left, right) = LineIntersectRectanglePosition(p1, p2, rect);
            var (rectTopLeft, rectTopRight, rectBottomLeft, rectBottomRight) = FindRectCorners(rect);

            // 2 trapezoids with cut going vertically
            if (top != null && bottom != null)
            {
                float area1 = AreaOfTrapezoid(top.GetValueOrDefault().x - rectTopLeft.x,
                    top.GetValueOrDefault().x - rectBottomLeft.x, rect.Size.y);

                return (area1, rect.Area - area1);
            }
            // 2 trapezoids with cut going horizontally
            else if (left != null && right != null)
            {
                float area1 = AreaOfTrapezoid(left.GetValueOrDefault().y - rectTopLeft.y,
                    right.GetValueOrDefault().y - rectTopRight.y, rect.Size.y);

                return (area1, rect.Area - area1);
            }
            else if (top != null && left != null) return AdjacentSidesResult(rectTopLeft, top.GetValueOrDefault(), left.GetValueOrDefault());
            else if (top != null && right != null) return AdjacentSidesResult(rectTopRight, top.GetValueOrDefault(), right.GetValueOrDefault());
            else if (bottom != null && left != null) return AdjacentSidesResult(rectBottomLeft, bottom.GetValueOrDefault(), left.GetValueOrDefault());
            else return AdjacentSidesResult(rectBottomRight, bottom.GetValueOrDefault(), right.GetValueOrDefault());
        }

        public static (Vector2?, Vector2?, Vector2?, Vector2?) LineIntersectRectanglePosition(Vector2 p1, Vector2 p2, Rect2 rect)
        {
            // Based on code at https://stackoverflow.com/a/67460723/12650706 written by user Maari.
            // Find intersection points of a line and a rectangle.
            // Is assumed that line and rectangle are colliding with two points.
            // Returns points as (top, bottom, left, right)

            var (m, b) = LineEquationFromPoints(p1, p2);

            var (rectTopLeft, rectTopRight, rectBottomLeft, rectBottomRight) = FindRectCorners(rect);

            var horiziontalPoint = Vector2.Zero;
            var verticalPoint = Vector2.Zero;

            Vector2? topIntersection = null;
            Vector2? bottomIntersection = null;
            Vector2? leftIntersection = null;
            Vector2? rightIntersection = null;

            // rect top
            if (LineIntersectsLine(rectTopLeft, rectTopLeft, p1, p2))
            {
                topIntersection = new Vector2(LineIntersectsYValue(m, b, rectTopLeft.y), rectTopLeft.y);
            }
            // rect bottom
            if (LineIntersectsLine(rectBottomLeft, rectBottomLeft, p1, p2))
            {
                topIntersection = new Vector2(LineIntersectsYValue(m, b, rectBottomLeft.y), rectBottomLeft.y);
            }

            // Return early to prevent edge cases on corners
            if (topIntersection != null && bottomIntersection != null)
                return (topIntersection, bottomIntersection, null, null);


            // rect left
            if (LineIntersectsLine(rectTopLeft, rectBottomLeft, p1, p2))
            {
                leftIntersection = new Vector2(rectTopLeft.x, LineIntersectsXValue(m, b, rectTopLeft.x));
            }

            // Return early to prevent edge cases on corners
            if (topIntersection != null && leftIntersection != null)
                return (topIntersection, null, leftIntersection, null);
            if (bottomIntersection != null && leftIntersection != null)
                return (null, bottomIntersection, leftIntersection, null);

            // rect right
            if (LineIntersectsLine(rectTopRight, rectBottomRight, p1, p2))
            {
                rightIntersection = new Vector2(rectTopRight.x, LineIntersectsXValue(m, b, rectTopRight.x));
            }

            return (topIntersection, bottomIntersection, leftIntersection, rightIntersection);
        }


        public static bool LineIntersectsLine(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2)
        {
            // Extracted from code at https://stackoverflow.com/a/67460723/12650706 written by user Maari

            float q = (l1p1.y - l2p1.y) * (l2p2.x - l2p1.x) - (l1p1.x - l2p1.x) * (l2p2.y - l2p1.y);
            float d = (l1p2.x - l1p1.x) * (l2p2.y - l2p1.y) - (l1p2.y - l1p1.y) * (l2p2.x - l2p1.x);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.y - l2p1.y) * (l1p2.x - l1p1.x) - (l1p1.x - l2p1.x) * (l1p2.y - l1p1.y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

        public static float LineIntersectsXValue(float m, float b, float x)
        {
            // y-height where the line intercepts an x-value
            // (so it's like y-intercept but offset)

            return m * x + b;

        }

        public static float LineIntersectsYValue(float m, float b, float y)
        {
            // x-height where the lin intercepts an y-value
            // (so it's like x-intercept but offset)
            
            return (y - b) / m;
        }

        public static float YInterceptOfLine(float m, float b)
        {
            return b;
        }

        public static float XInterceptFromPoints(float m, float b)
        {   
            return b / m;
        }

        public static float LineGradientFromPoints(Vector2 p1, Vector2 p2)
        {
            return (p1.y - p2.y) / (p1.x - p2.x);
        }

        public static (float, float) LineEquationFromPoints(Vector2 p1, Vector2 p2)
        {
            // Find what values of mx + b fit the points.
            // returns (m, b)

            var m = LineGradientFromPoints(p1, p2);
            var b = p1.y - (p1.x * m);
            return (m, b);
        }

        public static float AreaOfTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            // Get area of triangle from three points, using Heron's formula:
            // Optim: Using base-height formula would probably be faster but it's harder to implement

            // Calculate side lengths
            var a = p1.DistanceTo(p2);
            var b = p3.DistanceTo(p3);
            var c = p1.DistanceTo(p1);
            
            // S = semiperimiter
            var s = 0.5f * + (a * b * c);
            
            var needToSquareRoot = s * (s - a) * (s - b) * (s - c);
            return Mathf.Sqrt(needToSquareRoot);
        }

        public static float AreaOfTrapezoid(float topLength, float bottomLength, float height)
        {
            return (topLength + bottomLength) * height / 2;
        }

        public static (Vector2, Vector2, Vector2, Vector2) FindRectCorners(Rect2 rect)
        {
            return (rect.Position,
                new Vector2(rect.End.x, rect.Position.y),
                new Vector2(rect.Position.x, rect.End.y),
                rect.End);
        }
    }
}