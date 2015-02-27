using Microsoft.Xna.Framework;

namespace Hail.Helpers
{
    public static class VectorHelper
    {
        public static Vector3 Move(Vector3 v1, Vector3 v2, float maxDistance)
        {
            float distance = (v2 - v1).Length();
            if (maxDistance > distance)
                return v2;

            float lerp = HandyMath.ScaleValue(maxDistance, distance, 1);
            return Vector3.Lerp(v1, v2, lerp);
        }
    }
}