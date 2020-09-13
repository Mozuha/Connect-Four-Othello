/*
 * Sep 3, 2020
 * Mizuki Hashimoto
 * 
 * This is a class of drawing to get the state of grid and print a grid.
 * This class work as a view class.
 */

using System;

namespace ConnectFourOthello {
	[Serializable]
  public class Drawing {
		public void Print(Grid grid) {
			grid.GetGridSize(out int row, out int col);
			for (int i = 0; i < row; i++) {
				for (int j = 0; j < col; j++) {
					Console.Write(grid[i, j].Symbol + " ");
					if (j == col - 1) Console.WriteLine();
				}
			}
		}
	}
}