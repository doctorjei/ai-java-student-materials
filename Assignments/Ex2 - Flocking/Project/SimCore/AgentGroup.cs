using System;
using System.Collections.Generic;
using System.Drawing;

using Uwu.Data;
using Uwu.Simulation;


namespace Uwu.Simulation.Steering
{
    public abstract class AgentGroup
    {
        public static readonly Vector2 DEFAULT_POSITION = Vector2.Empty;
        public const double DEFAULT_GROUP_RADIUS = 50.0;

        #region Static Elements
        static InventoryFactory<MovingObject> agentFactory;
        static Random rng = new Random(); // For colors, etc.
        #endregion

        #region Abstract Methods
        /// <summary>Update all boids' positions in this group based on time passed.</summary>
        public abstract void Update(float deltaTime);
        #endregion

        #region Fields & Properties
        public double GroupRadius { get; set; } = DEFAULT_GROUP_RADIUS;
        public Vector2 AveragePosition { get; set; } = DEFAULT_POSITION;
        public List<MovingObject> Boids { get; protected set; }
        public Color GroupColor { get; set; }
        #endregion

        #region Constructor & Post-constructor initializer
        public AgentGroup()
        {
            if (agentFactory == null)
                agentFactory = InventoryFactory<MovingObject>.Instance;

            Initialize();
        }

        protected void Initialize()
        {
            Boids = new List<MovingObject>();
            GroupColor = Color.FromArgb(rng.Next(128)+127, rng.Next(128)+127, rng.Next(128)+127);
        }
        #endregion

        #region Read Methods ("Getters")
        /// <summary>Indicate if a specific agent is in this group.</summary>
        public bool Contains(MovingObject agent) { return Boids.Contains(agent); }

        // <summary>String representation of the TaskForce.</summary>
        public override string ToString() { return $"[#={Boids.Count}, ARGB={GroupColor.ToArgb():X}]"; }
        #endregion

        #region Write Methods ("Mutators")
        /// <summary>Remove all agents from this group.</summary>
        public void Clear()
        {
            foreach (MovingObject agent in Boids)
                agentFactory.Relinquish(agent);

            Boids.Clear();
        }

        /// <summary>Remove an agent from this group.</summary>
        private void Decrement()
        {
            if (Boids.Count > 0)
            {
                agentFactory.Relinquish(Boids[Boids.Count - 1]);
                Boids.RemoveAt(Boids.Count - 1);
            }
        }

        /// <summary>Adds a new randomized agent of type MovingObject.</summary>
        private void Increment()
        {
            MovingObject agent = agentFactory.Request();

            // Set up the agent's critical values.
            agent.Initialize();
            agent.Heading = rng.NextDouble() * Math.PI * 2;
            agent.Velocity *= Convert.ToSingle(((rng.NextDouble() / 2 + 0.25) * agent.MaxSpeed));
            Vector2 offset = rng.NextDouble() * GroupRadius * Vector2.Normalize(agent.Velocity);
            agent.Position = AveragePosition + offset;
            agent.Update(float.Epsilon);
            Boids.Add(agent);
        }

        /// <summary>Add/remove agents to reach specific size with new agents near average position.</summary>
        public void SetCount(int Count)
        {
            while (Count > Boids.Count)
                Increment();

            while (Count < Boids.Count)
                Decrement();
        }
        #endregion
    }
}
