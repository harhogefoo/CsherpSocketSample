using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // サーバに送信するデータを入力してもらう
            Console.WriteLine("文字列を入力し，Enterキーを押して下さい．");
            string sendMsg = Console.ReadLine();
            // 何も入力されなかった時は終了
            if (sendMsg == null || sendMsg.Length == 0)
            {
                return;
            }

            // サーバのIPアドレスとポート番号
            string ipOrHost = "127.0.0.1";
            // string ipOrHost = "localhost";
            int port = 50000;

            // TcpClientを作成し，サーバと接続する
            System.Net.Sockets.TcpClient tcp =
              new System.Net.Sockets.TcpClient(ipOrHost, port);
            Console.WriteLine("サーバ({0}:{1})と接続しました({2}:{3}.",
                ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address,
                ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port,
                ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address,
                ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Port);

            // NetworkStreamを取得する
            System.Net.Sockets.NetworkStream ns = tcp.GetStream();

            // サーバにデータを送信する
            // 文字列をByte型配列に変換
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            byte[] sendBytes = enc.GetBytes(sendMsg + "\n");
            // データを送信する
            ns.Write(sendBytes, 0, sendBytes.Length);
            Console.WriteLine(sendMsg);

            // サーバから送られたデータを受信する
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[256];
            int resSize = 0;
            do
            {
                // データの一部を受信する
                resSize = ns.Read(resBytes, 0, resBytes.Length);
                // Readが0を返した時はサーバが切断したと判断
                if (resSize == 0)
                {
                    Console.WriteLine("サーバが切断しました");
                    break;
                }

                // 受信したデータを蓄積する
                ms.Write(resBytes, 0, resSize);
            } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');
            // 受信したデータを文字列に変換
            string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            ms.Close();
            // 末尾の\nを削除
            resMsg = resMsg.TrimEnd('\n');
            Console.WriteLine(resMsg);

            // 閉じる
            ns.Close();
            tcp.Close();
            Console.WriteLine("切断しました．");

            Console.ReadLine();
        }
    }
}
