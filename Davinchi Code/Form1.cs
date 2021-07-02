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
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;



namespace Davinchi_Code
{
    public partial class FrmMain : Form
    {
        IPEndPoint clientPoint; // client point 설정
        IPEndPoint serverPoint; // server point 설정


        // 화면 해상도 변수들
        private int Xsize = Screen.PrimaryScreen.Bounds.Width;
        private int Ysize = Screen.PrimaryScreen.Bounds.Height;

        public FrmMain()
        {
            InitializeComponent(); // 폼 컴포넌트 초기화
            CheckForIllegalCrossThreadCalls = false;
            clientPoint = new IPEndPoint(IPAddress.Parse(internetComponent.getMyIP()), 0); // client 정보 bind
            serverPoint = new IPEndPoint(IPAddress.Parse(internetComponent.getServerIP()), 7700); // server 정보 bind
            resize(); // 폼 리사이징
            Thread t1 = new Thread(new ThreadStart(()=>init())); // 쓰레드 생성
            t1.IsBackground = true; // 데몬 쓰레드 설정
            t1.Start(); // 쓰레드 스타트
        }

        private void init() // socket 실행 부분
        {
            bool flag = true; // boolean형식 flag = true
            while (flag) // flag가 true일 동안
            {
                flag = false; // flag = false
                try // try
                {
                    TcpClient client = new TcpClient(clientPoint); // client 초기화
                    client.Connect(serverPoint); // server에 연결
                    NetworkStream stream = client.GetStream(); // Client Stream 가져오기


                    sendMessage(stream, "HELO"); // 서버에 첫인사 (_ _)

                    byte[] data = new byte[256]; // data : 수신용 버퍼
                    string RE = ""; // Response 받을 변수

                    int bytes; // 사이즈
                    while ((bytes = stream.Read(data, 0, data.Length)) != 0) // stream으로부터 읽어옴.
                    // 내용은 data에, 길이는 bytes에 저장
                    {
                        RE = Encoding.Default.GetString(data, 0, bytes).Split(' ')[0];
                        // data의 내용을 bytes만큼 RE로 인코딩
                        // 후 ' '의 앞을 따옴

                        // 서버로부터의 메시지가 뭔지 체크
                        if (RE.Equals("250")) // 서버가 HELO를 받음
                            sendMessage(stream, "STAT"); // 현재의 STAT을 반환해주세요! 요청
                        else if (RE.Equals("330") || RE.Equals("400")) // STAT : 서버가 정상 준비
                        {
                            CmdMulti.Enabled = true; // CmdMulti를 사용 가능하게 변경
                            lblNotify.Text = "Server is now on!"; // lblNotify 설정 ( 서버 연결 완료 공지 )
                            timCheck.Enabled = false; // Timer enable;
                            sendMessage(stream, "QUIT"); // QUIT 전송
                        }
                        else if (RE.Equals("340")) // STAT : Server is now off
                            sendMessage(stream, "STRT"); // STRT전송
                        else if (RE.Equals("221")) // 서버가 QUIT를 정상적으로 받음
                            break; // while문 탈출

                    }

                    stream.Close(); // stream 종료
                    client.Close(); // client 종료
                }
                catch (SocketException) // Socket 연결이 종료됬을 시, 처리시간이 오래 걸릴 시
                {
                    flag = true; // flag = true, while문 반복
                }
                catch (InvalidOperationException) // 현재 작업이 유효한 상태가 아닐 시
                {
                    flag = true; // flag = true, while문 반복
                }
            }

        }

        private void sendMessage(NetworkStream stream, String data) // 메시지를 전송하는 메소드
        {
            byte[] message = new byte[256]; // 인코딩된 message
            message = System.Text.Encoding.Default.GetBytes(data); // 인코딩 과정
            stream.Write(message, 0, message.Length); // 소켓 스트림으로 전송
        }

        private void timCheck_Tick(object sender, EventArgs e) // 좀 쓸데없는 타이머
        {
            if (lblNotify.Text.Equals("Server의 상태를 확인중입니다 ..."))
                lblNotify.Text = "Server의 상태를 확인중입니다 .";
            else if (lblNotify.Text.Equals("Server의 상태를 확인중입니다 ."))
                lblNotify.Text = "Server의 상태를 확인중입니다 ..";
            else
                lblNotify.Text = "Server의 상태를 확인중입니다 ...";
        }

        private void resize()
        {
            // Width 조정
            this.Width = this.Width * (Xsize / 1280);
            picTitle.Width = picTitle.Width * (Xsize / 1280);
            lblNotify.Width = lblNotify.Width * (Xsize / 1280);
            CmdSingle.Width = CmdSingle.Width * (Xsize / 1280);
            CmdMulti.Width = CmdMulti.Width * (Xsize / 1280);
            CmdExit.Width = CmdExit.Width * (Xsize / 1280);

            // Height 조정
            this.Height = this.Height * (Ysize / 1024);
            picTitle.Height = picTitle.Height * (Ysize / 1024);
            lblNotify.Height = lblNotify.Height * (Ysize / 1024);
            CmdSingle.Height = CmdSingle.Height * (Ysize / 1024);
            CmdMulti.Height = CmdMulti.Height * (Ysize / 1024);
            CmdExit.Height = CmdExit.Height * (Ysize / 1024);

            // 위치 조정
            picTitle.Top = picTitle.Top * (Ysize / 1024);
            CmdSingle.Top = picTitle.Top + picTitle.Height + 80 * (Ysize / 1024);
            // 80 * (Ysize / 1024) = Title과 Button간의 Gap
            CmdMulti.Top = CmdSingle.Top + CmdSingle.Height + 7 * (Ysize / 1024);
            CmdExit.Top = CmdMulti.Top + CmdMulti.Height + 7 * (Ysize / 1024);
            // 7 * (Ysize / 1024) = Button간의 Gap
            lblNotify.Top = (CmdSingle.Top - (CmdSingle.Top - (picTitle.Top + picTitle.Height)) / 2) - lblNotify.Height / 2;
            // lblNotify위치 조정


        }



        /* 
         *
         * 이 밑은 Event 부분입니다.
         * 
         */

        

        private void CmdMulti_Click(object sender, EventArgs e) // 멀티 플레이 클릭시
        {
            // FrmGame을 띄워줌
            Thread t2 = new Thread(new ThreadStart(() => // 쓰레드 생성
            {
                Application.Run(new FrmGame());
            }));

            t2.Start(); // 쓰레드 시작
            this.Close();


        }

        private void CmdExit_Click(object sender, EventArgs e) // CmdExit가 클릭 될 경우
        {
            if (MessageBox.Show("설마 잔인 무도하게 종료할거에요?ㅠㅠ 우 ㅅ유.. 진짜로?너무해요 ㅠㅠ", "진짜로?ㅠㅠ", MessageBoxButtons.YesNo) == DialogResult.Yes) // 다이얼로그 등판
            {
                this.DialogResult = DialogResult.Abort;
                System.Environment.Exit(0); // 종료
            }
        }
    }
}