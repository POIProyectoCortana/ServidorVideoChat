using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using InternetApplications.ChatApp;

namespace Chat_Server
{
    public class Client
    {
        public static int StreamBuffer = 1024;
        public int Id { get; set; }
        public string Nickname { get; set; }
        public TcpClient ClientSocket;
        public NetworkStream ClientSocketStream;
        public Queue<Mensaje> MensajeQueue;
        public List<Mensaje> MensajeList;
        public Thread Chat;
        byte[] Datos;
        public byte[] FixedDatos;
        public Mensaje Msj;
        frm_ServerMain Parent;

        public Client(frm_ServerMain Server,TcpClient Accepted,int ID)
        {
            //INICIALIZA LA CLASE 
            //INICIA LA CONEXION DEL NETWORKSTREAM PARA EL ENVIO Y RECEPCION DE DATOS
            this.Id = ID;
            Parent = Server;
            ClientSocket = Accepted;
            ClientSocketStream = ClientSocket.GetStream();
            FixedDatos = new byte[frm_ServerMain.BufferSize];            
            Mensaje Msj = new Mensaje();
            MensajeQueue = new Queue<Mensaje>();
            MensajeList = new List<Mensaje>();
        }
        public void Start()
        {
            Chat = new Thread(ChatService);
            Chat.Start();
            //ChatService();                    
        }
        public void GetDataFromClient()
        {
            Msj=new Mensaje(Mensaje.TipoDeMensaje.ClientId, "SERVER", "User", Id.ToString(), DateTime.Now);
            EnviarDatos(Msj);
            Msj.Clear();
            Msj=RecibirDatos();
            Nickname = Msj.Contenido;
            Msj.Clear();
        }
        void Listening()
        {           
            if(ClientSocketStream.DataAvailable)
            {
                Msj.Clear();
                Msj = RecibirDatos();
                frm_ServerMain.MensajeQueue.Enqueue(Msj);
                //Parent.AddMsj(Msj);
            }
            
        }            
        void Writening() 
        {
            //if (MensajeQueue.Count>0)
            //{
            //    Msj.Clear();

            //    Mensaje M =MensajeQueue.Dequeue();
            //    EnviarDatos(Msj);                    
            //}  
            if (MensajeList.Count > 0)
            {
                EnviarDatos(MensajeList[0]);
                MensajeList.RemoveAt(0);
            }
        }
        void ChatService()
        {
            while (true)
            {
                Listening();
                Writening();
            }
        }
        Mensaje RecibirDatos()
        {
            Array.Clear(FixedDatos, 0, FixedDatos.Length);
            this.ClientSocketStream.Read(FixedDatos, 0, FixedDatos.Length);            
            Mensaje M = new Mensaje();
            M.ParseBinaryToMessage(FixedDatos);
            this.ClientSocketStream.Flush();            
            return M; 
        }
        void EnviarDatos(Mensaje M)
        {
            //Array.Clear(Datos, 0, Datos.Length);
            this.Datos = M.ParseMessageToBinary();
            this.ClientSocketStream.Write(this.Datos, 0, this.Datos.Length);
            ClientSocketStream.Flush();            
        }
    }
}
