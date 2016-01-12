using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace _1312210
{

    public class GamePlay
    {
        public long[] Defend_Score; 
        public long[] Attack_Score;
        public bool endGame;
        public int[,] board;
        public int chess;
        public int row, col;

        public GamePlay()
        {
            endGame = false;
            board = new int[12, 12];
            chess = 144;
            Defend_Score = new long[7] { 0, 3, 27, 99, 729, 6561, 59049 };
            Attack_Score = new long[7] { 0, 9, 54, 162, 1458, 13112, 118008 };
            row = 0;
            col = 0; 
        }

        #region Check End Game
        public bool HorizontalCheck(int row, int col, int player)
        {
            int count = 1;
            int tempcol = col;
            int blocked = 0;
            while (true)
            {
                ++tempcol;
                if (tempcol > 11)
                    break;
                if (board[row, tempcol] == player)
                    count++;
                else if (board[row, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            tempcol = col;
            while (true)
            {
                --tempcol;
                if (tempcol < 0)
                    break;
                if (board[row, tempcol] == player)
                    count++;
                else if (board[row, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            if ((blocked == 2 && count < 5) || count < 5)
                return false;
            else
                return true;
        }
        public bool VerticalCheck(int row, int col, int player)
        {
            int count = 1;
            int temprow = row;
            int blocked = 0;
            while (true)
            {
                ++temprow;
                if (temprow > 11)
                    break;
                if (board[temprow, col] == player)
                    count++;
                else if (board[temprow, col] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            temprow = row;
            while (true)
            {
                --temprow;
                if (temprow < 0)
                    break;
                if (board[temprow, col] == player)
                    count++;
                else if (board[temprow, col] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            if ((blocked == 2 && count < 5) || count < 5)
                return false;
            else
                return true;
        }
        public bool DiagonalCheck(int row, int col, int player)
        {
            int count = 1;
            int temprow = row;
            int tempcol = col;
            int blocked = 0;
            while (true)
            {
                ++temprow;
                ++tempcol;
                if (tempcol > 11 || temprow > 11)
                    break;
                if (board[temprow, tempcol] == player)
                    count++;
                else if (board[temprow, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            temprow = row;
            tempcol = col;
            while (true)
            {
                --temprow;
                --tempcol;
                if (tempcol < 0 || temprow < 0)
                    break;
                if (board[temprow, tempcol] == player)
                    count++;
                else if (board[temprow, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }

            if ((blocked == 2 && count < 5) || count < 5)
            {
                count = 1;
                blocked = 0;
                tempcol = col;
                temprow = row;
                while (true)
                {
                    ++temprow;
                    --tempcol;
                    if (temprow > 11 || tempcol < 0)
                        break;
                    if (board[temprow, tempcol] == player)
                        count++;
                    else if (board[temprow, tempcol] == 0)
                        break;
                    else
                    {
                        blocked++;
                        break;
                    }
                }
                temprow = row;
                tempcol = col;
                while (true)
                {
                    --temprow;
                    ++tempcol;
                    if (tempcol > 11 || temprow < 0)
                        break;
                    if (board[temprow, tempcol] == player)
                        count++;
                    else if (board[temprow, tempcol] == 0)
                        break;
                    else
                    {
                        blocked++;
                        break;
                    }
                }

                if ((blocked == 2 && count < 5) || count < 5)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }
        public int IsEndGame(int row, int col, int player)
        {
            if (HorizontalCheck(row, col, player) || VerticalCheck(row, col, player) || DiagonalCheck(row, col, player))
                return 1;
            else if (chess == 0)
                return 2;
            else
                return 0;
        }
        #endregion

        #region AI

        #region Calculate Attack Point
        public long HorizotalAttack(int row, int col, int player)
        {
            int tempcol = col;
            int count = 0;
            int blocked = 0;
            long scoreTemp = 0;
            while (true)
            {
                ++tempcol;
                if (tempcol > 11)
                    break;
                if (board[row, tempcol] == player)
                    count++;
                else if (board[row, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            tempcol = col;
            while (true)
            {
                --tempcol;
                if (tempcol < 0)
                    break;
                if (board[row, tempcol] == player)
                    count++;
                else if (board[row, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            scoreTemp += Attack_Score[count];
            scoreTemp -= Defend_Score[blocked];
            if (blocked == 2)
                return 0;
            else
                return scoreTemp;
        }

        public long VerticalAttack(int row, int col, int player)
        {
            int count = 0;
            int temprow = row;
            int blocked = 0;
            long scoreTemp = 0;
            while (true)
            {
                ++temprow;
                if (temprow > 11)
                    break;
                if (board[temprow, col] == player)
                    count++;
                else if (board[temprow, col] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            temprow = row;
            while (true)
            {
                --temprow;
                if (temprow < 0)
                    break;
                if (board[temprow, col] == player)
                    count++;
                else if (board[temprow, col] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            scoreTemp += Attack_Score[count];
            scoreTemp -= Defend_Score[blocked];
            if (blocked == 2)
                return 0;
            else
                return scoreTemp;
        }

        public long Diagonal1Attack(int row, int col, int player)
        {
            int count = 0;
            int temprow = row;
            int tempcol = col;
            int blocked = 0;
            long scoreTemp = 0;
            while (true)
            {
                ++temprow;
                ++tempcol;
                if (tempcol > 11 || temprow > 11)
                    break;
                if (board[temprow, tempcol] == player)
                    count++;
                else if (board[temprow, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            temprow = row;
            tempcol = col;
            while (true)
            {
                --temprow;
                --tempcol;
                if (tempcol < 0 || temprow < 0)
                    break;
                if (board[temprow, tempcol] == player)
                    count++;
                else if (board[temprow, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }

            scoreTemp += Attack_Score[count];
            scoreTemp -= Defend_Score[blocked];
            if (blocked == 2)
                return 0;
            else
                return scoreTemp;
        }

        public long Diagonal2Attack(int row, int col, int player)
        {
            int count = 0;
            int temprow = row;
            int tempcol = col;
            int blocked = 0;
            long scoreTemp = 0;

            while (true)
            {
                ++temprow;
                --tempcol;
                if (temprow > 11 || tempcol < 0)
                    break;
                if (board[temprow, tempcol] == player)
                    count++;
                else if (board[temprow, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }
            temprow = row;
            tempcol = col;
            while (true)
            {
                --temprow;
                ++tempcol;
                if (tempcol > 11 || temprow < 0)
                    break;
                if (board[temprow, tempcol] == player)
                    count++;
                else if (board[temprow, tempcol] == 0)
                    break;
                else
                {
                    blocked++;
                    break;
                }
            }

            scoreTemp += Attack_Score[count];
            scoreTemp -= Defend_Score[blocked];
            if (blocked == 2)
                return 0;
            else
                return scoreTemp;
        }

        #endregion

        #region Calculate Defend Point
        public long HorizotalDefend(int row, int col, int player)
        {
            int tempcol = col;
            int count = 1;
            int blocked = 0;
            long scoreTemp = 0;
            while (true)
            {
                ++tempcol;
                if (tempcol > 11)
                    break;
                if (board[row, tempcol] == 0)
                    break;
                else if (board[row, tempcol] != player)
                    count++;
                else
                {
                    blocked++;
                    break;
                }
            }
            tempcol = col;
            while (true)
            {
                --tempcol;
                if (tempcol < 0)
                    break;
                if (board[row, tempcol] == 0)
                    break;
                else if (board[row, tempcol] != player)
                    count++;
                else
                {
                    blocked++;
                    break;
                }
            }
            scoreTemp += Defend_Score[count];
            scoreTemp -= Attack_Score[blocked];
            if (blocked == 2)
                return 0;
            else
                return scoreTemp;
        }

        public long VerticalDefend(int row, int col, int player)
        {
            int count = 0;
            int temprow = row;
            int blocked = 0;
            long scoreTemp = 0;
            while (true)
            {
                ++temprow;
                if (temprow > 11)
                    break;
                if (board[temprow, col] == 0)
                    break;
                else if (board[temprow, col] != player)
                    count++;
                else
                {
                    blocked++;
                    break;
                }
            }
            temprow = row;
            while (true)
            {
                --temprow;
                if (temprow < 0)
                    break;
                if (board[temprow, col] == 0)
                    break;
                else if (board[temprow, col] != player)
                    count++;
                else
                {
                    blocked++;
                    break;
                }
            }
            scoreTemp += Defend_Score[count];
            scoreTemp -= Attack_Score[blocked];
            if (blocked == 2)
                return 0;
            else
                return scoreTemp;
        }

        public long Diagonal1Defend(int row, int col, int player)
        {
            int count = 0;
            int temprow = row;
            int tempcol = col;
            int blocked = 0;
            long scoreTemp = 0;
            while (true)
            {
                ++temprow;
                ++tempcol;
                if (tempcol > 11 || temprow > 11)
                    break;
                if (board[temprow, tempcol] == 0)
                    break;
                else if (board[temprow, tempcol] != player)
                    count++;
                else
                {
                    blocked++;
                    break;
                }
            }
            temprow = row;
            tempcol = col;
            while (true)
            {
                --temprow;
                --tempcol;
                if (tempcol < 0 || temprow < 0)
                    break;
                if (board[temprow, tempcol] == 0)
                    break;
                else if (board[temprow, tempcol] != player)
                    count++;
                else
                {
                    blocked++;
                    break;
                }
            }

            scoreTemp += Defend_Score[count];
            scoreTemp -= Attack_Score[blocked];
            if (blocked == 2)
                return 0;
            else
                return scoreTemp;
        }

        public long Diagonal2Defend(int row, int col, int player)
        {
            int count = 0;
            int temprow = row;
            int tempcol = col;
            int blocked = 0;
            long scoreTemp = 0;
            while (true)
            {
                ++temprow;
                --tempcol;
                if (temprow > 11 || tempcol < 0)
                    break;
                if (board[temprow, tempcol] == 0)
                    break;
                else if (board[temprow, tempcol] != player)
                    count++;
                else
                {
                    blocked++;
                    break;
                }
            }
            temprow = row;
            tempcol = col;
            while (true)
            {
                --temprow;
                ++tempcol;
                if (tempcol > 11 || temprow < 0)
                    break;
                if (board[temprow, tempcol] == 0)
                    break;
                else if (board[temprow, tempcol] != player)
                    count++;
                else
                {
                    blocked++;
                    break;
                }
            }

            scoreTemp += Defend_Score[count];
            scoreTemp -= Attack_Score[blocked];
            if (blocked == 2)
                return 0;
            else
                return scoreTemp;
        }
        #endregion

        public void ChooseChess()
        {
            int player = 2;
            long score = 0;
            col = 0;
            row = 0;
            for(int i = 0; i < 12; i++)
            {
                for(int j = 0; j < 12; j++)
                {
                    long attack = 0, defend = 0;
                    if(board[i,j] == 0)
                    {
                        attack += HorizotalAttack(i, j, player);
                        attack += VerticalAttack(i, j, player);
                        attack += Diagonal1Attack(i, j, player);
                        attack += Diagonal2Attack(i, j, player);

                        defend += HorizotalDefend(i, j, player);
                        defend += VerticalDefend(i, j, player);
                        defend += Diagonal1Defend(i, j, player);
                        defend += Diagonal2Defend(i, j, player);

                        if(defend < attack)
                        {
                            if(score < attack)
                            {
                                score = attack;
                                row = i;
                                col = j;
                            }
                        }
                        else
                        {
                            if (score < defend)
                            {
                                score = defend;
                                row = i;
                                col = j;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
