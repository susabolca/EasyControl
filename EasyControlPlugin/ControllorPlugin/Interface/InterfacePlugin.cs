using ControllorPlugin;
using System;
using System.Collections.Generic;
using System.Net;

public interface InterfacePlugin
{
    /// <summary>
    /// ID字符串（分配的GUID）
    /// </summary>
    string PluginID { get; }
    /// <summary>
    /// 插件是否启用
    /// </summary>
    bool Open { get; set; }
    /// <summary>
    /// 插件是否自动启动
    /// </summary>
    bool Auto { get; set; }
    /// <summary>
    /// 初始化
    /// </summary>
    void Init();
    /// <summary>
    /// 插件名称
    /// </summary>
    string GetName();
    /// <summary>
    /// 插件模块列表
    /// </summary>
    List<Node> GetModuleList();
    /// <summary>
    /// 自动开启插件功能
    /// </summary>
    void AutoOpen();
    /// <summary>
    /// 模块关闭时触发
    /// </summary>
    void NodeCloseEvent(int mIndex);
    /// <summary>
    /// 插件输入数据，当有数据更新时触发
    /// </summary>
    void ValueChangeEvent(int mIndex, int pIndex);
    /// <summary>
    /// 插件自身处理数据的循环
    /// </summary>
    void Update();
    /// <summary>
    /// 处理消息
    /// </summary>
    void DefWndProc(int message);
    #region 属性UI控制
    event EventHandler ButtonLeftClick;
    event EventHandler ButtonRightClick;
    event EventHandler SwitchButtonChange;
    event EventHandler TextEditorChange;
    event EventHandler TrackBarChange;
    /// <summary>
    /// 按钮左键点击事件
    /// </summary>
    void OnButtonLeftClick(string id);
    /// <summary>
    /// 按钮右键点击事件
    /// </summary>
    void OnButtonRightClick(string id);
    /// <summary>
    /// 开关按钮切换事件
    /// </summary>
    void OnSwitchButtonChange(string id, bool value);
    /// <summary>
    /// 文本输入框输入事件
    /// </summary>
    void OnTextEditorChange(string id, string value);
    /// <summary>
    /// 滑条滑动事件
    /// </summary>
    void OnTrackBarChange(string id, int value);
    #endregion
    #region UDP
    /// <summary>
    /// 创建UDP事件
    /// </summary>
    event EventHandler CreateUDP;
    event EventHandler SendUDP;
    /// <summary>
    /// UDP收到信息
    /// </summary>
    void OnReceiveUdpMsg(EndPoint client, byte[] msg);
    #endregion
}

