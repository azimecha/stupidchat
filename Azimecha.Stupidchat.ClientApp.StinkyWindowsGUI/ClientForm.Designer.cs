using System;
using System.Windows.Forms;

namespace Azimecha.Stupidchat.ClientApp.StinkyWindowsGUI {
    partial class ClientForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm));
            this.MainSplitLayout = new System.Windows.Forms.SplitContainer();
            this.ServersTreeView = new System.Windows.Forms.TreeView();
            this.ServerImageList = new System.Windows.Forms.ImageList(this.components);
            this.ServerSplitLayout = new System.Windows.Forms.SplitContainer();
            this.ChannelSplitLayout = new System.Windows.Forms.SplitContainer();
            this.MessagesLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.SendingLayout = new System.Windows.Forms.TableLayoutPanel();
            this.MessageTextBox = new System.Windows.Forms.TextBox();
            this.SendMessageButton = new System.Windows.Forms.Button();
            this.MembersListView = new System.Windows.Forms.ListView();
            this.UserImageList = new System.Windows.Forms.ImageList(this.components);
            this.MainStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.MainStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ServersToolStrip = new System.Windows.Forms.ToolStrip();
            this.ServerConnectButton = new System.Windows.Forms.ToolStripButton();
            this.ServerDisconnectButton = new System.Windows.Forms.ToolStripButton();
            this.ServerSetNickButton = new System.Windows.Forms.ToolStripButton();
            this.UserToolStrip = new System.Windows.Forms.ToolStrip();
            this.UserSetNameButton = new System.Windows.Forms.ToolStripButton();
            this.UserSetBioButton = new System.Windows.Forms.ToolStripButton();
            this.UserSetAvatarButton = new System.Windows.Forms.ToolStripButton();
            this.UserLogOutButton = new System.Windows.Forms.ToolStripButton();
            this.NewConnectionWorker = new System.ComponentModel.BackgroundWorker();
            this.SavedConnectionsWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitLayout)).BeginInit();
            this.MainSplitLayout.Panel1.SuspendLayout();
            this.MainSplitLayout.Panel2.SuspendLayout();
            this.MainSplitLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ServerSplitLayout)).BeginInit();
            this.ServerSplitLayout.Panel1.SuspendLayout();
            this.ServerSplitLayout.Panel2.SuspendLayout();
            this.ServerSplitLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChannelSplitLayout)).BeginInit();
            this.ChannelSplitLayout.Panel1.SuspendLayout();
            this.ChannelSplitLayout.Panel2.SuspendLayout();
            this.ChannelSplitLayout.SuspendLayout();
            this.SendingLayout.SuspendLayout();
            this.MainStripContainer.BottomToolStripPanel.SuspendLayout();
            this.MainStripContainer.ContentPanel.SuspendLayout();
            this.MainStripContainer.TopToolStripPanel.SuspendLayout();
            this.MainStripContainer.SuspendLayout();
            this.MainStatusStrip.SuspendLayout();
            this.ServersToolStrip.SuspendLayout();
            this.UserToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSplitLayout
            // 
            this.MainSplitLayout.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MainSplitLayout.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.MainSplitLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplitLayout.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MainSplitLayout.Location = new System.Drawing.Point(0, 0);
            this.MainSplitLayout.Name = "MainSplitLayout";
            // 
            // MainSplitLayout.Panel1
            // 
            this.MainSplitLayout.Panel1.Controls.Add(this.ServersTreeView);
            this.MainSplitLayout.Panel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.MainSplitLayout.Panel1MinSize = 80;
            // 
            // MainSplitLayout.Panel2
            // 
            this.MainSplitLayout.Panel2.Controls.Add(this.ServerSplitLayout);
            this.MainSplitLayout.Panel2MinSize = 200;
            this.MainSplitLayout.Size = new System.Drawing.Size(797, 437);
            this.MainSplitLayout.SplitterDistance = 186;
            this.MainSplitLayout.TabIndex = 0;
            // 
            // ServersTreeView
            // 
            this.ServersTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ServersTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServersTreeView.HotTracking = true;
            this.ServersTreeView.ImageIndex = 0;
            this.ServersTreeView.ImageList = this.ServerImageList;
            this.ServersTreeView.Location = new System.Drawing.Point(0, 0);
            this.ServersTreeView.Margin = new System.Windows.Forms.Padding(0);
            this.ServersTreeView.Name = "ServersTreeView";
            this.ServersTreeView.SelectedImageIndex = 0;
            this.ServersTreeView.Size = new System.Drawing.Size(182, 433);
            this.ServersTreeView.TabIndex = 0;
            this.ServersTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ServersTreeView_NodeMouseClick);
            // 
            // ServerImageList
            // 
            this.ServerImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ServerImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.ServerImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ServerSplitLayout
            // 
            this.ServerSplitLayout.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ServerSplitLayout.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.ServerSplitLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServerSplitLayout.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.ServerSplitLayout.Location = new System.Drawing.Point(0, 0);
            this.ServerSplitLayout.Name = "ServerSplitLayout";
            // 
            // ServerSplitLayout.Panel1
            // 
            this.ServerSplitLayout.Panel1.Controls.Add(this.ChannelSplitLayout);
            this.ServerSplitLayout.Panel1MinSize = 120;
            // 
            // ServerSplitLayout.Panel2
            // 
            this.ServerSplitLayout.Panel2.Controls.Add(this.MembersListView);
            this.ServerSplitLayout.Panel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ServerSplitLayout.Panel2MinSize = 80;
            this.ServerSplitLayout.Size = new System.Drawing.Size(607, 437);
            this.ServerSplitLayout.SplitterDistance = 444;
            this.ServerSplitLayout.TabIndex = 0;
            // 
            // ChannelSplitLayout
            // 
            this.ChannelSplitLayout.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ChannelSplitLayout.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.ChannelSplitLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChannelSplitLayout.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.ChannelSplitLayout.Location = new System.Drawing.Point(0, 0);
            this.ChannelSplitLayout.Name = "ChannelSplitLayout";
            this.ChannelSplitLayout.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ChannelSplitLayout.Panel1
            // 
            this.ChannelSplitLayout.Panel1.Controls.Add(this.MessagesLayout);
            this.ChannelSplitLayout.Panel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ChannelSplitLayout.Panel1MinSize = 120;
            // 
            // ChannelSplitLayout.Panel2
            // 
            this.ChannelSplitLayout.Panel2.Controls.Add(this.SendingLayout);
            this.ChannelSplitLayout.Panel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ChannelSplitLayout.Panel2MinSize = 20;
            this.ChannelSplitLayout.Size = new System.Drawing.Size(444, 437);
            this.ChannelSplitLayout.SplitterDistance = 406;
            this.ChannelSplitLayout.TabIndex = 0;
            // 
            // MessagesLayout
            // 
            this.MessagesLayout.AutoScroll = true;
            this.MessagesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessagesLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.MessagesLayout.Location = new System.Drawing.Point(0, 0);
            this.MessagesLayout.Name = "MessagesLayout";
            this.MessagesLayout.Size = new System.Drawing.Size(440, 402);
            this.MessagesLayout.TabIndex = 0;
            this.MessagesLayout.WrapContents = false;
            // 
            // SendingLayout
            // 
            this.SendingLayout.BackColor = System.Drawing.SystemColors.Window;
            this.SendingLayout.ColumnCount = 2;
            this.SendingLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SendingLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.SendingLayout.Controls.Add(this.MessageTextBox, 0, 0);
            this.SendingLayout.Controls.Add(this.SendMessageButton, 1, 0);
            this.SendingLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SendingLayout.Location = new System.Drawing.Point(0, 0);
            this.SendingLayout.Name = "SendingLayout";
            this.SendingLayout.RowCount = 1;
            this.SendingLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SendingLayout.Size = new System.Drawing.Size(440, 23);
            this.SendingLayout.TabIndex = 0;
            this.SendingLayout.Click += new System.EventHandler(this.SendingLayout_Click);
            // 
            // MessageTextBox
            // 
            this.MessageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MessageTextBox.Location = new System.Drawing.Point(4, 3);
            this.MessageTextBox.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MessageTextBox.Name = "MessageTextBox";
            this.MessageTextBox.Size = new System.Drawing.Size(332, 16);
            this.MessageTextBox.TabIndex = 0;
            this.MessageTextBox.TextChanged += new System.EventHandler(this.MessageTextBox_TextChanged);
            // 
            // SendMessageButton
            // 
            this.SendMessageButton.BackColor = System.Drawing.SystemColors.Control;
            this.SendMessageButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SendMessageButton.Enabled = false;
            this.SendMessageButton.Location = new System.Drawing.Point(340, 0);
            this.SendMessageButton.Margin = new System.Windows.Forms.Padding(0);
            this.SendMessageButton.Name = "SendMessageButton";
            this.SendMessageButton.Size = new System.Drawing.Size(100, 23);
            this.SendMessageButton.TabIndex = 1;
            this.SendMessageButton.Text = "Send";
            this.SendMessageButton.UseVisualStyleBackColor = false;
            this.SendMessageButton.Click += new System.EventHandler(this.SendMessageButton_Click);
            // 
            // MembersListView
            // 
            this.MembersListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MembersListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MembersListView.LargeImageList = this.UserImageList;
            this.MembersListView.Location = new System.Drawing.Point(0, 0);
            this.MembersListView.Name = "MembersListView";
            this.MembersListView.Size = new System.Drawing.Size(155, 433);
            this.MembersListView.TabIndex = 0;
            this.MembersListView.UseCompatibleStateImageBehavior = false;
            this.MembersListView.View = System.Windows.Forms.View.List;
            // 
            // UserImageList
            // 
            this.UserImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.UserImageList.ImageSize = new System.Drawing.Size(32, 32);
            this.UserImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // MainStripContainer
            // 
            // 
            // MainStripContainer.BottomToolStripPanel
            // 
            this.MainStripContainer.BottomToolStripPanel.Controls.Add(this.MainStatusStrip);
            // 
            // MainStripContainer.ContentPanel
            // 
            this.MainStripContainer.ContentPanel.AutoScroll = true;
            this.MainStripContainer.ContentPanel.Controls.Add(this.MainSplitLayout);
            this.MainStripContainer.ContentPanel.Size = new System.Drawing.Size(797, 437);
            this.MainStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainStripContainer.Location = new System.Drawing.Point(0, 0);
            this.MainStripContainer.Name = "MainStripContainer";
            this.MainStripContainer.Size = new System.Drawing.Size(797, 484);
            this.MainStripContainer.TabIndex = 1;
            this.MainStripContainer.Text = "toolStripContainer1";
            // 
            // MainStripContainer.TopToolStripPanel
            // 
            this.MainStripContainer.TopToolStripPanel.Controls.Add(this.UserToolStrip);
            this.MainStripContainer.TopToolStripPanel.Controls.Add(this.ServersToolStrip);
            // 
            // MainStatusStrip
            // 
            this.MainStatusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainStatusLabel});
            this.MainStatusStrip.Location = new System.Drawing.Point(0, 0);
            this.MainStatusStrip.Name = "MainStatusStrip";
            this.MainStatusStrip.Size = new System.Drawing.Size(797, 22);
            this.MainStatusStrip.TabIndex = 0;
            // 
            // MainStatusLabel
            // 
            this.MainStatusLabel.Name = "MainStatusLabel";
            this.MainStatusLabel.Size = new System.Drawing.Size(42, 17);
            this.MainStatusLabel.Text = "Ready.";
            // 
            // ServersToolStrip
            // 
            this.ServersToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.ServersToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServerConnectButton,
            this.ServerDisconnectButton,
            this.ServerSetNickButton});
            this.ServersToolStrip.Location = new System.Drawing.Point(3, 0);
            this.ServersToolStrip.Name = "ServersToolStrip";
            this.ServersToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ServersToolStrip.Size = new System.Drawing.Size(242, 25);
            this.ServersToolStrip.TabIndex = 0;
            // 
            // ServerConnectButton
            // 
            this.ServerConnectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ServerConnectButton.Image = ((System.Drawing.Image)(resources.GetObject("ServerConnectButton.Image")));
            this.ServerConnectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ServerConnectButton.Name = "ServerConnectButton";
            this.ServerConnectButton.Size = new System.Drawing.Size(56, 22);
            this.ServerConnectButton.Text = "Connect";
            this.ServerConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // ServerDisconnectButton
            // 
            this.ServerDisconnectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ServerDisconnectButton.Enabled = false;
            this.ServerDisconnectButton.Image = ((System.Drawing.Image)(resources.GetObject("ServerDisconnectButton.Image")));
            this.ServerDisconnectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ServerDisconnectButton.Name = "ServerDisconnectButton";
            this.ServerDisconnectButton.Size = new System.Drawing.Size(70, 22);
            this.ServerDisconnectButton.Text = "Disconnect";
            this.ServerDisconnectButton.Click += new System.EventHandler(this.ServerDisconnectButton_Click);
            // 
            // ServerSetNickButton
            // 
            this.ServerSetNickButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ServerSetNickButton.Enabled = false;
            this.ServerSetNickButton.Image = ((System.Drawing.Image)(resources.GetObject("ServerSetNickButton.Image")));
            this.ServerSetNickButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ServerSetNickButton.Name = "ServerSetNickButton";
            this.ServerSetNickButton.Size = new System.Drawing.Size(104, 22);
            this.ServerSetNickButton.Text = "Set My Nickname";
            this.ServerSetNickButton.Click += new System.EventHandler(this.ServerSetNickButton_Click);
            // 
            // UserToolStrip
            // 
            this.UserToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.UserToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UserSetNameButton,
            this.UserSetBioButton,
            this.UserSetAvatarButton,
            this.UserLogOutButton});
            this.UserToolStrip.Location = new System.Drawing.Point(253, 0);
            this.UserToolStrip.Name = "UserToolStrip";
            this.UserToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.UserToolStrip.Size = new System.Drawing.Size(280, 25);
            this.UserToolStrip.TabIndex = 1;
            // 
            // UserSetNameButton
            // 
            this.UserSetNameButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.UserSetNameButton.Image = ((System.Drawing.Image)(resources.GetObject("UserSetNameButton.Image")));
            this.UserSetNameButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UserSetNameButton.Name = "UserSetNameButton";
            this.UserSetNameButton.Size = new System.Drawing.Size(103, 22);
            this.UserSetNameButton.Text = "Set Display Name";
            this.UserSetNameButton.Click += new System.EventHandler(this.UserSetNameButton_Click);
            // 
            // UserSetBioButton
            // 
            this.UserSetBioButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.UserSetBioButton.Image = ((System.Drawing.Image)(resources.GetObject("UserSetBioButton.Image")));
            this.UserSetBioButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UserSetBioButton.Name = "UserSetBioButton";
            this.UserSetBioButton.Size = new System.Drawing.Size(47, 22);
            this.UserSetBioButton.Text = "Set Bio";
            this.UserSetBioButton.Click += new System.EventHandler(this.UserSetBioButton_Click);
            // 
            // UserSetAvatarButton
            // 
            this.UserSetAvatarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.UserSetAvatarButton.Image = ((System.Drawing.Image)(resources.GetObject("UserSetAvatarButton.Image")));
            this.UserSetAvatarButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UserSetAvatarButton.Name = "UserSetAvatarButton";
            this.UserSetAvatarButton.Size = new System.Drawing.Size(64, 22);
            this.UserSetAvatarButton.Text = "Set Avatar";
            this.UserSetAvatarButton.Click += new System.EventHandler(this.UserSetAvatarButton_Click);
            // 
            // UserLogOutButton
            // 
            this.UserLogOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.UserLogOutButton.Image = ((System.Drawing.Image)(resources.GetObject("UserLogOutButton.Image")));
            this.UserLogOutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UserLogOutButton.Name = "UserLogOutButton";
            this.UserLogOutButton.Size = new System.Drawing.Size(54, 22);
            this.UserLogOutButton.Text = "Log Out";
            this.UserLogOutButton.Click += new System.EventHandler(this.UserLogOutButton_Click);
            // 
            // NewConnectionWorker
            // 
            this.NewConnectionWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.NewConnectionWorker_DoWork);
            this.NewConnectionWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.NewConnectionWorker_RunWorkerCompleted);
            // 
            // SavedConnectionsWorker
            // 
            this.SavedConnectionsWorker.WorkerReportsProgress = true;
            this.SavedConnectionsWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SavedConnectionsWorker_DoWork);
            this.SavedConnectionsWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.SavedConnectionsWorker_ProgressChanged);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 484);
            this.Controls.Add(this.MainStripContainer);
            this.Name = "ClientForm";
            this.Text = "Chat Client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ClientForm_FormClosed);
            this.MainSplitLayout.Panel1.ResumeLayout(false);
            this.MainSplitLayout.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitLayout)).EndInit();
            this.MainSplitLayout.ResumeLayout(false);
            this.ServerSplitLayout.Panel1.ResumeLayout(false);
            this.ServerSplitLayout.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ServerSplitLayout)).EndInit();
            this.ServerSplitLayout.ResumeLayout(false);
            this.ChannelSplitLayout.Panel1.ResumeLayout(false);
            this.ChannelSplitLayout.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ChannelSplitLayout)).EndInit();
            this.ChannelSplitLayout.ResumeLayout(false);
            this.SendingLayout.ResumeLayout(false);
            this.SendingLayout.PerformLayout();
            this.MainStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.MainStripContainer.BottomToolStripPanel.PerformLayout();
            this.MainStripContainer.ContentPanel.ResumeLayout(false);
            this.MainStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.MainStripContainer.TopToolStripPanel.PerformLayout();
            this.MainStripContainer.ResumeLayout(false);
            this.MainStripContainer.PerformLayout();
            this.MainStatusStrip.ResumeLayout(false);
            this.MainStatusStrip.PerformLayout();
            this.ServersToolStrip.ResumeLayout(false);
            this.ServersToolStrip.PerformLayout();
            this.UserToolStrip.ResumeLayout(false);
            this.UserToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer MainSplitLayout;
        private SplitContainer ServerSplitLayout;
        private TreeView ServersTreeView;
        private ListView MembersListView;
        private SplitContainer ChannelSplitLayout;
        private TableLayoutPanel SendingLayout;
        private TextBox MessageTextBox;
        private Button SendMessageButton;
        private ToolStripContainer MainStripContainer;
        private StatusStrip MainStatusStrip;
        private ToolStripStatusLabel MainStatusLabel;
        private System.ComponentModel.BackgroundWorker NewConnectionWorker;
        private FlowLayoutPanel MessagesLayout;
        private System.ComponentModel.BackgroundWorker SavedConnectionsWorker;
        private ToolStrip UserToolStrip;
        private ToolStripButton UserSetNameButton;
        private ToolStripButton UserSetBioButton;
        private ToolStripButton UserSetAvatarButton;
        private ToolStrip ServersToolStrip;
        private ToolStripButton ServerConnectButton;
        private ToolStripButton ServerDisconnectButton;
        private ToolStripButton ServerSetNickButton;
        private ToolStripButton UserLogOutButton;
        private ImageList UserImageList;
        private ImageList ServerImageList;
    }
}