namespace WcfServerMGR
{
  partial class frm_SvcMgr
  {
    /// <summary>
    /// 必需的设计器变量。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// 清理所有正在使用的资源。
    /// </summary>
    /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows 窗体设计器生成的代码

    /// <summary>
    /// 设计器支持所需的方法 - 不要
    /// 使用代码编辑器修改此方法的内容。
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStopServer = new System.Windows.Forms.Button();
            this.notFyIconSys = new System.Windows.Forms.NotifyIcon(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(213, 352);
            this.btnStartServer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(100, 29);
            this.btnStartServer.TabIndex = 0;
            this.btnStartServer.Text = "启动服务";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // btnStopServer
            // 
            this.btnStopServer.Enabled = false;
            this.btnStopServer.Location = new System.Drawing.Point(428, 352);
            this.btnStopServer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStopServer.Name = "btnStopServer";
            this.btnStopServer.Size = new System.Drawing.Size(100, 29);
            this.btnStopServer.TabIndex = 2;
            this.btnStopServer.Text = "停止服务";
            this.btnStopServer.UseVisualStyleBackColor = true;
            this.btnStopServer.Click += new System.EventHandler(this.btnStopServer_Click);
            // 
            // notFyIconSys
            // 
            this.notFyIconSys.Icon = global::WcfServerMGR.Properties.Resources.ServerIcon;
            this.notFyIconSys.Text = "安卓系统服务端";
            this.notFyIconSys.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notFyIconSys_MouseDoubleClick);
            // 
            // listView1
            // 
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(17, 15);
            this.listView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(844, 308);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            // 
            // frm_SvcMgr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 396);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btnStopServer);
            this.Controls.Add(this.btnStartServer);
            this.Icon = global::WcfServerMGR.Properties.Resources.ServerIcon;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(894, 432);
            this.Name = "frm_SvcMgr";
            this.Text = "数据服务平台";
            this.Load += new System.EventHandler(this.frm_SvcMgr_Load);
            this.SizeChanged += new System.EventHandler(this.frm_SvcMgr_SizeChanged);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnStartServer;
    private System.Windows.Forms.Button btnStopServer;
    private System.Windows.Forms.NotifyIcon notFyIconSys;
    private System.Windows.Forms.ListView listView1;
  }
}

