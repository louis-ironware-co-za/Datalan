Imports System.ComponentModel
Imports System.Text
Imports System.Data.SqlClient

Partial Public Class xfuiLogin

#Region "#INIT"

    Shared Sub New()
        DevExpress.UserSkins.BonusSkins.Register()
    End Sub
    Public Sub New()
        InitializeComponent()
        Me.Opacity = 0
    End Sub

#End Region '#INIT

#Region "#VAR"

    'Private cs As String = "Data Source=signaltradingserver.database.windows.net;Initial Catalog=SignalBase;User ID=signaladmin;Password=********;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

    Public UserId As String = String.Empty
    Public Password As String = String.Empty
    Public IsAdmin As Boolean = False
    Public ServerConnection As String = String.Empty

#End Region '#VAR

#Region "#FORM"

    '#FORM LOAD
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'SignalTradingDataBaseDataSet.UserTable' table. You can move, or remove it, as needed.
        'Me.UserTableTableAdapter.Fill(Me.SignalTradingDataBaseDataSet.UserTable)

        '#CONNECT SERVER LABEL
        'ServerConnection = UserTableAdapter.Connection.DataSource
        ServerConnectionLabelControl.Text = ServerConnection

        'TODO - BaseEncrypt
        UserNameTextEdit.Text = UserId
        PasswordTextEdit.Text = Password


    End Sub
    '#FORM SHOWN
    Private Sub xfuilogin_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.Opacity = 100
        Me.BringToFront()
    End Sub

    Public AuthFile As String = String.Empty

    '#LOGIN
    Private Sub LoginSimpleButton_Click(sender As Object, e As EventArgs) Handles LoginSimpleButton.Click
        'Dim result As DialogResult = Nothing
        'If RaiseConnectServer() Then
        '    UserId = UserNameTextEdit.Text
        '    Password = PasswordTextEdit.Text
        '    If AuthenticateUser(UserId, Password) Then


        '        '#UPDATE LOGIN

        '#REV: Login
        If Me.DialogResult = DialogResult.OK Then
            Me.Close()
        Else
            Me.DialogResult = DialogResult.None
        End If


        'End If
    End Sub

    '#CANCEL BUTTON
    Private Sub CancelSimpleButton_Click(sender As Object, e As EventArgs) Handles CancelSimpleButton.Click
        Me.Close()
    End Sub

#End Region '#FORM

#Region "FUNCTIONS"

    Private Function RaiseConnectServer() As Boolean
        Try
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Connecting to server...")
            '#CONNECTSERVER
            Dim nRetries As Integer = 0
            While nRetries < 3
                'Me.Opacity = 0
                Try
                    'Me.UserTableAdapter.Fill(Me.SignalTradingDataBaseDataSet.UserTable)
                    nRetries = 4
                Catch ex As Exception
                    nRetries += 1
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Server connection failed. Attempting retry... (" & nRetries.ToString & " of 3)")
                End Try
            End While
            If nRetries = 3 Then
                MsgBox("Error connecting to server, service not available please try again in a few minutes.", MsgBoxStyle.Information, "Server: " & ServerConnection)
                Return False
            End If
            Return True

        Catch ex As Exception
            MsgBox("Error: " & ex.Message, MsgBoxStyle.Critical, "BitBuyBot")
            Return False
        Finally
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        End Try
    End Function

    '#AUTHENTICATE/LOGIN
    Private Function AuthenticateUser(ByVal username As String, ByVal password As String) As Boolean
        If String.IsNullOrEmpty(username) Then
            MessageBox.Show("Invalid Entry: Enter a valid User ID to continue", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            UserNameTextEdit.Focus()
            Return False
        End If
        If String.IsNullOrEmpty(password) Then
            MessageBox.Show("Invalid Entry: Enter a valid Password to continue", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            PasswordTextEdit.Focus()
            Return False
        End If

        ''#USERTABLE
        ''Dim dt As DataTable = SignalTradingDataBaseDataSet.Tables("UserTable") ': dtSymbols.Clear() 'New DataTable
        'Dim itemFound As Integer = UserTableBindingSource.Find("UserName", UserNameTextEdit.EditValue)
        'If itemFound > -1 Then
        '    Dim row As DataRow = dt.Rows(itemFound)
        '    Dim uname As String = CType(row("UserName"), String)
        '    Dim pword As String = CType(row("Password"), String)
        '    IsAdmin = CType(row("IsAdmin"), Boolean)
        '    '#UPDATE
        '    Dim datenow As DateTime = DateTime.Now
        '    Dim TokenId As String = datenow.ToShortTimeString
        '    Dim Host As Net.IPHostEntry = Nothing
        '    Dim HostName As String = String.Empty
        '    Dim IPAddress As String = String.Empty
        '    Try
        '        Host = System.Net.Dns.GetHostEntry(HostName)
        '        HostName = Host.HostName
        '        IPAddress = Host.AddressList(0).ToString()
        '        TokenId = HostName & "." & IPAddress
        '    Catch ex As Exception
        '        MsgBox("Failed to update Host Id, Select Ok to continue...", vbOKOnly, "Server Connection")
        '    End Try
        '    '#UPDATE LOGIN
        '    Try
        '        row.BeginEdit()
        '        row("IsConnected") = True
        '        row("TimeStamp") = datenow
        '        row("TokenId") = TokenId
        '        row.EndEdit()
        '        Me.Validate()
        '        Me.UserTableBindingSource.EndEdit()
        '        Me.TableAdapterManager.UpdateAll(Me.SignalTradingDataBaseDataSet)
        '        'dt.AcceptChanges()
        '    Catch ex As Exception
        '        MsgBox("Failed to update Server, Select Ok to continue...", vbOKOnly, "Server Connection")

        '    End Try
        'Else
        '    MessageBox.Show("Invalid Credentials:" & vbCrLf & "The User ID and Password you have entered does not match", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        '    Return False
        'End If
        Return True

        'MessageBox.Show("Authentication Failed:" & vbCrLf & "The User ID and password you have entered does not match. Please check your login info and try again.", "Authentication Failure")
    End Function

    Private Sub UserTableBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs)
        'Me.Validate()
        'Me.UserTableBindingSource.EndEdit()
        'Me.TableAdapterManager.UpdateAll(Me.SignalTradingDataBaseDataSet)
    End Sub

    Private Sub IsPostToTelegramCheckEdit_CheckStateChanged(sender As Object, e As EventArgs) Handles IsPostToTelegramCheckEdit.CheckStateChanged
        Dim IsChecked As CheckState = IsPostToTelegramCheckEdit.CheckState

        If IsChecked = CheckState.Checked Then

            '#LOGIN DIALOG
            'Me.Opacity = 0
            'Dim xfConnect As New xfConnectDialog
            'xfConnect.UserId = UserId
            'xfConnect.Password = Password
            'If xfConnect.ShowDialog = DialogResult.OK Then
            '    Me.Opacity = 100
            '    IsPostToTelegramCheckEdit.CheckState = CheckState.Unchecked
            'Else
            '    Me.Opacity = 100
            '    IsPostToTelegramCheckEdit.CheckState = CheckState.Unchecked
            'End If
            'Me.Opacity = 100
            'IsPostToTelegramCheckEdit.CheckState = CheckState.Unchecked
        End If

    End Sub

    Private Sub IsPostToTelegramCheckEdit_CheckedChanged(sender As Object, e As EventArgs) Handles IsPostToTelegramCheckEdit.CheckedChanged

    End Sub

#End Region '#FUNCTIONS

End Class


'    '#LOGIN
'    Private Sub okSimpleButton_Click(sender As Object, e As EventArgs) Handles LoginSimpleButton.Click
'        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
'        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Connecting to server...")

'        Dim nRetries As Integer = 0
'        While nRetries < 3
'            Me.Opacity = 0
'            Try
'                Me.UserTableTableAdapter.Fill(Me.SignalBaseDataSet.UserTable)
'                nRetries = 4
'            Catch ex As Exception
'                nRetries += 1
'                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Server connection failed. Attempting retry... (" & nRetries.ToString & " of 3)")
'            End Try
'        End While
'        If nRetries = 3 Then
'            MsgBox("Error connecting to server, service not available please try again in a few minutes.")
'        End If

'        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Validating credentials...")
'        If AuthenticateUser() Then Me.Close()
'        Me.Opacity = 100

'        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()


'        'Return
'        'Try
'        '    Dim myConnection As SqlConnection = Nothing
'        '    myConnection = New SqlConnection(cs)
'        '    Dim myCommand As SqlCommand = Nothing
'        '    myCommand = New SqlCommand("SELECT UserName,Password FROM UserTable WHERE Username = @UserName AND Password = @Password", myConnection)
'        '    Dim uName As SqlParameter = New SqlParameter("@Username", SqlDbType.VarChar)
'        '    Dim uPassword As SqlParameter = New SqlParameter("@Password", SqlDbType.VarChar)
'        '    uName.Value = UserNameTextEdit.EditValue
'        '    uPassword.Value = PasswordTextEdit.EditValue
'        '    myCommand.Parameters.Add(uName)
'        '    myCommand.Parameters.Add(uPassword)
'        '    myCommand.Connection.Open()
'        '    Dim myReader As SqlDataReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)

'        '    If myReader.Read() = True Then
'        '        MessageBox.Show("You have logged in successfully " & UserNameTextEdit.EditValue)
'        '        Me.Hide()
'        '    Else
'        '        MessageBox.Show("Login Failed...Try again !", "Invalid Login Access Denied", MessageBoxButtons.OK, MessageBoxIcon.[Error])
'        '        UserNameTextEdit.EditValue.Clear()
'        '        PasswordTextEdit.EditValue.Clear()
'        '        UserNameTextEdit.Focus()
'        '    End If

'        '    If myConnection.State = ConnectionState.Open Then
'        '        myConnection.Dispose()
'        '    End If

'        'Catch ex As Exception
'        '    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
'        'End Try
'    End Sub

'    '#CANCEL
'    Private Sub CancelSimpleButton_Click(sender As Object, e As EventArgs) Handles CancelSimpleButton.Click
'        Me.Close()

'    End Sub

'#End Region '#FORM

'#Region "FUNCTIONS"

'    '#AUTHENTICATE/LOGIN
'    Private Function AuthenticateUser() As Boolean
'        If UserNameTextEdit.EditValue = "" Then
'            MessageBox.Show("Please enter user name", "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
'            UserNameTextEdit.Focus()
'            Return False
'        End If
'        If PasswordTextEdit.EditValue = "" Then
'            MessageBox.Show("Please enter password", "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
'            PasswordTextEdit.Focus()
'            Return False
'        End If

'        Dim dt As DataTable = New DataTable
'        dt = SignalBaseDataSet.UserTable ': dt.Clear()
'        Dim itemFound As Integer = UserTableBindingSource.Find("UserName", UserNameTextEdit.EditValue)
'        If itemFound > -1 Then
'            Dim row As DataRow = dt.Rows(itemFound)
'            Dim uname As String = row("UserName")
'            Dim pword As String = row("Password")
'            Dim IsAdmin As Object = row("IsAdmin")
'            If IsAdmin Then

'            Else

'            End If

'        Else
'            MsgBox("Invalid  ") '& Chr(39) & Pair & Chr(39) & " do not exist.", MsgBoxStyle.Information, "BitBuyBot")
'            Return False
'        End If

'        Return True

'    End Function

'#End Region '#FUNCTIONS

'End Class
