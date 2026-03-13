// Added: 2025 Jeremiah Blanchard (University of Florida)
using System.Collections.Generic;

namespace Uwu.GamePlaying;

/// <summary>
/// Class representing a gameplaying state. Generic Types:
/// 
/// A - *Actor*. Representation of entity (e.g., player) on whose behalf to seek an action
/// M - *Move*. Represention of actions the actor can take (which the AI will identify)
/// E - *Entry*. Representation of components of game state (e.g., board squares / card hands)
/// 
/// "Game-like" (challenge-based) systems usually have subdividable states of similar form that are
/// storable as collections. Chess uses a 2D-board to as the game state; card games use a 1D set of
/// hands; a Rubik's cube's state is a 3D array. This class supports multiple dimensions. By
/// default a state is a 2D 8x8 grid common to chess, checkers, and other board games.
/// </summary>
public interface State : Indexer
{
    public static readonly int DIMENSION_COUNT; // Default number of dimensions
    public static readonly int GRID_SIZE;       // Default size in each dimension
    public static readonly int MAX_DIMENSIONS;  // Max number of dimensions supported

    // Ready to go: dimensions & data representation.
    new public static int[] Dimensions { get; protected set; } = [];

    /// <summary>Default move struct - move in 2D space along with a rank.</summary>
    public class Move(Indexable<int, int> key)
    {
        public int this[int idx] { get { return key[idx]; } }
    }
}

// Special cases; player marker is also board state, move is simple 3D actor entry (e.g., Reversi).
public abstract class State<A> : State<A, State.Move> { }
public abstract class State<A, M> : State<A, M, A> { }

public abstract class State<A, M, E> : State
{
    // Abstract methods (required).
    public abstract bool IsTerminalState();              // Is the game is over?
    public abstract bool IsValidMove(A current, M move); // Is the move valid for player?
    public abstract void MakeMove(M move);               // Apply move by current player to board

    // Dictionary & key system are hidden & meant to be transparent to the end user.
    protected Dictionary<IndexKey<State<A, M, E>>, E> Entries { get; set; } = [];

    // Actor who controls the state - whose "turn" it is (next, current)
    public A? Current { get; protected set; } = default;
    abstract public A Next { get; } // Does not account for exceptional ordering! (e.g., forfeit)

    // Key provider for indexing into the state.
    public static IndexKey<State<A, M, E>>.KeyProvider KeyAt { get; } = new();

    // Ready to go: dimensions & data representation.
    public static int[] Dimensions { get; protected set; } = new int[State.DIMENSION_COUNT];

    // Index magic; variable number of indices. Handles negative indices & checks bounds.
    public E this[params int[] idx]
    {
        get => Entries[KeyAt[idx]];
        set => Entries[KeyAt[idx]] = value;
    }

    // Shortcut to base ToString for use in extension methods.
    public string? ToBaseString() { return base.ToString(); }

    // Constructors - fresh and copy variants.
    public State() : base() { }

    // Copy constructor.
    public State(State<A, M, E> _state) : base()
    {
        foreach (var pair in _state.Entries)
            Entries[pair.Key] = pair.Value;

        Current = _state.Current;
    }
}

public static class AppConfigExtensions
{
    public static string PadCenter(this string str, int length, char pad = ' ',
        bool leanLeft = true) => str.PadLeft(str.Length + (length - str.Length) / 2 +
            (leanLeft ? 0 : (length - str.Length) % 2), pad).PadRight(length, pad);

    public static string ToString<A, M, E>(this State<A, M, E> self) => self.ToString(" ");

    public static string ToString<A, M, E>(
        this State<A, M, E> self, bool indexed) => self.ToString(" ", indexed);

    public static string ToString<A, M, E>(
        this State<A, M, E> self, int thiccness) => self.ToString(" ", true, thiccness);

    public static string ToString<A, M, E>(this State<A, M, E> self,
        bool indexed, int thiccness) => self.ToString(" ", indexed, thiccness);

    public static string ToString<A, M, E>(this State<A, M, E> self,
        string joiner, int thiccness) => self.ToString(joiner, true, thiccness);

    public static string ToString<A, M, E>(
        this State<A, M, E> self, string joiner, bool indexed = true, int thiccness = 0)
    {
        int[] Dim = State<A, M, E>.Dimensions;
        string extra = "";
        for (int i = 0; i < thiccness; i++)
            extra += " ";

        string preamble = $"[{Dim.Length}-dimensional {self.GetType()}, controlled by {self.Current}: ";

        // Only handle 1D and 2D data directly; user can override others. Default to base ToString.
        if (Dim.Length < 1 || Dim.Length > 2)
            return preamble + self.ToBaseString();

        string[] elements = new string[Dim[0]];

        // Handle the 1D case.
        if (Dim.Length == 1)
        {
            string[] header1d = new string[Dim[0]];

            for (int i = 0; i < Dim[0]; i++)
            {
                elements[i] = self[i]?.ToString() ?? "{null}";
                header1d[i] = $"{i}".PadCenter(elements[i].Length, '_');
            }

            // Include the header if indexed; otherwise, just build a string from the elements.
            return preamble + (indexed ? $"\n{extra}{string.Join(joiner, header1d)}\n" : "") +
                $"[{extra}{string.Join(joiner, elements)}{extra}]\n";
        }

        // The only option left is 2D!
        string[][] items = new string[Dim[0]][];
        string[] header2d = new string[Dim[1]];
        int[] widths = new int[Dim[1]];

        for (int i = 0; i < Dim[0]; i++)
        {
            items[i] = new string[Dim[1]];
            // Create strings from all elements; at the same time, track column widths.
            for (int j = 0; j < widths.Length; j++)
            {
                items[i][j] = self[i, j]?.ToString() ?? "{null}";
                widths[j] = int.Max(widths[j], items[i][j].Length);
            }
        }

        int indexWidth = indexed ? $"{Dim[0] - 1}".Length : 0;

        // Now, construct rows with their own indices and padding.
        for (int i = 0; i < Dim[0]; i++)
        {
            for (int j = 0; j < header2d.Length; j++)
                items[i][j] = items[i][j].PadCenter(widths[j]);

            string indexString = indexed ? $"{i}".PadLeft(indexWidth) + " " : "";
            elements[i] = $"{indexString}[{extra}{string.Join(joiner, items[i])}{extra}]";
        }

        // Build header row using column widths if this printout is indexed.
        if (indexed)
        {
            for (int j = 0; j < widths.Length; j++)
                header2d[j] = ("" + j).PadCenter(widths[j], '_');

            preamble += $"\n{extra}{string.Join(joiner, header2d).PadLeft(elements[0].Length - 1)}";
        }

        return preamble + "\n" + string.Join("\n", elements);
    }
}
