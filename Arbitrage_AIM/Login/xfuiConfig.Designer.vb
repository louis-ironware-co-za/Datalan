Partial Public Class xfuiConfig
    Inherits DevExpress.XtraEditors.XtraForm

    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(xfuiConfig))
        Me.CancelSimpleButton = New DevExpress.XtraEditors.SimpleButton()
        Me.ServerConnectionLabelControl = New DevExpress.XtraEditors.LabelControl()
        Me.UserTableBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.PanelControl4 = New DevExpress.XtraEditors.PanelControl()
        Me.ServerTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.IPaddressListBoxControl = New DevExpress.XtraEditors.ListBoxControl()
        Me.PanelControl2 = New DevExpress.XtraEditors.PanelControl()
        Me.PasswordTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.PanelControl3 = New DevExpress.XtraEditors.PanelControl()
        Me.UserNameTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.RaiseIPSimpleButton = New DevExpress.XtraEditors.SimpleButton()
        Me.RaiseTestSimpleButton = New DevExpress.XtraEditors.SimpleButton()
        Me.PanelControl5 = New DevExpress.XtraEditors.PanelControl()
        Me.PanelControl6 = New DevExpress.XtraEditors.PanelControl()
        Me.LocalHostTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl4 = New DevExpress.XtraEditors.LabelControl()
        CType(Me.UserTableBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl4.SuspendLayout()
        CType(Me.ServerTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl1.SuspendLayout()
        CType(Me.IPaddressListBoxControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl2.SuspendLayout()
        CType(Me.PasswordTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl3.SuspendLayout()
        CType(Me.UserNameTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl5.SuspendLayout()
        CType(Me.PanelControl6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl6.SuspendLayout()
        CType(Me.LocalHostTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CancelSimpleButton
        '
        Me.CancelSimpleButton.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.CancelSimpleButton.Appearance.ForeColor = System.Drawing.Color.White
        Me.CancelSimpleButton.Appearance.Options.UseBackColor = True
        Me.CancelSimpleButton.Appearance.Options.UseForeColor = True
        Me.CancelSimpleButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat
        Me.CancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancelSimpleButton.Dock = System.Windows.Forms.DockStyle.Right
        Me.CancelSimpleButton.Location = New System.Drawing.Point(233, 5)
        Me.CancelSimpleButton.Name = "CancelSimpleButton"
        Me.CancelSimpleButton.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.[False]
        Me.CancelSimpleButton.Size = New System.Drawing.Size(75, 25)
        Me.CancelSimpleButton.TabIndex = 2
        Me.CancelSimpleButton.Text = "Cancel"
        Me.CancelSimpleButton.ToolTip = "Close"
        '
        'ServerConnectionLabelControl
        '
        Me.ServerConnectionLabelControl.Appearance.ForeColor = System.Drawing.Color.White
        Me.ServerConnectionLabelControl.Appearance.Options.UseForeColor = True
        Me.ServerConnectionLabelControl.Appearance.Options.UseTextOptions = True
        Me.ServerConnectionLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
        Me.ServerConnectionLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.ServerConnectionLabelControl.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ServerConnectionLabelControl.Location = New System.Drawing.Point(0, 315)
        Me.ServerConnectionLabelControl.Name = "ServerConnectionLabelControl"
        Me.ServerConnectionLabelControl.Padding = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.ServerConnectionLabelControl.Size = New System.Drawing.Size(318, 25)
        Me.ServerConnectionLabelControl.TabIndex = 5
        Me.ServerConnectionLabelControl.Text = "Select Connect to continue..."
        '
        'PanelControl4
        '
        Me.PanelControl4.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.PanelControl4.Appearance.Options.UseBackColor = True
        Me.PanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl4.Controls.Add(Me.ServerTextEdit)
        Me.PanelControl4.Controls.Add(Me.LabelControl3)
        Me.PanelControl4.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelControl4.Location = New System.Drawing.Point(0, 35)
        Me.PanelControl4.Name = "PanelControl4"
        Me.PanelControl4.Padding = New System.Windows.Forms.Padding(10, 5, 10, 5)
        Me.PanelControl4.Size = New System.Drawing.Size(318, 35)
        Me.PanelControl4.TabIndex = 7
        '
        'ServerTextEdit
        '
        Me.ServerTextEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ServerTextEdit.Location = New System.Drawing.Point(76, 5)
        Me.ServerTextEdit.Name = "ServerTextEdit"
        Me.ServerTextEdit.Properties.AllowFocused = False
        Me.ServerTextEdit.Properties.AutoHeight = False
        Me.ServerTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.ServerTextEdit.Properties.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.ServerTextEdit.Size = New System.Drawing.Size(232, 25)
        Me.ServerTextEdit.TabIndex = 3
        '
        'LabelControl3
        '
        Me.LabelControl3.Appearance.ForeColor = System.Drawing.Color.White
        Me.LabelControl3.Appearance.Options.UseForeColor = True
        Me.LabelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LabelControl3.Dock = System.Windows.Forms.DockStyle.Left
        Me.LabelControl3.Location = New System.Drawing.Point(10, 5)
        Me.LabelControl3.Name = "LabelControl3"
        Me.LabelControl3.Size = New System.Drawing.Size(66, 25)
        Me.LabelControl3.TabIndex = 1
        Me.LabelControl3.Text = "Server"
        '
        'PanelControl1
        '
        Me.PanelControl1.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.PanelControl1.Appearance.Options.UseBackColor = True
        Me.PanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl1.Controls.Add(Me.IPaddressListBoxControl)
        Me.PanelControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelControl1.Location = New System.Drawing.Point(0, 140)
        Me.PanelControl1.Name = "PanelControl1"
        Me.PanelControl1.Padding = New System.Windows.Forms.Padding(10, 5, 10, 5)
        Me.PanelControl1.Size = New System.Drawing.Size(318, 101)
        Me.PanelControl1.TabIndex = 3
        '
        'IPaddressListBoxControl
        '
        Me.IPaddressListBoxControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.IPaddressListBoxControl.Location = New System.Drawing.Point(10, 5)
        Me.IPaddressListBoxControl.Name = "IPaddressListBoxControl"
        Me.IPaddressListBoxControl.Size = New System.Drawing.Size(298, 91)
        Me.IPaddressListBoxControl.TabIndex = 0
        '
        'PanelControl2
        '
        Me.PanelControl2.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.PanelControl2.Appearance.Options.UseBackColor = True
        Me.PanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl2.Controls.Add(Me.PasswordTextEdit)
        Me.PanelControl2.Controls.Add(Me.LabelControl2)
        Me.PanelControl2.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelControl2.Location = New System.Drawing.Point(0, 105)
        Me.PanelControl2.Name = "PanelControl2"
        Me.PanelControl2.Padding = New System.Windows.Forms.Padding(10, 5, 10, 5)
        Me.PanelControl2.Size = New System.Drawing.Size(318, 35)
        Me.PanelControl2.TabIndex = 9
        '
        'PasswordTextEdit
        '
        Me.PasswordTextEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PasswordTextEdit.Location = New System.Drawing.Point(76, 5)
        Me.PasswordTextEdit.Name = "PasswordTextEdit"
        Me.PasswordTextEdit.Properties.AllowFocused = False
        Me.PasswordTextEdit.Properties.AutoHeight = False
        Me.PasswordTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PasswordTextEdit.Properties.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.PasswordTextEdit.Properties.UseSystemPasswordChar = True
        Me.PasswordTextEdit.Size = New System.Drawing.Size(232, 25)
        Me.PasswordTextEdit.TabIndex = 4
        '
        'LabelControl2
        '
        Me.LabelControl2.Appearance.ForeColor = System.Drawing.Color.White
        Me.LabelControl2.Appearance.Options.UseForeColor = True
        Me.LabelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LabelControl2.Dock = System.Windows.Forms.DockStyle.Left
        Me.LabelControl2.Location = New System.Drawing.Point(10, 5)
        Me.LabelControl2.Name = "LabelControl2"
        Me.LabelControl2.Size = New System.Drawing.Size(66, 25)
        Me.LabelControl2.TabIndex = 1
        Me.LabelControl2.Text = "Password"
        '
        'PanelControl3
        '
        Me.PanelControl3.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.PanelControl3.Appearance.Options.UseBackColor = True
        Me.PanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl3.Controls.Add(Me.UserNameTextEdit)
        Me.PanelControl3.Controls.Add(Me.LabelControl1)
        Me.PanelControl3.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelControl3.Location = New System.Drawing.Point(0, 70)
        Me.PanelControl3.Name = "PanelControl3"
        Me.PanelControl3.Padding = New System.Windows.Forms.Padding(10, 5, 10, 5)
        Me.PanelControl3.Size = New System.Drawing.Size(318, 35)
        Me.PanelControl3.TabIndex = 8
        '
        'UserNameTextEdit
        '
        Me.UserNameTextEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UserNameTextEdit.Location = New System.Drawing.Point(76, 5)
        Me.UserNameTextEdit.Name = "UserNameTextEdit"
        Me.UserNameTextEdit.Properties.AllowFocused = False
        Me.UserNameTextEdit.Properties.AutoHeight = False
        Me.UserNameTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.UserNameTextEdit.Properties.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.UserNameTextEdit.Size = New System.Drawing.Size(232, 25)
        Me.UserNameTextEdit.TabIndex = 3
        '
        'LabelControl1
        '
        Me.LabelControl1.Appearance.ForeColor = System.Drawing.Color.White
        Me.LabelControl1.Appearance.Options.UseForeColor = True
        Me.LabelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LabelControl1.Dock = System.Windows.Forms.DockStyle.Left
        Me.LabelControl1.Location = New System.Drawing.Point(10, 5)
        Me.LabelControl1.Name = "LabelControl1"
        Me.LabelControl1.Size = New System.Drawing.Size(66, 25)
        Me.LabelControl1.TabIndex = 1
        Me.LabelControl1.Text = "User Id"
        '
        'RaiseIPSimpleButton
        '
        Me.RaiseIPSimpleButton.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.RaiseIPSimpleButton.Appearance.ForeColor = System.Drawing.Color.White
        Me.RaiseIPSimpleButton.Appearance.Options.UseBackColor = True
        Me.RaiseIPSimpleButton.Appearance.Options.UseForeColor = True
        Me.RaiseIPSimpleButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat
        Me.RaiseIPSimpleButton.Dock = System.Windows.Forms.DockStyle.Right
        Me.RaiseIPSimpleButton.Location = New System.Drawing.Point(83, 5)
        Me.RaiseIPSimpleButton.Name = "RaiseIPSimpleButton"
        Me.RaiseIPSimpleButton.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.[False]
        Me.RaiseIPSimpleButton.Size = New System.Drawing.Size(75, 25)
        Me.RaiseIPSimpleButton.TabIndex = 11
        Me.RaiseIPSimpleButton.Text = "Refresh IP"
        Me.RaiseIPSimpleButton.ToolTip = "Refresh IP address"
        '
        'RaiseTestSimpleButton
        '
        Me.RaiseTestSimpleButton.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.RaiseTestSimpleButton.Appearance.ForeColor = System.Drawing.Color.White
        Me.RaiseTestSimpleButton.Appearance.Options.UseBackColor = True
        Me.RaiseTestSimpleButton.Appearance.Options.UseForeColor = True
        Me.RaiseTestSimpleButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat
        Me.RaiseTestSimpleButton.Dock = System.Windows.Forms.DockStyle.Right
        Me.RaiseTestSimpleButton.Location = New System.Drawing.Point(158, 5)
        Me.RaiseTestSimpleButton.Name = "RaiseTestSimpleButton"
        Me.RaiseTestSimpleButton.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.[False]
        Me.RaiseTestSimpleButton.Size = New System.Drawing.Size(75, 25)
        Me.RaiseTestSimpleButton.TabIndex = 10
        Me.RaiseTestSimpleButton.Text = "Connect"
        Me.RaiseTestSimpleButton.ToolTip = "Test database connection"
        '
        'PanelControl5
        '
        Me.PanelControl5.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.PanelControl5.Appearance.Options.UseBackColor = True
        Me.PanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl5.Controls.Add(Me.RaiseIPSimpleButton)
        Me.PanelControl5.Controls.Add(Me.RaiseTestSimpleButton)
        Me.PanelControl5.Controls.Add(Me.CancelSimpleButton)
        Me.PanelControl5.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelControl5.Location = New System.Drawing.Point(0, 241)
        Me.PanelControl5.Name = "PanelControl5"
        Me.PanelControl5.Padding = New System.Windows.Forms.Padding(10, 5, 10, 5)
        Me.PanelControl5.Size = New System.Drawing.Size(318, 35)
        Me.PanelControl5.TabIndex = 12
        '
        'PanelControl6
        '
        Me.PanelControl6.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.PanelControl6.Appearance.Options.UseBackColor = True
        Me.PanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl6.Controls.Add(Me.LocalHostTextEdit)
        Me.PanelControl6.Controls.Add(Me.LabelControl4)
        Me.PanelControl6.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelControl6.Location = New System.Drawing.Point(0, 0)
        Me.PanelControl6.Name = "PanelControl6"
        Me.PanelControl6.Padding = New System.Windows.Forms.Padding(10, 5, 10, 5)
        Me.PanelControl6.Size = New System.Drawing.Size(318, 35)
        Me.PanelControl6.TabIndex = 13
        '
        'LocalHostTextEdit
        '
        Me.LocalHostTextEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LocalHostTextEdit.Enabled = False
        Me.LocalHostTextEdit.Location = New System.Drawing.Point(76, 5)
        Me.LocalHostTextEdit.Name = "LocalHostTextEdit"
        Me.LocalHostTextEdit.Properties.AllowFocused = False
        Me.LocalHostTextEdit.Properties.AutoHeight = False
        Me.LocalHostTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.LocalHostTextEdit.Properties.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.LocalHostTextEdit.Size = New System.Drawing.Size(232, 25)
        Me.LocalHostTextEdit.TabIndex = 3
        '
        'LabelControl4
        '
        Me.LabelControl4.Appearance.ForeColor = System.Drawing.Color.White
        Me.LabelControl4.Appearance.Options.UseForeColor = True
        Me.LabelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LabelControl4.Dock = System.Windows.Forms.DockStyle.Left
        Me.LabelControl4.Location = New System.Drawing.Point(10, 5)
        Me.LabelControl4.Name = "LabelControl4"
        Me.LabelControl4.Size = New System.Drawing.Size(66, 25)
        Me.LabelControl4.TabIndex = 1
        Me.LabelControl4.Text = "Local Host"
        '
        'xfuiConfig
        '
        Me.Appearance.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.Appearance.Options.UseBackColor = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Stretch
        Me.BackgroundImageStore = Global.Arbitrage_AIM.My.Resources.Resources.aimar_ih_logo_300x185
        Me.ClientSize = New System.Drawing.Size(318, 340)
        Me.ControlBox = False
        Me.Controls.Add(Me.PanelControl5)
        Me.Controls.Add(Me.PanelControl1)
        Me.Controls.Add(Me.PanelControl2)
        Me.Controls.Add(Me.PanelControl3)
        Me.Controls.Add(Me.PanelControl4)
        Me.Controls.Add(Me.ServerConnectionLabelControl)
        Me.Controls.Add(Me.PanelControl6)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.IconOptions.Icon = CType(resources.GetObject("xfuiConfig.IconOptions.Icon"), System.Drawing.Icon)
        Me.IconOptions.Image = Global.Arbitrage_AIM.My.Resources.Resources.favicon
        Me.IconOptions.ShowIcon = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "xfuiConfig"
        Me.Opacity = 0R
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Server Configuration"
        Me.TopMost = True
        CType(Me.UserTableBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl4.ResumeLayout(False)
        CType(Me.ServerTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl1.ResumeLayout(False)
        CType(Me.IPaddressListBoxControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl2.ResumeLayout(False)
        CType(Me.PasswordTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl3.ResumeLayout(False)
        CType(Me.UserNameTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl5.ResumeLayout(False)
        CType(Me.PanelControl6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl6.ResumeLayout(False)
        CType(Me.LocalHostTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CancelSimpleButton As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents UserTableBindingSource As BindingSource
    Friend WithEvents ServerConnectionLabelControl As DevExpress.XtraEditors.LabelControl
    Friend WithEvents PanelControl4 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents ServerTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents IPaddressListBoxControl As DevExpress.XtraEditors.ListBoxControl
    Friend WithEvents PanelControl2 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PasswordTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents PanelControl3 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents UserNameTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents RaiseIPSimpleButton As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents RaiseTestSimpleButton As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents PanelControl5 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PanelControl6 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LocalHostTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl4 As DevExpress.XtraEditors.LabelControl

#End Region

End Class
