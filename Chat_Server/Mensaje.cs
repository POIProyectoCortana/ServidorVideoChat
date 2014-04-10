using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing;

namespace InternetApplications.ChatApp
{
    public class Mensaje
    {
        /// <summary>Proporciona una lista de los emoticones soportados por la aplicación</summary>
        public static readonly string[,] EmoticonList =
        {
            {":)","Emoticons\\bored.bmp",""},
            {";)","Emoticons\\bored.bmp",""},
            {":p","Emoticons\\bored.bmp",""},
            {":D","Emoticons\\bored.bmp",""},
            {":3","Emoticons\\bored.bmp",""},
            {"XD","Emoticons\\bored.bmp",""},
            {"DX","Emoticons\\bored.bmp",""}
        };

        /// <summary>Tipo de mensaje que se puden enviar entre cliente y servidor</summary>
        public enum TipoDeMensaje
        {
            Null,
            ClientLoggedIn,
            ClientLoggedOut,
            Message,
            ConnectionStatus,
            Connect_Request,
            Connect_Response,
            Disconnect_Request,
            Disconnect_Response,
            ClientNickname,
            ClientId,
            ClientStatus,
            Error
        }
        /// <summary>Obtiene o establece el tipo del mensaje</summary>
        public TipoDeMensaje Tipo { get; set; }
        /// <summary>Obtiene o establece el contenido del mensaje</summary>
        public string Contenido { get; set; }
        /// <summary>Obtiene o establece el alias del remitente del mensje</summary>
        public string Remitente { get; set; }
        /// <summary>Obtiene o establece el alias del destinatario del mensje</summary>
        public string Destinatario { get; set; }
        /// <summary>Obtiene o establece la fecha y hora en que se envió el mensaje</summary>
        public DateTime FechaHora { get; set; }
        /// <summary>Obtiene o establece la longitud del contenido del mensaje</summary>
        public int Longitud { get; set; }

        /// <summary>Constructor de la clase Message</summary>
        /// <param name="T">Tipo de mensaje que se enviará</param>
        /// <param name="M">Texto de contenido del mensaje</param>
        /// <param name="D">Nombre o nickname del destinatario del mensaje</param>
        public Mensaje(TipoDeMensaje T, string R, string D, string M, DateTime F)
        {
            this.Contenido = M;
            this.Tipo = T;
            this.Destinatario = D;
            this.Remitente = R;
            this.FechaHora = FechaHora;
            this.Longitud = M.Length;
        }
        /// <summary>Constructor de la clase Message</summary>
        public Mensaje()
        {
        }
        /// <summary>Método que convierte la clase Message a una representacion en string</summary>
        /// <returns>String que representa el objeto Message</returns>
        public override string ToString()
        {
            return Tipo.ToString() + '~' + Remitente + '~' + Destinatario + '~' + Contenido;
        }

        public static void PrintMessageWithEmoticon(System.Windows.Forms.RichTextBox rtb_cointainer, string msj)
        {

            int TextAlreadyLength = rtb_cointainer.TextLength;
            rtb_cointainer.AppendText(msj);
            for (int i = 0; i < EmoticonList.Length / 3; i++)
            {
                int indexfound = 0;
                int indexNext = 0;
                int EmoticonLength = EmoticonList[i, 0].Length;
                //bool found = true;
                while (true)
                {
                    indexfound = msj.IndexOf(EmoticonList[i, 0], indexNext);
                    if (indexfound >= 0)
                    {
                        Bitmap image = new Bitmap(EmoticonList[i, 1]);
                        System.Windows.Forms.Clipboard.SetDataObject(image);
                        rtb_cointainer.Select(indexfound + TextAlreadyLength, EmoticonLength);
                        rtb_cointainer.Paste();
                        indexNext += EmoticonLength;
                    }
                    else { break; }
                }
            }
        }

        /// <summary>Funcion imprime el contenido del objeto Mesage en un Richtextbox con emoticones si el mensaje contiene la combinacion de caracteres</summary>
        /// <param name="rtb_cointainer">Referencia del RitchTextBox donde se va a imterpretar el mensaje</param>
        public void PrintMessageWithEmoticon(System.Windows.Forms.RichTextBox rtb_cointainer)
        {
            int TextAlreadyLength = rtb_cointainer.TextLength;
            rtb_cointainer.AppendText(this.Contenido);
            for (int i = 0; i > EmoticonList.Length / 3; i++)
            {
                int indexfound = 0;
                int indexNext = 0;
                int EmoticonLength = EmoticonList[i, 0].Length;
                while (true)
                {
                    indexfound = this.Contenido.IndexOf(EmoticonList[i, 0], indexNext);
                    if (indexfound >= 0)
                    {
                        Bitmap image = new Bitmap(EmoticonList[i, 1]);
                        System.Windows.Forms.Clipboard.SetDataObject(image);
                        rtb_cointainer.Select(indexfound + TextAlreadyLength, EmoticonLength);
                        rtb_cointainer.Paste();
                        indexNext += EmoticonLength;
                    }
                    else { break; }
                }
            }
        }
        /// <summary>Funcion que vacia los datos contenidos en el objeto Mensaje</summary>
        public void Clear()
        {
            this.Contenido = "";
            this.Tipo = TipoDeMensaje.Null;
            this.Destinatario = "";
            this.Remitente = "";
            this.FechaHora = DateTime.Today;
            this.Longitud = 0;
        }

        /// <summary>Funcion establece los valores que puede contener el objeti Mensaje</summary>
        public void SetData(TipoDeMensaje T, string R, string D, string M, DateTime F)
        {
            this.Contenido = M;
            this.Tipo = T;
            this.Destinatario = D;
            this.Remitente = R;
            this.FechaHora = FechaHora;
            this.Longitud = M.Length;
        }
        public byte[] ParseMessageToBinary()
        {
            byte[] StreamData;
            string data = this.Tipo.ToString() + '~' + this.Remitente + '~' + this.Destinatario + '~' + this.Contenido + '~' + this.FechaHora.ToString();
            data = data.Trim();
            StreamData = Encoding.Default.GetBytes(data);
            return StreamData;
        }
        public void ParseBinaryToMessage(byte[] D)
        {
            string Data = Encoding.Default.GetString(D);
            int IndexExtras = Data.IndexOf("\0");
            Data = Data.Remove(IndexExtras);
            Data = Data.Trim();
            string[] MensajeMatrix = Data.Split('~');
            //Mesage.MessageType Type = (Mesage.MessageType)Enum.Parse(typeof(Mesage.MessageType), MensajeMatrix[0]);
            //DateTime Time = DateTime.Parse(MensajeMatrix[4]);
            this.Tipo = (TipoDeMensaje)Enum.Parse(typeof(TipoDeMensaje), MensajeMatrix[0]);
            this.FechaHora = DateTime.Parse(MensajeMatrix[4]);
            this.Destinatario = MensajeMatrix[2];
            this.Remitente = MensajeMatrix[1];
            this.Contenido = MensajeMatrix[3];
        }
    }
}


