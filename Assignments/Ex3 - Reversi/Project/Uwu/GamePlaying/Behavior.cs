// Modified: 2009 Jeremiah Blanchard (Full Sail)
// Modified: 2011 Derek Bliss (Full Sail University)
// Modified: 2019, 2025 Jeremiah Blanchard (University of Florida)

namespace Uwu.GamePlaying;

// Default parameter type is int
public interface Behavior<G, A, M> : Behavior<G, A, M, A, int> where G : State<A, M, A> { }
public interface Behavior<G, A, M, E> : Behavior<G, A, M, E, int> where G : State<A, M, E> { }

public interface Behavior<G, A, M, E, R> where G : State<A, M, E>
{
    /// <summary>
    /// This is a simplified behavior class, as our agent has only one type of behavior to implement.
    /// </summary>
    /// <param name="A">The actor on whose behalf the AI will seek the an action.</param>
    /// <para name="M">The move that the behavior will seek to determine.</para>
    /// <param name="_state">The state that represents the shared game state</param>
    /// <param name="_remainder">Any remaining values to be passed (e.g., difficultly)</param>
    /// <returns>"Best" move within given constraints / null if valid move could not be found.</returns>
    public M Run(G _state, R _remainder);
    public static System.IO.StreamWriter? DebugConsole { get; set; } = null;
}