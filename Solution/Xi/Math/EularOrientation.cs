using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// A configurable eular orientation.
    /// </summary>
    public class EularOrientation
    {
        /// <summary>
        /// Create an EularOrientation.
        /// </summary>
        public EularOrientation()
        {
            RefreshOrientation();
        }

        /// <summary>
        /// The calculated orientation matrix.
        /// </summary>
        public Matrix OrientationMatrix { get { return _orientationMatrix; } }

        /// <summary>
        /// The calculated orientation quaternion.
        /// </summary>
        public Quaternion OrientationQuaternion { get { return _orientationQuaternion; } }

        /// <summary>
        /// The orientation right vector.
        /// </summary>
        public Vector3 Right { get { return _orientationMatrix.Right; } }

        /// <summary>
        /// The orientation left vector.
        /// </summary>
        public Vector3 Left { get { return _orientationMatrix.Left; } }

        /// <summary>
        /// The orientation up vector.
        /// </summary>
        public Vector3 Up { get { return _orientationMatrix.Up; } }

        /// <summary>
        /// The orientation down vector.
        /// </summary>
        public Vector3 Down { get { return _orientationMatrix.Down; } }

        /// <summary>
        /// The orientation forward vector.
        /// </summary>
        public Vector3 Forward { get { return _orientationMatrix.Forward; } }

        /// <summary>
        /// The orientation backward vector.
        /// </summary>
        public Vector3 Backward { get { return _orientationMatrix.Backward; } }

        /// <summary>
        /// The axis of the first rotation applied.
        /// </summary>
        public EularAxis Axis1
        {
            get { return _axis1; }
            set
            {
                _axis1 = value;
                RefreshOrientation();
            }
        }

        /// <summary>
        /// The axis of the second rotation applied.
        /// </summary>
        public EularAxis Axis2
        {
            get { return _axis2; }
            set
            {
                _axis2 = value;
                RefreshOrientation();
            }
        }

        /// <summary>
        /// The axis of the third rotation applied.
        /// </summary>
        public EularAxis Axis3
        {
            get { return _axis3; }
            set
            {
                _axis3 = value;
                RefreshOrientation();
            }
        }

        /// <summary>
        /// The angle of the first rotation.
        /// </summary>
        public float Angle1
        {
            get { return _angle1; }
            set
            {
                if (_angle1 == value) return;
                _angle1 = value;
                RefreshOrientation();
            }
        }

        /// <summary>
        /// The angle of the second rotation.
        /// </summary>
        public float Angle2
        {
            get { return _angle2; }
            set
            {
                if (_angle2 == value) return;
                _angle2 = value;
                RefreshOrientation();
            }
        }

        /// <summary>
        /// The angle of the third rotation.
        /// </summary>
        public float Angle3
        {
            get { return _angle3; }
            set
            {
                if (_angle3 == value) return;
                _angle3 = value;
                RefreshOrientation();
            }
        }

        /// <summary>
        /// Get the orientation in matrix form.
        /// </summary>
        public void GetOrientation(out Matrix orientation)
        {
            orientation = OrientationMatrix;
        }

        /// <summary>
        /// Get the orientation in quaternion form.
        /// </summary>
        public void GetOrientation(out Quaternion orientation)
        {
            orientation = OrientationQuaternion;
        }

        /// <summary>
        /// Clear the axis specifications to X, Y, and Z.
        /// </summary>
        public void ClearAxes()
        {
            Axis1 = EularAxis.X;
            Axis2 = EularAxis.Y;
            Axis3 = EularAxis.Z;
        }

        /// <summary>
        /// Clear the angles.
        /// </summary>
        public void ClearAngles()
        {
            Angle1 = Angle2 = Angle3 = 0;
        }

        /// <summary>
        /// Clear the angles and axis specifications.
        /// </summary>
        public void Clear()
        {
            ClearAxes();
            ClearAngles();
        }

        private void RefreshOrientation()
        {
            RefreshOrientationMatrix();
            RefreshOrientationQuaternion();
        }

        private void RefreshOrientationMatrix()
        {
            Matrix rotation1, rotation2, rotation3, rotation1And2;
            CreateRotation(Angle1, Axis1, out rotation1);
            CreateRotation(Angle2, Axis2, out rotation2);
            CreateRotation(Angle3, Axis3, out rotation3);
            Matrix.Multiply(ref rotation1, ref rotation2, out rotation1And2);
            Matrix.Multiply(ref rotation1And2, ref rotation3, out _orientationMatrix);
        }

        private void RefreshOrientationQuaternion()
        {
            Quaternion.CreateFromRotationMatrix(ref _orientationMatrix, out _orientationQuaternion);
        }

        private static void CreateRotation(float angle, EularAxis axis, out Matrix rotation)
        {
            switch (axis)
            {
                case EularAxis.X: Matrix.CreateRotationX(angle, out rotation); break;
                case EularAxis.Y: Matrix.CreateRotationY(angle, out rotation); break;
                default: Matrix.CreateRotationZ(angle, out rotation); break;
            }
        }

        private Matrix _orientationMatrix = Matrix.Identity;
        private Quaternion _orientationQuaternion = Quaternion.Identity;
        private EularAxis _axis1 = EularAxis.X;
        private EularAxis _axis2 = EularAxis.Y;
        private EularAxis _axis3 = EularAxis.Z;
        private float _angle1;
        private float _angle2;
        private float _angle3;
    }
}
