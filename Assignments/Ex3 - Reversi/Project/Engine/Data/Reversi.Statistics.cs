// Modified: 2011 Derek Bliss (Full Sail University)
// Modified: 2019, 2025 Jeremiah Blanchard (University of Florida)

namespace Uwu.Games.Reversi.Data
{
	/// <summary>Summary description for Statistics.</summary>
	public class Statistics
	{
		// Define the game statistics.
		public int BlackWins;
		public int WhiteWins;
		public int OverallDraws;
		public int BlackTotalScore;
		public int WhiteTotalScore;
		public int ComputerWins;
		public int UserWins;
		public int VsComputerDraws;
		public int ComputerTotalScore;
		public int UserTotalScore;

		public Statistics() { this.Reset(); } // Creates new Statistics object (zeroed out)

		// Creates a new Statistics object by copying and existing one.
		public Statistics(Statistics statistics)
		{
			this.BlackWins          = statistics.BlackWins;
			this.WhiteWins          = statistics.WhiteWins;
			this.OverallDraws       = statistics.OverallDraws;
			this.BlackTotalScore    = statistics.BlackTotalScore;
			this.WhiteTotalScore    = statistics.WhiteTotalScore;
			this.ComputerWins       = statistics.ComputerWins;
			this.UserWins           = statistics.UserWins;
			this.VsComputerDraws    = statistics.VsComputerDraws;
			this.ComputerTotalScore = statistics.ComputerTotalScore;
			this.UserTotalScore     = statistics.UserTotalScore;
		}

		// Resets the game statistics.
		public void Reset()
		{
			// Set all counts to zero.
			this.BlackWins          = 0;
			this.WhiteWins          = 0;
			this.OverallDraws       = 0;
			this.BlackTotalScore    = 0;
			this.WhiteTotalScore    = 0;
			this.ComputerWins       = 0;
			this.UserWins           = 0;
			this.VsComputerDraws    = 0;
			this.ComputerTotalScore = 0;
			this.UserTotalScore     = 0;
		}

		// Updates the game statistics.
		public void Update(int blackScore, int whiteScore, bool isBlackComputer, bool isWhiteComputer)
		{
			// Update the overall Black vs. White counts.
			this.BlackTotalScore += blackScore;
			this.WhiteTotalScore += whiteScore;

			if (blackScore > whiteScore)
				this.BlackWins++;
			else if (whiteScore > blackScore)
				this.WhiteWins++;
			else
				this.OverallDraws++;

			// If both players are human or both are computer, we're done.
			if (isBlackComputer == isWhiteComputer)
				return;

			// Otherwise, update the Computer vs. User counts.
			int computerScore = isBlackComputer ? blackScore : whiteScore;
			int userScore = isBlackComputer ? whiteScore : blackScore;

			// Update the scores and counts.
			this.ComputerTotalScore += computerScore;
			this.UserTotalScore += userScore;

			if (computerScore > userScore)
				this.ComputerWins++;
			else if (userScore > computerScore)
				this.UserWins++;
			else
				this.VsComputerDraws++;
		}
	}
}
