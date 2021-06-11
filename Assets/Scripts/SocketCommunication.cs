using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class SocketCommunication : MonoBehaviour
{
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    public GameObject Pointer;
	public string signal = "None";

    Thread mThread;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector2 receivedCall = Vector2.zero;
    // Vector3 receivedPos = Vector3.zero;
    string figura;
    bool running;

    void Update()
    {
        transform.position = receivedCall;
    }

    void Start()
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

        //SendAndReceiveData();

        //dodatkowy w¹tek
        //*(mutex) semafory dla synchronizacji pracy w¹tków
        running = true;
        while (running)
        {
            client = listener.AcceptTcpClient();
            //jest to funkcja blokuj¹ca
            if (client != null)
            {
                SendAndReceiveData();
            }

        }
        listener.Stop();
    }

    void SendAndReceiveData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        //1.Stream koñczy siê w momencie otrzymania danych
        //2.

        //Nie przepuszczamy informacji wiekszych od wielkosci Buforow.
        //Obsluga Exceptions

        //Receiving Data from the Host
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        //###################################
        //# Walidacja przychodz¹cych danych #
        //###################################
        if (dataReceived != null)
        {
            signal = dataReceived;
            //EXAMPLE OF VALIDATION FOR the module - CAMERAMOUSE

            if (dataReceived == "LEFT")
            {
                print("COMMAND: " + dataReceived);
                //Pointer.GetComponent<PointerController>().MoveLeft();
            }
            else if (dataReceived == "RIGHT")
            {
                print("COMMAND: " + dataReceived);
                //Pointer.GetComponent<PointerController>().MoveRight();
            }
            else if (dataReceived == "UP")
            {
                print("COMMAND: " + dataReceived);
                //Pointer.GetComponent<PointerController>().MoveUp();
            }
            else if (dataReceived == "DOWN")
            {
                print("COMMAND: " + dataReceived);
                //Pointer.GetComponent<PointerController>().MoveDown();
            }
            else
            {
                print("COMMAND: received in bad format - " + dataReceived);
            }

            //Sending Data to Host
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("SERVER: I received and executed your message.");
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
        }
        else
        {
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("SERVER: **Nie dostalem zadnych danych**");
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
        }
    }

    public static Vector2 StringToVector2(string sVector)
    {
        //Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        //Split the items
        string[] sArray = sVector.Split(',');
        Vector2 result = new Vector2(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]));
        return result;
    }
}