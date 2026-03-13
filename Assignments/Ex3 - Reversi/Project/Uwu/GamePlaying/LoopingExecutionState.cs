// Added: 2025 Jeremiah Blanchard (University of Florida)
namespace Uwu.GamePlaying;

/// <summary>
/// Interface for looping executable simulation state for gameplaying (turn-based) scenarios.
/// While built for incorporation in a gameplaying state, it can also be implemented independently.
/// </summary>
public interface LoopingExecutionState<M>
{
    abstract public void NewSimulation();
    abstract public void StartSimulation();
    abstract public bool StartTurn();
    abstract public void MakeMove(M move);
    abstract public void EndMove();
    abstract public void EndSimulation();
}