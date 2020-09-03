/*
 * Sep 3, 2020
 * Mizuki Hashimoto
 * 
 * This is a class of grid. Its role is to apply changes to the grid with the values passed by main class,
 * and check the grid status.
 * This class work as a model class.
 */

using System;

namespace ConnectFourOthello {
	public class Grid {
		private readonly Disc[,] grid;
		private readonly static int MAX_ROW = 6;
		private readonly static int MAX_COLUMN = 4;
		private readonly static EmptyDisc empty = new EmptyDisc();
		private readonly static RedDisc red = new RedDisc();
		private readonly static YellowDisc yellow = new YellowDisc();

		// constructor
		public Grid() {  
			grid = new Disc[MAX_ROW, MAX_COLUMN];

			// set all squares to be empty
			for (int i = 0; i < MAX_ROW; i++) {
				for (int j = 0; j < MAX_COLUMN; j++) {
					grid[i, j] = empty;
				}
			}
		}

		// indexer
		public Disc this[int i, int j] {  
			get => grid[i, j];
			set => grid[i, j] = value;
        }

		// check whether a column is fully filled, return current top index of a column if not
		public int CheckStack(int j) {
			int top = -1;

			for (int i = MAX_ROW - 1; i >= 0; i--) {
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
			return 0 <= i && i < MAX_ROW && 0 <= j && j < MAX_COLUMN && grid[i, j].Color == disc.Color;
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
			numDisc += CountDisc(i, j, 0, 1, disc);	   // right
			numDisc += CountDisc(i, j, 1, 1, disc);	   // bottom right
			numDisc += CountDisc(i, j, 1, 0, disc);	   // bottom
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
			// horizontal check
			for (int i = 0; i < MAX_ROW; i++) {
				if (grid[i, 0].Color == disc.Color && grid[i, 1].Color == disc.Color 
					&& grid[i, 2].Color == disc.Color && grid[i, 3].Color == disc.Color) {
					return true;
                }
            }

			// vertical check
			for (int i = 0; i < MAX_ROW - 3; i++) {
				for (int j = 0; j < MAX_COLUMN; j++) {
					if (grid[i, j].Color == disc.Color && grid[i + 1, j].Color == disc.Color
						&& grid[i + 2, j].Color == disc.Color && grid[i + 3, j].Color == disc.Color) {
						return true;
					}
				}
			}

			// diagonal check, top right to bottom left
			for (int i = 0; i < 3; i++) {
				if (grid[i, 3].Color == disc.Color && grid[i + 1, 2].Color == disc.Color
					&& grid[i + 2, 1].Color == disc.Color && grid[i + 3, 0].Color == disc.Color) {
					return true;
				}
			}

			// diagonal check, top left to bottom right
			for (int i = 0; i < 3; i++) {
				if (grid[i, 0].Color == disc.Color && grid[i + 1, 1].Color == disc.Color
					&& grid[i + 2, 2].Color == disc.Color && grid[i + 3, 3].Color == disc.Color) {
					return true;
				}
			}

			return false;
		}

		// if all squares of grid are filled without four disc sequence, then the result is draw
		public bool CheckDraw() {
			int count = 0;
			for (int j = 0; j < MAX_COLUMN; j++) {
				if (grid[0, j].Color != empty.Color) count++;
            }
			if (count == MAX_COLUMN) return true;
			else return false;
        }
	}
}
