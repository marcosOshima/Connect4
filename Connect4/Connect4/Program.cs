using System;

namespace Connect4
{
    class Program
    {
        static char[,] board = new char[6, 7]; // 6 rows, 7 columns
        static char currentPlayer = 'X';

        static void Main(string[] args)
        {
            InitializeBoard(); // Initialize the game board
            Console.WriteLine("Lets Play Connect 4!");
            Console.WriteLine("Player 1: X | Player 2: O\n");

            bool gameEnded = false; // Flag to track if the game has ended
            int movesCount = 0; // Counter for moves made

            while (!gameEnded)
            {
                DrawBoard(); // Display the current state of the board
                int column = NextMove(); // Get the player's move

                if (!IsValidMove(column)) // Check if the move is valid
                {
                    Console.WriteLine("Column is full! Choose another column.");
                    continue;
                }

                PlaceMove(column); // Place the player's move on the board
                movesCount++; // Increment the moves count

                if (CheckWin()) // Check if the current player has won
                {
                    DrawBoard(); // Display the final board state
                    Console.WriteLine($"Player {currentPlayer} wins!");
                    gameEnded = true; // End the game
                }
                else if (movesCount == 42) // Check if it's a draw
                {
                    DrawBoard();
                    Console.WriteLine("It's a draw!");
                    gameEnded = true;
                }
                else
                {
                    SwitchPlayer(); // Switch to the next player
                }
            }
        }

        static void InitializeBoard()
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    board[row, col] = ' '; // Initialize each cell with empty space
                }
            }
        }

        static void DrawBoard()
        {
            Console.WriteLine("  1 2 3 4 5 6 7");
            Console.WriteLine("---------------");
            for (int row = 0; row < 6; row++)
            {
                Console.Write("|");
                for (int col = 0; col < 7; col++)
                {
                    Console.Write(board[row, col]); // Display the contents of each cell
                    Console.Write("|");
                }
                Console.WriteLine();
                Console.WriteLine("---------------");
            }
        }

        static int NextMove()
        {
            Console.Write($"\nPlayer {currentPlayer}'s turn. Enter column number (1-7): ");
            return int.Parse(Console.ReadLine()) - 1; // Subtract 1 to adjust for array index
        }

        static bool IsValidMove(int column)
        {
            return column >= 0 && column < 7 && board[0, column] == ' '; // Check if the column is not full
        }

        static void PlaceMove(int column)
        {
            for (int row = 5; row >= 0; row--)
            {
                if (board[row, column] == ' ')
                {
                    board[row, column] = currentPlayer; // Place the player's move in the first empty cell of the column
                    break;
                }
            }
        }

        static bool CheckWin()
        {
            // Check horizontally
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[row, col] == currentPlayer &&
                        board[row, col + 1] == currentPlayer &&
                        board[row, col + 2] == currentPlayer &&
                        board[row, col + 3] == currentPlayer)
                    {
                        return true; // Return true if four consecutive cells are occupied by the same player horizontally
                    }
                }
            }

            // Check vertically
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (board[row, col] == currentPlayer &&
                        board[row + 1, col] == currentPlayer &&
                        board[row + 2, col] == currentPlayer &&
                        board[row + 3, col] == currentPlayer)
                    {
                        return true; // Return true if four consecutive cells are occupied by the same player vertically
                    }
                }
            }

            // Check diagonally (bottom-left to top-right)
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[row, col] == currentPlayer &&
                        board[row + 1, col + 1] == currentPlayer &&
                        board[row + 2, col + 2] == currentPlayer &&
                        board[row + 3, col + 3] == currentPlayer)
                    {
                        return true; // Return true if four consecutive cells are occupied by the same player diagonally (bottom-left to top-right)
                    }
                }
            }

            // Check diagonally (bottom-right to top-left)
            for (int row = 0; row < 3; row++)
            {
                for (int col = 3; col < 7; col++)
                {
                    if (board[row, col] == currentPlayer &&
                        board[row + 1, col - 1] == currentPlayer &&
                        board[row + 2, col - 2] == currentPlayer &&
                        board[row + 3, col - 3] == currentPlayer)
                    {
                        return true; // Return true if four consecutive cells are occupied by the same player diagonally (bottom-right to top-left)
                    }
                }
            }

            return false; // Return false if no win condition is met
        }

        static void SwitchPlayer()
        {
            currentPlayer = (currentPlayer == 'X') ? 'O' : 'X'; // Switch the current player
        }
    }
}
