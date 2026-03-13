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
        // TODO: Perform minimax for each move to determine which is best.
        Move best_move = null;
        State new_state;
        
        for (int i = 0; i < State.GRID_SIZE; i++)
        {
            for (int j = 0; j < State.GRID_SIZE; j++)
            {
                // Consider move at location (row, column) -> (i, j)
                if (board.IsValidMove(board.Current, State.NewMove(i, j)))
                {
                    new_state = new State(board);
                    Move move = State.NewMove(i, j);
                    new_state.MakeMove(move);

                    Player nextPlayer = (board.Current == Player.Black) ? Player.White : Player.Black;

                    if (new_state.HasAnyValidMove(nextPlayer))
                    {
                        new_state.setCurrent(nextPlayer);
                    }

                    if (new_state.IsTerminalState() || (depth == 1))
                    {
                        move.Rank = Evaluate(new_state);
                    }
                    else
                    {
                        move.Rank = GetBestMove(new_state, depth - 1).Rank;
                    }

                    if (best_move == null)
                    {
                        best_move = move;
                    }
                    else if ((int)board.Current == 1)
                    {
                        if (move.Rank > best_move.Rank)
                        {
                            best_move = move;
                        }
                    }
                    else
                    {
                        if (move.Rank < best_move.Rank)
                        {
                            best_move = move;
                        }
                    }
                }
            }
        }

        return best_move; // Return the best move found.
    }

    #region Recommended Helper Functions
    private int Evaluate(State board)
    {
        // TODO: Evaluation method for state
        int score = 0;

        for (int i = 0; i < State.GRID_SIZE; i++)
        {
            for (int j = 0; j < State.GRID_SIZE; j++)
            {
                if (i == 0)
                {
                    if ((j == 0) || (j == State.GRID_SIZE - 1))
                    {
                        score += (int)board[i, j] * 100;
                    }
                    else
                    {
                        score += (int)board[i, j] * 10;
                    }
                }
                else if (i == State.GRID_SIZE - 1)
                {
                    if ((j == 0) || (j == State.GRID_SIZE - 1))
                    {
                        score += (int)board[i, j] * 100;
                    }
                    else
                    {
                        score += (int)board[i, j] * 10;
                    }
                }
                else if ((j == 0) || (j == State.GRID_SIZE - 1))
                {
                    score += (int)board[i, j] * 10;
                }
                else
                {
                    score += (int)board[i, j];
                }
            }
        }

        if (board.IsTerminalState())
        {
            if (board.score > 0)
            {
                score += 10000;
            }
            else if (board.score < 0)
            {
                score -= 10000;
            }
        }

        return score;
    }
    #endregion
}