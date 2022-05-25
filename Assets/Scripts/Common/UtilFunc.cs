using UnityEngine;

namespace Common
{
    public static class UtilFunc
    {
        public static readonly Vector2[] Dir = {Vector2.left, Vector2.right, Vector2.up, Vector2.down,};
        public static float Dist2d(Vector3 v1, Vector3 v2) => ((Vector2) v1 - (Vector2) v2).magnitude;
        public static bool SamePos(Vector3 v1, Vector3 v2) => ((Vector2) v1 - (Vector2) v2).sqrMagnitude < 1e-3;

        public static void SetAlpha(SpriteRenderer renderer, float alpha)
        {
            var color = renderer.color;
            color = new Color(color.r, color.g, color.b, alpha);
            renderer.color = color;
        }
    }
}