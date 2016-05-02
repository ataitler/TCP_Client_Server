using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPClient
{
    public partial class ChatScreen : Form
    {
        public ChatScreen()
        {
            InitializeComponent();
        }

        private const int port = 11000;

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private Socket client;

        // The response from the remote device.
        private static String response = String.Empty;

        private void StartClient()
        {
            // Connect to a remote device.
            try
            {
                IPAddress ipAddress = IPAddress.Loopback;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // start listenning
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                // There might be more data, so store the data received so far.
                string rec = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                response = "Recieved: " + rec;
                Console.WriteLine(response);
                if (rec.IndexOf("ATTACK_RIGHT") > -1)
                {
                    Console.WriteLine("Thank You, action set!");
                }
                else
                {
                    Console.WriteLine("Thank You");
                    Send(client, "Thank You!");
                }
                
                if (client.Connected)
                {
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }

                // send a msg back to the server

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            if (client.Connected)
            {
                // Begin sending the data to the remote device.
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                //sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine("async: " + e.ToString());
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            StartClient();
        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            } 
        }

    }
}
