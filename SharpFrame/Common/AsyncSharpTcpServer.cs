using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncSharpTCP
{
    /// <summary>
    /// TCP\ip服务器
    /// </summary>
    public class AsyncSharpTcpServer
    {
        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event Action<DateTime, IPEndPoint, string> OnTCPReadEvent;

        /// <summary>
        /// 客户端断开事件
        /// </summary>
        public event Action<DateTime, Exception> DisconnectionEvent;

        /// <summary>
        /// 客户端连接事件
        /// </summary>
        public event Action<DateTime, IPEndPoint> SuccessfuConnectEvent;

        private object lockObject = new object();

        private Socket socketCore = null;

        private byte[] _buffer = new byte[1024];
        /// <summary>
        /// 接收缓存区大小
        /// </summary>
        public byte[] buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        /// <summary>
        /// 客户端连接队列
        /// </summary>
        private List<ClientSession> sockets = new List<ClientSession>();

        private bool _isconnet;
        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsCommet
        {
            get { return _isconnet; }
            set { _isconnet = value; }
        }

        /// <summary>
        /// 打开服务器
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        public AsyncSharpTcpServer(string ip, int port)
        {
            IPAddress pcip = IPAddress.Parse(ip);
            socketCore = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketCore.Bind(new IPEndPoint(pcip, port));
            socketCore.Listen(1024);
            socketCore.BeginAccept(new AsyncCallback(AsyncAcceptCallback), socketCore);
        }

        /// <summary>
        /// 异步传入的连接申请请求
        /// </summary>
        /// <param name="iar">异步对象</param>
        private void AsyncAcceptCallback(IAsyncResult iar)
        {
            if (iar.AsyncState is Socket server_socket)
            {
                Socket client = null;
                ClientSession session = new ClientSession();
                try
                {
                    client = server_socket.EndAccept(iar);
                    session.Socket = client;
                    session.EndPoint = (IPEndPoint)client.RemoteEndPoint;
                    IsCommet = true;
                    client.BeginReceive(buffer, 0, 2048, SocketFlags.None, new AsyncCallback(ReceiveCallBack), session);
                    lock (session)
                    {
                        sockets.Add(session);
                    }
                    SuccessfuConnectEvent?.BeginInvoke(DateTime.Now, new IPEndPoint(session.EndPoint.Address, session.EndPoint.Port), null, null);
                }
                catch (ObjectDisposedException)//Server Close
                {
                    IsCommet = false;
                    lock (lockObject)
                    {
                        sockets.Remove(session);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    DisconnectionEvent?.BeginInvoke(DateTime.Now, ex, null, null);
                    IsCommet = false;
                    lock (lockObject)
                    {
                        sockets.Remove(session);
                    }
                    client?.Close();
                }
                server_socket.BeginAccept(new AsyncCallback(AsyncAcceptCallback), server_socket);
            }
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            if (ar.AsyncState is ClientSession client)
            {
                string msg = "";
                try
                {
                    int length = client.Socket.EndReceive(ar);
                    if (length == 0)
                    {
                        client.Socket.Close();
                        lock (lockObject)
                        {
                            sockets.Remove(client);
                        }
                        return;
                    };
                    client.Socket.BeginReceive(buffer, 0, 2048, SocketFlags.None, new AsyncCallback(ReceiveCallBack), client);
                    byte[] data = new byte[length];
                    Array.Copy(buffer, 0, data, 0, length);
                    msg = Encoding.UTF8.GetString(data, 0, length); //接收数据
                    OnTCPReadEvent?.BeginInvoke(DateTime.Now, new IPEndPoint(client.EndPoint.Address, client.EndPoint.Port), msg, null, null);
                }
                catch (Exception ex)
                {
                    DisconnectionEvent?.BeginInvoke(DateTime.Now, ex, null, null);
                    lock (lockObject)
                    {
                        if (ex.Message == "远程主机强迫关闭了一个现有的连接。")
                        {
                            IsCommet = false;
                            sockets.Remove(client);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 异步数据发送
        /// </summary>
        /// <param name="ip">客户端ip</param>
        /// <param name="meg">发送字符串</param>
        public void AsyncWrite(string ip, string meg)
        {
            var emp = sockets.Find(e => e.EndPoint.Address.ToString() == ip);
            if (emp != null)
            {
                byte[] msgBytes = Encoding.ASCII.GetBytes(meg);
                emp.Socket.BeginSend(msgBytes, 0, msgBytes.Length, SocketFlags.None, null, emp.Socket);
            }
        }

        /// <summary>
        /// 异步数据发送
        /// </summary>
        /// <param name="ip">客户端ip</param>
        /// <param name="port">客户端端口号</param>
        /// <param name="meg">发送字符串</param>
        public void AsyncWrite(string ip, int port, string meg)
        {
            var emp = sockets.Find(e => e.EndPoint.Address.ToString() == ip && e.EndPoint.Port == port);
            if (emp != null)
            {
                byte[] msgBytes = Encoding.ASCII.GetBytes(meg);
                emp.Socket.BeginSend(msgBytes, 0, msgBytes.Length, SocketFlags.None, null, emp.Socket);
            }
        }

        /// <summary>
        /// 异步数据发送
        /// </summary>
        /// <param name="meg">发送字符串</param>
        public void AsyncWrite(string meg)
        {
            var emp = sockets[0];
            if (emp != null)
            {
                byte[] msgBytes = Encoding.ASCII.GetBytes(meg);
                emp.Socket.BeginSend(msgBytes, 0, msgBytes.Length, SocketFlags.None, null, emp.Socket);
            }
        }

        /// <summary>
        /// 服务器关闭
        /// </summary>
        public void CloseTCPServer()
        {
            foreach (var socket in sockets)
            {
                socket?.Socket?.Close();
            }
            IsCommet = false;
            sockets.Clear();
            socketCore.Dispose();
        }
    }

    internal class ClientSession
    {
        public Socket Socket { get; set; }

        public IPEndPoint EndPoint { get; set; }

        public override string ToString() => EndPoint.ToString();
    }
}
