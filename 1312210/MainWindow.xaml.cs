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
using Quobject.SocketIoClientDotNet.Client;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace _1312210
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string playerName = "Guest";
        int player = 1;
        double chessHeight;
        GamePlay gp = new GamePlay();
        List<List<Button>> btnList;
        public int choosedRow = 0, choosedCol = 0;
        Socket socket;
        bool isError = false;
        Thread thread;
        string[] botChat = { "This is auto BOT, nice to play with you ^^!", "What a game!!!!", "Nice!!!!", "Well played!", "Good Game!!!", "You are good!!!", "I like your style ^^!", "am I good?", "Not bad!!!", "Have fun!!!"};
        Random rd = new Random();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateBoard();
            //Play with BOT Offline
            if(ChosingMode.mode == 1)
            {
                MessageBoxResult rs = MessageBox.Show("Do you want to go first?", "Option", MessageBoxButton.YesNo);
                if (rs == MessageBoxResult.Yes)
                    player = 1;
                else if(rs == MessageBoxResult.No)
                {
                    choosedRow = 5;
                    choosedCol = 5;
                    btnList[choosedRow][choosedCol].Content = AddChess(choosedRow, choosedCol, 2);
                    gp.board[choosedRow,choosedCol] = 2;
                    --gp.chess;
                }           
            }
            //Play online with other
            else if(ChosingMode.mode == 3)
            {
                if (socket != null && thread != null)
                {
                    socket.Close();
                    thread.Interrupt();
                }
                thread = new Thread(ListenData);
                thread.IsBackground = true;
                thread.Start();
            }
            //Use Bot to play online
            else if(ChosingMode.mode == 4)
            {
                playerName = "BOT Mon";
                if (socket != null && thread != null)
                {
                    socket.Close();
                    thread.Interrupt();
                }
                thread = new Thread(ListenData);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        #region Button Click

        //Click on a chess
        private void newbtn_Click(object sender, RoutedEventArgs e)
        {
            if(ChosingMode.mode == 2)
            {
                string temp = (sender as Button).Tag.ToString();
                string[] temp1 = temp.Split(':');
                int colTemp = Convert.ToInt32(temp1[1]) - 1;
                int rowTemp = Convert.ToInt32(temp1[0]) - 1;
                if (gp.board[rowTemp, colTemp] != 0)
                {
                    MessageBox.Show("This chess is checked");
                    return;
                }
                (sender as Button).Content = AddChess(rowTemp, colTemp, player);
                gp.board[rowTemp, colTemp] = player;
                --gp.chess;

                if (gp.IsEndGame(rowTemp, colTemp, player) == 1)
                {
                    MessageBox.Show(string.Format("Player {0} won the game", player));
                    this.Close();
                }
                else if (gp.IsEndGame(rowTemp, colTemp, player) == 2)
                {
                    MessageBox.Show("Draw");
                    this.Close();
                }
                ChangePlayer(ref player);   
            }
            else if(ChosingMode.mode == 1)
            {
                string temp = (sender as Button).Tag.ToString();
                string[] temp1 = temp.Split(':');
                int colTemp = Convert.ToInt32(temp1[1]) - 1;
                int rowTemp = Convert.ToInt32(temp1[0]) - 1;
                if (gp.board[rowTemp, colTemp] != 0)
                {
                    MessageBox.Show("This chess is checked");
                    return;
                }

                (sender as Button).Content = AddChess(rowTemp, colTemp, player);
                gp.board[rowTemp, colTemp] = player;
                --gp.chess;

                if (gp.IsEndGame(rowTemp, colTemp, 1) == 1)
                {
                    MessageBox.Show("You won the game");
                    this.Close();
                }
                else if(gp.IsEndGame(rowTemp, colTemp, 1) == 2)
                {
                    MessageBox.Show("Draw");
                    this.Close();
                }

                Thread aiThread = new Thread(new ThreadStart(gp.ChooseChess));
                aiThread.Start();
                Thread.Sleep(10);
                choosedCol = gp.col;
                choosedRow = gp.row;
                btnList[choosedRow][choosedCol].Content = AddChess(choosedRow, choosedCol, 2);
                gp.board[choosedRow, choosedCol] = 2;
                --gp.chess;

                if (gp.IsEndGame(choosedRow, choosedCol, 2) == 1)
                {
                    MessageBox.Show("PC won the game");
                    this.Close();
                }
                else if (gp.IsEndGame(choosedRow, choosedCol, 2) == 2)
                {
                    MessageBox.Show("Draw");
                    this.Close();
                }                
            }
            else if(ChosingMode.mode == 3)
            {
                string temp = (sender as Button).Tag.ToString();
                string[] temp1 = temp.Split(':');
                int colTemp = Convert.ToInt32(temp1[1]) - 1;
                int rowTemp = Convert.ToInt32(temp1[0]) - 1;
                if (gp.board[rowTemp, colTemp] != 0)
                {
                    MessageBox.Show("This chess is checked");
                    return;
                }
                socket.Emit("MyStepIs", JObject.FromObject(new { row = rowTemp, col = colTemp }));
            }
        }

        //Change name
        private void btn_name_Click(object sender, RoutedEventArgs e)
        {
            playerName = "";
            playerName = tb_name.Text.ToString();
            if (playerName != "")
            {
                MessageBox.Show("You have changed your name to: " + playerName);
            }
            else
                playerName = "Guest";
            if(ChosingMode.mode == 3 || ChosingMode.mode == 4)
                socket.Emit("MyNameIs", playerName);
        }

        //Send message by button
        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            Chat();
        }

        //Send message by press Enter key on keyboard
        private void tb_message_TextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Chat();
            }
        }
        #endregion

        #region Recieve Data
        public void ListenData()
        {
            string serverAddress = ConfigurationManager.AppSettings["ServerAddress"];
            socket = IO.Socket(serverAddress);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Dispatcher.Invoke(() =>
                {
                });
            });
            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                Dispatcher.Invoke(() =>
                {
                    rtb_conversation.AppendText(((Newtonsoft.Json.Linq.JObject)data).ToString() + Environment.NewLine);
                });
            });
            socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                if (!isError)
                {
                    Dispatcher.Invoke(() =>
                    {
                        rtb_conversation.AppendText("Cannot connect to SERVER" + Environment.NewLine);
                    });
                    isError = true;
                }
            });

            socket.On(Socket.EVENT_ERROR, (data) =>
            {
                Dispatcher.Invoke(() =>
                {
                    rtb_conversation.AppendText(((Newtonsoft.Json.Linq.JObject)data).ToString() + Environment.NewLine);
                });
            });


            socket.On("ChatMessage", (data) =>
            {
                string message = ((Newtonsoft.Json.Linq.JObject)data)["message"].ToString();
                string timeNow = DateTime.Now.ToString("HH:mm:ss tt");

                if (message == "Welcome!")
                {
                    Dispatcher.Invoke(() =>
                    {
                        rtb_conversation.AppendText(string.Format("Server - {0}", DateTime.Now.ToString("HH:mm:ss tt")) + Environment.NewLine);
                        rtb_conversation.AppendText(string.Format("{0}", message) + Environment.NewLine);
                        rtb_conversation.AppendText("--------------------------------------------------------" + Environment.NewLine);
                    });

                    if (playerName != "Guest")
                    {
                        socket.Emit("MyNameIs", playerName);
                    }
                    socket.Emit("ConnectToOtherPlayer");

                }
                //Message which annouces player connect to server and recieve turn
                else if (message.Contains("<br />"))
                {
                    int index = message.IndexOf("<br />");
                    string str1 = message.Substring(0, index);
                    string str2 = message.Substring(index + 6);
                    Dispatcher.Invoke(() =>
                    {
                        rtb_conversation.AppendText(string.Format("Server - {0}", DateTime.Now.ToString("HH:mm:ss tt")) + Environment.NewLine);
                        rtb_conversation.AppendText(str1 + Environment.NewLine);
                        rtb_conversation.AppendText(str2 + Environment.NewLine);
                        rtb_conversation.AppendText("--------------------------------------------------------" + Environment.NewLine);
                        rtb_conversation.ScrollToEnd();
                    });
                    //If BOT online and have first turn, lets it plays at [5,5]
                    if (ChosingMode.mode == 4)
                    {
                        socket.Emit("ChatMessage", botChat[0]);
                        if (str2 != "You are the second player!")
                        {
                            choosedRow = 5;
                            choosedCol = 5;
                            socket.Emit("MyStepIs", JObject.FromObject(new { row = 5, col = 5 }));
                        }
                    }

                }
                //Message from other player
                else if (((Newtonsoft.Json.Linq.JObject)data).Count > 1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        rtb_conversation.AppendText(string.Format("{0} - {1}", ((Newtonsoft.Json.Linq.JObject)data)["from"].ToString(), DateTime.Now.ToString("HH:mm:ss tt")) + Environment.NewLine);
                        rtb_conversation.AppendText(string.Format("{0}", message) + Environment.NewLine);
                        rtb_conversation.AppendText("--------------------------------------------------------" + Environment.NewLine);
                        rtb_conversation.ScrollToEnd();
                    });
                }
                //Message from server
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        rtb_conversation.AppendText(string.Format("Server - {0}", DateTime.Now.ToString("HH:mm:ss tt")) + Environment.NewLine);
                        rtb_conversation.AppendText(string.Format("{0}", message) + Environment.NewLine);
                        rtb_conversation.AppendText("--------------------------------------------------------" + Environment.NewLine);
                        rtb_conversation.ScrollToEnd();
                    });
                }
            });

            //End of round
            socket.On("EndGame", (data) =>
            {
                Dispatcher.Invoke(() =>
                {
                    string message = ((Newtonsoft.Json.Linq.JObject)data)["message"].ToString();
                    rtb_conversation.AppendText(string.Format("Server - {0}", DateTime.Now.ToString("HH:mm:ss tt")) + Environment.NewLine);
                    rtb_conversation.AppendText(string.Format("{0}", message) + Environment.NewLine);
                    rtb_conversation.AppendText("--------------------------------------------------------" + Environment.NewLine);
                    rtb_conversation.ScrollToEnd();
                    MessageBox.Show(message);
                    thread.Interrupt();
                    socket.Close();
                    this.Close();
                });
            });


            //Recieve game info
            socket.On("NextStepIs", (data) =>
            {
                int player = int.Parse(((Newtonsoft.Json.Linq.JObject)data)["player"].ToString()) + 1;
                int row = int.Parse(((Newtonsoft.Json.Linq.JObject)data)["row"].ToString());
                int col = int.Parse(((Newtonsoft.Json.Linq.JObject)data)["col"].ToString());
                //player = 1: client turn
                if (player == 1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        btnList[row][col].Content = AddChess(row, col, player);
                        gp.board[row, col] = player;                      
                    });
                }
                //player = 2 other turn
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        btnList[row][col].Content = AddChess(row, col, player);
                        gp.board[row, col] = player;                        
                    });
                    if (ChosingMode.mode == 4)
                        {
                            Thread aiThread = new Thread(new ThreadStart(gp.ChooseChess));
                            aiThread.Start();
                            Thread.Sleep(10);
                            choosedCol = gp.col;
                            choosedRow = gp.row;
                            gp.board[choosedRow, choosedCol] = 2;
                            socket.Emit("MyStepIs", JObject.FromObject(new { row = choosedRow, col = choosedCol }));
                            socket.Emit("ChatMessage", botChat[rd.Next(1, botChat.Count())] );
                        }
                    
                }
            });
        }
        #endregion

        #region function
        private void Chat()
        {
            string msg = tb_message.Text.ToString().Trim();
            if (msg != "")
            {         
                if (ChosingMode.mode == 3 || ChosingMode.mode == 4)
                {
                    socket.Emit("ChatMessage", msg);
                }
                else
                {
                    rtb_conversation.AppendText(string.Format("{0} - {1}", playerName, DateTime.Now.ToString("HH:mm:ss tt")) + Environment.NewLine);
                    rtb_conversation.AppendText(string.Format("{0}", msg) + Environment.NewLine);
                    rtb_conversation.AppendText("--------------------------------------------------------" + Environment.NewLine);
                }
                msg = "";
                tb_message.Clear();
                rtb_conversation.ScrollToEnd();         
            }
        }

        private void CreateBoard()
        {
            if (btnList != null)
            {
                for (int i = 0; i < btnList.Count(); i++)
                {
                    btnList[i].Clear();
                }
                btnList.Clear();
            }       
            btnList = new List<List<Button>>();
            double height = boardCanvas.ActualHeight / 12;
            chessHeight = height * 0.8;
            double topTemp = 0, leftTemp = 0;
            for (int i = 1; i <= 12; i++)
            {
                List<Button> tempBtnList = new List<Button>();
                for (int j = 1; j <= 12; j++)
                {
                    Button newbtn = new Button();
                    newbtn.Height = newbtn.Width = height;
                    newbtn.Tag = i.ToString() + " : " + j.ToString();
                    Canvas.SetTop(newbtn, topTemp);
                    Canvas.SetLeft(newbtn, leftTemp);
                    newbtn.Click += new RoutedEventHandler(newbtn_Click);
                    leftTemp += height;
                    boardCanvas.Children.Add(newbtn);
                    tempBtnList.Add(newbtn);
                    gp.board[i - 1, j - 1] = 0;

                    if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                    {
                        SolidColorBrush cl = new SolidColorBrush();
                        cl.Color = Color.FromRgb(226, 226, 226);
                        newbtn.Background = cl;
                    }
                    else
                    {
                        SolidColorBrush cl = new SolidColorBrush();
                        cl.Color = Color.FromRgb(243, 243, 243);
                        newbtn.Background = cl;
                    }
                }
                leftTemp = 0;
                topTemp += height;
                btnList.Add(tempBtnList);
            }
        }

        private Image AddChess(int row, int col, int player)
        {
            BitmapImage bmg = new BitmapImage();
            bmg.BeginInit();
            if(col % 2 == 0 && row % 2 == 0 || col % 2 == 1 && row % 2 == 1)
            {
                if(player == 1)
                    bmg.UriSource = new Uri(@"Resources/black1.png", UriKind.RelativeOrAbsolute);
                else
                    bmg.UriSource = new Uri(@"Resources/white2.png", UriKind.RelativeOrAbsolute);
            }
            else
            {
                if (player == 1)
                    bmg.UriSource = new Uri(@"Resources/black2.png", UriKind.RelativeOrAbsolute);
                else
                    bmg.UriSource = new Uri(@"Resources/white1.png", UriKind.RelativeOrAbsolute);
            }
            bmg.EndInit();
            Image img = new Image();
            img.Stretch = Stretch.Fill;
            img.Height = img.Width = chessHeight;
            img.Source = bmg;
            return img;
        }

        private void ChangePlayer(ref int player)
        {
            if (player == 1)
                player = 2;
            else
                player = 1;
        }
        #endregion
    }
}
