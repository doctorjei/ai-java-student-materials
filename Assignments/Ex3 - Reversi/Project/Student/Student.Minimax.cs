namespace Uwu.Games.Reversi.Student;

using ReversiBehavior = GamePlaying.Behavior<State, Player, Move, Player, int>;
public class Minimax : ReversiBehavior
{
    public Minimax() { }

    // Starts look-ahead process to find best move.
    public Move Run(State board, int lookAheadDepth) => GetBestMove(board, lookAheadDepth);

    // Uses look ahead to evaluate all valid moves for given player color & returns best move
    // found. This method will only be called if there's at least one valid move for player.
    private Move GetBestMove(State board, int depth)
    {
        //
        // TODO: Perform minimax for each move to determine which is best.
        //
        // Return the best move found.
        return State.NewMove(-1, -1); // Delete this... return what you find!
    }

    #region Recommended Helper Functions
    private int Evaluate(State board)
    {
        // TODO: Evaluation method for state
        return 0;
    }
    #endregion
}