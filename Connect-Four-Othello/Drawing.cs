/*
 * Sep 3, 2020
 * Mizuki Hashimoto
 * 
 * This is a class of drawing to get the state of grid and print a grid.
 * This class work as a view class.
 */

using System;

namespace ConnectFourOthello {
	public class Drawing {
		public void Print(Grid grid) {
			for (int i = 0; i < 6; i++) {
				for (int j = 0; j < 4; j++) {
					Console.Write(grid[i, j].Symbol + " ");
					if (j == 3) Console.WriteLine();
                }
            }
        }
	}
}
