using System;

namespace Uwu.Simulation
{
    [Serializable]
    public struct Vector2
    {
        private float _x, _y;

        /// <summary>Initializes a new instance of the Vector2 class.</summary>
        ///
        /// <param name="x">Initial Vector2.X value.</param>
        /// <param name="y">Initial Vector2.Y value.</param>
        public Vector2(float x, float y)
        {
            _x = Convert.ToSingle(x);
            _y = Convert.ToSingle(y);
        }
        public Vector2() : this(0, 0) { }

        /// <summary>Returns the sum (addition) of two Vector2 instances.</summary>
        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs._x + rhs._x, lhs._y + rhs._y);
        }

        /// <summary>Returns the negation of a Vector2.</summary>
        public static Vector2 operator -(Vector2 vec) { return new Vector2(-vec._x, -vec._y); }

        /// <summary>Returns the difference (subtraction) of two Vector2 instances.</summary>
        public static Vector2 operator -(Vector2 lhs, Vector2 rhs) { return (lhs + -rhs); }

        /// <summary>Returns product of scalar value and Vector2 (and mirror variant).</summary>
        ///
        /// <param name="scalar">Source scalar value (left hand side).</param>
        /// <param name="vector">Source Vector2 (right hand side).</param>
        /// <returns>A Vector2 structure that is product of left and right parameters.</returns>
        public static Vector2 operator *(double scalar, Vector2 vector)
        {
            float single = Convert.ToSingle(scalar);
            return new Vector2(vector._x * single, vector._y * single);
        }
        public static Vector2 operator *(Vector2 vec, double scalar) { return (scalar * vec); }
        
        /// <summary>Returns product of a Vector2 ajd the reciprical of a scalar.</summary>
        public static Vector2 operator /(Vector2 vec, double scalar) { return (1/scalar) * vec; }

        /// <summary>Compares current instance to another (equality check)</summary>
        /// <returns>Value of true if objects are the same and false otherwise.</returns>
        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return lhs._x == rhs._x && lhs._y == rhs._y;
        }

        /// <summary>Mirror of equality check (inequality check)</summary>
        public static bool operator !=(Vector2 lhs, Vector2 rhs) { return !(lhs == rhs); }

        /// <summary>Returns the normalized version of a vector.</summary>
        public static Vector2 Normalize(Vector2 vector)
        {
            if(!double.IsNaN(vector.Length) && vector.Length != 0)
                return vector / vector.Length;
            else
                return vector;
        }

        /// <summary>Returns the origin vector (and variants).</summary>
        public static Vector2 Empty { get { return new Vector2(0, 0); } }
        public static Vector2 Zero { get { return Empty; } }

        /// <summary>Compare this Vector2D to another one (equality check variant).</summary>
        public bool Equals(Vector2 other) { return this == other; }

        /// <summary>Compare this object to another object.</summary>
        /// 
        /// <param name="obj">Object to compare against.</param>
        /// <returns>Value of true if the values are the same and false otherwise.</returns>
        public override bool Equals(object obj) { return obj is Vector2 && this == (Vector2)obj; }

        /// <summary>Returns a hash value for this object.</summary>
        public override int GetHashCode() { return new {X, Y}.GetHashCode(); }

        /// <summary>Normalizes this vector.</summary>
        public void Normalize()
        {
            if(!double.IsNaN(Length) && Length != 0)
                this = this / Length;
        }

        /// <summary>Retrieves or sets the x component of a 2D vector.</summary>
        public float X { get => _x; set => _x = value; }

        /// <summary>Retrieves or sets the y component of a 2D vector.</summary>
        public float Y { get => _y; set => _y = value; }

        /// <summary>Returns the square of the length of this vector.</summary>
        public float LengthSquared { get { return Convert.ToSingle(_x * _x + _y * _y); } }

        /// <summary>Returns the length of this vector.</summary>
        public float Length { get { return Convert.ToSingle(Math.Sqrt(LengthSquared)); } }
    }
}
