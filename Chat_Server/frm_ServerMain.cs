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
using InternetApplications.ChatApp;

namespace Chat_Server
{
    public partial class frm_ServerMain : Form
    {
        int IdGenerator;
        bool Encendido;
        List<Client> ClientList;
        TcpListener ServerListener;
        byte[] Datos;
        byte[] FixedDatos;
        IPAddress ServerAddress;
        public static int BufferSize = 512;
        public static Queue<Mensaje> MensajeQueue= new Queue<Mensaje>();
        public static List<Mensaje> MnsajeList = new List<Mensaje>();

        public frm_ServerMain()
        {
            Encendido = false;
            InitializeComponent();
            ServerAddress = IPAddress.Parse(GetThisMachineIP());
            ServerListener = new TcpListener(ServerAddress, 1234);
            lbl_DataServer.Text = "IP: " + GetThisMachineIP() + " \n Puerto: 1234";
            ClientList = new List<Client>();
            FixedDatos = new byte[BufferSize];
            IdGenerator = 1;
        }  
        private void btn_Iniciar_Click(object sender, EventArgs e)
        {
            if (!Encendido)
            {
                //AQUI ENCIENDE EL SERVIDOR
                try
                {
                    ServerListener.Start();
                    Encendido = !Encendido;
                    lbl_DEstado.Text = "Encendido";
                    lbl_DEstado.ForeColor = System.Drawing.Color.DarkGreen;
                    lbl_DEstado.BackColor = System.Drawing.Color.LightGreen;
                    btn_Iniciar.Text = "Detener";
                    tmr_Server.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"ERROR",MessageBoxButtons.OK);                    
                }                
            }
            else
            {
                //AQUI APAGA EL SERVIDOR
                try
                {
                    ServerListener.Stop();
                    Encendido = !Encendido;
                    lbl_DEstado.Text = "Apagado";
                    lbl_DEstado.ForeColor = System.Drawing.Color.DarkRed;
                    lbl_DEstado.BackColor = System.Drawing.Color.LightCoral;
                    btn_Iniciar.Text = "Iniciar";
                    tmr_Server.Enabled = false;
                    tmr_Server.Enabled = false;                 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK);             
                }
            }
        }
        public string GetThisMachineIP()
        {
            IPHostEntry Host;
            string localIP = "";
            Host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in Host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
        public void ListenerProcess()
        {
            while(ServerListener.Pending())
            {
                TcpClient NewTCPClient= ServerListener.AcceptTcpClient();
                Client NewClient = new Client(this,NewTCPClient,IdGenerator);
                NewClient.GetDataFromClient();            
                ClientList.Add(NewClient);
                int Index = ClientList.IndexOf(NewClient);
                ClientList[Index].Start();
                lbl_conectados.Text = "Clientes conectados: " + ClientList.Count.ToString();
                IdGenerator++;
            }
        }       
        private void tmr_Server_Tick(object sender, EventArgs e)
        {
            ListenerProcess();
            EntregarMensajes();
        }
        public void AddMsj(Mensaje M)
        {
            MensajeQueue.Enqueue(M);
        }
        public void EntregarMensajes()
        {
            while(MensajeQueue.Count>0)
            {
                Mensaje Entregar = MensajeQueue.Dequeue();
                int Clientes = ClientList.Count;                
                if(Entregar.Destinatario=="ALL")
                foreach(Client C in ClientList)
                {
                    if (Entregar.Destinatario == "ALL")
                    {
                        //C.MensajeQueue.Enqueue(Entregar);
                        C.MensajeList.Add(Entregar);
                    }
                    else if (C.Nickname == Entregar.Destinatario)
                    {
                        //C.MensajeQueue.Enqueue(neEntregar);
                    }
                }
            }
        }

        private void frm_ServerMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Mensaje M = new Mensaje(Mensaje.TipoDeMensaje.Disconnect_Request, "SERVER", "ALL", "ServerClosed", DateTime.Now);
            MensajeQueue.Enqueue(M);
        }
    }
    
}
