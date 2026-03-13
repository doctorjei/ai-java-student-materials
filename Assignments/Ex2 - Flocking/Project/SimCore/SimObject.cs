namespace Uwu.Simulation
{
    public class SimObject
    {
        public const double DEFAULT_COLLISION_RADIUS = 8.0;
        public static readonly Vector2 DEFAULT_POSITION = Vector2.Empty;

        #region Properties
        public Vector2 Position { get; set; } = DEFAULT_POSITION;
        public double CollisionRadius { get; set; } = DEFAULT_COLLISION_RADIUS;
        #endregion

        #region Methods
        public virtual void Initialize()
        {
            Position = DEFAULT_POSITION;
            CollisionRadius = DEFAULT_COLLISION_RADIUS;
        }
        #endregion
    }
}
