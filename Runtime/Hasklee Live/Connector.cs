using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Hasklee {

public struct Frame
{
    public byte[] binData;
    public string luaCommand;
}

public class Connector
{
    public Connector(string ip, bool anyip, Int32 port)
    {
        listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress localAddr;
        if (anyip == true)
        {
            localAddr = IPAddress.Any;
        }
        else
        {
            localAddr = IPAddress.Parse(ip);
        }
        IPEndPoint remoteEP = new IPEndPoint(localAddr, port);
        listenSocket.Bind(remoteEP);
        listenSocket.Listen(4);

        listenThread = new Thread(Listen);
        listenThread.Start();
    }

    private List<Frame> frames = new List<Frame>();
    public List<Frame> Frames
    {
        get => frames;
        private set => frames = value;

    }

    private List<FrameReceiver> frameReceivers = new List<FrameReceiver>();
    private Semaphore framesSemaphore = new Semaphore(1, 1);
    private int semWaitTime = 1000;

    private Thread listenThread;
    private Socket listenSocket;

    public void AddFrame(Frame s)
    {
        frames.Add(s);
    }

    public void CleanFrames()
    {
        frames.Clear();
    }

    public void CleanUp()
    {
        listenThread.Abort();
        listenSocket.Close();
        foreach (var frameReceiver in frameReceivers)
        {
            frameReceiver.CleanUp();
        }
    }

    public void FramesSemRelease()
    {
        framesSemaphore.Release();
    }

    public bool FramesSemWait()
    {
        return framesSemaphore.WaitOne(semWaitTime);
    }

    public void RemoveFrameReceiver(FrameReceiver ch)
    {
        frameReceivers.Remove(ch);
    }

    private void Listen()
    {
        try
        {
            while (true)
            {
                Socket socket;
                socket = listenSocket.Accept();
                frameReceivers.Add(new FrameReceiver(socket, this));
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }
    }
}

public class FrameReceiver
{
    public FrameReceiver(Socket s, Connector c)
    {
        socket = s;
        connector = c;
        Thread handleThread = new Thread(Read);
        handleThread.Start();
    }

    private Connector connector;
    private Socket socket;

    public void CleanUp()
    {
        socket.Shutdown(SocketShutdown.Both);
        socket.Disconnect(false);
    }

    private void Read()
    {
        try
        {
            int bytesRec;
            byte[] ssize = new byte[8];

            bytesRec = socket.Receive(ssize);
            long binDataSize = BitConverter.ToInt64(ssize, 0);
            bytesRec = socket.Receive(ssize);
            long luaCommandSize = BitConverter.ToInt64(ssize, 0);

            byte[] binData = new byte[binDataSize];
            string luaCommand = "";

            if (binDataSize > 0)
            {
                int totalBytesReceived = 0;
                byte[] buffer = new byte[1024];
                int bytesReceived;
                do
                {
                    bytesReceived = socket.Receive(buffer);
                    Buffer.BlockCopy(buffer, 0, binData, totalBytesReceived, bytesReceived);
                    totalBytesReceived += bytesReceived;
                }
                while (bytesReceived > 0);
            }
            else if (luaCommandSize > 0)
            {
                byte[] luaCommandB = new byte[luaCommandSize];
                if (luaCommandSize > 0)
                {
                    bytesRec = socket.Receive(luaCommandB);
                    luaCommand = Encoding.UTF8.GetString(luaCommandB, 0, bytesRec);
                }
            }

            Frame f = new Frame();
            f.binData = binData;
            f.luaCommand = luaCommand;

            connector.FramesSemWait();
            connector.AddFrame(f);
            connector.FramesSemRelease();

            CleanUp();
            connector.RemoveFrameReceiver(this);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }
    }
}

}
