using System;
using Microsoft.Xna.Framework;

namespace Hail.Helpers
{
    public static class QuaternionHelper
    {
        /// <summary>
        /// Rotates q1 up to maxAngle radians in an attempt to match q2,
        /// and returns the resulting Quaternion.
        /// BUGGY - DO NOT USE.
        /// Has a hemisphere issue that reverses rotation at a certain point.
        /// </summary>
        /// <param name="q1">Starting rotation.</param>
        /// <param name="q2">Goal rotation.</param>
        /// <param name="maxAngle">Max amount in radians to rotate q1.</param>
        /// <returns>Quaternion representing q1 rotated up to maxAngle radians toward q2.</returns>
        public static Quaternion LerpLimited(Quaternion q1, Quaternion q2, float maxAngle)
        {
            if (maxAngle < 0)
                throw new ArgumentException("maxAngle must be >= 0", "maxAngle");
            //float dot = Quaternion.Dot(q1, q2);
            // TODO: determine if removal of double/float casts will cause any problems
            float dot = q1.X*q2.X + q1.Y*q2.Y + q1.Z*q2.Z + q1.W*q2.W;
            var sin = (float) Math.Sqrt(1 - dot*dot);
            var angle = (float) Math.Atan2(sin, dot);
            float fT = Math.Min(1, maxAngle/angle);
            angle = Math.Min(maxAngle, angle);
            float invSin = 1/sin;
            float coeff0 = (float) Math.Sin((1f - fT)*angle)*invSin;
            float coeff1 = (float) Math.Sin(fT*angle)*invSin;
            return Quaternion.Normalize(Quaternion.Multiply(q1, coeff0) + Quaternion.Multiply(q2, coeff1));
        }

        /// <summary>
        /// Gets the angle in radians between q1 and q2.
        /// Equivalent to acos(dot(q1,q2))
        /// </summary>
        /// <param name="q1">Quaternion 1</param>
        /// <param name="q2">Quaternion 2</param>
        /// <returns>Angle in radians between q1 and q2</returns>
        public static float AngleBetween(Quaternion q1, Quaternion q2)
        {
            //float dot = Quaternion.Dot(q1, q2);
            // TODO: determine if removal of double/float casts will cause any problems
            // ^ Hasn't so far.
            float dot = q1.X*q2.X + q1.Y*q2.Y + q1.Z*q2.Z + q1.W*q2.W;
            var sin = (float) Math.Sqrt(1 - dot*dot);
            var atan2 = (float) Math.Atan2(sin, dot);
            return atan2; // >= MathHelper.PiOver2 ? MathHelper.Pi - atan2 : atan2;
        }

        /// <summary>
        /// Rotates q1 located at pos1 up to maxAngle radians in an attempt to face pos2,
        /// and returns the resulting Quaternion.
        /// </summary>
        /// <param name="q1">Starting rotation.</param>
        /// <param name="pos1">Position of object being rotated.</param>
        /// <param name="pos2">Position being rotated to.</param>
        /// <param name="maxAngle">Max amount in radians to rotate q1.</param>
        /// <returns>Quaternion representing q1 at pos1 rotated up to maxAngle radians toward pos2.</returns>
        public static Quaternion Face(Quaternion q1, Vector3 pos1, Vector3 pos2, float maxAngle = MathHelper.Pi)
        {
            if (pos1 == pos2)
                return q1;
            // This will rotate around global Up. Result is that the angle is always based on rotations around
            // the Y axis, which means even if you're looking nearly straight down, you'll take just as long to
            // rotate a small amount to the right/left as if you were looking straight forward.
            // TODO: find a way to keep looking up while allowing e.g. fast rotationsdirectly across verticals

            // Set the Up vector

            float xDiff = pos1.X - pos2.X;
            float zDiff = pos1.Z - pos2.Z;

            // Can use the current Up vector for entities that shouldn't be constantly upright.
            // TODO: make this an option?
            //Vector3 upVector = Matrix.CreateFromQuaternion(transform.Rotation).Up;
            Vector3 upVector = Vector3.Up;
            if (Math.Abs(xDiff) <= 0.01 && Math.Abs(zDiff) <= 0.01)
                upVector = pos1.Y > pos2.Y ? Vector3.Forward : Vector3.Backward;
                //upVector = Matrix.CreateFromQuaternion(q1).Forward;
            

            // Rotation we want to be at when we're done
            Quaternion goalRotation = Quaternion.CreateFromRotationMatrix(
                Matrix.Transpose(Matrix.CreateLookAt(pos1, pos2, upVector))
                );

            // If maxangle is already greater than a 180deg turn, then we can just skip
            // the quaternion operation and return the target rotation.
            return maxAngle > MathHelper.PiOver2 ? goalRotation : Face(q1, goalRotation, maxAngle);
        }

        /// <summary>
        /// Rotates q1 up to maxAngle radians in an attempt to match q2,
        /// and returns the resulting Quaternion.
        /// </summary>
        /// <param name="q1">Starting rotation.</param>
        /// <param name="q2">Goal rotation.</param>
        /// <param name="maxAngle">Max amount in radians to rotate q1.</param>
        /// <returns>Quaternion representing q1 rotated up to maxAngle radians toward q2.</returns>
        public static Quaternion Face(Quaternion q1, Quaternion q2, float maxAngle)
        {
            if (maxAngle >= MathHelper.PiOver2)
                return q2;

            // Angle between q1 and q2 rotations
            float angle = AngleBetween(q1, q2);
            //var angle = (float) Math.Acos(Quaternion.Dot(q1, q2)); // slower perf?
            //float angle = HandyMath.FastAcos(Quaternion.Dot(q1, q2)); // slows down toward end

            // [S]Lerp always takes the shorter path
            if (angle > MathHelper.PiOver2)
                angle = MathHelper.Pi - angle;

            // Already there / no rotation necessary
            if (float.IsNaN(angle) || angle <= maxAngle)
                return q2;

            // retired due to weird hemisphere-related rotation reversal bug
            //transform.Rotation = maxAngle <= angle
            //? goalRotation
            //: QuaternionHelper.LerpLimited(transform.Rotation, goalRotation,
            //Math.Min(maxAngle, angle));

            // [S]Lerp takes a value representing a 0-1 point between q1 and q2.
            // We need to find out what point in that scale represents our angle.
            float lerp = HandyMath.ScaleValue(maxAngle, angle, 1);

            // TODO: make Slerp an option for higher precision?
            return Quaternion.Lerp(q1, q2, lerp);
        }
    }
}