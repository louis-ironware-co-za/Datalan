<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class xf_About
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(xf_About))
        Me.ApplicationTitleLabelControl = New System.Windows.Forms.Label()
        Me.Copyright = New System.Windows.Forms.Label()
        Me.VersionLabelControl = New System.Windows.Forms.Label()
        Me.tmr_Startup = New System.Windows.Forms.Timer(Me.components)
        Me.StatusLabelControl = New System.Windows.Forms.Label()
        Me.closeLabel = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'ApplicationTitleLabelControl
        '
        Me.ApplicationTitleLabelControl.BackColor = System.Drawing.Color.Transparent
        Me.ApplicationTitleLabelControl.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ApplicationTitleLabelControl.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ApplicationTitleLabelControl.ForeColor = System.Drawing.Color.RoyalBlue
        Me.ApplicationTitleLabelControl.Location = New System.Drawing.Point(0, 150)
        Me.ApplicationTitleLabelControl.Name = "ApplicationTitleLabelControl"
        Me.ApplicationTitleLabelControl.Padding = New System.Windows.Forms.Padding(0, 0, 0, 20)
        Me.ApplicationTitleLabelControl.Size = New System.Drawing.Size(460, 58)
        Me.ApplicationTitleLabelControl.TabIndex = 4
        Me.ApplicationTitleLabelControl.Text = "BitBuyBot 2020"
        Me.ApplicationTitleLabelControl.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'Copyright
        '
        Me.Copyright.BackColor = System.Drawing.Color.Transparent
        Me.Copyright.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Copyright.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Copyright.Location = New System.Drawing.Point(0, 208)
        Me.Copyright.Name = "Copyright"
        Me.Copyright.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.Copyright.Size = New System.Drawing.Size(460, 15)
        Me.Copyright.TabIndex = 6
        Me.Copyright.Text = " Copyright"
        Me.Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'VersionLabelControl
        '
        Me.VersionLabelControl.BackColor = System.Drawing.Color.Transparent
        Me.VersionLabelControl.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.VersionLabelControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.VersionLabelControl.ForeColor = System.Drawing.Color.White
        Me.VersionLabelControl.Location = New System.Drawing.Point(0, 223)
        Me.VersionLabelControl.Name = "VersionLabelControl"
        Me.VersionLabelControl.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.VersionLabelControl.Size = New System.Drawing.Size(460, 17)
        Me.VersionLabelControl.TabIndex = 5
        Me.VersionLabelControl.Text = " Version {0}.{1:00}"
        Me.VersionLabelControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tmr_Startup
        '
        Me.tmr_Startup.Interval = 1
        Me.tmr_Startup.Tag = "0"
        '
        'StatusLabelControl
        '
        Me.StatusLabelControl.BackColor = System.Drawing.Color.Transparent
        Me.StatusLabelControl.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.StatusLabelControl.ForeColor = System.Drawing.Color.White
        Me.StatusLabelControl.Location = New System.Drawing.Point(0, 240)
        Me.StatusLabelControl.Name = "StatusLabelControl"
        Me.StatusLabelControl.Padding = New System.Windows.Forms.Padding(40, 0, 0, 0)
        Me.StatusLabelControl.Size = New System.Drawing.Size(460, 20)
        Me.StatusLabelControl.TabIndex = 7
        Me.StatusLabelControl.Text = " Loading...  "
        Me.StatusLabelControl.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'closeLabel
        '
        Me.closeLabel.AutoSize = True
        Me.closeLabel.BackColor = System.Drawing.Color.Transparent
        Me.closeLabel.Dock = System.Windows.Forms.DockStyle.Right
        Me.closeLabel.ForeColor = System.Drawing.Color.White
        Me.closeLabel.Location = New System.Drawing.Point(447, 0)
        Me.closeLabel.Name = "closeLabel"
        Me.closeLabel.Size = New System.Drawing.Size(13, 13)
        Me.closeLabel.TabIndex = 8
        Me.closeLabel.Text = "X"
        Me.closeLabel.Visible = False
        '
        'xf_splash
        '
        Me.Appearance.BackColor = System.Drawing.SystemColors.Control
        Me.Appearance.Options.UseBackColor = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(460, 260)
        Me.ControlBox = False
        Me.Controls.Add(Me.closeLabel)
        Me.Controls.Add(Me.ApplicationTitleLabelControl)
        Me.Controls.Add(Me.Copyright)
        Me.Controls.Add(Me.VersionLabelControl)
        Me.Controls.Add(Me.StatusLabelControl)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "xf_splash"
        Me.Opacity = 0.7R
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "xf_splash"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ApplicationTitleLabelControl As System.Windows.Forms.Label
    Friend WithEvents Copyright As System.Windows.Forms.Label
    Friend WithEvents VersionLabelControl As System.Windows.Forms.Label
    Friend WithEvents tmr_Startup As System.Windows.Forms.Timer
    Friend WithEvents StatusLabelControl As System.Windows.Forms.Label
    Friend WithEvents closeLabel As System.Windows.Forms.Label
End Class
