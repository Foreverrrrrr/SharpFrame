using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.Common
{
    public class Cloud_Client
    { /// <summary>
      /// 服务器IP
      /// </summary>
        public string Target_IP { get; private set; }

        /// <summary>
        /// 服务器端口号
        /// </summary>
        public int Target_Port { get; private set; }

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConnect { get; private set; }

        /// <summary>
        /// 接收缓冲区
        /// </summary>
        private byte[] ReadBuffer { get; set; } = new byte[1024 * 1024];

        /// <summary>
        /// 接收事件
        /// </summary>
        public event Action<DateTime, string> ReceiveEvent;

        /// <summary>
        /// 断开事件
        /// </summary>
        public event Action<DateTime, Exception> DisconnectionEvent;

        /// <summary>
        /// 连接事件
        /// </summary>
        public event Action<DateTime, IPAddress> SuccessfuConnectEvent;

        private TaskCompletionSource<bool> ReadCompletedTask = new TaskCompletionSource<bool>();

        private System.Net.Sockets.TcpClient tcpClient { get; set; }

        /// <summary>
        /// 异步句柄
        /// </summary>
        public IAsyncResult Connect_Read { get; private set; }

        /// <summary>
        /// 接收字符串
        /// </summary>
        public string AsynRead { get; private set; }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="targetip">服务器ip</param>
        /// <param name="targetport">服务器端口号</param>
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
                    Connect_Read = tcpClient.GetStream().BeginRead(ReadBuffer, 0, ReadBuffer.Length, new AsyncCallback(AsyncRead), tcpClient);
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
                    AsynRead = Uri.UnescapeDataString(str);
                    ReceiveEvent?.BeginInvoke(DateTime.Now, str, null, null);
                    ReadCompletedTask.TrySetResult(true);
                    Connect_Read = tcpClient.GetStream().BeginRead(ReadBuffer, 0, ReadBuffer.Length, new AsyncCallback(AsyncRead), tcpClient);
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

        /// <summary>
        /// 异步数据发送，同步等待数据返回
        /// </summary>
        /// <param name="msg">发送字符串</param>
        /// <param name="encoding">字符串编码</param>
        /// <returns>返回字符串</returns>
        public string SyncSendReceive(string msg, Encoding encoding)
        {
            try
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    byte[] msgBytes = encoding.GetBytes(msg);
                    tcpClient.GetStream().BeginWrite(msgBytes, 0, msgBytes.Length, (ar) =>
                    {
                        tcpClient.GetStream().EndWrite(ar);
                    }, null);
                    Connect_Read.AsyncWaitHandle.WaitOne();
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
            return AsynRead;
        }

        /// <summary>
        /// 异步数据发送，同步等待数据返回
        /// </summary>
        /// <param name="msg">发送字符串</param>
        /// <returns>返回字符串</returns>
        public string SyncSendReceive(string msg)
        {
            try
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
                    tcpClient.GetStream().BeginWrite(msgBytes, 0, msgBytes.Length, (ar) =>
                    {
                        tcpClient.GetStream().EndWrite(ar);
                    }, null);
                    Connect_Read.AsyncWaitHandle.WaitOne();
                    ReadCompletedTask.Task.Wait();
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
            return AsynRead;
        }

        /// <summary>
        /// 异步数据发送，接收数据校验
        /// </summary>
        /// <param name="msg">发送字符串</param>
        /// <param name="read_msg">接收校验字符串</param>
        /// <param name="encoding">字符串编码格式</param>
        /// <returns>是否校验成功</returns>
        public bool SyncSendReceive(string msg, string read_msg, Encoding encoding)
        {
            try
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    byte[] msgBytes = encoding.GetBytes(msg);
                    tcpClient.GetStream().BeginWrite(msgBytes, 0, msgBytes.Length, (ar) =>
                    {
                        tcpClient.GetStream().EndWrite(ar);
                    }, null);
                    Connect_Read.AsyncWaitHandle.WaitOne();
                    ReadCompletedTask.Task.Wait();
                    if (read_msg == AsynRead)
                        return true;
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
            return false;
        }

        /// <summary>
        /// 异步数据发送，接收数据校验
        /// </summary>
        /// <param name="msg">发送字符串</param>
        /// <param name="read_msg">接收校验字符串</param>
        /// <returns>是否校验成功</returns>
        public bool SyncSendReceive(string msg, string read_msg)
        {
            try
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
                    tcpClient.GetStream().BeginWrite(msgBytes, 0, msgBytes.Length, (ar) =>
                    {
                        tcpClient.GetStream().EndWrite(ar);
                    }, null);
                    Connect_Read.AsyncWaitHandle.WaitOne();
                    ReadCompletedTask.Task.Wait();
                    if (read_msg == AsynRead)
                        return true;
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
            return false;
        }

        /// <summary>
        /// 异步数据发送
        /// </summary>
        /// <param name="msg">发送字符串</param>
        /// <param name="encoding">字符串编码</param>
        public void SendMessage(string msg, Encoding encoding)
        {
            byte[] msgBytes = encoding.GetBytes(msg);
            tcpClient.GetStream().BeginWrite(msgBytes, 0, msgBytes.Length, (ar) =>
            {
                tcpClient.GetStream().EndWrite(ar);
            }, null);
        }

        /// <summary>
        /// 异步数据发送
        /// </summary>
        /// <param name="msg">发送字符串</param>
        public void SendMessage(string msg)
        {
            byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
            tcpClient.GetStream().BeginWrite(msgBytes, 0, msgBytes.Length, (ar) =>
            {
                tcpClient.GetStream().EndWrite(ar);
            }, null);
        }

        /// <summary>
        /// TCP断开
        /// </summary>
        public void Close()
        {
            if (tcpClient != null && tcpClient.Client.Connected)
                tcpClient.Close();
            if (!tcpClient.Client.Connected)
            {
                tcpClient.Close();
            }
        }
    }
}
