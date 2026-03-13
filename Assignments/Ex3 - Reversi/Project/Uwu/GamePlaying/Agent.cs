// ===================================================================
// Agent code.
// Note: These are executed in the worker thread.
// ===================================================================
//
// Modified: 2009 Jeremiah Blanchard (Full Sail)
// Modified: 2011 Derek Bliss (Full Sail University)
// Modified: 2019, 2025 Jeremiah Blanchard (University of Florida)

namespace Uwu.GamePlaying;

public interface Agent<G, A, M> : Agent<G, A, M, A, int> where G : State<A, M, A> { }
public interface Agent<G, A, M, E> : Agent<G, A, M, E, int> where G : State<A, M, E> { }

public interface Agent<G, A, M, E, R> where G : State<A, M, E>
{
    public bool IsHuman();                 // Is this a human agent?
    public M DetermineMove(G _state);      // Returns move based on state.
}
