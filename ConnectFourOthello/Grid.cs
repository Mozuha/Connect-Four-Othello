/*
 * Sep 12, 2020
 * Mizuki Hashimoto
 * 
 * This is a class of grid. Its role is to apply changes to the grid with the values passed by main class,
 * and check the grid status. It now holds the state of turn and result so it can be serialized.
 * This class work as a model class.
 */

using System;

namespace ConnectFourOthello {
  [Serializable]
  public class Grid {
		private readonly Disc[,] grid;
		private readonly static EmptyDisc empty = new EmptyDisc();
		private readonly static RedDisc red = new RedDisc();
		private readonly static YellowDisc yellow = new YellowDisc();
		private int turn;
		private string result;

		// constructor
		public Grid() {
			grid = new Disc[6, 4];
			turn = 1;
			result = "unfinished";

			// set all squares to be empty
			for (int i = 0; i < 6; i++) {
				for (int j = 0; j < 4; j++) {
					grid[i, j] = empty;
				}
			}
		}

		// constructor with number of row and column specified
		public Grid(int row, int col) {
			grid = new Disc[row, col];
			turn = 1;
			result = "unfinished";

			// set all squares to be empty
			for (int i = 0; i < row; i++) {
				for (int j = 0; j < col; j++) {
					grid[i, j] = empty;
				}
			}
		}

		// indexer
		public Disc this[int i, int j] {
			get => grid[i, j];
			set => grid[i, j] = value;
		}

		// property to get current turn or increment current turn
		public int CurrentTurn {
			get => turn;
			set => turn = value;
		}

		// property to get result
		public string Result {
			get => result;
		}

		// get size of grid
		public void GetGridSize(out int row, out int col) {
			row = grid.GetLength(0);
			col = grid.GetLength(1);
		}

		// check whether a column is fully filled, return current top index of a column if not
		public int CheckStack(int j) {
			int top = -1;
			GetGridSize(out int row, out int col);

			for (int i = row - 1; i >= 0; i--) {
				if (grid[i, j].Color == empty.Color) {
					top = i;
					break;
				}
			}

			if (top == -1) return -1;  // cannot drop a disc anymore at this column
			else return top;  // top index of this column
		}

		// ensure it fits the range and check the disc type in a square
		private bool CheckASquare(int i, int j, Disc disc) {
			GetGridSize(out int row, out int col);
			return 0 <= i && i < row && 0 <= j && j < col && grid[i, j].Color == disc.Color;
		}

		// count the number of discs surrounded by same disc type for one direction out of eight
		private int CountDisc(int i, int j, int di, int dj, Disc disc) {
			int i1 = i + di;
			int j1 = j + dj;
			int numDisc = 0;

			Disc opponent = red;
			if (disc.Color == "red") opponent = yellow;
			else if (disc.Color == "yellow") opponent = red;

			// count the number of discs while encounter same disc type
			while (CheckASquare(i1, j1, opponent)) {
				i1 += di;
				j1 += dj;
				numDisc++;
			}

			if (CheckASquare(i1, j1, disc)) return numDisc;
			else return 0;
		}

		// count the number of all discs surrounded by same disc type for eight directions
		private int CountDisc(int i, int j, Disc disc) {
			int numDisc = 0;

			numDisc += CountDisc(i, j, -1, 0, disc);   // top
			numDisc += CountDisc(i, j, -1, 1, disc);   // top right
			numDisc += CountDisc(i, j, 0, 1, disc);    // right
			numDisc += CountDisc(i, j, 1, 1, disc);    // bottom right
			numDisc += CountDisc(i, j, 1, 0, disc);    // bottom
			numDisc += CountDisc(i, j, 1, -1, disc);   // bottom left
			numDisc += CountDisc(i, j, 0, -1, disc);   // left
			numDisc += CountDisc(i, j, -1, -1, disc);  // top left

			return numDisc;
		}

		// flip surrounded discs for one direction
		private void Flip(int i, int j, int di, int dj, Disc disc) {
			int numFlippableDisc = CountDisc(i, j, di, dj, disc);

			for (int k = 1; k <= numFlippableDisc; k++) {
				grid[i + (di * k), j + (dj * k)] = disc;
			}
		}

		// flip surrounded discs for eight direction
		private void Flip(int i, int j, Disc disc) {
			Flip(i, j, -1, 0, disc);   // top
			Flip(i, j, -1, 1, disc);   // top right
			Flip(i, j, 0, 1, disc);    // right
			Flip(i, j, 1, 1, disc);    // bottom right
			Flip(i, j, 1, 0, disc);    // bottom
			Flip(i, j, 1, -1, disc);   // bottom left
			Flip(i, j, 0, -1, disc);   // left
			Flip(i, j, -1, -1, disc);  // top left
		}

		// put a disc and flip surrounded opponent discs as needed
		public void PutDisc(int i, int j, Disc disc) {
			if (CheckASquare(i, j, empty)) {
				grid[i, j] = disc;
				CountDisc(i, j, disc);
				Flip(i, j, disc);
			}
		}

		// check whether four disc sequence formed
		public bool Check(Disc disc) {
			GetGridSize(out int row, out int col);

			// horizontal check
			for (int i = 0; i < row; i++) {
				if (grid[i, 0].Color == disc.Color && grid[i, 1].Color == disc.Color
					&& grid[i, 2].Color == disc.Color && grid[i, 3].Color == disc.Color) {
					result = disc.Color;
					return true;
				}
			}

			// vertical check
			for (int i = 0; i < row - 3; i++) {
				for (int j = 0; j < col; j++) {
					if (grid[i, j].Color == disc.Color && grid[i + 1, j].Color == disc.Color
						&& grid[i + 2, j].Color == disc.Color && grid[i + 3, j].Color == disc.Color) {
						result = disc.Color;
						return true;
					}
				}
			}

			// diagonal check, bottom left to top right
			for (int i = 3; i < row; i++) {
				for (int j = 0; j < col - 3; j++) {
					if (grid[i, j].Color == disc.Color && grid[i - 1, j + 1].Color == disc.Color
					&& grid[i - 2, j + 2].Color == disc.Color && grid[i - 3, j + 3].Color == disc.Color) {
						result = disc.Color;
						return true;
					}
				}
			}

			// diagonal check, bottom right to top left
			for (int i = 3; i < row; i++) {
				for (int j = 3; j < col; j++) {
					if (grid[i, j].Color == disc.Color && grid[i - 1, j - 1].Color == disc.Color
					&& grid[i - 2, j - 2].Color == disc.Color && grid[i - 3, j - 3].Color == disc.Color) {
						result = disc.Color;
						return true;
					}
				}
			}

			return false;
		}

		// if all squares of grid are filled without four disc sequence, then the result is draw
		public bool CheckDraw() {
			int count = 0;
			GetGridSize(out int row, out int col);

			for (int j = 0; j < col; j++) {
				if (grid[0, j].Color != empty.Color) count++;
			}
			if (count == col) {
				result = "draw";
				return true;
			}
			else return false;
		}
	}
}