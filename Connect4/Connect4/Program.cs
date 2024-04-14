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
        public Player Player1;
        public Player Player2;
        public Player CurrentPlayer { get; private set; }

        public BoardController(Player p1, Player p2)
        {
            Player1 = p1;
            Player2 = p2;
            CurrentPlayer = Player1; // Start with player1
            Board = new char[6, 7]; // 6 rows, 7 columns
            InitializeBoard();
        }

        public void replaceBoard(char[,] newBoard)
        {
            char[,] tempBoard = new char[6, 7];
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    tempBoard[row, col] = newBoard[row, col];
                }
            }
            Board = tempBoard;
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
            Console.Clear();
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
                        Console.ForegroundColor = (cell == Player1.Symbol) ? Player1.Color : Player2.Color;
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
            CurrentPlayer = (CurrentPlayer == Player1) ? Player2 : Player1;
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
        public HumanPlayer(char symbol, ConsoleColor color) : base(symbol, color) {}

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
    
    class AIPlayer : Player
    {
        public AIPlayer(char symbol, ConsoleColor color, int difficulty) : base(symbol, color) 
        {
            this.difficulty = difficulty;
        }

        private int difficulty;
        public override int MakeMove(char[,] board)
        {
            if (difficulty == 1)
            {
                return MakeRandomMove(board);
            }
            else
            {
                return MakeBestMove(board);
            }
        }

        private int MakeRandomMove(char[,] board)
        {
            Random random = new Random();
            int column = random.Next(0, 7);
            while (board[0, column] != ' ')
            {
                column = random.Next(0, 7);
            }
            return column;
        }

        private int MakeBestMove(char[,] board)
        {
            int bestScore = int.MinValue;
            int bestColumn = 0;
            for (int col = 0; col < 7; col++)
            {
                if (board[0, col] == ' ')
                {
                    int score = ScorePosition(board, col);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestColumn = col;
                    }
                }
            }
            return bestColumn;
        }

        private protected int ScorePosition(char[,] board, int col, char symbol = 'O')
        {
            char aiPiece = symbol;
            char opponentPiece = aiPiece == 'O' ? 'X' : 'O';

            int row;
            for (row = 5; row >= 0; row--)
            {
                if (board[row, col] == ' ')
                {
                    break;
                }
            }

            int score = 0;

            char neighborDown = ' ';
            char neighbor2Down = ' ';
            char neighborLeft = ' ';
            char neighbor2Left = ' ';
            char neighborRight = ' ';
            char neighbor2Right = ' ';
            char neighborUpLeft = ' ';
            char neighbor2UpLeft = ' '; 
            char neighborUpRight = ' ';
            char neighbor2UpRight = ' ';
            char neighborDownLeft = ' ';
            char neighbor2DownLeft = ' ';
            char neighborDownRight = ' ';
            char neighbor2DownRight = ' ';


            if (row < 5) // down
                neighborDown = board[row + 1, col];
            if (row < 4)
                neighbor2Down = board[row + 2, col];
            if (col > 0) // left
                neighborLeft = board[row, col - 1];
            if (col > 1)
                neighbor2Left = board[row, col - 2];
            if (col < 6) // right
                neighborRight = board[row, col + 1];
            if (col < 5)
                neighbor2Right = board[row, col + 2];
            if (row > 0 && col > 0) // up-left
                neighborUpLeft = board[row - 1, col - 1];
            if (row > 1 && col > 1)
                neighbor2UpLeft = board[row - 2, col - 2];
            if (row > 0 && col < 6) // up-right
                neighborUpRight = board[row - 1, col + 1];
            if (row > 1 && col < 5)
                neighbor2UpRight = board[row - 2, col + 2];
            if (row < 5 && col > 0) // down-left
                neighborDownLeft = board[row + 1, col - 1];
            if (row < 4 && col > 1)
                neighbor2DownLeft = board[row + 2, col - 2];
            if (row < 5 && col < 6) // down-right
                neighborDownRight = board[row + 1, col + 1];
            if (row < 4 && col < 5)
                neighbor2DownRight = board[row + 2, col + 2];

            BoardController potentialBoard = new BoardController(new HumanPlayer('X', ConsoleColor.DarkCyan), new HumanPlayer('O', ConsoleColor.DarkMagenta));
            
            potentialBoard.replaceBoard(board);
            potentialBoard.PlaceMove(col, aiPiece);
            bool willWin = potentialBoard.CheckWin(aiPiece);
            if (willWin)
            {
                return 1000000;
            }

            potentialBoard.replaceBoard(board);
            potentialBoard.PlaceMove(col, opponentPiece);
            bool willPreventLoss = potentialBoard.CheckWin(opponentPiece);
            if (willPreventLoss)
            {
                return 1000000;
            }

            bool willThrowGame = false;
            for (int i = 0; i < 7; i++)
            {
                potentialBoard.replaceBoard(board);
                potentialBoard.PlaceMove(col, aiPiece);
                potentialBoard.PlaceMove(i, opponentPiece);
                if (potentialBoard.CheckWin(opponentPiece))
                {
                    willThrowGame = true;
                    break;
                }
            }
            if (willThrowGame)
            {
                return -1000000;
            }

            bool isAdjacentToExistingPiece = neighborDown == aiPiece 
            || neighborLeft == aiPiece 
            || neighborRight == aiPiece 
            || neighborUpLeft == aiPiece 
            || neighborUpRight == aiPiece 
            || neighborDownLeft == aiPiece 
            || neighborDownRight == aiPiece;

            int willMake3Count = 0;
            int willPrevent3Count = 0;
            // Check if placing piece will make 3 in a row
            // Edge piece [ X ∙ ∙ ], [ ∙ ∙ X ]
            if (neighborDown == aiPiece && neighbor2Down == aiPiece)
                willMake3Count++;
            if (neighborLeft == aiPiece && neighbor2Left == aiPiece)
                willMake3Count++;
            if (neighborRight == aiPiece && neighbor2Right == aiPiece)
                willMake3Count++;
            if (neighborUpLeft == aiPiece && neighbor2UpLeft == aiPiece)
                willMake3Count++;
            if (neighborUpRight == aiPiece && neighbor2UpRight == aiPiece)
                willMake3Count++;
            if (neighborDownLeft == aiPiece && neighbor2DownLeft == aiPiece)
                willMake3Count++;
            if (neighborDownRight == aiPiece && neighbor2DownRight == aiPiece)
                willMake3Count++;
            // Middle piece [ ∙ X ∙ ]
            if (neighborRight == aiPiece && neighborLeft == aiPiece)
                willMake3Count++;
            if (neighborUpRight == aiPiece && neighborDownLeft == aiPiece)
                willMake3Count++;
            if (neighborUpLeft == aiPiece && neighborDownRight == aiPiece)
                willMake3Count++;
            // Check if placing piece will prevent opponent from making 3 in a row
            // Edge piece [ X ∙ ∙ ], [ ∙ ∙ X ]
            if (neighborDown == opponentPiece && neighbor2Down == opponentPiece)
                willPrevent3Count++;
            if (neighborLeft == opponentPiece && neighbor2Left == opponentPiece)
                willPrevent3Count++;
            if (neighborRight == opponentPiece && neighbor2Right == opponentPiece)
                willPrevent3Count++;    
            if (neighborUpLeft == opponentPiece && neighbor2UpLeft == opponentPiece)
                willPrevent3Count++;
            if (neighborUpRight == opponentPiece && neighbor2UpRight == opponentPiece)
                willPrevent3Count++;
            if (neighborDownLeft == opponentPiece && neighbor2DownLeft == opponentPiece)
                willPrevent3Count++;
            if (neighborDownRight == opponentPiece && neighbor2DownRight == opponentPiece)
                willPrevent3Count++;
            // Middle piece [ ∙ X ∙ ]
            if (neighborRight == opponentPiece && neighborLeft == opponentPiece)
                willPrevent3Count++;
            if (neighborUpRight == opponentPiece && neighborDownLeft == opponentPiece)
                willPrevent3Count++;


            score += willMake3Count * 1000;
            score += willPrevent3Count * 800;
            score += isAdjacentToExistingPiece ? 100 : 0;
            return score;
                

        }
    }

    class Program
    {
        static BoardController boardController;

        static void Main(string[] args)
        {
            ShowMenu();

            void StartGame(bool useAI = false)
            {
                Player player1 = new HumanPlayer('X', ConsoleColor.DarkCyan);
                Player player2;
                if (!useAI)
                {
                    player2 = new HumanPlayer('O', ConsoleColor.DarkMagenta);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Select AI difficulty:");
                    Console.WriteLine(" 1  Easy");
                    Console.WriteLine("[2] Hard");
                    Console.Write("Enter your choice: ");
                    int difficulty;
                    string input = Console.ReadLine();
                    if(input == "") input = "2";
                    try
                    {
                        difficulty = int.Parse(input);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid difficulty.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        ShowMenu();
                        return;
                    }

                    if (difficulty != 1 && difficulty != 2)
                    {
                        Console.WriteLine("Invalid difficulty. Please enter a valid difficulty.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        ShowMenu();
                        return;
                    }

                    Console.Clear();
                    Console.WriteLine("Select your symbol:");
                    Console.WriteLine("[X]");
                    Console.WriteLine(" O");
                    Console.Write("Enter your choice: ");
                    char symbol = 'X'; 
                    string symbolStr = Console.ReadLine();
                    try 
                    {
                        if (symbolStr == "") symbol = 'X';
                        else symbol = char.Parse(symbolStr);
                        symbol = char.ToUpper(symbol);
                        if(symbol == 1) symbol = 'X';
                        else if(symbol == 2) symbol = 'O';
                        
                        if (symbol != 'X' && symbol != 'O')
                            throw new FormatException();
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid symbol.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        ShowMenu();
                    }
                    symbol = char.ToUpper(symbol);
                    if (symbol != 'X' && symbol != 'O')
                    {
                        Console.WriteLine("Invalid symbol. Please enter a valid symbol.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        ShowMenu();
                    }

                    if (symbol == 'X')
                    {
                        player2 = new AIPlayer('O', ConsoleColor.DarkMagenta, difficulty);
                    }
                    else
                    {
                        player1 = new AIPlayer('X', ConsoleColor.DarkCyan, difficulty);
                        player2 = new HumanPlayer('O', ConsoleColor.DarkMagenta);
                    }
                }
                
                boardController = new BoardController(player1, player2);

                bool gameEnded = false;
                int movesCount = 0;

                Console.Clear();
                Console.WriteLine("Let's Play Connect 4!");
                Console.WriteLine("Player 1: X | Player 2: O\n");

                GameLoop(ref gameEnded, ref movesCount);
                
            }
            void GameLoop(ref bool gameEnded, ref int movesCount)
                {
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

                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            ShowMenu();
                        }
                        else if (movesCount == 42)
                        {
                            boardController.DrawBoard();
                            Console.WriteLine("It's a draw!");
                            gameEnded = true;
                            
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            ShowMenu();
                        }
                        else
                        {
                            boardController.SwitchPlayer();
                        }
                    }
                }

            void ShowMenu()
            {
                Console.Clear();
                Console.WriteLine("Welcome to Connect 4!");
                Console.WriteLine("----------------------");
                Console.WriteLine("[1] Play against a friend");
                Console.WriteLine(" 2  Play against AI");
                Console.WriteLine(" 0  Exit");

                Console.Write("Enter your choice: ");

                string input = Console.ReadLine();
                switch (input)
                {
                    case "":
                    case "1":
                        StartGame();
                        break;
                    case "2":
                        StartGame(true);
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Please choose a valid option.");
                        break;
                }

            }
        }
    }
}

