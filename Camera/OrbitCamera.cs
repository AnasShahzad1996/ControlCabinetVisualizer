using OpenTK.Mathematics;

namespace CubeViewer.Camera
{
    public class OrbitCamera
    {
        public Vector3 Target = Vector3.Zero;
        public float Distance = 5f;
        public float Yaw = -90f;
        public float Pitch = 0f;

        public Vector3 GetPosition()
        {
            float radPitch = MathHelper.DegreesToRadians(Pitch);
            float radYaw = MathHelper.DegreesToRadians(Yaw);
            Vector3 pos;
            pos.X = Target.X + Distance * (float)(Math.Cos(radPitch) * Math.Cos(radYaw));
            pos.Y = Target.Y + Distance * (float)Math.Sin(radPitch);
            pos.Z = Target.Z + Distance * (float)(Math.Cos(radPitch) * Math.Sin(radYaw));
            return pos;
        }

        public Matrix4 GetViewMatrix() => Matrix4.LookAt(GetPosition(), Target, Vector3.UnitY);
    }
}