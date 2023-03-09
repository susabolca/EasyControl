using System.Net;

namespace ControllorPlugin
{
    public class UdpEventArgs : System.EventArgs
    {
        public byte[] Msg;
        public int Port;
        public EndPoint tarClient;
        public UdpEventArgs(int _port)
        {
            Port = _port;
        }
        public UdpEventArgs(byte[] _msg, int _port, EndPoint _tarClient)
        {
            Msg = _msg;
            Port = _port;
            tarClient = _tarClient;
        }
    }
    public class ButtonClickArgs : System.EventArgs
    {
        public string id;
        public ButtonClickArgs(string _id)
        {
            id = _id;
        }
    }
    public class SwitchButtonChangeArgs : System.EventArgs
    {
        public string id;
        public bool open;
        public SwitchButtonChangeArgs(string _id, bool _open)
        {
            id = _id;
            open = _open;
        }
    }
    public class TextEditorChangeArgs : System.EventArgs
    {
        public string id;
        public string text;
        public TextEditorChangeArgs(string _id, string _text)
        {
            id = _id;
            text = _text;
        }
    }
    public class TrackBarChangeArgs : System.EventArgs
    {
        public string id;
        public int value;
        public TrackBarChangeArgs(string _id, int _value)
        {
            id = _id;
            value = _value;
        }
    }
}
