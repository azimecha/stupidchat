using System.Windows.Forms;

namespace Azimecha.Stupidchat.ServerApp.WindowsGUI {
    partial class ServerForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerForm));
            this.BottomStatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabelLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.TopToolStrip = new System.Windows.Forms.ToolStrip();
            this.ServerInfoDropdown = new System.Windows.Forms.ToolStripDropDownButton();
            this.ServerNameItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ServerDescItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ServerIconItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartStopButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MemberPowerDropdown = new System.Windows.Forms.ToolStripDropDownButton();
            this.PowerReducedItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PowerNormalItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PowerModeratorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PowerAdministratorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MemberRemoveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ModifyChannelDropdown = new System.Windows.Forms.ToolStripDropDownButton();
            this.ChannelNameItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChannelDescriptionItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChannelDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.LogClearButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.MessageDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.RefreshButton = new System.Windows.Forms.ToolStripButton();
            this.MainSplit = new System.Windows.Forms.SplitContainer();
            this.LeftSplit = new System.Windows.Forms.SplitContainer();
            this.UsersLayout = new System.Windows.Forms.TableLayoutPanel();
            this.MembersLabel = new System.Windows.Forms.Label();
            this.MembersList = new System.Windows.Forms.ListBox();
            this.ChannelsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.ChannelsList = new System.Windows.Forms.ListBox();
            this.ChannelsLabel = new System.Windows.Forms.Label();
            this.RightSplit = new System.Windows.Forms.SplitContainer();
            this.LogMessagesLayout = new System.Windows.Forms.TableLayoutPanel();
            this.LogLabel = new System.Windows.Forms.Label();
            this.LogMessageList = new System.Windows.Forms.ListBox();
            this.ChannelMessagesLayout = new System.Windows.Forms.TableLayoutPanel();
            this.MessagesLabel = new System.Windows.Forms.Label();
            this.ChannelMessagesList = new System.Windows.Forms.ListBox();
            this.CreateChannelDropdown = new System.Windows.Forms.ToolStripDropDownButton();
            this.CreateTextChannelItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateVoiceChannelItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomStatusStrip.SuspendLayout();
            this.TopToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplit)).BeginInit();
            this.MainSplit.Panel1.SuspendLayout();
            this.MainSplit.Panel2.SuspendLayout();
            this.MainSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LeftSplit)).BeginInit();
            this.LeftSplit.Panel1.SuspendLayout();
            this.LeftSplit.Panel2.SuspendLayout();
            this.LeftSplit.SuspendLayout();
            this.UsersLayout.SuspendLayout();
            this.ChannelsLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RightSplit)).BeginInit();
            this.RightSplit.Panel1.SuspendLayout();
            this.RightSplit.Panel2.SuspendLayout();
            this.RightSplit.SuspendLayout();
            this.LogMessagesLayout.SuspendLayout();
            this.ChannelMessagesLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomStatusStrip
            // 
            this.BottomStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelLabel,
            this.StatusLabel});
            this.BottomStatusStrip.Location = new System.Drawing.Point(0, 521);
            this.BottomStatusStrip.Name = "BottomStatusStrip";
            this.BottomStatusStrip.Size = new System.Drawing.Size(933, 22);
            this.BottomStatusStrip.TabIndex = 0;
            this.BottomStatusStrip.Text = "statusStrip1";
            // 
            // StatusLabelLabel
            // 
            this.StatusLabelLabel.Name = "StatusLabelLabel";
            this.StatusLabelLabel.Size = new System.Drawing.Size(76, 17);
            this.StatusLabelLabel.Text = "Server status:";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(72, 17);
            this.StatusLabel.Text = "Not running";
            // 
            // TopToolStrip
            // 
            this.TopToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServerInfoDropdown,
            this.StartStopButton,
            this.toolStripSeparator1,
            this.MemberPowerDropdown,
            this.MemberRemoveButton,
            this.toolStripSeparator2,
            this.CreateChannelDropdown,
            this.ModifyChannelDropdown,
            this.ChannelDeleteButton,
            this.toolStripSeparator3,
            this.LogClearButton,
            this.toolStripSeparator4,
            this.MessageDeleteButton,
            this.RefreshButton});
            this.TopToolStrip.Location = new System.Drawing.Point(0, 0);
            this.TopToolStrip.Name = "TopToolStrip";
            this.TopToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.TopToolStrip.Size = new System.Drawing.Size(933, 25);
            this.TopToolStrip.TabIndex = 1;
            this.TopToolStrip.Text = "toolStrip1";
            // 
            // ServerInfoDropdown
            // 
            this.ServerInfoDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ServerInfoDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServerNameItem,
            this.ServerDescItem,
            this.ServerIconItem});
            this.ServerInfoDropdown.Image = ((System.Drawing.Image)(resources.GetObject("ServerInfoDropdown.Image")));
            this.ServerInfoDropdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ServerInfoDropdown.Name = "ServerInfoDropdown";
            this.ServerInfoDropdown.Size = new System.Drawing.Size(95, 22);
            this.ServerInfoDropdown.Text = "Set Server Info";
            // 
            // ServerNameItem
            // 
            this.ServerNameItem.Name = "ServerNameItem";
            this.ServerNameItem.Size = new System.Drawing.Size(134, 22);
            this.ServerNameItem.Text = "Name";
            this.ServerNameItem.Click += new System.EventHandler(this.ServerNameItem_Click);
            // 
            // ServerDescItem
            // 
            this.ServerDescItem.Name = "ServerDescItem";
            this.ServerDescItem.Size = new System.Drawing.Size(134, 22);
            this.ServerDescItem.Text = "Description";
            this.ServerDescItem.Click += new System.EventHandler(this.ServerDescItem_Click);
            // 
            // ServerIconItem
            // 
            this.ServerIconItem.Name = "ServerIconItem";
            this.ServerIconItem.Size = new System.Drawing.Size(134, 22);
            this.ServerIconItem.Text = "Icon URL";
            this.ServerIconItem.Click += new System.EventHandler(this.ServerIconItem_Click);
            // 
            // StartStopButton
            // 
            this.StartStopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StartStopButton.Image = ((System.Drawing.Image)(resources.GetObject("StartStopButton.Image")));
            this.StartStopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StartStopButton.Name = "StartStopButton";
            this.StartStopButton.Size = new System.Drawing.Size(70, 22);
            this.StartStopButton.Text = "Start Server";
            this.StartStopButton.Click += new System.EventHandler(this.StartStopButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // MemberPowerDropdown
            // 
            this.MemberPowerDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MemberPowerDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PowerReducedItem,
            this.PowerNormalItem,
            this.PowerModeratorItem,
            this.PowerAdministratorItem});
            this.MemberPowerDropdown.Enabled = false;
            this.MemberPowerDropdown.Image = ((System.Drawing.Image)(resources.GetObject("MemberPowerDropdown.Image")));
            this.MemberPowerDropdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MemberPowerDropdown.Name = "MemberPowerDropdown";
            this.MemberPowerDropdown.Size = new System.Drawing.Size(101, 22);
            this.MemberPowerDropdown.Text = "Member Power";
            // 
            // PowerReducedItem
            // 
            this.PowerReducedItem.Name = "PowerReducedItem";
            this.PowerReducedItem.Size = new System.Drawing.Size(147, 22);
            this.PowerReducedItem.Text = "Reduced";
            this.PowerReducedItem.Click += new System.EventHandler(this.PowerReducedItem_Click);
            // 
            // PowerNormalItem
            // 
            this.PowerNormalItem.Name = "PowerNormalItem";
            this.PowerNormalItem.Size = new System.Drawing.Size(147, 22);
            this.PowerNormalItem.Text = "Normal";
            this.PowerNormalItem.Click += new System.EventHandler(this.PowerNormalItem_Click);
            // 
            // PowerModeratorItem
            // 
            this.PowerModeratorItem.Name = "PowerModeratorItem";
            this.PowerModeratorItem.Size = new System.Drawing.Size(147, 22);
            this.PowerModeratorItem.Text = "Moderator";
            this.PowerModeratorItem.Click += new System.EventHandler(this.PowerModeratorItem_Click);
            // 
            // PowerAdministratorItem
            // 
            this.PowerAdministratorItem.Name = "PowerAdministratorItem";
            this.PowerAdministratorItem.Size = new System.Drawing.Size(147, 22);
            this.PowerAdministratorItem.Text = "Administrator";
            this.PowerAdministratorItem.Click += new System.EventHandler(this.PowerAdministratorItem_Click);
            // 
            // MemberRemoveButton
            // 
            this.MemberRemoveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MemberRemoveButton.Enabled = false;
            this.MemberRemoveButton.Image = ((System.Drawing.Image)(resources.GetObject("MemberRemoveButton.Image")));
            this.MemberRemoveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MemberRemoveButton.Name = "MemberRemoveButton";
            this.MemberRemoveButton.Size = new System.Drawing.Size(102, 22);
            this.MemberRemoveButton.Text = "Remove Member";
            this.MemberRemoveButton.Click += new System.EventHandler(this.MemberRemoveButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // ModifyChannelDropdown
            // 
            this.ModifyChannelDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ModifyChannelDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChannelNameItem,
            this.ChannelDescriptionItem});
            this.ModifyChannelDropdown.Enabled = false;
            this.ModifyChannelDropdown.Image = ((System.Drawing.Image)(resources.GetObject("ModifyChannelDropdown.Image")));
            this.ModifyChannelDropdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ModifyChannelDropdown.Name = "ModifyChannelDropdown";
            this.ModifyChannelDropdown.Size = new System.Drawing.Size(105, 22);
            this.ModifyChannelDropdown.Text = "Modify Channel";
            // 
            // ChannelNameItem
            // 
            this.ChannelNameItem.Name = "ChannelNameItem";
            this.ChannelNameItem.Size = new System.Drawing.Size(180, 22);
            this.ChannelNameItem.Text = "Name";
            this.ChannelNameItem.Click += new System.EventHandler(this.ChannelNameItem_Click);
            // 
            // ChannelDescriptionItem
            // 
            this.ChannelDescriptionItem.Name = "ChannelDescriptionItem";
            this.ChannelDescriptionItem.Size = new System.Drawing.Size(180, 22);
            this.ChannelDescriptionItem.Text = "Description";
            this.ChannelDescriptionItem.Click += new System.EventHandler(this.ChannelDescriptionItem_Click);
            // 
            // ChannelDeleteButton
            // 
            this.ChannelDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ChannelDeleteButton.Enabled = false;
            this.ChannelDeleteButton.Image = ((System.Drawing.Image)(resources.GetObject("ChannelDeleteButton.Image")));
            this.ChannelDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ChannelDeleteButton.Name = "ChannelDeleteButton";
            this.ChannelDeleteButton.Size = new System.Drawing.Size(91, 22);
            this.ChannelDeleteButton.Text = "Delete Channel";
            this.ChannelDeleteButton.Click += new System.EventHandler(this.ChannelDeleteButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // LogClearButton
            // 
            this.LogClearButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.LogClearButton.Image = ((System.Drawing.Image)(resources.GetObject("LogClearButton.Image")));
            this.LogClearButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.LogClearButton.Name = "LogClearButton";
            this.LogClearButton.Size = new System.Drawing.Size(61, 22);
            this.LogClearButton.Text = "Clear Log";
            this.LogClearButton.Click += new System.EventHandler(this.LogClearButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // MessageDeleteButton
            // 
            this.MessageDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MessageDeleteButton.Enabled = false;
            this.MessageDeleteButton.Image = ((System.Drawing.Image)(resources.GetObject("MessageDeleteButton.Image")));
            this.MessageDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MessageDeleteButton.Name = "MessageDeleteButton";
            this.MessageDeleteButton.Size = new System.Drawing.Size(93, 22);
            this.MessageDeleteButton.Text = "Delete Message";
            this.MessageDeleteButton.Click += new System.EventHandler(this.MessageDeleteButton_Click);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.RefreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.RefreshButton.Image = ((System.Drawing.Image)(resources.GetObject("RefreshButton.Image")));
            this.RefreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(50, 22);
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // MainSplit
            // 
            this.MainSplit.BackColor = System.Drawing.SystemColors.Window;
            this.MainSplit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MainSplit.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.MainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplit.Location = new System.Drawing.Point(0, 25);
            this.MainSplit.Name = "MainSplit";
            // 
            // MainSplit.Panel1
            // 
            this.MainSplit.Panel1.Controls.Add(this.LeftSplit);
            // 
            // MainSplit.Panel2
            // 
            this.MainSplit.Panel2.Controls.Add(this.RightSplit);
            this.MainSplit.Size = new System.Drawing.Size(933, 496);
            this.MainSplit.SplitterDistance = 310;
            this.MainSplit.TabIndex = 2;
            // 
            // LeftSplit
            // 
            this.LeftSplit.BackColor = System.Drawing.SystemColors.Window;
            this.LeftSplit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LeftSplit.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.LeftSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LeftSplit.Location = new System.Drawing.Point(0, 0);
            this.LeftSplit.Name = "LeftSplit";
            this.LeftSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // LeftSplit.Panel1
            // 
            this.LeftSplit.Panel1.Controls.Add(this.UsersLayout);
            // 
            // LeftSplit.Panel2
            // 
            this.LeftSplit.Panel2.Controls.Add(this.ChannelsLayout);
            this.LeftSplit.Size = new System.Drawing.Size(310, 496);
            this.LeftSplit.SplitterDistance = 249;
            this.LeftSplit.TabIndex = 0;
            // 
            // UsersLayout
            // 
            this.UsersLayout.ColumnCount = 1;
            this.UsersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.UsersLayout.Controls.Add(this.MembersLabel, 0, 0);
            this.UsersLayout.Controls.Add(this.MembersList, 0, 1);
            this.UsersLayout.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.UsersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UsersLayout.Location = new System.Drawing.Point(0, 0);
            this.UsersLayout.Name = "UsersLayout";
            this.UsersLayout.RowCount = 2;
            this.UsersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.UsersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.UsersLayout.Size = new System.Drawing.Size(306, 245);
            this.UsersLayout.TabIndex = 0;
            // 
            // MembersLabel
            // 
            this.MembersLabel.BackColor = System.Drawing.SystemColors.Control;
            this.MembersLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MembersLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MembersLabel.Location = new System.Drawing.Point(0, 0);
            this.MembersLabel.Margin = new System.Windows.Forms.Padding(0);
            this.MembersLabel.Name = "MembersLabel";
            this.MembersLabel.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.MembersLabel.Size = new System.Drawing.Size(306, 24);
            this.MembersLabel.TabIndex = 0;
            this.MembersLabel.Text = "Members";
            this.MembersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MembersList
            // 
            this.MembersList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MembersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MembersList.FormattingEnabled = true;
            this.MembersList.ItemHeight = 15;
            this.MembersList.Location = new System.Drawing.Point(0, 24);
            this.MembersList.Margin = new System.Windows.Forms.Padding(0);
            this.MembersList.Name = "MembersList";
            this.MembersList.Size = new System.Drawing.Size(306, 221);
            this.MembersList.TabIndex = 1;
            this.MembersList.SelectedIndexChanged += new System.EventHandler(this.MembersList_SelectedIndexChanged);
            this.MembersList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MembersList_MouseDoubleClick);
            // 
            // ChannelsLayout
            // 
            this.ChannelsLayout.ColumnCount = 1;
            this.ChannelsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChannelsLayout.Controls.Add(this.ChannelsList, 0, 1);
            this.ChannelsLayout.Controls.Add(this.ChannelsLabel, 0, 0);
            this.ChannelsLayout.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ChannelsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChannelsLayout.Location = new System.Drawing.Point(0, 0);
            this.ChannelsLayout.Name = "ChannelsLayout";
            this.ChannelsLayout.RowCount = 2;
            this.ChannelsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.ChannelsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChannelsLayout.Size = new System.Drawing.Size(306, 239);
            this.ChannelsLayout.TabIndex = 1;
            // 
            // ChannelsList
            // 
            this.ChannelsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChannelsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChannelsList.FormattingEnabled = true;
            this.ChannelsList.ItemHeight = 15;
            this.ChannelsList.Location = new System.Drawing.Point(0, 24);
            this.ChannelsList.Margin = new System.Windows.Forms.Padding(0);
            this.ChannelsList.Name = "ChannelsList";
            this.ChannelsList.Size = new System.Drawing.Size(306, 215);
            this.ChannelsList.TabIndex = 2;
            this.ChannelsList.SelectedIndexChanged += new System.EventHandler(this.ChannelsList_SelectedIndexChanged);
            this.ChannelsList.SelectedValueChanged += new System.EventHandler(this.ChannelsList_SelectedValueChanged);
            this.ChannelsList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ChannelsList_MouseDoubleClick);
            // 
            // ChannelsLabel
            // 
            this.ChannelsLabel.BackColor = System.Drawing.SystemColors.Control;
            this.ChannelsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ChannelsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChannelsLabel.Location = new System.Drawing.Point(0, 0);
            this.ChannelsLabel.Margin = new System.Windows.Forms.Padding(0);
            this.ChannelsLabel.Name = "ChannelsLabel";
            this.ChannelsLabel.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.ChannelsLabel.Size = new System.Drawing.Size(306, 24);
            this.ChannelsLabel.TabIndex = 1;
            this.ChannelsLabel.Text = "Channels";
            this.ChannelsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RightSplit
            // 
            this.RightSplit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RightSplit.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.RightSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightSplit.Location = new System.Drawing.Point(0, 0);
            this.RightSplit.Name = "RightSplit";
            this.RightSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // RightSplit.Panel1
            // 
            this.RightSplit.Panel1.Controls.Add(this.LogMessagesLayout);
            // 
            // RightSplit.Panel2
            // 
            this.RightSplit.Panel2.Controls.Add(this.ChannelMessagesLayout);
            this.RightSplit.Size = new System.Drawing.Size(619, 496);
            this.RightSplit.SplitterDistance = 216;
            this.RightSplit.TabIndex = 0;
            // 
            // LogMessagesLayout
            // 
            this.LogMessagesLayout.ColumnCount = 1;
            this.LogMessagesLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LogMessagesLayout.Controls.Add(this.LogLabel, 0, 0);
            this.LogMessagesLayout.Controls.Add(this.LogMessageList, 0, 1);
            this.LogMessagesLayout.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.LogMessagesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogMessagesLayout.Location = new System.Drawing.Point(0, 0);
            this.LogMessagesLayout.Name = "LogMessagesLayout";
            this.LogMessagesLayout.RowCount = 2;
            this.LogMessagesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.LogMessagesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LogMessagesLayout.Size = new System.Drawing.Size(615, 212);
            this.LogMessagesLayout.TabIndex = 1;
            // 
            // LogLabel
            // 
            this.LogLabel.BackColor = System.Drawing.SystemColors.Control;
            this.LogLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LogLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogLabel.Location = new System.Drawing.Point(0, 0);
            this.LogLabel.Margin = new System.Windows.Forms.Padding(0);
            this.LogLabel.Name = "LogLabel";
            this.LogLabel.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LogLabel.Size = new System.Drawing.Size(615, 24);
            this.LogLabel.TabIndex = 0;
            this.LogLabel.Text = "Log";
            this.LogLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LogMessageList
            // 
            this.LogMessageList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LogMessageList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogMessageList.FormattingEnabled = true;
            this.LogMessageList.ItemHeight = 15;
            this.LogMessageList.Location = new System.Drawing.Point(0, 24);
            this.LogMessageList.Margin = new System.Windows.Forms.Padding(0);
            this.LogMessageList.Name = "LogMessageList";
            this.LogMessageList.Size = new System.Drawing.Size(615, 188);
            this.LogMessageList.TabIndex = 1;
            this.LogMessageList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LogMessageList_MouseDoubleClick);
            // 
            // ChannelMessagesLayout
            // 
            this.ChannelMessagesLayout.ColumnCount = 1;
            this.ChannelMessagesLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChannelMessagesLayout.Controls.Add(this.MessagesLabel, 0, 0);
            this.ChannelMessagesLayout.Controls.Add(this.ChannelMessagesList, 0, 1);
            this.ChannelMessagesLayout.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ChannelMessagesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChannelMessagesLayout.Location = new System.Drawing.Point(0, 0);
            this.ChannelMessagesLayout.Name = "ChannelMessagesLayout";
            this.ChannelMessagesLayout.RowCount = 2;
            this.ChannelMessagesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.ChannelMessagesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChannelMessagesLayout.Size = new System.Drawing.Size(615, 272);
            this.ChannelMessagesLayout.TabIndex = 1;
            // 
            // MessagesLabel
            // 
            this.MessagesLabel.BackColor = System.Drawing.SystemColors.Control;
            this.MessagesLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MessagesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessagesLabel.Location = new System.Drawing.Point(0, 0);
            this.MessagesLabel.Margin = new System.Windows.Forms.Padding(0);
            this.MessagesLabel.Name = "MessagesLabel";
            this.MessagesLabel.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.MessagesLabel.Size = new System.Drawing.Size(615, 24);
            this.MessagesLabel.TabIndex = 0;
            this.MessagesLabel.Text = "Messages (select channel to display)";
            this.MessagesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ChannelMessagesList
            // 
            this.ChannelMessagesList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChannelMessagesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChannelMessagesList.FormattingEnabled = true;
            this.ChannelMessagesList.ItemHeight = 15;
            this.ChannelMessagesList.Location = new System.Drawing.Point(0, 24);
            this.ChannelMessagesList.Margin = new System.Windows.Forms.Padding(0);
            this.ChannelMessagesList.Name = "ChannelMessagesList";
            this.ChannelMessagesList.Size = new System.Drawing.Size(615, 248);
            this.ChannelMessagesList.TabIndex = 1;
            this.ChannelMessagesList.SelectedIndexChanged += new System.EventHandler(this.ChannelMessagesList_SelectedIndexChanged);
            this.ChannelMessagesList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ChannelMessagesList_MouseDoubleClick);
            // 
            // CreateChannelDropdown
            // 
            this.CreateChannelDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.CreateChannelDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateTextChannelItem,
            this.CreateVoiceChannelItem});
            this.CreateChannelDropdown.Image = ((System.Drawing.Image)(resources.GetObject("CreateChannelDropdown.Image")));
            this.CreateChannelDropdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CreateChannelDropdown.Name = "CreateChannelDropdown";
            this.CreateChannelDropdown.Size = new System.Drawing.Size(101, 22);
            this.CreateChannelDropdown.Text = "Create Channel";
            // 
            // CreateTextChannelItem
            // 
            this.CreateTextChannelItem.Name = "CreateTextChannelItem";
            this.CreateTextChannelItem.Size = new System.Drawing.Size(180, 22);
            this.CreateTextChannelItem.Text = "Text";
            this.CreateTextChannelItem.Click += new System.EventHandler(this.CreateTextChannelItem_Click);
            // 
            // CreateVoiceChannelItem
            // 
            this.CreateVoiceChannelItem.Name = "CreateVoiceChannelItem";
            this.CreateVoiceChannelItem.Size = new System.Drawing.Size(180, 22);
            this.CreateVoiceChannelItem.Text = "Voice";
            this.CreateVoiceChannelItem.Click += new System.EventHandler(this.CreateVoiceChannelItem_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 543);
            this.Controls.Add(this.MainSplit);
            this.Controls.Add(this.TopToolStrip);
            this.Controls.Add(this.BottomStatusStrip);
            this.Name = "ServerForm";
            this.Text = "Chat Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.BottomStatusStrip.ResumeLayout(false);
            this.BottomStatusStrip.PerformLayout();
            this.TopToolStrip.ResumeLayout(false);
            this.TopToolStrip.PerformLayout();
            this.MainSplit.Panel1.ResumeLayout(false);
            this.MainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplit)).EndInit();
            this.MainSplit.ResumeLayout(false);
            this.LeftSplit.Panel1.ResumeLayout(false);
            this.LeftSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LeftSplit)).EndInit();
            this.LeftSplit.ResumeLayout(false);
            this.UsersLayout.ResumeLayout(false);
            this.ChannelsLayout.ResumeLayout(false);
            this.RightSplit.Panel1.ResumeLayout(false);
            this.RightSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RightSplit)).EndInit();
            this.RightSplit.ResumeLayout(false);
            this.LogMessagesLayout.ResumeLayout(false);
            this.ChannelMessagesLayout.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private StatusStrip BottomStatusStrip;
        private ToolStripStatusLabel StatusLabelLabel;
        private ToolStripStatusLabel StatusLabel;
        private ToolStrip TopToolStrip;
        private ToolStripButton StartStopButton;
        private SplitContainer MainSplit;
        private SplitContainer LeftSplit;
        private TableLayoutPanel UsersLayout;
        private TableLayoutPanel ChannelsLayout;
        private Label MembersLabel;
        private ListBox MembersList;
        private ListBox ChannelsList;
        private Label ChannelsLabel;
        private ToolStripButton ChannelDeleteButton;
        private SplitContainer RightSplit;
        private TableLayoutPanel LogMessagesLayout;
        private Label LogLabel;
        private ListBox LogMessageList;
        private TableLayoutPanel ChannelMessagesLayout;
        private Label MessagesLabel;
        private ListBox ChannelMessagesList;
        private ToolStripButton LogClearButton;
        private ToolStripButton MessageDeleteButton;
        private ToolStripButton RefreshButton;
        private ToolStripDropDownButton MemberPowerDropdown;
        private ToolStripMenuItem PowerReducedItem;
        private ToolStripMenuItem PowerNormalItem;
        private ToolStripMenuItem PowerModeratorItem;
        private ToolStripMenuItem PowerAdministratorItem;
        private ToolStripDropDownButton ModifyChannelDropdown;
        private ToolStripMenuItem ChannelNameItem;
        private ToolStripMenuItem ChannelDescriptionItem;
        private ToolStripButton MemberRemoveButton;
        private ToolStripDropDownButton ServerInfoDropdown;
        private ToolStripMenuItem ServerNameItem;
        private ToolStripMenuItem ServerDescItem;
        private ToolStripMenuItem ServerIconItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripDropDownButton CreateChannelDropdown;
        private ToolStripMenuItem CreateTextChannelItem;
        private ToolStripMenuItem CreateVoiceChannelItem;
    }
}