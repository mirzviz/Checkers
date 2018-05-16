using System;
using System.Collections.Generic;
using System.Text;
using CheckersLogic;

namespace CheckersGameProject
{
    public class UI
    {
        private CheckersPlayer m_Player1;
        private CheckersPlayer m_Player2;
        private int m_MultiplayerMode;
        private Board m_Board;

        public UI()
        {
            m_Player1 = new CheckersPlayer();
            m_Player2 = new CheckersPlayer();
            System.Console.WriteLine("Please enter your name: ");
            m_Player1.Name = System.Console.ReadLine();
            askForBoardSize();
            System.Console.WriteLine("Enter 0 for SINGLEPLAYER. Enter 1 for MULTIPLAYER");
            m_MultiplayerMode = int.Parse(Console.ReadLine());
            if (m_MultiplayerMode == 1)
            {
                System.Console.WriteLine("Please enter second player's name: ");
                m_Player2.Name = Console.ReadLine();
            }
            else
            {
                m_Player2.Name = "PC";
            }
        }

        public void StartACheckersGame()
        {
            AnotherGame:
            while (m_Board.GameIsOnKeepPlaying)
            {
                Ex02.ConsoleUtils.Screen.Clear();
                printBoard();
                printWhosTurn();
                if(m_MultiplayerMode == 0 && m_Board.Turn == Square.O)    
                {
                    m_Board.ComputerPlay();
                }
                else
                { 
                    REINPUT:
                    string inputMove = Console.ReadLine();
                    if(inputMove == "Q")
                    {
                        return;
                    }

                    bool inputIsLegal = checkInput(inputMove, m_Board.BoardSize);
                    if (!inputIsLegal)
                    {
                        Console.WriteLine("Input incorrect, try again:");
                        goto REINPUT;
                    }

                    Move moveString;
                    stringToMove(inputMove, out moveString);
                    if (!m_Board.Play(moveString))
                    {
                        Console.WriteLine("The move is illegal, try again:");
                        goto REINPUT;
                    }
                }
            }

            Console.WriteLine("The winner is: {0}", findWinnersNameAndGivePointsToWinner());
            printPointsStatus();
            string anotherGameChoiceInput;
            do
            {
                Console.WriteLine("Enter 1 to keep playing. Enter 'Q' to quit.");
                anotherGameChoiceInput = Console.ReadLine();
            }
            while (anotherGameChoiceInput != "1" && anotherGameChoiceInput != "Q");
            if(anotherGameChoiceInput == "1")
            {
                m_Board.initializeBoard();
                goto AnotherGame;
            }
        }

        private void askForBoardSize()
        {
            Start:
            System.Console.WriteLine("Please enter the board size (6,8,10) : ");
            string input = System.Console.ReadLine();
            if(input != "6" && input != "8" && input != "10")
            {
                goto Start;
            }

            m_Board = new Board(int.Parse(input));
        }

        private string findWinnersNameAndGivePointsToWinner()
        {
            int difference = ((m_Board.NumberOfK * 4) + m_Board.NumberOfX) - ((m_Board.NumberOfU * 4) + m_Board.NumberOfO);
            string winnersName;
            if(difference > 0)
            {                                       // X won = player1 won
                winnersName = m_Player1.Name;
                m_Player1.Points += difference;
            }
            else
            {
                winnersName = m_Player2.Name;
                m_Player2.Points -= difference;
            }

            return winnersName;
        }

        private void printPointsStatus()
        {
            Console.WriteLine("{0} points: {1}", m_Player1.Name, m_Player1.Points);
            Console.WriteLine("{0} points: {1}", m_Player2.Name, m_Player2.Points);
        }

        private void printWhosTurn()
        {
            if (m_Board.Turn == Square.X)
            {
                Console.WriteLine("Turn {0}:", m_Player1.Name);
            }

            if (m_Board.Turn == Square.O && m_MultiplayerMode == 1)
            {
                Console.WriteLine("Turn {0}:", m_Player2.Name);    
            }
        }

        private bool checkInput(string i_Input, int i_BoardSize)
        {
            bool inputIsLeagal = false;
            if (i_Input.Length == 5)
            {
                if (i_Input[0] - 65 >= 0 && i_Input[0] - 65 < i_BoardSize)
                {
                    if (i_Input[1] - 97 >= 0 && i_Input[1] - 97 < i_BoardSize)
                    {
                        if (i_Input[2] == '>')
                        {
                            if (i_Input[3] - 65 >= 0 && i_Input[3] - 65 < i_BoardSize)
                            {
                                if (i_Input[4] - 97 >= 0 && i_Input[4] - 97 < i_BoardSize)
                                {
                                    inputIsLeagal = true;
                                }
                            }
                        }
                    }
                }
            }

            return inputIsLeagal;
        }

        private void stringToMove(string i_InputMove, out Move moveString)
        {
            char sourceCol = i_InputMove[0];
            char sourceRow = i_InputMove[1];
            char destCol = i_InputMove[3];
            char destRow = i_InputMove[4];

            moveString = new Move(sourceRow - 97, sourceCol - 65, destRow - 97, destCol - 65);
        }

        private void printBoard()
        {
            char bigLetter = 'A';
            Console.Write(" ");
            for (int k = 0; k < m_Board.BoardSize; k++)
            {
                System.Console.Write("  {0} ", bigLetter);
                bigLetter++;
            }

            Console.WriteLine();
            printPartition();
            char smallLetter = 'a';
            for (int i = 0; i < m_Board.BoardSize; i++)
            {
                Console.Write("{0}|", smallLetter);
                smallLetter++;
                for (int j = 0; j < m_Board.BoardSize; j++)
                {
                    Square currentSquare = m_Board.BoardMatrix[i, j];
                    if (currentSquare == Square.EMPTY)
                    {
                        Console.Write("   |");
                    }
                    else
                    {
                        Console.Write(" {0} |", currentSquare.ToString());
                    }
                }

                Console.WriteLine();
                printPartition();
            }
        }

        private void printPartition()
        {
            for (int i = 0; i < m_Board.BoardSize; i++)
            {
                Console.Write("====");
            }

            Console.WriteLine("==");
        }
    }
}
