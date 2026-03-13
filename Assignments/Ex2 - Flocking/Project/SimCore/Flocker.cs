namespace Uwu.Simulation.Steering
{
    public abstract class Flocker : AgentGroup
    {
        #region Properties
        public double AlignmentStrength { get; set; } = 0.0;
        public double CohesionStrength { get; set; } = 0.0;
        public double SeparationStrength { get; set; } = 0.0;
        #endregion
    }
}
