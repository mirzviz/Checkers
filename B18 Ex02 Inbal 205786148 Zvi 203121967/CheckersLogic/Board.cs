using System;

namespace CheckersLogic
{
    public enum Square
    {
        U = -2,
        O = -1,
        EMPTY = 0,
        X = 1,
        K = 2
    }

    public class Board
    {
        //----------------------------------------Members--------------------------------
        private Square m_Turn;
        private bool m_GameIsOnKeepPlaying = true;
        private int m_XMovesCounter = 0;
        private int m_XJumpsCounter = 0;
        private int m_KMovesCounter = 0;
        private int m_KJumpsCounter = 0;
        private int m_OMovesCounter = 0;
        private int m_OJumpsCounter = 0;
        private int m_UMovesCounter = 0;
        private int m_UJumpsCounter = 0;
        private Move[] m_XMovesArr;
        private Move[] m_XJumpsArr;
        private Move[] m_OMovesArr;
        private Move[] m_OJumpsArr;
        private Move[] m_UMovesArr;
        private Move[] m_UJumpsArr;
        private Move[] m_KMovesArr;
        private Move[] m_KJumpsArr;
        private int m_BoardSize;
        private Square[,] m_Board;
        private int m_numOfX;
        private int m_numOfO;
        private int m_numOfU;
        private int m_numOfK;

        public Board(int i_Size)
        {
            m_BoardSize = i_Size;
            m_Board = new Square[i_Size, i_Size];
            initializeBoard();
            m_XMovesArr = new Move[m_numOfX * 2];
            m_XJumpsArr = new Move[m_numOfX * 2];
            m_KMovesArr = new Move[m_numOfX * 2];
            m_KJumpsArr = new Move[m_numOfX * 2];
            m_OMovesArr = new Move[m_numOfO * 2];
            m_OJumpsArr = new Move[m_numOfO * 2];
            m_UMovesArr = new Move[m_numOfO * 2];
            m_UJumpsArr = new Move[m_numOfO * 2];
        }

        public void initializeBoard()
        {
            m_GameIsOnKeepPlaying = true;
            m_numOfX = ((m_BoardSize / 2) - 1) * (m_BoardSize / 2);
            m_numOfO = m_numOfX;
            m_numOfK = 0;
            m_numOfU = 0;
            m_Turn = Square.X;
            bool aLeagalSquare;
            for (int i = 0; i < (m_BoardSize / 2) - 1; i++)
            {
                for (int j = 0; j < m_BoardSize; j++)
                {
                    aLeagalSquare = (i + j) % 2 == 1;
                    if (aLeagalSquare)
                    {
                        m_Board[i, j] = Square.O;
                    }
                    else
                    {
                        m_Board[i, j] = Square.EMPTY;
                    }
                }
            }

            for(int i = (m_BoardSize / 2) - 1; i < (m_BoardSize / 2) + 1; i++)
            {
                for(int j = 0; j < m_BoardSize; j++)
                {
                    m_Board[i, j] = Square.EMPTY;
                }
            }

            for (int i = (m_BoardSize / 2) + 1; i < m_BoardSize; i++)
            {
                for (int j = 0; j < m_BoardSize; j++)
                {
                    aLeagalSquare = (i + j) % 2 == 1;
                    if (aLeagalSquare)
                    {
                        m_Board[i, j] = Square.X;
                    }
                    else
                    {
                        m_Board[i, j] = Square.EMPTY;
                    }
                }
            }
        }

        public void ComputerPlay()
        {
            Random r = new Random();
            while(m_Turn == Square.O)
            {
                fillAllLeagalMoves();
                if (m_UJumpsCounter > 0)
                {
                    int i = r.Next(m_UJumpsCounter);
                    Play(m_UJumpsArr[i]);
                    goto END;
                }

                if (m_OJumpsCounter > 0)
                {
                    int i = r.Next(m_OJumpsCounter);
                    Play(m_OJumpsArr[i]);
                    goto END;
                }

                if (m_UMovesCounter > 0)
                {
                    int i = r.Next(m_UMovesCounter);
                    Play(m_UMovesArr[i]);
                    goto END;
                }

                if (m_OMovesCounter > 0)
                {
                    int i = r.Next(m_OMovesCounter);
                    Play(m_OMovesArr[i]);
                    goto END;
                }

                END:
                {
                }
            }
        }

        public bool Play(Move i_Move)
        {
            bool consecutiveJumpsExist = false;
            Square sourceSquare = m_Board[i_Move.m_SourceRow, i_Move.m_SourceCol];
            bool playIsOK = true;
            if (!m_GameIsOnKeepPlaying)
            {                                    // game has ended before this move
                playIsOK = false;
                goto End;
            }

            if (m_Turn == Square.X && sourceSquare <= 0)
            {                                 // it's X's turn but the source square is not X or K
                playIsOK = false;
                goto End;
            }

            if (m_Turn == Square.O && sourceSquare >= 0)
            {                       // it's O's turn but the source square is not O or U
                playIsOK = false;
                goto End;
            }

            playIsOK = false;
            fillAllLeagalMoves();
            if (m_Turn == Square.X)
            {                                   // (X&K)'s turn
                if (sourceSquare == Square.X)
                {                       // source square is X
                    if (i_Move.isJump())
                    {                           // the move is a jump
                        if (m_XJumpsCounter > 0)
                        {
                            for (int i = 0; i < m_XJumpsCounter; i++)
                            {
                                if (m_XJumpsArr[i].Equals(i_Move))
                                {
                                    playIsOK = true;
                                    makeAMove(i_Move, Square.X);
                                    deleteTheJumpDead(i_Move);
                                    consecutiveJumpsExist = checkForConsecutiveJumps(Square.X, i_Move);
                                }
                            }
                        }
                    }
                    else
                    {               // the move is not a jump
                        if (m_XJumpsCounter == 0 && m_KJumpsCounter == 0)
                        {                                           // if there are no possible jumps
                            if (m_XMovesCounter > 0)
                            {
                                for (int i = 0; i < m_XMovesCounter; i++)
                                {
                                    if (m_XMovesArr[i].Equals(i_Move))
                                    {
                                        playIsOK = true;
                                        makeAMove(i_Move, Square.X);
                                    }
                                }
                            }
                        }
                    }
                }

                if (sourceSquare == Square.K)
                {                                           // source square is K
                    if (i_Move.isJump())
                    {                                                // the move is a jump
                        if (m_KJumpsCounter > 0)
                        {
                            for (int i = 0; i < m_KJumpsCounter; i++)
                            {
                                if (m_KJumpsArr[i].Equals(i_Move))
                                {
                                    playIsOK = true;
                                    makeAMove(i_Move, Square.K);
                                    deleteTheJumpDead(i_Move);
                                    consecutiveJumpsExist = checkForConsecutiveJumps(Square.K, i_Move);
                                }
                            }
                        }
                    }
                    else
                    {                               // the move is not a jump
                        if (m_XJumpsCounter == 0 && m_KJumpsCounter == 0)
                        {                                                  // if there are no possible jumps
                            if (m_KMovesCounter > 0)
                            {
                                for (int i = 0; i < m_KMovesCounter; i++)
                                {
                                    if (m_KMovesArr[i].Equals(i_Move))
                                    {
                                        playIsOK = true;
                                        makeAMove(i_Move, Square.K);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (m_Turn == Square.O)
            {                                                // (O&U)'s turn
                if (sourceSquare == Square.O)
                {                                                // source square is O
                    if (i_Move.isJump())
                    {                                          // the move is a jump
                        if (m_OJumpsCounter > 0)
                        {
                            for (int i = 0; i < m_OJumpsCounter; i++)
                            {
                                if (m_OJumpsArr[i].Equals(i_Move))
                                {
                                    playIsOK = true;
                                    makeAMove(i_Move, Square.O);
                                    deleteTheJumpDead(i_Move);
                                    consecutiveJumpsExist = checkForConsecutiveJumps(Square.O, i_Move);
                                }
                            }
                        }
                    }
                    else
                    {                    // the move is not a jump
                        if (m_OJumpsCounter == 0 && m_UJumpsCounter == 0)
                        {                                                     // if there are no possible jumps
                            if (m_OMovesCounter > 0)
                            {
                                for (int i = 0; i < m_OMovesCounter; i++)
                                {
                                    if (m_OMovesArr[i].Equals(i_Move))
                                    {
                                        playIsOK = true;
                                        makeAMove(i_Move, Square.O);
                                    }
                                }
                            }
                        }
                    }
                }

                if (sourceSquare == Square.U)
                {                                    // source square is U
                    if (i_Move.isJump())
                    {                                   // the move is a jump
                        if (m_UJumpsCounter > 0)
                        {
                            for (int i = 0; i < m_UJumpsCounter; i++)
                            {
                                if (m_UJumpsArr[i].Equals(i_Move))
                                {
                                    playIsOK = true;
                                    makeAMove(i_Move, Square.U);
                                    deleteTheJumpDead(i_Move);
                                    consecutiveJumpsExist = checkForConsecutiveJumps(Square.U, i_Move);
                                }
                            }
                        }
                    }
                    else
                    {                       // the move is not a jump
                        if (m_OJumpsCounter == 0 && m_UJumpsCounter == 0)
                        {                                            // if there are no possible jumps
                            if (m_UMovesCounter > 0)
                            {
                                for (int i = 0; i < m_UMovesCounter; i++)
                                {
                                    if (m_UMovesArr[i].Equals(i_Move))
                                    {
                                        playIsOK = true;
                                        makeAMove(i_Move, Square.U);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (playIsOK && !consecutiveJumpsExist)
            {
                m_Turn = (Square)((int)m_Turn * -1);
                searchForAWinner();
            }

            End:
            return playIsOK;
        }

        private void searchForAWinner()
        {
            fillAllLeagalMoves();
            if (m_Turn > 0)
            {                                            
                if (m_numOfX == 0 && m_numOfK == 0)
                {                                
                    m_GameIsOnKeepPlaying = false;
                }

                if (m_XMovesCounter == 0 && m_XJumpsCounter == 0 && m_KMovesCounter == 0 && m_KJumpsCounter == 0)
                {                                                        // no leagal moves
                    m_GameIsOnKeepPlaying = false;
                }
            }
            else
            {                    // o&u's turn
                if (m_numOfO == 0 && m_numOfU == 0)
                {                                            // no squares
                    m_GameIsOnKeepPlaying = false;
                }

                if (m_OMovesCounter == 0 && m_OJumpsCounter == 0 && m_UMovesCounter == 0 && m_UJumpsCounter == 0)
                {                                               // no leagal moves
                    m_GameIsOnKeepPlaying = false;
                }
            }
        }

        private void deleteTheJumpDead(Move i_Move)
        {
            if (i_Move.m_DestinationRow - i_Move.m_SourceRow < 0)
            {                                                                   // jump up
                if (i_Move.m_DestinationCol - i_Move.m_SourceCol > 0)
                {                                                               // jump right
                    reduceSquareCounter(i_Move.m_SourceRow - 1, i_Move.m_SourceCol + 1);
                    m_Board[i_Move.m_SourceRow - 1, i_Move.m_SourceCol + 1] = Square.EMPTY;
                }
                else
                {                                               // jump left
                    reduceSquareCounter(i_Move.m_SourceRow - 1, i_Move.m_SourceCol - 1);
                    m_Board[i_Move.m_SourceRow - 1, i_Move.m_SourceCol - 1] = Square.EMPTY;
                }
            }
            else
            {                                                                   // jump down
                if (i_Move.m_DestinationCol - i_Move.m_SourceCol > 0)
                {                                                                      // jump right
                    reduceSquareCounter(i_Move.m_SourceRow + 1, i_Move.m_SourceCol + 1);
                    m_Board[i_Move.m_SourceRow + 1, i_Move.m_SourceCol + 1] = Square.EMPTY;
                }
                else
                {                                                           // jump left
                    reduceSquareCounter(i_Move.m_SourceRow + 1, i_Move.m_SourceCol - 1);
                    m_Board[i_Move.m_SourceRow + 1, i_Move.m_SourceCol - 1] = Square.EMPTY;
                }
            }
        }

        private void reduceSquareCounter(int i_Row, int i_Col)
        {
            if (m_Board[i_Row, i_Col] == Square.X)
            {
                m_numOfX--;
            }

            if (m_Board[i_Row, i_Col] == Square.K)
            {
                m_numOfK--;
            }

            if (m_Board[i_Row, i_Col] == Square.O)
            {
                m_numOfO--;
            }

            if (m_Board[i_Row, i_Col] == Square.U)
            {
                m_numOfU--;
            }
        }

        private void makeAMove(Move i_Move, Square i_MoveMaker)
        {
            if (i_MoveMaker == Square.X)
            {
                if (i_Move.m_DestinationRow == 0)
                {                                                                // is there a need for crowning?
                    m_Board[i_Move.m_DestinationRow, i_Move.m_DestinationCol] = Square.K;  // crown
                    m_numOfX--;
                    m_numOfK++;
                    m_Board[i_Move.m_SourceRow, i_Move.m_SourceCol] = Square.EMPTY;
                }
                else
                {
                    makeAMove(i_Move);
                }
            }

            if (i_MoveMaker == Square.O)
            {
                if (i_Move.m_DestinationRow == m_BoardSize - 1)
                {                                                                   // is there a need for crowning?
                    m_Board[i_Move.m_DestinationRow, i_Move.m_DestinationCol] = Square.U;  // crown
                    m_numOfO--;
                    m_numOfU++;
                    m_Board[i_Move.m_SourceRow, i_Move.m_SourceCol] = Square.EMPTY;
                }
                else
                {
                    makeAMove(i_Move);
                }
            }

            if (i_MoveMaker == Square.U || i_MoveMaker == Square.K)
            {
                makeAMove(i_Move);
            }
        }

        private void makeAMove(Move i_Move)
        {
            m_Board[i_Move.m_DestinationRow, i_Move.m_DestinationCol] = m_Board[i_Move.m_SourceRow, i_Move.m_SourceCol];
            m_Board[i_Move.m_SourceRow, i_Move.m_SourceCol] = Square.EMPTY;
        }

        private bool checkForConsecutiveJumps(Square i_Square, Move i_Move)
        {
            bool consecutiveJumpsExist = false;
            if (i_Square == Square.X)
            {
                consecutiveJumpsExist = isJumpUpLeftLeagal(i_Move.m_DestinationRow, i_Move.m_DestinationCol)
                    || isJumpUpRightLeagal(i_Move.m_DestinationRow, i_Move.m_DestinationCol);
            }

            if (i_Square == Square.O)
            {
                consecutiveJumpsExist = isJumpDownLeftLeagal(i_Move.m_DestinationRow, i_Move.m_DestinationCol)
                    || isJumpDownRightLeagal(i_Move.m_DestinationRow, i_Move.m_DestinationCol);
            }

            if (i_Square == Square.K || i_Square == Square.U)
            {
                consecutiveJumpsExist = isJumpUpLeftLeagal(i_Move.m_DestinationRow, i_Move.m_DestinationCol)
                    || isJumpUpRightLeagal(i_Move.m_DestinationRow, i_Move.m_DestinationCol)
                    || isJumpDownLeftLeagal(i_Move.m_DestinationRow, i_Move.m_DestinationCol)
                    || isJumpDownRightLeagal(i_Move.m_DestinationRow, i_Move.m_DestinationCol);
            }

            return consecutiveJumpsExist;
        }

        public void fillAllLeagalMoves()
        {
            m_XMovesCounter = 0;
            m_XJumpsCounter = 0;
            m_KMovesCounter = 0;
            m_KJumpsCounter = 0;
            m_OMovesCounter = 0;
            m_OJumpsCounter = 0;
            m_UMovesCounter = 0;
            m_UJumpsCounter = 0;
            for (int i = 0; i < m_BoardSize; i++)
            {
                for (int j = 0; j < m_BoardSize; j++)
                {
                    bool currentSquareIsX = m_Board[i, j] == Square.X;
                    bool currentSquareIsK = m_Board[i, j] == Square.K;
                    bool currentSquareIsO = m_Board[i, j] == Square.O;
                    bool currentSquareIsU = m_Board[i, j] == Square.U;

                    if (currentSquareIsX)
                    {
                        if (isWalkUpRightLeagal(i, j))
                        {
                            m_XMovesArr[m_XMovesCounter++] = new Move(i, j, i - 1, j + 1);
                        }

                        if (isWalkUpLeftLeagal(i, j))
                        {
                            m_XMovesArr[m_XMovesCounter++] = new Move(i, j, i - 1, j - 1);
                        }

                        if (isJumpUpRightLeagal(i, j))
                        {
                            m_XJumpsArr[m_XJumpsCounter++] = new Move(i, j, i - 2, j + 2);
                        }

                        if (isJumpUpLeftLeagal(i, j))
                        {
                            m_XJumpsArr[m_XJumpsCounter++] = new Move(i, j, i - 2, j - 2);
                        }
                    }

                    if (currentSquareIsK)
                    {
                        if (isWalkUpRightLeagal(i, j))
                        {
                            m_KMovesArr[m_KMovesCounter++] = new Move(i, j, i - 1, j + 1);
                        }

                        if (isWalkUpLeftLeagal(i, j))
                        {
                            m_KMovesArr[m_KMovesCounter++] = new Move(i, j, i - 1, j - 1);
                        }

                        if (isWalkDownRightLeagal(i, j))
                        {
                            m_KMovesArr[m_KMovesCounter++] = new Move(i, j, i + 1, j + 1);
                        }

                        if (isWalkDownLeftLeagal(i, j))
                        {
                            m_KMovesArr[m_KMovesCounter++] = new Move(i, j, i + 1, j - 1);
                        }

                        if (isJumpUpRightLeagal(i, j))
                        {
                            m_KJumpsArr[m_KJumpsCounter++] = new Move(i, j, i - 2, j + 2);
                        }

                        if (isJumpUpLeftLeagal(i, j))
                        {
                            m_KJumpsArr[m_KJumpsCounter++] = new Move(i, j, i - 2, j - 2);
                        }

                        if (isJumpDownRightLeagal(i, j))
                        {
                            m_KJumpsArr[m_KJumpsCounter++] = new Move(i, j, i + 2, j + 2);
                        }

                        if (isJumpDownLeftLeagal(i, j))
                        {
                            m_KJumpsArr[m_KJumpsCounter++] = new Move(i, j, i + 2, j - 2);
                        }
                    }

                    if (currentSquareIsO)
                    {
                        if (isWalkDownRightLeagal(i, j))
                        {
                            m_OMovesArr[m_OMovesCounter++] = new Move(i, j, i + 1, j + 1);
                        }

                        if (isWalkDownLeftLeagal(i, j))
                        {
                            m_OMovesArr[m_OMovesCounter++] = new Move(i, j, i + 1, j - 1);
                        }

                        if (isJumpDownRightLeagal(i, j))
                        {
                            m_OJumpsArr[m_OJumpsCounter++] = new Move(i, j, i + 2, j + 2);
                        }

                        if (isJumpDownLeftLeagal(i, j))
                        {
                            m_OJumpsArr[m_OJumpsCounter++] = new Move(i, j, i + 2, j - 2);
                        }
                    }

                    if (currentSquareIsU)
                    {
                        if (isWalkUpRightLeagal(i, j))
                        {
                            m_UMovesArr[m_UMovesCounter++] = new Move(i, j, i - 1, j + 1);
                        }

                        if (isWalkUpLeftLeagal(i, j))
                        {
                            m_UMovesArr[m_UMovesCounter++] = new Move(i, j, i - 1, j - 1);
                        }

                        if (isWalkDownRightLeagal(i, j))
                        {
                            m_UMovesArr[m_UMovesCounter++] = new Move(i, j, i + 1, j + 1);
                        }

                        if (isWalkDownLeftLeagal(i, j))
                        {
                            m_UMovesArr[m_UMovesCounter++] = new Move(i, j, i + 1, j - 1);
                        }

                        if (isJumpUpRightLeagal(i, j))
                        {
                            m_UJumpsArr[m_UJumpsCounter++] = new Move(i, j, i - 2, j + 2);
                        }

                        if (isJumpUpLeftLeagal(i, j))
                        {
                            m_UJumpsArr[m_UJumpsCounter++] = new Move(i, j, i - 2, j - 2);
                        }

                        if (isJumpDownRightLeagal(i, j))
                        {
                            m_UJumpsArr[m_UJumpsCounter++] = new Move(i, j, i + 2, j + 2);
                        }

                        if (isJumpDownLeftLeagal(i, j))
                        {
                            m_UJumpsArr[m_UJumpsCounter++] = new Move(i, j, i + 2, j - 2);
                        }
                    }
                }
            }
        }

        private bool isWalkUpRightLeagal(int i, int j)
        {
            bool walkUpRightIsOnBoard = isOnBoard(i - 1, j + 1);
            bool isEmptyWalkUpRight = false;
            if (walkUpRightIsOnBoard)
            {
                isEmptyWalkUpRight = m_Board[i - 1, j + 1] == Square.EMPTY;
            }

            return walkUpRightIsOnBoard && isEmptyWalkUpRight;
        }

        private bool isWalkUpLeftLeagal(int i, int j)
        {
            bool walkUpLeftIsOnBoard = isOnBoard(i - 1, j - 1);
            bool isEmptyUpLeft = false;
            if (walkUpLeftIsOnBoard)
            {
                isEmptyUpLeft = m_Board[i - 1, j - 1] == Square.EMPTY;
            }

            return walkUpLeftIsOnBoard && isEmptyUpLeft;
        }

        private bool isWalkDownLeftLeagal(int i, int j)
        {
            bool walkDownLeftIsOnBoard = isOnBoard(i + 1, j - 1);
            bool isEmptyDownLeft = false;
            if (walkDownLeftIsOnBoard)
            {
                isEmptyDownLeft = m_Board[i + 1, j - 1] == Square.EMPTY;
            }

            return walkDownLeftIsOnBoard && isEmptyDownLeft;
        }

        private bool isWalkDownRightLeagal(int i, int j)
        {
            bool walkDownRightIsOnBoard = isOnBoard(i + 1, j + 1);
            bool isEmptyDownRight = false;
            if (walkDownRightIsOnBoard)
            {
                isEmptyDownRight = m_Board[i + 1, j + 1] == Square.EMPTY;
            }

            return walkDownRightIsOnBoard && isEmptyDownRight;
        }

        private bool isJumpUpRightLeagal(int i, int j)
        {
            bool jumpUpRightIsOnBoard = isOnBoard(i - 2, j + 2);
            bool isEmptyJumpUpRight = false;
            bool jumpUpRightKillsTheEnemy = false;
            if (jumpUpRightIsOnBoard)
            {
                isEmptyJumpUpRight = m_Board[i - 2, j + 2] == Square.EMPTY;
                if (isEmptyJumpUpRight)
                {
                    jumpUpRightKillsTheEnemy = ((m_Board[i, j] == Square.X || m_Board[i, j] == Square.K) && (m_Board[i - 1, j + 1] == Square.O || m_Board[i - 1, j + 1] == Square.U)) || (m_Board[i, j] == Square.U && (m_Board[i - 1, j + 1] == Square.X || m_Board[i - 1, j + 1] == Square.K));
                }
            }

            return jumpUpRightIsOnBoard && jumpUpRightKillsTheEnemy && isEmptyJumpUpRight;
        }

        private bool isJumpUpLeftLeagal(int i, int j)
        {
            bool jumpUpLeftIsOnBoard = isOnBoard(i - 2, j - 2);
            bool isEmptyJumpUpLeft = false;
            bool jumpUpLeftKillsTheEnemy = false;
            if (jumpUpLeftIsOnBoard)
            {
                isEmptyJumpUpLeft = m_Board[i - 2, j - 2] == Square.EMPTY;
                if (isEmptyJumpUpLeft)
                {
                    jumpUpLeftKillsTheEnemy = ((m_Board[i, j] == Square.X || m_Board[i, j] == Square.K) && (m_Board[i - 1, j - 1] == Square.O || m_Board[i - 1, j - 1] == Square.U)) || (m_Board[i, j] == Square.U && (m_Board[i - 1, j - 1] == Square.X || m_Board[i - 1, j - 1] == Square.K));
                }
            }

            return jumpUpLeftIsOnBoard && jumpUpLeftKillsTheEnemy && isEmptyJumpUpLeft;
        }

        private bool isJumpDownRightLeagal(int i, int j)
        {
            bool jumpDownRightIsOnBoard = isOnBoard(i + 2, j + 2);
            bool isEmptyJumpDownRight = false;
            bool jumpDownRightKillsTheEnemy = false;
            if (jumpDownRightIsOnBoard)
            {
                isEmptyJumpDownRight = m_Board[i + 2, j + 2] == Square.EMPTY;
                if (isEmptyJumpDownRight)
                {
                    jumpDownRightKillsTheEnemy = ((m_Board[i, j] == Square.O || m_Board[i, j] == Square.U) && (m_Board[i + 1, j + 1] == Square.X || m_Board[i + 1, j + 1] == Square.K)) || (m_Board[i, j] == Square.K && (m_Board[i + 1, j + 1] == Square.O || m_Board[i + 1, j + 1] == Square.U));
                }
            }

            return jumpDownRightIsOnBoard && jumpDownRightKillsTheEnemy && isEmptyJumpDownRight;
        }

        private bool isJumpDownLeftLeagal(int i, int j)
        {
            bool jumpDownLeftIsOnBoard = isOnBoard(i + 2, j - 2);
            bool isEmptyJumpDownLeft = false;
            bool jumpDownLeftKillsTheEnemy = false;
            if (jumpDownLeftIsOnBoard)
            {
                isEmptyJumpDownLeft = m_Board[i + 2, j - 2] == Square.EMPTY;
                if (isEmptyJumpDownLeft)
                {
                    jumpDownLeftKillsTheEnemy = ((m_Board[i, j] == Square.O || m_Board[i, j] == Square.U) && (m_Board[i + 1, j - 1] == Square.X || m_Board[i + 1, j - 1] == Square.K)) || (m_Board[i, j] == Square.K && (m_Board[i + 1, j - 1] == Square.O || m_Board[i + 1, j - 1] == Square.U));
                }
            }

            return jumpDownLeftIsOnBoard && jumpDownLeftKillsTheEnemy && isEmptyJumpDownLeft;
        }

        private bool isOnBoard(int i_Row, int i_Col)
        {
            return i_Row >= 0 && i_Row < m_BoardSize && i_Col >= 0 && i_Col < m_BoardSize;
        }

        public Square Turn
        {
            get
            {
                return m_Turn;
            }
        }

        public bool GameIsOnKeepPlaying
        {
            get
            {
                return m_GameIsOnKeepPlaying;
            }
        }

        public Square Winner   // use only if the game is over
        {
            get
            {
                return (Square)((int)m_Turn * -1);
            }
        }

        public int BoardSize
        {
            get
            {
                return m_BoardSize;
            }
        }

        public int NumberOfX
        {
            get
            {
                return m_numOfX;
            }
        }

        public int NumberOfO
        {
            get
            {
                return m_numOfO;
            }
        }

        public int NumberOfK
        {
            get
            {
                return m_numOfK;
            }
        }

        public int NumberOfU
        {
            get
            {
                return m_numOfU;
            }
        }

        public Square[,] BoardMatrix
        {
            get
            {
                return m_Board;
            }
        }
    }
}
