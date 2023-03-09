using System;
using System.Xml;

namespace ControllorPlugin
{
    public static class EasyXml
    {
        #region 获取参数
        public static bool GetAttribute<TEnum>(XmlNode node, string name, out TEnum value) where TEnum : struct
        {
            if (node == null)
            {
                value = default(TEnum);
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    TEnum t = default(TEnum);
                    if (Enum.TryParse<TEnum>(attri.Value, out t))
                    {
                        value = t;
                        return true;
                    }
                }
            }
            value = default(TEnum);
            return false;
        }
        public static bool GetAttribute(XmlNode node, string name, out byte value)
        {
            if (node == null)
            {
                value = 0;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    byte _temp = 0;
                    if (byte.TryParse(attri.Value, out _temp))
                    {
                        value = _temp;
                        return true;
                    }
                }
            }
            value = 0;
            return false;
        }
        public static bool GetAttribute(XmlNode node, string name, out int value)
        {
            if (node == null)
            {
                value = 0;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    int _temp = 0;
                    if (int.TryParse(attriValue, out _temp))
                    {
                        value = _temp;
                        return true;
                    }
                }
            }
            value = -1;
            return false;
        }
        public static bool GetAttribute(XmlNode node, string name, out float value)
        {
            if (node == null)
            {
                value = 0f;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    float _temp = 0f;
                    if (float.TryParse(attriValue, out _temp))
                    {
                        value = _temp;
                        return true;
                    }
                }
            }
            value = 0f;
            return false;
        }
        public static bool GetAttribute(XmlNode node, string name, out string value)
        {
            if (node == null)
            {
                value = "";
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    value = attri.Value;
                    return true;
                }
            }
            value = "";
            return false;
        }
        public static bool GetAttribute(XmlNode node, string name, out bool value)
        {
            if (node == null)
            {
                value = false;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    if (!bool.TryParse(attriValue, out value))
                    {
                        value = true;
                        return false;
                    }
                    return true;
                }
            }
            value = false;
            return false;
        }
        #endregion
    }
}
