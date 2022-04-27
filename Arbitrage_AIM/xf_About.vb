Public Class xf_About
    'Inherits System.Windows.Forms.Label

    'Override the Paint property. This will allow you to handle how the control and it's text are painted  
    Private Sub ApplicationTitleLabelControl_Paint(sender As Object, e As PaintEventArgs) Handles ApplicationTitleLabelControl.Paint

        ''Draw a $ sign in Red  
        'e.Graphics.DrawString("2020", Me.Font, Brushes.Red, New Point(0, 0))

        ''Measure the width of the $ character so you know where the remainder of the text should be drawn  
        'Dim SignWidth As Single = e.Graphics.MeasureString("2020", Me.Font).Width

        ''Draw the remainder of the string in Blue  
        'e.Graphics.DrawString(Me.Text, Me.Font, Brushes.DarkBlue, New Point(SignWidth, 0))

    End Sub


    'Private ts As New TimeSpan(0, 0, 0)
    'Private dt_Start As DateTime

    '<LOAD FORMS>
#Region "LOAD FORMS"

    'Private Sub f_SplashScreen_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    '    Me.tmr_Startup.Enabled = False
    'End Sub

    'Private Sub tmr_Startup_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmr_Startup.Tick
    '    Dim dt_Stop As New DateTime
    '    dt_Stop = DateTime.Now
    '    ts = dt_Stop.Subtract(dt_Start)
    '    Application.DoEvents()
    '    Me.Label1.Text = Me.Label1.Text & "..."
    '    Application.DoEvents()
    '    If ts.Seconds > 1 Then
    '        tmr_Startup.Enabled = False
    '        'fSplashScreen = Me
    '        Me.Visible = False
    '        Me.Close()
    '    End If

    '    'Dim FormsThread As Threading.Thread
    '    'FormsThread = New Threading.Thread(AddressOf FormStarter)
    '    'FormsThread.Start()
    'End Sub

    Private Sub xf_Splash_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            Me.StatusLabelControl.Text = "..."
            Application.DoEvents()
            Me.Visible = True
            'Application title
            ApplicationTitleLabelControl.Text = "BitBuyBot2021" ' My.Application.Info.Title
            Dim _version As Version = My.Application.Info.Version
            VersionLabelControl.Text = "Version: " & _version.ToString

            'Copyright info
            Copyright.Text = My.Application.Info.Copyright
            '#Forms load on Splash Screen Visible event>
            'dt_Start = DateTime.Now
            Application.DoEvents()
            'Threading.Thread.Sleep(2000)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    'Private Sub f_SplashScreen_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
    '    If Me.Visible = True Then
    '        tmr_Startup.Interval = 1000
    '        tmr_Startup.Enabled = True
    '        dt_Start = DateTime.Now
    '        Application.DoEvents()
    '        Exit Sub
    '    End If
    'End Sub

#End Region '<LOAD FORMS>


    Private Sub closeLabel_Click(sender As Object, e As EventArgs) Handles closeLabel.Click
        Me.Close()
    End Sub

End Class