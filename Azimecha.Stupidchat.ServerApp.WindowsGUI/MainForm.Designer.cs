using System.Windows.Forms;

namespace Azimecha.Stupidchat.ServerApp.WindowsGUI {
    partial class MainForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.BottomStatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabelLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.TopToolStrip = new System.Windows.Forms.ToolStrip();
            this.StartStopButton = new System.Windows.Forms.ToolStripButton();
            this.ChannelCreateButton = new System.Windows.Forms.ToolStripButton();
            this.ChannelDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.LogClearButton = new System.Windows.Forms.ToolStripButton();
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LogLabel = new System.Windows.Forms.Label();
            this.LogMessageList = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.MessagesLabel = new System.Windows.Forms.Label();
            this.ChannelMessagesList = new System.Windows.Forms.ListBox();
            this.MemberPowerDropdown = new System.Windows.Forms.ToolStripDropDownButton();
            this.PowerReducedItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PowerNormalItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PowerModeratorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PowerAdministratorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ModifyChannelDropdown = new System.Windows.Forms.ToolStripDropDownButton();
            this.ChannelNameItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DescriptionItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomStatusStrip
            // 
            this.BottomStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelLabel,
            this.StatusLabel});
            this.BottomStatusStrip.Location = new System.Drawing.Point(0, 428);
            this.BottomStatusStrip.Name = "BottomStatusStrip";
            this.BottomStatusStrip.Size = new System.Drawing.Size(800, 22);
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
            this.StartStopButton,
            this.MemberPowerDropdown,
            this.ChannelCreateButton,
            this.ModifyChannelDropdown,
            this.ChannelDeleteButton,
            this.LogClearButton,
            this.MessageDeleteButton,
            this.RefreshButton});
            this.TopToolStrip.Location = new System.Drawing.Point(0, 0);
            this.TopToolStrip.Name = "TopToolStrip";
            this.TopToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.TopToolStrip.Size = new System.Drawing.Size(800, 25);
            this.TopToolStrip.TabIndex = 1;
            this.TopToolStrip.Text = "toolStrip1";
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
            // ChannelCreateButton
            // 
            this.ChannelCreateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ChannelCreateButton.Image = ((System.Drawing.Image)(resources.GetObject("ChannelCreateButton.Image")));
            this.ChannelCreateButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ChannelCreateButton.Name = "ChannelCreateButton";
            this.ChannelCreateButton.Size = new System.Drawing.Size(92, 22);
            this.ChannelCreateButton.Text = "Create Channel";
            this.ChannelCreateButton.Click += new System.EventHandler(this.ChannelCreateButton_Click);
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
            this.MainSplit.Size = new System.Drawing.Size(800, 403);
            this.MainSplit.SplitterDistance = 266;
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
            this.LeftSplit.Size = new System.Drawing.Size(266, 403);
            this.LeftSplit.SplitterDistance = 203;
            this.LeftSplit.TabIndex = 0;
            // 
            // UsersLayout
            // 
            this.UsersLayout.ColumnCount = 1;
            this.UsersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.UsersLayout.Controls.Add(this.MembersLabel, 0, 0);
            this.UsersLayout.Controls.Add(this.MembersList, 0, 1);
            this.UsersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UsersLayout.Location = new System.Drawing.Point(0, 0);
            this.UsersLayout.Name = "UsersLayout";
            this.UsersLayout.RowCount = 2;
            this.UsersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.UsersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.UsersLayout.Size = new System.Drawing.Size(262, 199);
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
            this.MembersLabel.Size = new System.Drawing.Size(262, 24);
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
            this.MembersList.Size = new System.Drawing.Size(262, 175);
            this.MembersList.TabIndex = 1;
            this.MembersList.SelectedIndexChanged += new System.EventHandler(this.MembersList_SelectedIndexChanged);
            // 
            // ChannelsLayout
            // 
            this.ChannelsLayout.ColumnCount = 1;
            this.ChannelsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChannelsLayout.Controls.Add(this.ChannelsList, 0, 1);
            this.ChannelsLayout.Controls.Add(this.ChannelsLabel, 0, 0);
            this.ChannelsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChannelsLayout.Location = new System.Drawing.Point(0, 0);
            this.ChannelsLayout.Name = "ChannelsLayout";
            this.ChannelsLayout.RowCount = 2;
            this.ChannelsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.ChannelsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChannelsLayout.Size = new System.Drawing.Size(262, 192);
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
            this.ChannelsList.Size = new System.Drawing.Size(262, 168);
            this.ChannelsList.TabIndex = 2;
            this.ChannelsList.SelectedIndexChanged += new System.EventHandler(this.ChannelsList_SelectedIndexChanged);
            this.ChannelsList.SelectedValueChanged += new System.EventHandler(this.ChannelsList_SelectedValueChanged);
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
            this.ChannelsLabel.Size = new System.Drawing.Size(262, 24);
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
            this.RightSplit.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // RightSplit.Panel2
            // 
            this.RightSplit.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.RightSplit.Size = new System.Drawing.Size(530, 403);
            this.RightSplit.SplitterDistance = 176;
            this.RightSplit.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.LogLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LogMessageList, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(526, 172);
            this.tableLayoutPanel1.TabIndex = 1;
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
            this.LogLabel.Size = new System.Drawing.Size(526, 24);
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
            this.LogMessageList.Size = new System.Drawing.Size(526, 148);
            this.LogMessageList.TabIndex = 1;
            this.LogMessageList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LogMessageList_MouseDoubleClick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.MessagesLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ChannelMessagesList, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(526, 219);
            this.tableLayoutPanel2.TabIndex = 1;
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
            this.MessagesLabel.Size = new System.Drawing.Size(526, 24);
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
            this.ChannelMessagesList.Size = new System.Drawing.Size(526, 195);
            this.ChannelMessagesList.TabIndex = 1;
            this.ChannelMessagesList.SelectedIndexChanged += new System.EventHandler(this.ChannelMessagesList_SelectedIndexChanged);
            this.ChannelMessagesList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ChannelMessagesList_MouseDoubleClick);
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
            this.PowerReducedItem.Size = new System.Drawing.Size(180, 22);
            this.PowerReducedItem.Text = "Reduced";
            this.PowerReducedItem.Click += new System.EventHandler(this.PowerReducedItem_Click);
            // 
            // PowerNormalItem
            // 
            this.PowerNormalItem.Name = "PowerNormalItem";
            this.PowerNormalItem.Size = new System.Drawing.Size(180, 22);
            this.PowerNormalItem.Text = "Normal";
            this.PowerNormalItem.Click += new System.EventHandler(this.PowerNormalItem_Click);
            // 
            // PowerModeratorItem
            // 
            this.PowerModeratorItem.Name = "PowerModeratorItem";
            this.PowerModeratorItem.Size = new System.Drawing.Size(180, 22);
            this.PowerModeratorItem.Text = "Moderator";
            this.PowerModeratorItem.Click += new System.EventHandler(this.PowerModeratorItem_Click);
            // 
            // PowerAdministratorItem
            // 
            this.PowerAdministratorItem.Name = "PowerAdministratorItem";
            this.PowerAdministratorItem.Size = new System.Drawing.Size(180, 22);
            this.PowerAdministratorItem.Text = "Administrator";
            this.PowerAdministratorItem.Click += new System.EventHandler(this.PowerAdministratorItem_Click);
            // 
            // ModifyChannelDropdown
            // 
            this.ModifyChannelDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ModifyChannelDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChannelNameItem,
            this.DescriptionItem});
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
            // 
            // DescriptionItem
            // 
            this.DescriptionItem.Name = "DescriptionItem";
            this.DescriptionItem.Size = new System.Drawing.Size(180, 22);
            this.DescriptionItem.Text = "Description";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MainSplit);
            this.Controls.Add(this.TopToolStrip);
            this.Controls.Add(this.BottomStatusStrip);
            this.Name = "MainForm";
            this.Text = "Chat Server";
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
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
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
        private ToolStripButton ChannelCreateButton;
        private ToolStripButton ChannelDeleteButton;
        private SplitContainer RightSplit;
        private TableLayoutPanel tableLayoutPanel1;
        private Label LogLabel;
        private ListBox LogMessageList;
        private TableLayoutPanel tableLayoutPanel2;
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
        private ToolStripMenuItem DescriptionItem;
    }
}