using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class socket_communication : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector3 receivedPos = Vector3.zero;
    string figura;
    bool running;

    private void Update()
    {
        transform.position = receivedPos;
    }

    private void Start()
    {
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();

        running = true;
        while (running)
        {
            SendAndReceiveData();
        }
        listener.Stop();
    }

    void SendAndReceiveData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        //Receiving Data from the Host
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        //#################################
        //#Walidacja przychodz¹cych danych#
        //#################################
        if (dataReceived != null)
        {

            if (dataReceived.Length == 3)//wspolrzedna np. E4
            {
                print("Figura ma przemiescic sie na pole: " + dataReceived);
            }
            else if (dataReceived.Length == 1)//figura np. H(hetman)
            {
                if (dataReceived == "H")
                {
                    figura = "Hetman";
                }
                else if (dataReceived == "K")
                {
                    figura = "Król";
                }
                else if (dataReceived == "G")
                {
                    figura = "Goniec";
                }
                else if (dataReceived == "S")
                {
                    figura = "Skoczek";
                }
                else if (dataReceived == "W")
                {
                    figura = "Wie¿a";
                }
                else if (dataReceived == "P")
                {
                    figura = "Pion";
                }
                else
                {
                    figura = "*zly format figury*";
                }
                print("Figura która wykonuje ruch to: " + figura);
            }
            else if (dataReceived.Length == 5)//wektor przemieszczenia np. 2,0,0
            {
                //Using received data
                receivedPos = StringToVector3(dataReceived);
                print("Otrzymalem polecenie od Clienta Pythona, kostka zostala przesunieta!");
            }
            else
            {
                print("Otrzymalismy INNA WIADOMOSC, a wiadomosc wyglada nastepujaco: " + dataReceived);
            }

            //Sending Data to Host
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Dostalem twoja wiadomosc! Polecenie wykonano :)");
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
        }
        else
        {
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("**Nie dostalem zadnych danych**");
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
        }
    }

    public static Vector3 StringToVector3(string sVector)
    {
        //Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        //Split the items
        string[] sArray = sVector.Split(',');
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));
        return result;
    }
}
