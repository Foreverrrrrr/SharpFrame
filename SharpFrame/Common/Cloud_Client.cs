using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.Common
{
    public class Cloud_Client
    {
        public string Target_IP { get; set; }

        public int Target_Port { get; set; }

        public bool IsConnect { get; set; }

        public byte[] ReadBuffer { get; set; } = new byte[1024 * 1024];

        public event Action<DateTime, string> ReceiveEvent;

        public event Action<DateTime, Exception> DisconnectionEvent;

        public event Action<DateTime, IPAddress> SuccessfuConnectEvent;

        private System.Net.Sockets.TcpClient tcpClient { get; set; }

        public Cloud_Client(string targetip, int targetport)
        {
            AsyncNewTcp(targetip, targetport);
        }

        private void AsyncNewTcp(string targetip, int targetport)
        {
            this.Target_IP = targetip;
            this.Target_Port = targetport;
            try
            {
                tcpClient = new System.Net.Sockets.TcpClient();
                tcpClient.BeginConnect(IPAddress.Parse(Target_IP), Target_Port, new AsyncCallback(AsyncConnect), tcpClient);
            }
            catch (Exception ex)
            {
                DisconnectionEvent?.BeginInvoke(DateTime.Now, ex, null, null);
                throw new Exception(ex.Message + "\r" + ex.StackTrace);
            }
        }

        private void AsyncConnect(IAsyncResult async)
        {
            async.AsyncWaitHandle.WaitOne(3000);
            if (!tcpClient.Connected)
            {
                IsConnect = false;
                tcpClient.Close();
                tcpClient = null;
                AsyncNewTcp(Target_IP, Target_Port);
            }
            else
            {
                try
                {
                    IsConnect = true;
                    SuccessfuConnectEvent?.BeginInvoke(DateTime.Now, IPAddress.Parse(Target_IP), null, null);
                    tcpClient.EndConnect(async);
                    tcpClient.GetStream().BeginRead(ReadBuffer, 0, ReadBuffer.Length, new AsyncCallback(AsyncRead), tcpClient);
                }
                catch (Exception ex)
                {
                    DisconnectionEvent?.BeginInvoke(DateTime.Now, ex, null, null);
                    IsConnect = false;
                    tcpClient.Close();
                    tcpClient = null;
                    AsyncNewTcp(Target_IP, Target_Port);
                }
            }
        }

        private void AsyncRead(IAsyncResult async)
        {
            try
            {
                int len = tcpClient.GetStream().EndRead(async);
                if (len > 0)
                {
                    IsConnect = true;
                    string str = Encoding.ASCII.GetString(ReadBuffer, 0, len);
                    str = Uri.UnescapeDataString(str);
                    ReceiveEvent?.BeginInvoke(DateTime.Now, str, null, null);
                    tcpClient.GetStream().BeginRead(ReadBuffer, 0, ReadBuffer.Length, new AsyncCallback(AsyncRead), tcpClient);
                }
                else
                {
                    throw new Exception("监测到服务器关闭");
                }
            }
            catch (Exception ex)
            {
                DisconnectionEvent?.BeginInvoke(DateTime.Now, ex, null, null);
                IsConnect = false;
                tcpClient.Close();
                tcpClient = null;
                AsyncNewTcp(Target_IP, Target_Port);
            }
        }

        public void SendMessage(string msg, Encoding encoding)
        {
            byte[] msgBytes = encoding.GetBytes(msg);
            tcpClient.GetStream().BeginWrite(msgBytes, 0, msgBytes.Length, (ar) =>
            {
                tcpClient.GetStream().EndWrite(ar);
            }, null);
        }

        public void Close()
        {
            if (tcpClient != null && tcpClient.Client.Connected)
                tcpClient.Close();
            if (!tcpClient.Client.Connected)
            {
                tcpClient.Close();
            }
            tcpClient.Dispose();
        }
    }
}
