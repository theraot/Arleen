using OpenTK;

namespace Arleen.Geometry
{
    public struct Transformation
    {
        public static Transformation Identity = new Transformation(OpenTK.Matrix4d.Identity);
        private readonly Matrix4d _matrix;

        public Transformation(Matrix4d matrix)
        {
            _matrix = matrix;
        }

        public OpenTK.Matrix4d Matrix
        {
            get
            {
                return _matrix;
            }
        }

        public Transformation Invert()
        {
            return new Transformation(Matrix4d.Invert(_matrix));
        }

        public Transformation Rotate(Quaterniond quaternion)
        {
            return new Transformation(_matrix * Matrix4d.Rotate(quaternion));
        }

        public Transformation Scale(double scale)
        {
            return new Transformation(_matrix * Matrix4d.Scale(scale));
        }

        public Transformation Scale(Vector3d scale)
        {
            return new Transformation(_matrix * Matrix4d.Scale(scale));
        }

        public Transformation Scale(double x, double y, double z)
        {
            return new Transformation(_matrix * Matrix4d.Scale(x, y, z));
        }

        public Transformation Translate(Vector3d vector)
        {
            return new Transformation(_matrix * Matrix4d.CreateTranslation(vector));
        }

        public Transformation Translate(double x, double y, double z)
        {
            return new Transformation(_matrix * Matrix4d.CreateTranslation(x, y, z));
        }
    }
}