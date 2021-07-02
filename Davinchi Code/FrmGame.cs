using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;


namespace Davinchi_Code
{
    public partial class FrmGame : Form
    {
        // picture box를 저장할 array list
        private PictureBox[] pic1 = new PictureBox[14];
        private PictureBox[] pic2 = new PictureBox[14];
        private PictureBox[] pic3 = new PictureBox[14];
        private PictureBox[] pic4 = new PictureBox[14];
        private Label[] NickName = new Label[4];
        private Label[] lblO = new Label[14];

        // 화면 해상도 변수들
        private int Xsize = Screen.PrimaryScreen.Bounds.Width;
        private int Ysize = Screen.PrimaryScreen.Bounds.Height;

        // 필요 변수들
        private bool star; // 시작했는지 여부
        private int mode; // 0 : 바닥 모드, 1 : 일반 모드
        private int playerNum; // 전체 플레이어 인원 수
        private int MyNum; // Socket용 Player Number
        private string[] PlayerNick; // 플레이어들 닉네임 저장용
        private int Turn; // 차례
        private string TurnPhase; // Turn 차례
        private int restB, restW; // 남은 검정 카드 개수랑 하얀 카드 개수

        public int getB()
        {
            return restB;
        }

        public int getW()
        {
            return restW;
        }

        public int getmNum()
        {
            return MyNum;
        }

        private string[ , ] Placard; // 각 플레이어별 판
        private int[] PlaCardVal; // 각 플레이어별 남은 카드 수
        private bool[] PlayerLive; // 죽었니 살았니
        private string iscanpass; // Pass 가능?
        private string Nick; // 내 닉네임
        private int TheAnswerCo; // 실제 정답
        private int AnswerClickIndex; // 클릭된 인덱스
        private Answer ansClass; // 정답 입력받는 클래스
        private bool chat;
        private BackgroundWorker worker;

        // 소케케케케케케케케케켓
        IPEndPoint clientPoint; // client point 설정
        IPEndPoint serverPoint; // server point 설정
        NetworkStream stream;
        TcpClient client;
        sendClass sC;
        string sendingMessage;
        string RE = ""; // Response 받을 변수

        void worker_DoWork(object sender, DoWorkEventArgs e) // 듣는 쓰레드
        {
            chat = false;

            byte[] data = new byte[256]; // data : 수신용 버퍼

            int bytes; // 사이즈
            while ((bytes = stream.Read(data, 0, data.Length)) != 0) // stream으로부터 읽어옴.
            {
                RE = Encoding.Default.GetString(data, 0, bytes).Split(' ')[0];
                worker.ReportProgress(0);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            String[] firstsplit;
            firstsplit = RE.Split("!^#".ToCharArray());
            string[] dc;


            for (int k = 0; k < firstsplit.Length; k++)
            {
                RE = firstsplit[k];
                Console.Write(RE);
                if (RE.Contains("%"))
                {
                    dc = RE.Split('%');
                    if (dc[0].Equals("DC"))
                    {
                        if (dc[1].Equals("start"))
                        {
                            TxtLog.Visible = true;
                            TxtAllChat.Visible = true;
                            TxtChat.Visible = true;
                            playerNum = Convert.ToInt32(dc[2]);
                            CmdCardSort.Enabled = true;
                            CmdCardSort.Text = "카드 뽑기";
                            star = true;
                            picArr(playerNum);
                            lblWait.Visible = false;
                            LstPlayer.Visible = false;
                            PlayerNick = new string[playerNum];
                            Placard = new string[playerNum, 14];
                            PlaCardVal = new int[playerNum];
                            PlayerLive = new bool[playerNum];
                        }
                        else if (dc[1].Equals("myindex"))
                        {
                            MyNum = Convert.ToInt32(dc[2]);
                            NickName[0].Text = PlayerNick[MyNum];
                            NickName[1].Text = PlayerNick[(MyNum + 1) % playerNum];
                            if (playerNum == 3)
                                NickName[2].Text = PlayerNick[(MyNum + 2) % playerNum];
                            else if (playerNum == 4)
                            {
                                NickName[2].Text = PlayerNick[(MyNum + 2) % playerNum];
                                NickName[3].Text = PlayerNick[(MyNum + 3) % playerNum];
                            }
                        }
                        else if (dc[1].Equals("myturn") || dc[1].Equals("otherturn"))
                            Turn = Convert.ToInt32(dc[2]) % playerNum;
                        else if (dc[1].Equals("turnindex"))
                            TurnPhase = dc[2];
                        else if (dc[1].Equals("remainBlack"))
                            restB = Convert.ToInt32(dc[2]);
                        else if (dc[1].Equals("remainWhite"))
                            restW = Convert.ToInt32(dc[2]);
                        else if (dc[1].Equals("card"))
                        {
                            PlaCardVal[Convert.ToInt32(dc[2])] = dc.Length - 3;
                            for (int j = 3; j < dc.Length; j++)
                                Placard[Convert.ToInt32(dc[2]), j - 3] = dc[j];
                        }
                        else if (dc[1].Equals("dead"))
                            PlayerLive[Convert.ToInt32(dc[2])] = false;
                        else if (dc[1].Equals("iscanpass"))
                            iscanpass = dc[2];
                        else if (dc[1].Equals("chat"))
                        {
                            MessageBox.Show(RE);
                            TxtAllChat.Text = TxtAllChat.Text + PlayerNick[Convert.ToInt32(dc[2])] + " : " + dc[3] + "\r\n";
                            chat = true;
                        }
                        else if (dc[1].Equals("end"))
                        {
                            if (MyNum == Convert.ToInt32(dc[2]))
                                MessageBox.Show("ㅊㅊ 이김 좋음?");
                            else
                                MessageBox.Show("ㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋ졌음 3년은 더 하고 오셔야될듯");
                            stream.Close();
                            client.Close();
                            this.Close();
                        }
                        else if (dc[1].Equals("error"))
                        {
                            MessageBox.Show("인원이 맞지 않습니다.");
                            return;
                        }
                        else if (dc[1].Equals("cardgot"))
                            TxtLog.Text = TxtLog.Text + "\r\n" + PlayerNick[Convert.ToInt32(dc[2])] + "님이 " + dc[3] + "카드를 가져오셨습니다. 위치는" + (Convert.ToInt32(dc[4]) + 1).ToString() + "번째입니다." + "\r\n";
                        else if (dc[1].Equals("cardchoice"))
                            TxtLog.Text = TxtLog.Text + "\r\n" + PlayerNick[Convert.ToInt32(dc[2])] + "님이 " + PlayerNick[(Convert.ToInt32(dc[2]) + Convert.ToInt32(dc[3])) % playerNum].ToString() + "님의 " + (Convert.ToInt32(dc[4]) + 1).ToString() + "번쨰 카드가" + dc[5] + "색깔의 " + dc[6] + "이라고 공격하셨습니다.\r\n";
                        else if (dc[1].Equals("nick"))
                        {
                            LstPlayer.Items.Clear();
                            for (int j = 2; j < dc.Length; j++)
                                LstPlayer.Items.Add(dc[j]);
                        }
                        else if (dc[1].Equals("inxnick"))
                            PlayerNick[Convert.ToInt32(dc[2])] = dc[3];
                    }

                }
            }

            if (star)
            {
                string TurnDir;
                if (Turn % playerNum == MyNum % playerNum)
                    TurnDir = "▼";
                else
                {
                    if (playerNum == 4)
                    {
                        if (Turn % playerNum == (MyNum + 1) % playerNum)
                            TurnDir = "▶";
                        else if (Turn % playerNum == (MyNum + 2) % playerNum)
                            TurnDir = "▲";
                        else
                            TurnDir = "◀";
                    }
                    else if (playerNum == 3)
                    {
                        if (Turn % playerNum == (MyNum + 1) % playerNum)
                            TurnDir = "▶";
                        else
                            TurnDir = "◀";
                    }
                    else
                        TurnDir = "▲";
                }

                if (TurnPhase != null)
                {
                    if (TurnPhase.Equals("CARDGET"))
                        CmdCardSort.Text = "카드를 뽑아 주세요\r\nW : " + restW + ", B : " + restB;
                    else if (TurnPhase.Equals("SELECT"))
                        CmdCardSort.Text = "상대의 카드를 맞춰 주세요";
                }

                if (restW + restB <= 0)
                    CmdCardSort.Text = "더이상 카드가 남아있지 않습니다.";

                CmdCardSort.Text = CmdCardSort.Text + "\r\n" + "Player = " + TurnDir;
                if (!chat)
                    getPicture();
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {


        }


        public FrmGame()
        {
            InitializeComponent();
            clientPoint = new IPEndPoint(IPAddress.Parse(internetComponent.getMyIP()), 0); // client 정보 bind
            serverPoint = new IPEndPoint(IPAddress.Parse(internetComponent.getServerIP()), 7788); // server 정보 bind
            initializecomponent();
            resize();
            netInit();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();

            /*
            Thread thrListen = new Thread(new ThreadStart(() => init()));
            thrListen.IsBackground = true;
            thrListen.Start();
            */

            Thread thrSend = new Thread(new ThreadStart(() =>
            {
                sC = new sendClass();
                initSend();
            }));

            thrSend.IsBackground = true;
            thrSend.Start();

            CheckForIllegalCrossThreadCalls = false;
        }

        private void netInit()
        {
            client = new TcpClient(clientPoint); // client 초기화
            client.Connect(serverPoint); // server에 연결
            stream = client.GetStream(); // Client Stream 가져오기
        }

        private void initSend()
        {
            while(true)
            {
                if (sendingMessage != null && !sendingMessage.Equals(""))
                {
                    sendMessage(stream, sendingMessage);
                    sendingMessage = "";
                }
            }
        }

        private void init() // 듣기 전용
        {
            
        }

        private void getPicture()
        {
            for(int i = 0; i < 14; i++)
            {
                pic1[i].Image = null;
                pic2[i].Image = null;
                pic3[i].Image = null;
                pic4[i].Image = null;
            }

            for(int i = 0; i < 14; i++)
                lblO[i].Visible = false;

            Application.DoEvents();

            for(int i = 0; i < playerNum; i++) // i = 플레이어 번호
            {
                for(int j = 0; j < PlaCardVal[(MyNum + i) % playerNum]; j++) // j = 카드 자리번호
                {
                    string card = Placard[(MyNum+i) % playerNum, j];
                    if (card.Length > 1)
                    {
                        if (card.Contains("O"))
                        {
                            card = parseLeft(card, card.Length - 1);
                            lblO[j].Visible = true;
                        }

                        if (parseLeft(card, 1).Equals("B"))
                        {
                            if (i == 0)
                                pic1[j].ImageLocation = Application.StartupPath + "\\img\\" + (Convert.ToInt32(parseRight(card, card.Length - 1)) * 2).ToString() + ".jpg";
                            else if (i == 1)
                                pic2[j].ImageLocation = Application.StartupPath + "\\img\\" + (Convert.ToInt32(parseRight(card, card.Length - 1)) * 2).ToString() + ".jpg";
                            else if (i == 2)
                                pic3[j].ImageLocation = Application.StartupPath + "\\img\\" + (Convert.ToInt32(parseRight(card, card.Length - 1)) * 2).ToString() + ".jpg";
                            else if (i == 3)
                                pic4[j].ImageLocation = Application.StartupPath + "\\img\\" + (Convert.ToInt32(parseRight(card, card.Length - 1)) * 2).ToString() + ".jpg";
                        }
                        else if (parseLeft(card, 1).Equals("W"))
                        {
                            if (i == 0)
                                pic1[j].ImageLocation = Application.StartupPath + "\\img\\" + (Convert.ToInt32(parseRight(card, card.Length - 1)) * 2 + 1).ToString() + ".jpg";
                            else if (i == 1)
                                pic2[j].ImageLocation = Application.StartupPath + "\\img\\" + (Convert.ToInt32(parseRight(card, card.Length - 1)) * 2 + 1).ToString() + ".jpg";
                            else if (i == 2)
                                pic3[j].ImageLocation = Application.StartupPath + "\\img\\" + (Convert.ToInt32(parseRight(card, card.Length - 1)) * 2 + 1).ToString() + ".jpg";
                            else if (i == 3)
                                pic4[j].ImageLocation = Application.StartupPath + "\\img\\" + (Convert.ToInt32(parseRight(card, card.Length - 1)) * 2 + 1).ToString() + ".jpg";
                        }
                    }
                    else
                    {
                        if(card.Equals("B"))
                        {
                            if(i == 0)
                                pic1[j].ImageLocation = Application.StartupPath + "\\img\\B.jpg";
                            else if (i == 1)
                                pic2[j].ImageLocation = Application.StartupPath + "\\img\\B.jpg";
                            else if (i == 2)
                                pic3[j].ImageLocation = Application.StartupPath + "\\img\\B.jpg";
                            else if (i == 3)
                                pic4[j].ImageLocation = Application.StartupPath + "\\img\\B.jpg";
                        }
                        else if(card.Equals("W"))
                        {
                            if(i == 0)
                                pic1[j].ImageLocation = Application.StartupPath + "\\img\\W.jpg";
                            else if (i == 1)
                                pic2[j].ImageLocation = Application.StartupPath + "\\img\\W.jpg";
                            else if (i == 2)
                                pic3[j].ImageLocation = Application.StartupPath + "\\img\\W.jpg";
                            else if (i == 3)
                                pic4[j].ImageLocation = Application.StartupPath + "\\img\\W.jpg";
                        }
                    }
                }
            }
        }

        private string parseLeft(string text, int textLength)
        {
            if (text.Length < textLength)
                textLength = text.Length;

            return text.Substring(0, textLength);
        }

        private string parseRight(string text, int textLength)
        {
            if(text.Length < textLength)
                textLength = text.Length;

            return text.Substring(text.Length-textLength, textLength);
        }

        private void sendMessage(NetworkStream stream, String data) // 메시지를 전송하는 메소드
        {
            sC.setMsg(stream, data);
            sC.sendMessage();
        }
        
        private void picArr(int num)
        {
            lock(this)
            {
                switch(num)
                {
                    case 2:
                        for (int i = 0; i < pic2.Length; i++)
                        {
                            pic2[i].Left = pic3[i].Left;
                            pic2[i].Top = pic3[i].Top;
                            NickName[1].Left = NickName[2].Left;
                            NickName[1].Top = NickName[2].Top;
                        }
                        break;
                    case 3:
                        for (int i = 0; i < pic2.Length; i++)
                        {
                            pic3[i].Left = pic4[i].Left;
                            pic3[i].Top = pic4[i].Top;
                            NickName[2].Left = NickName[3].Left;
                            NickName[2].Top = NickName[3].Top;
                            pic3[i].Visible = true;
                        }
                        break;
                    case 4:
                        for (int i = 0; i < pic2.Length; i++)
                        {
                            pic3[i].Visible = true;
                            pic4[i].Visible = true;
                        }
                        break;
                    default:
                        MessageBox.Show("오지레기", "오지레기");
                        break;
                }
            }
        }

        private void initializecomponent() // Component들 세팅 부분
        {
            // pic1 add
            pic1[0] = picPla11;
            pic1[1] = picPla12;
            pic1[2] = picPla13;
            pic1[3] = picPla14;
            pic1[4] = picPla15;
            pic1[5] = picPla16;
            pic1[6] = picPla17;
            pic1[7] = picPla18;
            pic1[8] = picPla19;
            pic1[9] = picPla110;
            pic1[10] = picPla111;
            pic1[11] = picPla112;
            pic1[12] = picPla113;
            pic1[13] = picPla114;

            // pic2 add
            pic2[0] = picPla21;
            pic2[1] = picPla22;
            pic2[2] = picPla23;
            pic2[3] = picPla24;
            pic2[4] = picPla25;
            pic2[5] = picPla26;
            pic2[6] = picPla27;
            pic2[7] = picPla28;
            pic2[8] = picPla29;
            pic2[9] = picPla210;
            pic2[10] = picPla211;
            pic2[11] = picPla212;
            pic2[12] = picPla213;
            pic2[13] = picPla214;

            // pic4 add
            pic3[0] = picPla31;
            pic3[1] = picPla32;
            pic3[2] = picPla33;
            pic3[3] = picPla34;
            pic3[4] = picPla35;
            pic3[5] = picPla36;
            pic3[6] = picPla37;
            pic3[7] = picPla38;
            pic3[8] = picPla39;
            pic3[9] = picPla310;
            pic3[10] = picPla311;
            pic3[11] = picPla312;
            pic3[12] = picPla313;
            pic3[13] = picPla314;

            // pic4 add
            pic4[0] = picPla41;
            pic4[1] = picPla42;
            pic4[2] = picPla43;
            pic4[3] = picPla44;
            pic4[4] = picPla45;
            pic4[5] = picPla46;
            pic4[6] = picPla47;
            pic4[7] = picPla48;
            pic4[8] = picPla49;
            pic4[9] = picPla410;
            pic4[10] = picPla411;
            pic4[11] = picPla412;
            pic4[12] = picPla413;
            pic4[13] = picPla414;

            // NickName add
            NickName[0] = lblNick1;
            NickName[1] = lblNick2;
            NickName[2] = lblNick3;
            NickName[3] = lblNick4;

            // lblO add
            lblO[0] = label1;
            lblO[1] = label2;
            lblO[2] = label3;
            lblO[3] = label4;
            lblO[4] = label5;
            lblO[5] = label6;
            lblO[6] = label7;
            lblO[7] = label8;
            lblO[8] = label9;
            lblO[9] = label10;
            lblO[10] = label11;
            lblO[11] = label12;
            lblO[12] = label13;
            lblO[13] = label14;
        }

        private void resize()
        {
            // 해상도 조정을 위한 변수들
            double X = (double)Xsize / 1280;
            double Y = (double)Ysize / 1024;
            int WGap = (int)(6 * X);
            int HGap = (int)(6 * Y);


            /*
             * 
             * Resizing
             * 
             */


            // 폼 크기 조정
            this.Width = (int)(this.Width * X) + 2 * WGap;
            this.Height = (int)(this.Height * Y) + 2 * HGap;

            // Component 크기 조정
            TxtAllChat.Width = this.Width / 2;
            TxtChat.Width = this.Width / 2;
            CmdCardSort.Width = (int)(CmdCardSort.Width * X);
            CmdPass.Width = CmdCardSort.Width;
            TxtLog.Width = (int)(TxtLog.Width * X);
            LstPlayer.Width = (int)(LstPlayer.Width * X);

            TxtAllChat.Height = (int)(TxtAllChat.Height * Y);
            TxtChat.Height = (int)(TxtChat.Height * Y);
            CmdCardSort.Height = (int)(CmdCardSort.Height * Y);
            CmdPass.Height = CmdCardSort.Height;
            TxtLog.Height = (int)(TxtLog.Height * Y);
            LstPlayer.Height = (int)(LstPlayer.Height * Y);

            TxtLog.Visible = false;
            TxtAllChat.Visible = false;
            TxtChat.Visible = false;


            for(int i = 0; i < pic1.Length; i++)
            {
                // pic Width 조정
                pic1[i].Width = (int)(pic1[i].Width * X);
                pic2[i].Width = (int)(pic2[i].Width * X);
                pic3[i].Width = (int)(pic3[i].Width * X);
                pic4[i].Width = (int)(pic4[i].Width * X);
                // lblO Width 조정
                lblO[i].Width = (int)(49 * X);
                // pic Height 조정
                pic1[i].Height = (int)(pic1[i].Height * Y);
                pic2[i].Height = (int)(pic2[i].Height * Y);
                pic3[i].Height = (int)(pic3[i].Height * Y);
                pic4[i].Height = (int)(pic4[i].Height * Y);
                // lblO Height 조정
                lblO[i].Height = (int)(17 * Y);
            }

            for(int i = 0; i < NickName.Length; i++)
            {
                // NickName Width 조정
                NickName[i].Width = (int)(NickName[i].Width * X);
                // NickName Height 조정
                NickName[i].Height = (int)(NickName[i].Height * Y);
                // Visible 처리
                NickName[i].Text = "";
            }

            /*
             * 
             * Replacing
             * 
             */



            for (int i = 0; i < pic3.Length; i++) // 3, 4 visible = false
            {
                pic3[i].Visible = false;
                pic4[i].Visible = false;
            }
            /* 위치 테스트용
            for (int i = 0; i < pic3.Length;i++)
            {
                pic1[i].Visible = true;
                pic2[i].Visible = true;
                pic3[i].Visible = true;
                pic4[i].Visible = true;
                pic1[i].ImageLocation = "C:\\Users\\mr.gong\\Desktop\\Davinchi Code\\Davinchi Code\\img\\" + i.ToString() + ".jpg";
                pic2[i].ImageLocation = "C:\\Users\\mr.gong\\Desktop\\Davinchi Code\\Davinchi Code\\img\\" + i.ToString() + ".jpg";
                pic3[i].ImageLocation = "C:\\Users\\mr.gong\\Desktop\\Davinchi Code\\Davinchi Code\\img\\" + i.ToString() + ".jpg";
                pic4[i].ImageLocation = "C:\\Users\\mr.gong\\Desktop\\Davinchi Code\\Davinchi Code\\img\\" + i.ToString() + ".jpg";
                
            }

            for (int i = 0; i < NickName.Length;i++)
            {
                NickName[i].Visible = true;
                NickName[i].Text = i.ToString() + "번째 플레이어";
            }
            */



            pic3[13].Left = pic4[0].Left + 2 * pic4[0].Width + WGap; // pic3 첫위치 잡아주기

            for(int i = 0; i < pic4.Length-1 ; i++)
            {
                pic4[i + 1].Top = pic4[i].Top + pic4[i].Height + HGap; // pic4 배치
                pic3[12 - i].Left = pic3[13 - i].Left + pic3[13 - i].Width + WGap; // pic3 left 위치 잡아주기
            }

            for (int i = 0; i < pic1.Length; i++)
            {
                pic1[i].Top = pic4[13].Top; // pic1 top 배치
                pic1[i].Left = pic3[13 - i].Left; // pic1 left 배치
                pic2[i].Top = pic4[13 - i].Top; // pic2 top 배치
                pic2[i].Left = pic3[0].Left + 2 * pic3[0].Width + WGap; // pic2 left 배치
                pic3[i].Top = pic4[0].Top; // pic3 top 배치
            }

            for(int i = 0; i < lblO.Length; i++) // lblO init
            {
                lblO[i].Text = "●";
                lblO[i].TextAlign = ContentAlignment.MiddleCenter;
                lblO[i].Left = pic1[i].Left;
                lblO[i].Top = pic1[i].Top - lblO[i].Height - (int)(4 * Y);
                lblO[i].Visible = false;
                lblO[i].ForeColor = System.Drawing.Color.Red;
            }

            NickName[0].Left = (this.Width - NickName[0].Width) / 2; // 내 닉네임 렢
            NickName[2].Left = (this.Width - NickName[2].Width) / 2; // 3번 플레이어 닉네임 렢

            NickName[0].Top = pic1[0].Top + pic1[0].Height + HGap; // 내 닉넴 탑
            NickName[2].Top = (pic3[0].Top + pic3[0].Height) + HGap; // 3번 플레이어 닉네임 탑

            NickName[1].Top = (this.Height - NickName[1].Height) / 2; // 2번 플레이어 탑
            NickName[3].Top = NickName[1].Top; // 4번 플레이어 탑

            NickName[1].Left = pic2[1].Left - NickName[1].Width - (int)(5 * X); // 2번 플레이어 렢
            NickName[3].Left = pic4[1].Left + NickName[3].Width - (int)(5 * X); // 4번 플레이어 렢

            // 귀찮다 나머지 위치 조정
            TxtLog.Left = (this.Width - TxtLog.Width) / 2;
            TxtLog.Top = NickName[2].Top + NickName[2].Height + HGap;

            CmdCardSort.Left = (this.Width - CmdCardSort.Width) / 2;
            CmdPass.Left = CmdCardSort.Left;
            CmdCardSort.Top = (this.Height - CmdCardSort.Height) / 2 - (int)((double)lblNick4.Height * 3 / 2);
            CmdPass.Top = CmdCardSort.Top + CmdCardSort.Height + HGap;

            TxtAllChat.Top = CmdPass.Top + CmdPass.Height + HGap;
            TxtChat.Top = TxtAllChat.Top + TxtAllChat.Height + HGap;
            TxtAllChat.Left = (this.Width - TxtAllChat.Width) / 2;
            TxtChat.Left = TxtAllChat.Left;

            LstPlayer.Top = (CmdPass.Top + CmdCardSort.Top + CmdPass.Height) / 2 - LstPlayer.Height / 2;
            LstPlayer.Left = CmdPass.Left - LstPlayer.Width - HGap;
            lblWait.Top = LstPlayer.Top - (int)(24 * Y);
            lblWait.Left = LstPlayer.Left;
            /*
             * 
             * 아래는 초기화 작업
             * 
             */

            star = false; // 시작여부 = false
        }
        

        /*
         * 
         * Event
         * 
         */ 

        private void FrmGame_Load(object sender, EventArgs e)
        {
            Nick = Microsoft.VisualBasic.Interaction.InputBox("닉네임을 입력하세요.");
            sendingMessage =  "DC%nick%" + Nick;
        }

        private void FrmGame_Unload(object sender, EventArgs e)
        {
            stream.Close();
            client.Close();
        }

        public void send_msg(string msg)
        {
            sendingMessage = msg;
        }

        private void CmdCardSort_Click(object sender, EventArgs e)
        {
            if(!star)
            {
                if (MessageBox.Show("일반모드로 실행하실래예?", "일반모드 OR 바닥모드", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    sendingMessage =  "DC%start%1";
                else
                    sendingMessage =  "DC%star%0";
            }
            else
            {
                if(restB + restW <= 0)
                    MessageBox.Show("더이상 카드를 뽑으실 수 없습니다.");
                else
                {
                    if(TurnPhase.Equals("CARDGET"))
                    {
                        if (Turn == MyNum)
                        {
                            // FrmGame을 띄워줌
                            Thread thpick = new Thread(new ThreadStart(() =>
                            {
                                Application.Run(new FrmPick(this));
                            }));

                            thpick.Start();

                        }
                        else
                            MessageBox.Show("자신의 차례가 아닙니다.");
                    }
                    else
                    {
                        if (Turn == MyNum)
                            MessageBox.Show("카드를 더이상 뽑으실 수 없습니다.");
                        else
                            MessageBox.Show("자신의 차례가 아닙니다.");
                    }
                }
            }
        }
        
        private void picPla2_Click(object sender, EventArgs e)
        {
            int index;
            for(index = 0; index < pic2.Length; index++)
            {
                if (sender.Equals(pic2[index]))
                    break;
            }

            if (Turn == MyNum)
            {
                if (TurnPhase.Equals("CARDGET"))
                    MessageBox.Show("카드나 뽑아");
                else
                {
                    if (!Placard[(MyNum + 1) % playerNum, index].Equals(""))
                    {
                        ansClass = new Answer(MyNum, 1, index, Placard[(MyNum + 1) % playerNum, index]);
                        sendingMessage = ansClass.getAns();
                    }
                    else
                    {
                        MessageBox.Show("그 곳에는 상대방의 카드가 없습니다.");
                    }
                }
            }
            else
                MessageBox.Show("자신의 차례가 아닙니다.");
        }

        private void picPla3_Click(object sender, EventArgs e)
        {
            int index;
            for (index = 0; index < pic3.Length; index++)
            {
                if (sender.Equals(pic2[index]))
                    break;
            }

            if (Turn == MyNum)
            {
                if (TurnPhase.Equals("CARDGET"))
                    MessageBox.Show("카드나 뽑아");
                else
                {
                    if (!Placard[(MyNum + 2) % playerNum, index].Equals(""))
                    {
                        ansClass = new Answer(MyNum, 2, index, Placard[(MyNum + 2) % playerNum, index]);
                        sendingMessage = ansClass.getAns();
                    }
                    else
                    {
                        MessageBox.Show("그 곳에는 상대방의 카드가 없습니다.");
                    }
                }
            }
            else
                MessageBox.Show("자신의 차례가 아닙니다.");
        }

        private void picPla4_Click(object sender, EventArgs e)
        {
            int index;
            for (index = 0; index < pic4.Length; index++)
            {
                if (sender.Equals(pic2[index]))
                    break;
            }

            if (Turn == MyNum)
            {
                if (TurnPhase.Equals("CARDGET"))
                    MessageBox.Show("카드나 뽑아");
                else
                {
                    if (!Placard[(MyNum + 3) % playerNum, index].Equals(""))
                    {
                        ansClass = new Answer(MyNum, 3, index, Placard[(MyNum + 3) % playerNum, index]);
                        sendingMessage = ansClass.getAns();
                    }
                    else
                    {
                        MessageBox.Show("그 곳에는 상대방의 카드가 없습니다.");
                    }
                }
            }
            else
                MessageBox.Show("자신의 차례가 아닙니다.");
        }

        private void CmdPass_Click(object sender, EventArgs e)
        {
            if (iscanpass != null && iscanpass.Equals("true"))
                // send message "DC%pass%" & mynum
                sendingMessage = "DC%pass%" + MyNum.ToString();
            else
                MessageBox.Show("패스할 수 없습니다.");
        }

        private void Chat_Pressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                string s = TxtChat.Text;
                TxtChat.Text = "";
                if (s.Length > 0)
                    parseRight(s, s.Length - 1);

                sendingMessage = "DC%chat%" + MyNum + "%" + s;
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
