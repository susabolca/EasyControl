using ControllorPlugin;
using System;
using System.Collections.Generic;
using System.Net;

namespace TestPlugin
{
    public class TestPlugin : InterfacePlugin
    {
        string testString = "";
        //================================================
        private List<Node> moduleList;

        public event EventHandler CreateUDP;
        public event EventHandler SendUDP;
        public event EventHandler ButtonLeftClick;
        public event EventHandler ButtonRightClick;
        public event EventHandler SwitchButtonChange;
        public event EventHandler TextEditorChange;
        public event EventHandler TrackBarChange;

        //用来储存ID，由软件维护，请不要做任何修改;
        public string PluginID
        {
            get { return "048ba9d9-e987-43d4-a428-296e9dd8d267"; }
        }
        //用来表示插件是否打开;
        public bool Open { get; set; }
        //用来表示是否开启自动启动
        public bool Auto { get; set; }
        //获取模块列表，需要在插件类中定义List<Module> 类型的变量并返回;
        public List<Node> GetModuleList()
        {
            return moduleList;
        }
        //获取模块名称，用来显示在插件列表中的名称，没有实际意义，只是显示用;
        public string GetName()
        {
            return "testPlugin";
        }
        //当自动启动打开后，加载时执行的函数
        public void AutoOpen()
        {
            Console.WriteLine("TestPlugin - AutoOpen !");
        }
        //每帧都会执行一次的获取数据事件;例子中是将属性中的值写入到接口中
        public void Update()
        {
            moduleList[0].NodePortList[0].ValueInt64 += 1;
            if (moduleList[0].NodePortList[0].ValueInt64 >= 255)
                moduleList[0].NodePortList[0].ValueInt64 = 0;
            moduleList[0].NodePortList[1].ValueDouble = moduleList[0].NodePortList[0].ValueInt64 / 3f;
            moduleList[0].NodePortList[2].ValueString = moduleList[0].NodePortList[0].ValueInt64.ToString();
        }
        //初始化，用来定义插件内容，会在加载插件时运行一次;
        public void Init()
        {
            Open = true;
            moduleList = new List<Node>();
            List<NodePort> pinList = new List<NodePort>();
            pinList.Add(new NodePort("输出端口Int", PortType.Out, 0));
            pinList.Add(new NodePort("输出端口Float", PortType.Out, 0f));
            pinList.Add(new NodePort("输出端口String", PortType.Out, "test"));
            pinList.Add(new NodePort("输入端口Int", PortType.In, 0));
            pinList.Add(new NodePort("输入端口Float", PortType.In, 0f));
            pinList.Add(new NodePort("输入端口String", PortType.In, "test"));
            Node test1 = new Node("测试模块", pinList);
            test1.Info = "This is Test Plugin Node 1";
            moduleList.Add(test1);
            List<NodePort> pinList2 = new List<NodePort>();
            pinList2.Add(new NodePort("SwitchButton - Int", PortType.Out, 0));
            pinList2.Add(new NodePort("TrackBar - Int", PortType.Out, 0));
            pinList2.Add(new NodePort("TrackBar - Float", PortType.Out, 0f));
            pinList2.Add(new NodePort("TextEditor - String", PortType.Out, "test"));
            Node test2 = new Node("测试模块2", pinList2);
            test2.Info = "This is Test Plugin Node 2";
            moduleList.Add(test2);
        }

        public void DefWndProc(int message)
        {

        }

        public void NodeCloseEvent(int mIndex)
        {
        }
        public void ValueChangeEvent(int mIndex, int pIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("TestPlugin - ValueChangeEvent : " + mIndex + " - " + pIndex);
        }
        public void OnButtonLeftClick(string id)
        {
            Console.WriteLine("TestPlugin - ButtonLeftClick : " + id);
            moduleList[1].NodePortList[3].ValueString = testString;
        }
        public void OnButtonRightClick(string id)
        {
            Console.WriteLine("TestPlugin - ButtonRightClick : " + id);
        }
        public void OnSwitchButtonChange(string id, bool value)
        {
            Console.WriteLine("TestPlugin - SwitchButtonChange : " + id + " - " + value);
            moduleList[1].NodePortList[0].ValueInt64 = value ? 1 : 0;
        }
        public void OnTextEditorChange(string id, string value)
        {
            Console.WriteLine("TestPlugin - TextEditorChange : " + id + " - " + value);
            testString = value;
        }
        public void OnTrackBarChange(string id, int value)
        {
            Console.WriteLine("TestPlugin - TrackBarChange : " + id + " - " + value);
            moduleList[1].NodePortList[1].ValueInt64 = value;
            moduleList[1].NodePortList[2].ValueDouble = value / 255f;
        }

        public void OnReceiveUdpMsg(EndPoint client, byte[] msg)
        {
            Console.WriteLine("TestPlugin - OnReceiveUdpMsg : " + msg + " - From : " + client.ToString());
        }
    }
}
