Partial Public Class xfuiLogin
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(xfuiLogin))
        Me.LoginSimpleButton = New DevExpress.XtraEditors.SimpleButton()
        Me.CancelSimpleButton = New DevExpress.XtraEditors.SimpleButton()
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.UserNameTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.PanelControl2 = New DevExpress.XtraEditors.PanelControl()
        Me.PasswordTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.ServerConnectionLabelControl = New DevExpress.XtraEditors.LabelControl()
        Me.PanelControl3 = New DevExpress.XtraEditors.PanelControl()
        Me.IsPostToTelegramCheckEdit = New DevExpress.XtraEditors.CheckEdit()
        Me.UserTableBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl1.SuspendLayout()
        CType(Me.UserNameTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl2.SuspendLayout()
        CType(Me.PasswordTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl3.SuspendLayout()
        CType(Me.IsPostToTelegramCheckEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UserTableBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LoginSimpleButton
        '
        Me.LoginSimpleButton.Appearance.BackColor = System.Drawing.Color.IndianRed
        Me.LoginSimpleButton.Appearance.ForeColor = System.Drawing.Color.White
        Me.LoginSimpleButton.Appearance.Options.UseBackColor = True
        Me.LoginSimpleButton.Appearance.Options.UseForeColor = True
        Me.LoginSimpleButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat
        Me.LoginSimpleButton.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.LoginSimpleButton.Location = New System.Drawing.Point(160, 208)
        Me.LoginSimpleButton.Name = "LoginSimpleButton"
        Me.LoginSimpleButton.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.[False]
        Me.LoginSimpleButton.Size = New System.Drawing.Size(75, 23)
        Me.LoginSimpleButton.TabIndex = 1
        Me.LoginSimpleButton.Text = "&Login"
        '
        'CancelSimpleButton
        '
        Me.CancelSimpleButton.Appearance.BackColor = System.Drawing.Color.IndianRed
        Me.CancelSimpleButton.Appearance.ForeColor = System.Drawing.Color.White
        Me.CancelSimpleButton.Appearance.Options.UseBackColor = True
        Me.CancelSimpleButton.Appearance.Options.UseForeColor = True
        Me.CancelSimpleButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat
        Me.CancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancelSimpleButton.Location = New System.Drawing.Point(240, 208)
        Me.CancelSimpleButton.Name = "CancelSimpleButton"
        Me.CancelSimpleButton.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.[False]
        Me.CancelSimpleButton.Size = New System.Drawing.Size(75, 23)
        Me.CancelSimpleButton.TabIndex = 2
        Me.CancelSimpleButton.Text = "&Cancel"
        '
        'PanelControl1
        '
        Me.PanelControl1.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.PanelControl1.Appearance.Options.UseBackColor = True
        Me.PanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl1.Controls.Add(Me.UserNameTextEdit)
        Me.PanelControl1.Location = New System.Drawing.Point(160, 136)
        Me.PanelControl1.Name = "PanelControl1"
        Me.PanelControl1.Size = New System.Drawing.Size(160, 24)
        Me.PanelControl1.TabIndex = 3
        '
        'UserNameTextEdit
        '
        Me.UserNameTextEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UserNameTextEdit.Location = New System.Drawing.Point(0, 0)
        Me.UserNameTextEdit.Name = "UserNameTextEdit"
        Me.UserNameTextEdit.Properties.AllowFocused = False
        Me.UserNameTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.White
        Me.UserNameTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.UserNameTextEdit.Properties.Appearance.Options.UseBackColor = True
        Me.UserNameTextEdit.Properties.Appearance.Options.UseForeColor = True
        Me.UserNameTextEdit.Properties.Appearance.Options.UseTextOptions = True
        Me.UserNameTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.UserNameTextEdit.Properties.AppearanceDisabled.Options.UseTextOptions = True
        Me.UserNameTextEdit.Properties.AppearanceDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.UserNameTextEdit.Properties.AppearanceFocused.Options.UseTextOptions = True
        Me.UserNameTextEdit.Properties.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.UserNameTextEdit.Properties.AppearanceReadOnly.Options.UseTextOptions = True
        Me.UserNameTextEdit.Properties.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.UserNameTextEdit.Properties.AutoHeight = False
        Me.UserNameTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.UserNameTextEdit.Properties.NullValuePrompt = "User Name"
        Me.UserNameTextEdit.Properties.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.UserNameTextEdit.Size = New System.Drawing.Size(160, 24)
        Me.UserNameTextEdit.TabIndex = 3
        '
        'PanelControl2
        '
        Me.PanelControl2.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.PanelControl2.Appearance.Options.UseBackColor = True
        Me.PanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl2.Controls.Add(Me.PasswordTextEdit)
        Me.PanelControl2.Location = New System.Drawing.Point(160, 168)
        Me.PanelControl2.Name = "PanelControl2"
        Me.PanelControl2.Size = New System.Drawing.Size(160, 24)
        Me.PanelControl2.TabIndex = 4
        '
        'PasswordTextEdit
        '
        Me.PasswordTextEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PasswordTextEdit.Location = New System.Drawing.Point(0, 0)
        Me.PasswordTextEdit.Name = "PasswordTextEdit"
        Me.PasswordTextEdit.Properties.AllowFocused = False
        Me.PasswordTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.White
        Me.PasswordTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.PasswordTextEdit.Properties.Appearance.Options.UseBackColor = True
        Me.PasswordTextEdit.Properties.Appearance.Options.UseForeColor = True
        Me.PasswordTextEdit.Properties.Appearance.Options.UseTextOptions = True
        Me.PasswordTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.PasswordTextEdit.Properties.AppearanceDisabled.Options.UseTextOptions = True
        Me.PasswordTextEdit.Properties.AppearanceDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.PasswordTextEdit.Properties.AppearanceFocused.Options.UseTextOptions = True
        Me.PasswordTextEdit.Properties.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.PasswordTextEdit.Properties.AppearanceReadOnly.Options.UseTextOptions = True
        Me.PasswordTextEdit.Properties.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.PasswordTextEdit.Properties.AutoHeight = False
        Me.PasswordTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PasswordTextEdit.Properties.NullValuePrompt = "Password"
        Me.PasswordTextEdit.Properties.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.PasswordTextEdit.Properties.UseSystemPasswordChar = True
        Me.PasswordTextEdit.Size = New System.Drawing.Size(160, 24)
        Me.PasswordTextEdit.TabIndex = 4
        '
        'ServerConnectionLabelControl
        '
        Me.ServerConnectionLabelControl.Appearance.ForeColor = System.Drawing.Color.White
        Me.ServerConnectionLabelControl.Appearance.Options.UseForeColor = True
        Me.ServerConnectionLabelControl.Appearance.Options.UseTextOptions = True
        Me.ServerConnectionLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
        Me.ServerConnectionLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.ServerConnectionLabelControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ServerConnectionLabelControl.Location = New System.Drawing.Point(136, 5)
        Me.ServerConnectionLabelControl.Name = "ServerConnectionLabelControl"
        Me.ServerConnectionLabelControl.Padding = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.ServerConnectionLabelControl.Size = New System.Drawing.Size(333, 22)
        Me.ServerConnectionLabelControl.TabIndex = 5
        Me.ServerConnectionLabelControl.Text = "Select Login to continue..."
        '
        'PanelControl3
        '
        Me.PanelControl3.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.PanelControl3.Appearance.Options.UseBackColor = True
        Me.PanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl3.Controls.Add(Me.ServerConnectionLabelControl)
        Me.PanelControl3.Controls.Add(Me.IsPostToTelegramCheckEdit)
        Me.PanelControl3.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelControl3.Location = New System.Drawing.Point(0, 242)
        Me.PanelControl3.Name = "PanelControl3"
        Me.PanelControl3.Padding = New System.Windows.Forms.Padding(5)
        Me.PanelControl3.Size = New System.Drawing.Size(474, 32)
        Me.PanelControl3.TabIndex = 6
        '
        'IsPostToTelegramCheckEdit
        '
        Me.IsPostToTelegramCheckEdit.Dock = System.Windows.Forms.DockStyle.Left
        Me.IsPostToTelegramCheckEdit.Location = New System.Drawing.Point(5, 5)
        Me.IsPostToTelegramCheckEdit.Name = "IsPostToTelegramCheckEdit"
        Me.IsPostToTelegramCheckEdit.Properties.AllowFocused = False
        Me.IsPostToTelegramCheckEdit.Properties.AutoHeight = False
        Me.IsPostToTelegramCheckEdit.Properties.Caption = "Configure Connection"
        Me.IsPostToTelegramCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio
        Me.IsPostToTelegramCheckEdit.Size = New System.Drawing.Size(131, 22)
        Me.IsPostToTelegramCheckEdit.TabIndex = 265
        Me.IsPostToTelegramCheckEdit.ToolTip = "Configure SQL Server Connection"
        Me.IsPostToTelegramCheckEdit.Visible = False
        '
        'xfuiLogin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Stretch
        Me.BackgroundImageStore = Global.Arbitrage_AIM.My.Resources.Resources.aimar_ih_logo_300x185
        Me.ClientSize = New System.Drawing.Size(474, 274)
        Me.ControlBox = False
        Me.Controls.Add(Me.CancelSimpleButton)
        Me.Controls.Add(Me.LoginSimpleButton)
        Me.Controls.Add(Me.PanelControl3)
        Me.Controls.Add(Me.PanelControl2)
        Me.Controls.Add(Me.PanelControl1)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.IconOptions.Icon = CType(resources.GetObject("xfuiLogin.IconOptions.Icon"), System.Drawing.Icon)
        Me.IconOptions.Image = Global.Arbitrage_AIM.My.Resources.Resources.favicon
        Me.IconOptions.ShowIcon = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "xfuiLogin"
        Me.Opacity = 0R
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.TopMost = True
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl1.ResumeLayout(False)
        CType(Me.UserNameTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl2.ResumeLayout(False)
        CType(Me.PasswordTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl3.ResumeLayout(False)
        CType(Me.IsPostToTelegramCheckEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UserTableBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents LoginSimpleButton As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CancelSimpleButton As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PanelControl2 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents UserTableBindingSource As BindingSource
    Friend WithEvents ServerConnectionLabelControl As DevExpress.XtraEditors.LabelControl
    Friend WithEvents UserNameTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents PasswordTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents PanelControl3 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents IsPostToTelegramCheckEdit As DevExpress.XtraEditors.CheckEdit

#End Region

End Class
