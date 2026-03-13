using System;

namespace Uwu.Simulation
{
    public class MovingObject : SimObject
    {
        public static readonly Vector2 DEFAULT_VELOCITY = Vector2.Empty;
        public const double DEFAULT_MAX_SPEED = 200.0;
        public const double DEFAULT_SAFE_RADIUS = 50.0;

        #region Properties
       /********************* Inherited **********************
        public Vector2 Position { get; set; } = Vector2.Empty;
        public double CollisionRadius { get; set; } = 0.0;
        ******************************************************/

        public Vector2 Velocity { get; set; } = DEFAULT_VELOCITY;
        public double MaxSpeed { get; set; } = DEFAULT_MAX_SPEED;
        public double SafeRadius { get; set; } = DEFAULT_SAFE_RADIUS;
        
        public double Heading
        {
            get
            {
                if (Velocity.X == 0 && Velocity.Y == 0)
                    return 0;

                Vector2 normalized = Vector2.Normalize(Velocity);

                double result = Math.Acos(normalized.X);
                result = normalized.Y < 0 ? -result : result;
                return (Double.IsNaN(result) || Double.IsInfinity(result)) ? 0 : result;
            }
            set
            {
                double currentSpeed = Velocity.Length;
                currentSpeed = currentSpeed > float.Epsilon * 16 ? currentSpeed : 1.0;
                float x = Convert.ToSingle(Math.Cos(value) * currentSpeed);
                float y = Convert.ToSingle(Math.Sin(value) * currentSpeed);
                Velocity = new Vector2(x, y);
            }
        }
        #endregion

        #region Methods
        public override void Initialize()
        {
            base.Initialize();
            Velocity = DEFAULT_VELOCITY;
            MaxSpeed = DEFAULT_MAX_SPEED;
            SafeRadius = DEFAULT_SAFE_RADIUS;
        }

        /// <summary>Updates this object's position based on change in time & velocity.</summary>
        public virtual void Update(float deltaTime)
        {
            if (Velocity.LengthSquared > MaxSpeed * MaxSpeed)
                Velocity = Vector2.Normalize(Velocity) * MaxSpeed;

            Position += Velocity * deltaTime;
        }
        #endregion
    }
}
