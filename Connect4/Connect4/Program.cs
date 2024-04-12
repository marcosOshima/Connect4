using System;

namespace Connect4
{
    // Exception for invalid column input
    public class InvalidColumnException : Exception
    {
        public InvalidColumnException(string message) : base(message)
        {
        }
    }

    // Exception for full column moves
    public class ColumnFullException : Exception
    {
        public ColumnFullException(string message) : base(message)
        {
        }
    }
    class BoardController
    {
        public char[,] Board { get; private set; }
        private Player player1;
        private Player player2;
        public Player CurrentPlayer { get; private set; }

        public BoardController(Player p1, Player p2)
        {
            player1 = p1;
            player2 = p2;
            CurrentPlayer = player1; // Start with player1
            Board = new char[6, 7]; // 6 rows, 7 columns
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    Board[row, col] = ' '; // Initialize each cell with empty space
                }
            }
        }

        public void DrawBoard()
        {
            Console.WriteLine(" 1 2 3 4 5 6 7");
            Console.WriteLine("---------------");
            for (int row = 0; row < 6; row++)
            {
                Console.Write("|");
                for (int col = 0; col < 7; col++)
                {
                    char cell = Board[row, col];
                    if (cell != ' ')
                    {
                        Console.ForegroundColor = (cell == player1.Symbol) ? player1.Color : player2.Color;
                    }
                    Console.Write(cell);
                    Console.ResetColor();
                    Console.Write("|");
                }
                Console.WriteLine();
                Console.WriteLine("---------------");
            }
        }

        public bool IsValidMove(int column)
        {
            return column >= 0 && column < 7 && Board[0, column] == ' ';
        }

        public void PlaceMove(int column, char symbol)
        {
            for (int row = 5; row >= 0; row--)
            {
                if (Board[row, column] == ' ')
                {
                    Board[row, column] = symbol;
                    break;
                }
            }
        }

        public bool CheckWin(char symbol)
        {
            // Check horizontally
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 4; col++) 
                {
                    if (Board[row, col] == symbol && Board[row, col + 1] == symbol &&
                        Board[row, col + 2] == symbol && Board[row, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            // Check vertically
            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row < 3; row++) 
                {
                    if (Board[row, col] == symbol && Board[row + 1, col] == symbol &&
                        Board[row + 2, col] == symbol && Board[row + 3, col] == symbol)
                    {
                        return true;
                    }
                }
            }

            // Check diagonally (bottom-left to top-right)
            for (int row = 3; row < 6; row++) 
            {
                for (int col = 0; col < 4; col++) 
                {
                    if (Board[row, col] == symbol && Board[row - 1, col + 1] == symbol &&
                        Board[row - 2, col + 2] == symbol && Board[row - 3, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            // Check diagonally (top-left to bottom-right)
            for (int row = 0; row < 3; row++) 
            {
                for (int col = 0; col < 4; col++) 
                {
                    if (Board[row, col] == symbol && Board[row + 1, col + 1] == symbol &&
                        Board[row + 2, col + 2] == symbol && Board[row + 3, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            // No win found
            return false;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = (CurrentPlayer == player1) ? player2 : player1;
        }
    }
    abstract class Player
    {
        public char Symbol { get; private set; }
        public ConsoleColor Color { get; private set; }
        protected Player(char symbol, ConsoleColor color)
        {
            Symbol = symbol;
            Color = color;
        }

        public abstract int MakeMove(char[,] board);
    }

    class HumanPlayer : Player
    {
        public HumanPlayer(char symbol, ConsoleColor color) : base(symbol, color)
        {
        }

        public override int MakeMove(char[,] board)
        {
            bool validMove = false;
            int column = -1;
            while (!validMove)
            {
                Console.ForegroundColor = Color;
                Console.Write($"\nPlayer {Symbol}'s turn. Enter column number (1-7): ");
                string input = Console.ReadLine();
                try
                {
                    column = int.Parse(input) - 1; 
                    if (column < 0 || column >= 7)
                        throw new InvalidColumnException("Invalid column number. Please enter a number between 1 and 7.");
                    if (board[0, column] != ' ')
                        throw new ColumnFullException("Column is full! Choose another column.");
                    validMove = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                catch (InvalidColumnException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (ColumnFullException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.ResetColor();
            return column;
        }

    }
    class Program
    {
        static BoardController boardController;

        static void Main(string[] args)
        {
            Console.WriteLine("Let's Play Connect 4!");
            Console.WriteLine("Player 1: X | Player 2: O\n");

            Player player1 = new HumanPlayer('X', ConsoleColor.DarkCyan);
            Player player2 = new HumanPlayer('O', ConsoleColor.DarkMagenta);
            boardController = new BoardController(player1, player2);

            bool gameEnded = false;
            int movesCount = 0;

            while (!gameEnded)
            {
                boardController.DrawBoard();
                int column = boardController.CurrentPlayer.MakeMove(boardController.Board);

                if (!boardController.IsValidMove(column))
                {
                    Console.WriteLine("Column is full! Choose another column.");
                    continue;
                }

                boardController.PlaceMove(column, boardController.CurrentPlayer.Symbol);
                movesCount++;

                if (boardController.CheckWin(boardController.CurrentPlayer.Symbol))
                {
                    boardController.DrawBoard();
                    Console.ForegroundColor = boardController.CurrentPlayer.Color;
                    Console.WriteLine($"Player {boardController.CurrentPlayer.Symbol} wins!");
                    Console.ResetColor();
                    gameEnded = true;
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
                else if (movesCount == 42)
                {
                    boardController.DrawBoard();
                    Console.WriteLine("It's a draw!");
                    gameEnded = true;
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
                else
                {
                    boardController.SwitchPlayer();
                }
            }
        }
    }
}
