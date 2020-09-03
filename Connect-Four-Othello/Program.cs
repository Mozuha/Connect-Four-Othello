/*
 * Sep 3, 2020
 * Mizuki Hashimoto
 * 
 * This is the main class. Its role is to conduct game flow; prompt message, receive input, 
 * validate input, send update details to grid class, receive the grid state, and print the result.
 * This class work like a control class.
 */

using System;
using System.Text.RegularExpressions;

namespace ConnectFourOthello {
    class Program {
        static void Main(string[] args) {
            string repeat;  // play again or not

            // repeat the game as much as the user wants
            do {
                Grid grid = new Grid();
                Drawing drawing = new Drawing();
                int turn = 1;
                string result;

                // repeat the process until game ends
                while (true) {
                    int position = ValidateInput(turn, grid);  // column where disc will be dropped
                    int top = grid.CheckStack(position);  // row where disc will be dropped

                    Console.WriteLine($"\nTurn {turn}\n");

                    // red turn
                    if (turn % 2 == 1) {
                        RedDisc red = new RedDisc();
                        grid.PutDisc(top, position, red);  // put a disc on grid and flip as needed
                        drawing.Print(grid);  // print a grid
                        Console.WriteLine();
                        if (grid.Check(red)) {  // check whether four red sequence exists, if yes, then finish the game
                            result = red.Color;
                            break;
                        }
                    }

                    // yellow turn
                    else {
                        YellowDisc yellow = new YellowDisc();
                        grid.PutDisc(top, position, yellow);  // put a disc on grid and flip as needed
                        drawing.Print(grid);  // print a grid
                        Console.WriteLine();
                        if (grid.Check(yellow)) {  // check whether four yellow sequence exists, if yes, then finish the game
                            result = yellow.Color;
                            break;
                        }
                    }

                    // if all squares of grid are filled without four disc sequence, then the result is draw
                    if (grid.CheckDraw()) {
                        result = "draw";
                        break;
                    }

                    turn++;
                }

                // print the result message
                if (result == "red") Console.WriteLine("Red (o) Win!");
                else if (result == "yellow") Console.WriteLine("Yellow (x) Win!");
                else Console.WriteLine("Draw.");

                // how many turns took
                Console.WriteLine($"Took {turn} turns.");

                // ask whether user wants to play again
                Console.Write("Would you like to play again? (y/n): ");
                repeat = Console.ReadLine();
                Console.WriteLine();
            } while (repeat == "y");
            
        }

        // check the input and returns valid value, repeat the prompt until user inputs valid input
        private static int ValidateInput(int turn, Grid grid) {
            bool valid;
            int position = 0;
            Regex rx = new Regex(@"^-?\d+");  // any integer

            do {
                if (turn % 2 == 1) {  // red turn
                    Console.Write("Red (o) turn. Where do you want to drop a disc? (0-3): ");
                }
                else {  // yellow turn
                    Console.Write("Yellow (x) turn. Where do you want to drop a disc? (0-3): ");
                }

                // validate input
                string input = Console.ReadLine();

                // input is integer but not within the range of 0-3
                if (rx.IsMatch(input) && (ParseInput(input) < 0 || 3 < ParseInput(input))) {
                    Console.WriteLine("Out of range. Cannot drop a disc there.");
                    valid = false;
                }
                // input is other than integer
                else if (ParseInput(input) == -1) {
                    Console.WriteLine("Invalid input. Please enter 0, 1, 2, or 3.");
                    valid = false;
                }
                // input is valid but that column is fully filled
                else if (ParseInput(input) != -1 && grid.CheckStack(ParseInput(input)) == -1) {
                    Console.WriteLine("That column is fully filled. Cannot drop a disc anymore there.");
                    valid = false;
                }
                // valid input
                else {
                    valid = true;
                    position = int.Parse(input);
                }
            } while (!valid);

            return position;
        }

        // try to int-lize and handle error
        private static int ParseInput(string input) {
            try {
                return int.Parse(input);
            }
            catch (Exception) {
                return -1;
            }
        }
    }
}
