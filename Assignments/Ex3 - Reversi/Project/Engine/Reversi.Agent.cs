// ===================================================================
// Agent code.
// Note: These are executed in the worker thread.
// ===================================================================
//
// Modified: 2009 Jeremiah Blanchard (Full Sail)
// Modified: 2011 Derek Bliss (Full Sail University)
// Modified: 2019, 2025 Jeremiah Blanchard (University of Florida)

namespace Uwu.Games.Reversi.Engine;

using ReversiBehavior = GamePlaying.Behavior<State, Player, Move, Player, int>;
public class Agent : GamePlaying.Agent<State, Player, Move>
{
    // Special enmerations for agents (e.g., type & level)
    public enum Type { Human, StudentAI, ExampleAI }
    public enum AiLevel { Beginner = 0, Intermediate = 1, Advanced = 3, Expert = 5 }

    // Defaiult values for agents.
    public const Player P1_COLOR = Player.Black;
    public const Type P1_TYPE = Type.Human;
    public const AiLevel P1_DIFFICULTY = AiLevel.Beginner;

    public const Player P2_COLOR = Player.White;
    public const Type P2_TYPE = Type.ExampleAI;
    public const AiLevel P2_DIFFICULTY = AiLevel.Expert;

    // AI agent's knowledge & behavior.
    public Player Color { get; set; } = Player.Empty;
    public AiLevel Level { get; set; } = AiLevel.Beginner;
    public Type Variant { get; set; } = Type.Human;
    public Move? Planned { get; set; } = null;

    public Agent(Player _color, Type _type, AiLevel _level)
    {
        Color = _color;
        Variant = _type;
        Level = _level;
        UpdateBehavior();
    }

    public Agent() : this(0, Type.Human, AiLevel.Beginner) { }
    public bool IsHuman() => Variant == Type.Human;
    public ReversiBehavior? Behavior { get; set; } = null;

    // Returns a move based on behavior supplied to this agent; returns null on failure.
    public Move DetermineMove(State _state)
    {
        Planned = Behavior!.Run(_state, (int)Level);

        if (Planned.Row != -1 && Planned.Column != -1 && _state.IsValidMove(Color, Planned))
            return Planned;

        throw new NullReferenceException("Behavior returned null or invalid move");
    }

    public void UpdateBehavior()
    {
        Behavior = Variant == Type.StudentAI ? new Student.Minimax() :
                Variant == Type.ExampleAI ? new Example.Minimaximus() : null;
    }

}
