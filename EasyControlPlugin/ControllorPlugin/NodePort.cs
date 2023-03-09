using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllorPlugin
{
    public enum PortValue { Int64, Double, String };
    public enum PortType { In, Out };
    public class NodePort
    {
        public static int maxIndex = 0;
        public int Index { get; private set; }
        //-----
        public long OldInt { get; private set; }
        public double OldFloat { get; private set; }
        public string OldString { get; private set; }
        //-----
        private long _valueInt64;
        private double _valueDouble;
        private string _valueString;
        private int dashTime = 0;
        public string Name { private set; get; }//名称; 
        public PortType IO { private set; get; }//IO方向; 
        public PortValue Type { private set; get; }//数据类型;   
        public long ValueInt64
        {
            set
            {
                OldInt = _valueInt64;
                _valueInt64 = value;
                if (dashTime < 50)
                {
                    dashTime = 50;
                }
            }
            get { return _valueInt64; }
        }
        public double ValueDouble
        {
            set
            {
                OldFloat = _valueDouble;
                _valueDouble = value;
                if (dashTime < 50)
                {
                    dashTime = 50;
                }
            }
            get { return _valueDouble; }
        }
        public string ValueString
        {
            set
            {
                OldString = _valueString;
                _valueString = value;
                if (dashTime < 50)
                {
                    dashTime = 50;
                }
            }
            get { return _valueString; }
        }

        public NodePort(string _Name, PortType _io, long Value)
        {
            Index = maxIndex;
            maxIndex++;
            Name = _Name;
            IO = _io;
            Type = PortValue.Int64;
            ValueInt64 = Value;
        }
        public NodePort(string _Name, PortType _io, double Value)
        {
            Index = maxIndex;
            maxIndex++;
            Name = _Name;
            IO = _io;
            Type = PortValue.Double;
            ValueDouble = Value;
        }
        public NodePort(string _Name, PortType _io, string Value)
        {
            Index = maxIndex;
            maxIndex++;
            Name = _Name;
            IO = _io;
            Type = PortValue.String;
            ValueString = Value;
        }
        public bool isDash()
        {
            if (dashTime > 0)
            {
                dashTime--;
                return true;
            }
            return false;
        }
    }
}
