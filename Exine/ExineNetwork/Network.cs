﻿using System.Collections.Concurrent;
using System.Net.Sockets;
using Exine.ExineControls;
//


namespace Exine.ExineNetwork
{
    static class Network
    {
        private static TcpClient _client;
        public static int ConnectAttempt = 0;
        public static int MaxAttempts = 20;
        public static bool ErrorShown;
        public static bool Connected;
        public static long TimeOutTime, TimeConnected, RetryTime = CMain.Time + 5000;

        private static ConcurrentQueue<Packet> recvBuffer;
        private static ConcurrentQueue<Packet> sendBuffer;

        static byte[] _rawData = new byte[0];
        static readonly byte[] _rawBytes = new byte[8 * 1024];

        public static void Connect()
        {
            if (_client != null)
                Disconnect();

            if (ConnectAttempt >= MaxAttempts)
            {
                if (ErrorShown)
                {
                    return;
                }

                ErrorShown = true;

                ExineMessageBox errorBox = new("서버에 연결하는 중 오류가 발생했습니다.", MirMessageBoxButtons.Cancel);
                errorBox.CancelButton.Click += (o, e) => Program.Form.Close();
                errorBox.Label.Text = $"최대 연결 시도 횟수에 도달했습니다.: {MaxAttempts}" +
                                      $"{Environment.NewLine}나중에 다시 시도하거나 연결 설정을 확인하세요..";
                errorBox.Show();
                errorBox.BringToFront();
                return;
            }

            ConnectAttempt++;

            try
            {
                _client = new TcpClient { NoDelay = true };
                _client?.BeginConnect(Settings.IPAddress, Settings.Port, Connection, null);
            }
            catch (ObjectDisposedException ex)
            {
                if (Settings.LogErrors) CMain.SaveError(ex.ToString());
                Disconnect();
            }
        }

        private static void Connection(IAsyncResult result)
        {
            try
            {
                _client?.EndConnect(result);

                if ((_client != null &&
                    !_client.Connected) ||
                    _client == null)
                {
                    Connect();
                    return;
                }

                //여기서 메인 화면 표시하거나 하는게 맞음
                recvBuffer = new ConcurrentQueue<Packet>();
                sendBuffer = new ConcurrentQueue<Packet>();
                _rawData = new byte[0];

                TimeOutTime = CMain.Time + Settings.TimeOut;
                TimeConnected = CMain.Time;

                BeginReceive();
            }
            catch (SocketException)
            {
                Thread.Sleep(100);
                Connect();
            }
            catch (Exception ex)
            {
                if (Settings.LogErrors) CMain.SaveError(ex.ToString());
                Disconnect();
            }
        }

        private static void BeginReceive()
        {
            if (_client == null || !_client.Connected) return;

            try
            {
                _client.Client.BeginReceive(_rawBytes, 0, _rawBytes.Length, SocketFlags.None, ReceiveData, _rawBytes);
            }
            catch
            {
                Disconnect();
            }
        }
        private static void ReceiveData(IAsyncResult result)
        {
            if (_client == null || !_client.Connected) return;

            int dataRead;

            try
            {
                dataRead = _client.Client.EndReceive(result);
            }
            catch
            {
                Disconnect();
                return;
            }

            if (dataRead == 0)
            {
                Disconnect();
            }

            byte[] rawBytes = result.AsyncState as byte[];

            byte[] temp = _rawData;
            _rawData = new byte[dataRead + temp.Length];
            Buffer.BlockCopy(temp, 0, _rawData, 0, temp.Length);
            Buffer.BlockCopy(rawBytes, 0, _rawData, temp.Length, dataRead);

            Packet p;
            List<byte> data = new List<byte>();

            while ((p = Packet.ReceivePacket(_rawData, out _rawData)) != null)
            {
                data.AddRange(p.GetPacketBytes());
                recvBuffer.Enqueue(p);
            }

            CMain.BytesReceived += data.Count;

            BeginReceive();
        }

        private static void BeginSend(List<byte> data)
        {
            if (_client == null || !_client.Connected || data.Count == 0) return;
            
            try
            {
                _client.Client.BeginSend(data.ToArray(), 0, data.Count, SocketFlags.None, SendData, null);
            }
            catch
            {
                Disconnect();
            }
        }
        private static void SendData(IAsyncResult result)
        {
            try
            {
                _client.Client.EndSend(result);
            }
            catch
            { }
        }

        public static void Disconnect()
        {
            if (_client == null) return;

            _client?.Close();

            TimeConnected = 0;
            Connected = false;
            sendBuffer = null;
            _client = null;

            recvBuffer = null;
        }

        public static void Process()
        {
            if (_client == null || !_client.Connected)
            {
                if (Connected)
                {
                    while (recvBuffer != null && !recvBuffer.IsEmpty)
                    {
                        if (!recvBuffer.TryDequeue(out Packet p) || p == null) continue;
                        if (!(p is ServerPacket.Disconnect) && !(p is ServerPacket.ClientVersion)) continue;

                        ExineScene.ActiveScene.ProcessRecvPacket(p);
                        recvBuffer = null;
                        return;
                    }

                    ExineMessageBox.Show("Lost connection with the server.", true);
                    Disconnect();
                    return;
                }
                else if (CMain.Time >= RetryTime)
                {
                    RetryTime = CMain.Time + 5000;
                    Connect();
                }
                return;
            }

            if (!Connected && TimeConnected > 0 && CMain.Time > TimeConnected + 5000)
            {
                Disconnect();
                Connect();
                return;
            }



            while (recvBuffer != null && !recvBuffer.IsEmpty)
            {
                if (!recvBuffer.TryDequeue(out Packet p) || p == null) continue;
                ExineScene.ActiveScene.ProcessRecvPacket(p);
            }


            if (CMain.Time > TimeOutTime && sendBuffer != null && sendBuffer.IsEmpty)
                sendBuffer.Enqueue(new ClientPacket.KeepAlive());

            if (sendBuffer == null || sendBuffer.IsEmpty) return;

            TimeOutTime = CMain.Time + Settings.TimeOut;

            List<byte> data = new List<byte>();
            while (!sendBuffer.IsEmpty)
            {
                if (!sendBuffer.TryDequeue(out Packet p)) continue;
                data.AddRange(p.GetPacketBytes());
            }

            CMain.BytesSent += data.Count;

            BeginSend(data);
        }
        
        public static void SendPacketToServer(Packet p)
        {
            if (sendBuffer != null && p != null)
                sendBuffer.Enqueue(p);
        }
    }
}
