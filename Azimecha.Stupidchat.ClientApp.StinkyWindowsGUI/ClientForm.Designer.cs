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
            this.MainSplitLayout = new System.Windows.Forms.SplitContainer();
            this.ServersLayout = new System.Windows.Forms.TableLayoutPanel();
            this.ServersTreeView = new System.Windows.Forms.TreeView();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.ServerSplitLayout = new System.Windows.Forms.SplitContainer();
            this.ChannelSplitLayout = new System.Windows.Forms.SplitContainer();
            this.MessagesLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.SendingLayout = new System.Windows.Forms.TableLayoutPanel();
            this.MessageTextBox = new System.Windows.Forms.TextBox();
            this.SendMessageButton = new System.Windows.Forms.Button();
            this.MembersListView = new System.Windows.Forms.ListView();
            this.MainStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.MainStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.NewConnectionWorker = new System.ComponentModel.BackgroundWorker();
            this.SavedConnectionsWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitLayout)).BeginInit();
            this.MainSplitLayout.Panel1.SuspendLayout();
            this.MainSplitLayout.Panel2.SuspendLayout();
            this.MainSplitLayout.SuspendLayout();
            this.ServersLayout.SuspendLayout();
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
            this.MainStripContainer.SuspendLayout();
            this.MainStatusStrip.SuspendLayout();
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
            this.MainSplitLayout.Panel1.Controls.Add(this.ServersLayout);
            this.MainSplitLayout.Panel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.MainSplitLayout.Panel1MinSize = 80;
            // 
            // MainSplitLayout.Panel2
            // 
            this.MainSplitLayout.Panel2.Controls.Add(this.ServerSplitLayout);
            this.MainSplitLayout.Panel2MinSize = 200;
            this.MainSplitLayout.Size = new System.Drawing.Size(797, 462);
            this.MainSplitLayout.SplitterDistance = 186;
            this.MainSplitLayout.TabIndex = 0;
            // 
            // ServersLayout
            // 
            this.ServersLayout.ColumnCount = 1;
            this.ServersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ServersLayout.Controls.Add(this.ServersTreeView, 0, 1);
            this.ServersLayout.Controls.Add(this.ConnectButton, 0, 0);
            this.ServersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServersLayout.Location = new System.Drawing.Point(0, 0);
            this.ServersLayout.Name = "ServersLayout";
            this.ServersLayout.RowCount = 2;
            this.ServersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.ServersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ServersLayout.Size = new System.Drawing.Size(182, 458);
            this.ServersLayout.TabIndex = 1;
            // 
            // ServersTreeView
            // 
            this.ServersTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ServersTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServersTreeView.HotTracking = true;
            this.ServersTreeView.Location = new System.Drawing.Point(0, 28);
            this.ServersTreeView.Margin = new System.Windows.Forms.Padding(0);
            this.ServersTreeView.Name = "ServersTreeView";
            this.ServersTreeView.Size = new System.Drawing.Size(182, 430);
            this.ServersTreeView.TabIndex = 0;
            this.ServersTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ServersTreeView_NodeMouseClick);
            // 
            // ConnectButton
            // 
            this.ConnectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConnectButton.Location = new System.Drawing.Point(0, 0);
            this.ConnectButton.Margin = new System.Windows.Forms.Padding(0);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(182, 28);
            this.ConnectButton.TabIndex = 1;
            this.ConnectButton.Text = "Connect to Server";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
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
            this.ServerSplitLayout.Size = new System.Drawing.Size(607, 462);
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
            this.ChannelSplitLayout.Size = new System.Drawing.Size(444, 462);
            this.ChannelSplitLayout.SplitterDistance = 430;
            this.ChannelSplitLayout.TabIndex = 0;
            // 
            // MessagesLayout
            // 
            this.MessagesLayout.AutoScroll = true;
            this.MessagesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessagesLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.MessagesLayout.Location = new System.Drawing.Point(0, 0);
            this.MessagesLayout.Name = "MessagesLayout";
            this.MessagesLayout.Size = new System.Drawing.Size(440, 426);
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
            this.SendingLayout.Size = new System.Drawing.Size(440, 24);
            this.SendingLayout.TabIndex = 0;
            this.SendingLayout.Click += new System.EventHandler(this.SendingLayout_Click);
            // 
            // MessageTextBox
            // 
            this.MessageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MessageTextBox.Location = new System.Drawing.Point(4, 4);
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
            this.SendMessageButton.Size = new System.Drawing.Size(100, 24);
            this.SendMessageButton.TabIndex = 1;
            this.SendMessageButton.Text = "Send";
            this.SendMessageButton.UseVisualStyleBackColor = false;
            this.SendMessageButton.Click += new System.EventHandler(this.SendMessageButton_Click);
            // 
            // MembersListView
            // 
            this.MembersListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MembersListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MembersListView.Location = new System.Drawing.Point(0, 0);
            this.MembersListView.Name = "MembersListView";
            this.MembersListView.Size = new System.Drawing.Size(155, 458);
            this.MembersListView.TabIndex = 0;
            this.MembersListView.UseCompatibleStateImageBehavior = false;
            this.MembersListView.View = System.Windows.Forms.View.List;
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
            this.MainStripContainer.ContentPanel.Size = new System.Drawing.Size(797, 462);
            this.MainStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainStripContainer.Location = new System.Drawing.Point(0, 0);
            this.MainStripContainer.Name = "MainStripContainer";
            this.MainStripContainer.Size = new System.Drawing.Size(797, 484);
            this.MainStripContainer.TabIndex = 1;
            this.MainStripContainer.Text = "toolStripContainer1";
            this.MainStripContainer.TopToolStripPanelVisible = false;
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
            this.MainSplitLayout.Panel1.ResumeLayout(false);
            this.MainSplitLayout.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitLayout)).EndInit();
            this.MainSplitLayout.ResumeLayout(false);
            this.ServersLayout.ResumeLayout(false);
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
            this.MainStripContainer.ResumeLayout(false);
            this.MainStripContainer.PerformLayout();
            this.MainStatusStrip.ResumeLayout(false);
            this.MainStatusStrip.PerformLayout();
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
        private TableLayoutPanel ServersLayout;
        private Button ConnectButton;
        private ToolStripContainer MainStripContainer;
        private StatusStrip MainStatusStrip;
        private ToolStripStatusLabel MainStatusLabel;
        private System.ComponentModel.BackgroundWorker NewConnectionWorker;
        private FlowLayoutPanel MessagesLayout;
        private System.ComponentModel.BackgroundWorker SavedConnectionsWorker;
    }
}