using System.Collections.Generic;
using Uwu.Simulation;
using Uwu.Simulation.Steering;

namespace Uwu.Simulation.Steering.StudentAI
{
    public class Flock : Flocker
    {
       /*********** Inherited Properties *************
        public double AlignmentStrength { get; set; }
        public double CohesionStrength { get; set; }
        public double SeparationStrength { get; set; }
        public double GroupRadius { get; set; }
        public Vector2 AveragePosition { get; set; }
        protected List<MovingObject> Boids { get; set; }
        **********************************************/

       /***** Useful Inherited Methods *****
        public Flocker(); (Constructor)
        protected void Initialize();
        public override string ToString();
        ************************************/

        #region Properties
        protected Vector2 AverageForward { get; set; } = Vector2.Empty;
        #endregion
 
        #region TODO
        public override void Update(float deltaTime)
        {
            // Update goes here
        }
        #endregion
    }
}
