using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using WcfService;
using System.ServiceModel.Description;

namespace WcfServerMGR
{
  public partial class frm_SvcMgr : Form
  {
    private ServiceHost SimpDbServer
    {
      get
      {
        if (simpDbServer == null || simpDbServer.State == CommunicationState.Closed)
        {
          simpDbServer = null;
          simpDbServer = new ServiceHost(typeof(WcfService.SimpDbServer));

        }
        return simpDbServer;
      }
    } 
    private ServiceHost simpDbServer = null;

    private ServiceHost JsonDbServer
    {
        get
        {
            if (jsonDbServer == null || jsonDbServer.State == CommunicationState.Closed)
            {
                jsonDbServer = null;
                jsonDbServer = new ServiceHost(typeof(WcfService.JsonDbServer));

            }
            return jsonDbServer;
        }
    }
    private ServiceHost jsonDbServer = null;

    
    private ServiceHost FileServer
    {
      get
      {
        if (fileServer == null || fileServer.State == CommunicationState.Closed)
        {
          fileServer = null;
          fileServer = new ServiceHost(typeof(WcfService.FileServer));
        }
        return fileServer;
      }
    }
    private ServiceHost fileServer = null;


    private ServiceHost BaseDataServer
    {
      get
      {
        if (baseDataServer == null || baseDataServer.State == CommunicationState.Closed)
        {
          baseDataServer = null;
          baseDataServer = new ServiceHost(typeof(BaseDataCacheServer.BaseDataServer));
        }
        return baseDataServer;
      }
    }
    private ServiceHost baseDataServer = null;

    private const string SUDID_KEY_REGE = "MF7097G06704851-BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A";
    public frm_SvcMgr()
    {
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        MessageBox.Show("运行失败,当前版本未注册,请联系开发商.");
        throw (new Exception("非法运行,未注册."));
      }
#endif
      InitializeComponent();
    }

    private void btnStartServer_Click(object sender, EventArgs e)
    {
      listView1.Items.Clear();
      try
      {
        SimpDbServer.Open();
        listView1.Items.Add(new ListViewItem(string.Format("-------------------SimpDbServer 服务启动成功-------------------------")));  
        AddServerInfo(SimpDbServer.Description.Endpoints);

        JsonDbServer.Open();
        listView1.Items.Add(new ListViewItem(string.Format("-------------------JsonDbServer 服务启动成功-------------------------")));
        AddServerInfo(JsonDbServer.Description.Endpoints);

        FileServer.Open();
        listView1.Items.Add(new ListViewItem(string.Format("-------------------FileServer 服务启动成功-------------------------")));
        AddServerInfo(FileServer.Description.Endpoints);

        BaseDataServer.Open();
        listView1.Items.Add(new ListViewItem(string.Format("-------------------BaseDataServer 服务启动成功-------------------------")));
        AddServerInfo(BaseDataServer.Description.Endpoints);
        btnStopServer.Enabled = true;
        btnStartServer.Enabled = false;
      }
      catch (Exception err)
      {
        listView1.Items.Add(new ListViewItem(string.Format("服务启动出错：{0}", err)));
      }
    }

    private void AddServerInfo(System.ServiceModel.Description.ServiceEndpointCollection endpoints)
    {
      for (int _i = 0, _iCnt = endpoints.Count; _i < _iCnt; _i++)
      {
        listView1.Items.Add(new ListViewItem(
                    string.Format("Binding类型：{0}   端口：{1}   Uri：{2}",
                                    endpoints[_i].Binding.Name,
                                    endpoints[_i].Address.Uri.Port,
                                    endpoints[_i].ListenUri)));
      }
    }

    private void btnStopServer_Click(object sender, EventArgs e)
    {
      listView1.Items.Clear();
      try
      {
        SimpDbServer.Abort();
        listView1.Items.Add(new ListViewItem("SimpDbServer 服务关闭"));

        JsonDbServer.Abort();
        listView1.Items.Add(new ListViewItem("JsonDbServer 服务关闭"));

        FileServer.Abort();
        listView1.Items.Add(new ListViewItem("FileServer 服务关闭"));

        BaseDataServer.Abort();
        listView1.Items.Add(new ListViewItem("BaseDataServer 服务关闭"));

        btnStartServer.Enabled = true;
        btnStopServer.Enabled = false;
      }
      catch (Exception err)
      {
        listView1.Items.Add(new ListViewItem(string.Format("服务关闭出错：{0}", err)));
      }
    }

    private void frm_SvcMgr_SizeChanged(object sender, EventArgs e)
    {
      if (this.WindowState == FormWindowState.Minimized) //判断是否最小化
      {
        this.Visible =
        this.ShowInTaskbar = false; //不显示在系统任务栏
        notFyIconSys.Visible = true; //托盘图标可见
      }
    }

    private void notFyIconSys_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (this.WindowState == FormWindowState.Minimized)
      {
        this.Visible =
        this.ShowInTaskbar = true; //显示在系统任务栏
        this.WindowState = FormWindowState.Normal; //还原窗体
        notFyIconSys.Visible = false; //托盘图标隐藏
      }
    }

    private void frm_SvcMgr_Load(object sender, EventArgs e)
    {
        btnStartServer_Click(null, null);
    }

    private void listView1_KeyDown(object sender, KeyEventArgs e)
    {
        if (!e.Control || e.KeyCode != Keys.C) return;
        if (listView1.SelectedItems.Count <= 0) return;
        if (listView1.SelectedItems[0].Text!="")
        {
            Clipboard.SetDataObject(listView1.SelectedItems[0].Text);
        }
    }
  }
}
