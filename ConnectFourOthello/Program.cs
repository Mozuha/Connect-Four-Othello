/*
 * Sep 3, 2020
 * Mizuki Hashimoto
 * 
 * This is the main class. Its role is to conduct game flow; prompt message, receive input, 
 * validate input, send update details to grid class, receive the grid state, and print the result.
 * This class work like a control class.
 */

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Linq;

namespace ConnectFourOthello {
  class Program {
    static void Main(string[] args) {
      string repeat;  // play again or not
      Drawing drawing = new Drawing();
      Grid grid = null;

      // repeat the game as much as the user wants
      do {
        string choice;

        do {
          Console.Write(
          "Welcome to Connect Four Othello! What would you like to do?\n" +
          "1. Start new game\n" +
          "2. Restore the previous state of the game\n" +
          "Enter your choice (1 or 2): "
          );
          choice = Console.ReadLine();
          Console.WriteLine();

          if (choice != "1" && choice != "2") {
            Console.WriteLine("Please enter either 1 or 2.\n");
          }
        } while (choice != "1" && choice != "2");
        
        if (choice == "1") {
          string setOrNot = ValidateYN(
            "Would you like to set the number of grid row and column?\n" +
            "If not, it will be set to 6 rows and 4 columns. (y/n): "
            );

          if (setOrNot == "y") {
            int row = 0, col = 0;
            string loadOrNot = ValidateYN("Would you like to load the number of grid row and column from a file? (y/n): ");

            if (loadOrNot == "y") {
              do {
                Console.Write("Enter the CSV or XML file name that contains the number of grid row and column: ");
                string fileName = ValidateFileExistence(Console.ReadLine());
                Console.WriteLine();

                if (fileName.Contains(".csv")) {
                  var tuple = LoadCSV(fileName);
                  row = tuple.Item1;
                  col = tuple.Item2;
                }

                else if (fileName.Contains(".xml")) {
                  var tuple = LoadXML(fileName);
                  row = tuple.Item1;
                  col = tuple.Item2;
                }

                else {
                  Console.WriteLine("Please enter the CSV or XML file name.\n");
                  continue;
                }
              } while (!ValidateLength(row, col));
            }

            else {
              do {
                Console.Write("Enter the number of grid row and column (row col): ");
                string input = Console.ReadLine();
                Console.WriteLine();
                row = ParseInput(input[0].ToString());
                col = ParseInput(input[2].ToString());
              } while (!ValidateLength(row, col));
            }

            grid = new Grid(row, col);
          }

          else grid = new Grid();
        }

        else if (choice == "2") {
          while (true) {
            Console.Write("Enter the XML file name that contains the previous state: ");
            string fileName = ValidateFileExistence(Console.ReadLine());
            Console.WriteLine();

            if (fileName.Contains(".xml")) {
              Deserialize(ref grid, fileName);
              break;
            }

            else Console.WriteLine("Please enter the XML file name.\n");
          }
        }

        // repeat the process until game ends
        while (true) {
          int position = ValidateDropPos(grid.CurrentTurn, grid);  // column where disc will be dropped
          int top = grid.CheckStack(position);  // row where disc will be dropped

          Console.WriteLine($"\nTurn {grid.CurrentTurn}\n");

          // red turn
          if (grid.CurrentTurn % 2 == 1) {
            RedDisc red = new RedDisc();
            grid.PutDisc(top, position, red);  // put a disc on grid and flip as needed
            drawing.Print(grid);  // print a grid
            Console.WriteLine();
            if (grid.Check(red)) break;  // check whether four red sequence exists, if yes, then finish the game
          }

          // yellow turn
          else {
            YellowDisc yellow = new YellowDisc();
            grid.PutDisc(top, position, yellow);  // put a disc on grid and flip as needed
            drawing.Print(grid);  // print a grid
            Console.WriteLine();
            if (grid.Check(yellow)) break;  // check whether four yellow sequence exists, if yes, then finish the game
          }

          // if all squares of grid are filled without four disc sequence, then the result is draw
          if (grid.CheckDraw()) break;

          grid.CurrentTurn++;

          // store the current state of the game if user wish to, and end the game
          if (Save(grid)) {
            Console.WriteLine(
              "Your game state has been saved.\n" +
              "Terminating the game..."
              );
            return;
          }
        }

        // print the result message
        if (grid.Result == "red") Console.WriteLine("Red (o) Win!");
        else if (grid.Result == "yellow") Console.WriteLine("Yellow (x) Win!");
        else Console.WriteLine("Draw.");

        // how many turns took
        Console.WriteLine($"Took {grid.CurrentTurn} turns.");

        // ask whether user wants to play again
        repeat = ValidateYN("Would you like to play again? (y/n): ");
      } while (repeat == "y");
    }

    // check the input is whether y or n
    private static string ValidateYN(string prompt) {
      string ans;

      do {
        Console.Write(prompt);
        ans = Console.ReadLine();
        Console.WriteLine();
        if (ans != "y" && ans != "Y" && ans != "n" && ans != "N") {
          Console.WriteLine("Please enter y or Y or n or N.\n");
        }
      } while (ans != "y" && ans != "Y" && ans != "n" && ans != "N");

      if (ans == "y" || ans == "Y") return "y";
      else return "n";
    }

    // check the input for row and column length and returns valid value,
    // repeat the prompt until user inputs valid input
    private static bool ValidateLength(int row, int col) {
      if (row < 4 || col < 4) {
        Console.WriteLine("It should be equal or greater than 4.\n");
        return false;
      }
      else return true;
    }

    // check the input for drop position and returns valid value, 
    // repeat the prompt until user inputs valid input
    private static int ValidateDropPos(int turn, Grid grid) {
      bool valid;
      int position = 0;
      Regex rx = new Regex(@"^-?\d+");  // any integer
      grid.GetGridSize(out _, out int col);

      do {
        if (turn % 2 == 1) {  // red turn
          Console.Write($"Red (o) turn. Where do you want to drop a disc? (0-{col-1}): ");
        }
        else {  // yellow turn
          Console.Write($"Yellow (x) turn. Where do you want to drop a disc? (0-{col-1}): ");
        }

        // validate input
        string input = Console.ReadLine();

        // input is integer but not within the column range
        if (rx.IsMatch(input) && (ParseInput(input) < 0 || col-1 < ParseInput(input))) {
          Console.WriteLine("Out of range. Cannot drop a disc there.");
          valid = false;
        }

        // input is other than integer
        else if (ParseInput(input) == -1) {
          Console.WriteLine($"Invalid input. Please enter the number between 0 to {col-1}.");
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
      } catch (Exception) {
        return -1;
      }
    }

    // prompt to save the game
    // if user wishes to save, store the state of the game into the user specified file
    private static bool Save(Grid grid) {
      string input = ValidateYN("Would you like to save and exit the game? (y/n): ");

      if (input == "y") {
        string fileName;

        while (true) {
          Console.Write("Enter the XML file name to store the state of the game: ");
          fileName = Console.ReadLine();
          Console.WriteLine();

          if (fileName.Contains(".xml")) break;

          else Console.WriteLine("Please enter the XML file name.\n");
        }

        Serialize(grid, fileName);

        return true;
      }

      return false;
    }

    // check whether the user entered file exists
    // repeat the prompt until user inputs valid input
    private static string ValidateFileExistence(string input) {
      while (!File.Exists(input)) {
        Console.WriteLine("File does not exist. Check whether the file path is correct.\n");
        Console.Write("Enter the file name: ");
        input = Console.ReadLine();
        Console.WriteLine();
      }

      return input;
    }

    // load csv file and get the number of grid row and column
    private static (int, int) LoadCSV(string fileName) {
      StreamReader reader = new StreamReader(fileName, Encoding.GetEncoding("UTF-8"));
      string[] infos = reader.ReadLine().Split(',');  // get numbers separated by comma
      int row = ParseInput(infos[0]);
      int col = ParseInput(infos[1]);
      reader.Close();

      return (row, col);
    }

    // load xml file and get the number of grid row and column
    private static (int, int) LoadXML(string fileName) {
      XElement xml = XElement.Load(fileName);
      int row = ParseInput(xml.Element("Row").Value);
      int col = ParseInput(xml.Element("Column").Value);

      return (row, col);
    }

    // serialize the grid object and store it into the user specified file
    private static void Serialize(Grid grid, string fileName) {
      Stream saveFileStream = File.Create(fileName);
      SoapFormatter serializer = new SoapFormatter();

      try {
        serializer.Serialize(saveFileStream, grid);
      }
      catch (SerializationException e) {
        Console.WriteLine(e.Message);
      }
      finally {
        saveFileStream.Close();
      }
    }

    // deserialize the grid object from the user specified file
    private static void Deserialize(ref Grid grid, string fileName) {
      Stream loadFileStream = File.OpenRead(fileName);
      SoapFormatter deserializer = new SoapFormatter();

      try {
        grid = (Grid)(deserializer.Deserialize(loadFileStream));
      }
      catch (SerializationException e) {
        Console.WriteLine(e.Message);
      }
      finally {
        loadFileStream.Close();
      }
    }
  }
}
