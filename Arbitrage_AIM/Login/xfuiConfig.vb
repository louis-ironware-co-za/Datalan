Imports System.ComponentModel
Imports System.Text
Imports System.Data.SqlClient
Imports System.Net
Imports DevExpress.XtraEditors

Partial Public Class xfuiConfig

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

    Private connstr As String = "Data Source=signaltradingserver.database.windows.net;Initial Catalog=SignalBase;User ID=signaladmin;Password=********;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
    Private HostName As String = String.Empty
    Public UserId As String = String.Empty
    Public Password As String = String.Empty
    'Public IsAdmin As Boolean = False
    Public ServerConnection As String = String.Empty

#End Region '#VAR

#Region "#FORM"

    '#FORM LOAD
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    '#FORM SHOWN
    Private Sub xfuiConfig_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        '#LOCAL HOSTNAME
        HostName = GetHostByName()
        LocalHostTextEdit.Text = HostName

        'ServerConnection
        'getserverbyname

        '#SERVER CONNECTTION 
        'ServerConnection = UserTableAdapter.Connection.DataSource
        'ServerConnectionLabelControl.Text = ServerConnection
        'ServerTextEdit.Text = ServerConnection ' "signaltradingserver.database.windows.net" ' UserTableAdapter.Connection.DataSource
        'ServerConnectionLabelControl.Text = ServerConnection

        '#USER
        UserNameTextEdit.Text = UserId
        PasswordTextEdit.Text = Password

        Me.Opacity = 100
        Me.BringToFront()
    End Sub

    '#CANCEL BUTTON
    Private Sub CancelSimpleButton_Click(sender As Object, e As EventArgs) Handles CancelSimpleButton.Click
        Me.Close()
    End Sub


#End Region '#FORM

#Region "#FORM BUTTONS"

    Private Sub RaiseIPSimpleButton_Click(sender As Object, e As EventArgs) Handles RaiseIPSimpleButton.Click
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Refreshing IP address...")

        Try
            IPaddressListBoxControl.Items.Clear()

            '#LOCAL HOSTNAME
            HostName = GetHostByName()
            LocalHostTextEdit.Text = HostName
            ServerConnectionLabelControl.Text = String.Empty
        Catch ex As Exception
            MsgBox("Error: IP Address not available", MsgBoxStyle.Information, "Local Host")
        End Try

        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()

    End Sub

    Private Sub RaiseTestSimpleButton_Click(sender As Object, e As EventArgs) Handles RaiseTestSimpleButton.Click

        ServerConnection = ServerTextEdit.Text

        If RaiseConnectionTest(ServerConnection) Then
            ServerConnectionLabelControl.Text = "Connected " & ServerConnection
        Else
            ServerConnectionLabelControl.Text = "Failed to connect to server " & ServerConnection

        End If



    End Sub

    '#IP ADDRESS -COPY TO CLIPBOARD
    Private Sub IPaddressListBoxControl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles IPaddressListBoxControl.SelectedIndexChanged
        Dim BoxControl As ListBoxControl = TryCast(IPaddressListBoxControl, ListBoxControl)
        Dim IPvAddress As String = String.Empty
        If IsNothing(BoxControl.SelectedItem) Then Exit Sub
        IPvAddress = BoxControl.SelectedItem.ToString

        IPvAddress = IPvAddress.Replace("IP Version 4 Address: ", "")
        IPvAddress = IPvAddress.Replace("IP Version 6 Address: ", "")

        If Not String.IsNullOrEmpty(IPvAddress) Then
            Clipboard.SetText(IPvAddress)
            ServerConnectionLabelControl.Text = "IP address " & IPvAddress & " copied to clipboard"
        Else
            Clipboard.Clear()
            ServerConnectionLabelControl.Text = "IP address " & IPvAddress & " copied to clipboard"
        End If
    End Sub

#End Region '#FORM BUTTONS


#Region "#FUNCTIONS"

    Private Function RaiseConnectionTest(ByVal ServerId As String) As Boolean

        ServerConnectionLabelControl.Text = "Connecting..."

        '#VALIDATE INPUT CREDENTIALS
        If String.IsNullOrEmpty(ServerId) Then
            MessageBox.Show("Invalid Entry: Enter a valid Server to continue", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            ServerTextEdit.Focus()
            'Return False
        End If
        If String.IsNullOrEmpty(UserId) Then
            MessageBox.Show("Invalid Entry: Enter a valid User ID to continue", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            UserNameTextEdit.Focus()
            'Return False
        End If
        If String.IsNullOrEmpty(Password) Then
            MessageBox.Show("Invalid Entry: Enter a valid Password to continue", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            PasswordTextEdit.Focus()
            'Return False
        End If

        Me.Opacity = 0


        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Connecting...")


        'Server=tcp:signaltrading.database.windows.net,1433;Initial Catalog=SignalTrading;Persist Security Info=False;User ID=SignalTradingAdmin;Password=Azu!@#$56789;
        'MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;

        'connstr = "Data Source=" & ServerId & ";Initial Catalog=SignalTrading;Persist Security Info=True;User ID=SignalTradingAdmin;Password=Azu!@#$56789"
        connstr = "Data Source=" & ServerId & ";Initial Catalog=SignalTrading;Persist Security Info=False;User ID=SignalTradingAdmin;Password=Azu!@#$56789;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;"


        '#CONNECTSERVER
        Dim nRetries As Integer = 0
        Try

            '#RETRY
            While nRetries < 3
                '   
                Try
                    Try
                        Dim conn As SqlConnection = Nothing
                        conn = New SqlConnection(connstr)
                        Dim comm As SqlCommand = Nothing
                        comm = New SqlCommand("SELECT UserName,Password FROM UserTable WHERE Alias = @Alias AND Password = @Password", conn)
                        Dim UserAlias As SqlParameter = New SqlParameter("@Alias", SqlDbType.VarChar)
                        Dim UserPassword As SqlParameter = New SqlParameter("@Password", SqlDbType.VarChar)
                        UserAlias.Value = UserNameTextEdit.EditValue
                        UserPassword.Value = PasswordTextEdit.EditValue
                        comm.Parameters.Add(UserAlias)
                        comm.Parameters.Add(UserPassword)
                        comm.Connection.Open()

                        If (conn.State = ConnectionState.Open) Then
                            'conn.Open()
                            Dim myReader As SqlDataReader = comm.ExecuteReader(CommandBehavior.CloseConnection)
                            If myReader.Read() = True Then
                                'MessageBox.Show("You have logged in successfully " & UserNameTextEdit.EditValue)
                                'Me.Hide()
                            Else
                                MessageBox.Show("Login Failed...Try again !", "Invalid Login Access Denied", MessageBoxButtons.OK, MessageBoxIcon.[Error])
                                'UserNameTextEdit.EditValue.Clear()
                                'PasswordTextEdit.EditValue.Clear()
                                UserNameTextEdit.Focus()
                            End If
                            conn.Dispose()

                        Else
                            MessageBox.Show("Login Failed...Try again !", "Invalid Login Access Denied", MessageBoxButtons.OK, MessageBoxIcon.[Error])
                            'UserNameTextEdit.EditValue.Clear()
                            'PasswordTextEdit.EditValue.Clear()
                            UserNameTextEdit.Focus()

                        End If

                    Catch ex As Exception
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
                    End Try
                    nRetries = 4

                Catch ex As Exception
                    nRetries += 1
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Server connection failed. Attempting retry... (" & nRetries.ToString & " of 3)")
                End Try

            End While

        Catch ex As Exception
            MsgBox("Error: " & ex.Message, MsgBoxStyle.Information, "Server: " & ServerConnection)
        Finally
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()

            Me.Opacity = 100

        End Try


        If nRetries = 3 Then
            MsgBox("Error connecting to server, service not available please try again in a few minutes.", MsgBoxStyle.Information, "Server: " & ServerConnection)
            'Return False
        End If


        'Me.Opacity = 0
        Try
            'Me.UserTableAdapter.Fill(Me.SignalTradingDataBaseDataSet.UserTable)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Table Adapter Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        End Try


        Return True
    End Function




    '#HOSTNAME - SERVER
    Private Function GetHostByName() As String
        '#HOST SERVER
        Dim Host As String = Dns.GetHostName
        'Dim IPvAddress As String = String.Empty
        'Dim IPv6Address As String = String.Empty

        'Console.WriteLine("Host Name : " & hostName & vbNewLine)
        For Each address In Dns.GetHostEntry(HostName).AddressList()
            Select Case Convert.ToInt32(address.AddressFamily)
                Case 2
                    Dim IPvAddress As String = String.Empty
                    IPvAddress = "IP Version 4 Address: " & address.ToString
                    IPaddressListBoxControl.Items.Add(IPvAddress)
                    Console.WriteLine("IP Version 4 Address: " & address.ToString)
                Case 23
                    Dim IPvAddress As String = String.Empty
                    IPvAddress = "IP Version 6 Address: " & address.ToString
                    IPaddressListBoxControl.Items.Add(IPvAddress)
                    Console.WriteLine("IP Version 6 Address: " & address.ToString)
            End Select
        Next


        Return Host
    End Function


#End Region '"#FUNCTIONS"

    Private Sub ConnectSimpleButton_Click(sender As Object, e As EventArgs)

        'connstr = "Data Source=" & ServerId & ";Initial Catalog=SignalTrading;Persist Security Info=True;User ID=SignalTradingAdmin;Password=Azu!@#$56789"

        'ServerConnectionLabelControl.Text = ServerConnection & "  " & IP4Address

        'Me.Opacity = 100

        'Try
        '    'DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
        '    'DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Connecting to server...")
        '    '#CONNECTSERVER
        '    Dim nRetries As Integer = 0

        '    '#RETRY
        '    While nRetries < 3
        '        '   Me.Opacity = 0
        '        Try
        '            Try
        '                Dim conn As SqlConnection = Nothing
        '                conn = New SqlConnection(cs)
        '                Dim comm As SqlCommand = Nothing
        '                comm = New SqlCommand("SELECT UserName,Password FROM UserTable WHERE Alias = @Alias AND Password = @Password", conn)
        '                Dim UserAlias As SqlParameter = New SqlParameter("@Alias", SqlDbType.VarChar)
        '                Dim UserPassword As SqlParameter = New SqlParameter("@Password", SqlDbType.VarChar)
        '                UserAlias.Value = UserNameTextEdit.EditValue
        '                UserPassword.Value = PasswordTextEdit.EditValue
        '                comm.Parameters.Add(UserAlias)
        '                comm.Parameters.Add(UserPassword)
        '                comm.Connection.Open()

        '                If (conn.State = ConnectionState.Open) Then
        '                    'conn.Open()
        '                    Dim myReader As SqlDataReader = comm.ExecuteReader(CommandBehavior.CloseConnection)
        '                    If myReader.Read() = True Then
        '                        'MessageBox.Show("You have logged in successfully " & UserNameTextEdit.EditValue)
        '                        'Me.Hide()
        '                    Else
        '                        MessageBox.Show("Login Failed...Try again !", "Invalid Login Access Denied", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        '                        'UserNameTextEdit.EditValue.Clear()
        '                        'PasswordTextEdit.EditValue.Clear()
        '                        UserNameTextEdit.Focus()
        '                    End If
        '                    conn.Dispose()

        '                Else
        '                    MessageBox.Show("Login Failed...Try again !", "Invalid Login Access Denied", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        '                    'UserNameTextEdit.EditValue.Clear()
        '                    'PasswordTextEdit.EditValue.Clear()
        '                    UserNameTextEdit.Focus()

        '                End If

        '            Catch ex As Exception
        '                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        '            End Try
        '            nRetries = 4

        '        Catch ex As Exception
        '            nRetries += 1
        '            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Server connection failed. Attempting retry... (" & nRetries.ToString & " of 3)")
        '        End Try
        '    End While

        '    If nRetries = 3 Then
        '        MsgBox("Error connecting to server, service not available please try again in a few minutes.", MsgBoxStyle.Information, "Server: " & ServerConnection)
        '        'Return False
        '    End If


        '    'Me.Opacity = 0
        '    Try
        '        Me.UserTableAdapter.Fill(Me.SignalTradingDataBaseDataSet.UserTable)
        '        nRetries = 4
        '    Catch ex As Exception
        '        nRetries += 1
        '        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Server connection failed. Attempting retry... (" & nRetries.ToString & " of 3)")
        '    End Try


        '    'Return True

        'Catch ex As Exception
        '    MsgBox("Error: " & ex.Message, MsgBoxStyle.Critical, "BitBuyBot")
        '    'Return False
        'Finally
        '    'DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        'End Try
    End Sub



    'Private Sub LoginSimpleButton_Click(sender As Object, e As EventArgs) Handles LoginSimpleButton.Click
    '    Dim result As DialogResult = Nothing

    '    UserId = UserNameTextEdit.Text
    '    Password = PasswordTextEdit.Text

    '    If String.IsNullOrEmpty(UserId) Then
    '        MessageBox.Show("Invalid Entry: Enter a valid User ID to continue", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
    '        UserNameTextEdit.Focus()
    '        'Return False
    '    End If
    '    If String.IsNullOrEmpty(Password) Then
    '        MessageBox.Show("Invalid Entry: Enter a valid Password to continue", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
    '        PasswordTextEdit.Focus()
    '        'Return False
    '    End If

    '    Try
    '        'DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
    '        'DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Connecting to server...")
    '        '#CONNECTSERVER
    '        Dim nRetries As Integer = 0

    '        '#RETRY
    '        While nRetries < 3
    '            Me.Opacity = 0
    '            Try
    '                Try
    '                    Dim conn As SqlConnection = Nothing
    '                    conn = New SqlConnection(cs)
    '                    Dim comm As SqlCommand = Nothing
    '                    comm = New SqlCommand("SELECT UserName,Password FROM UserTable WHERE Alias = @Alias AND Password = @Password", conn)
    '                    Dim UserAlias As SqlParameter = New SqlParameter("@Alias", SqlDbType.VarChar)
    '                    Dim UserPassword As SqlParameter = New SqlParameter("@Password", SqlDbType.VarChar)
    '                    UserAlias.Value = UserNameTextEdit.EditValue
    '                    UserPassword.Value = PasswordTextEdit.EditValue
    '                    comm.Parameters.Add(UserAlias)
    '                    comm.Parameters.Add(UserPassword)
    '                    comm.Connection.Open()

    '                    If (conn.State = ConnectionState.Open) Then
    '                        'conn.Open()
    '                        Dim myReader As SqlDataReader = comm.ExecuteReader(CommandBehavior.CloseConnection)
    '                        If myReader.Read() = True Then
    '                            'MessageBox.Show("You have logged in successfully " & UserNameTextEdit.EditValue)
    '                            'Me.Hide()
    '                        Else
    '                            MessageBox.Show("Login Failed...Try again !", "Invalid Login Access Denied", MessageBoxButtons.OK, MessageBoxIcon.[Error])
    '                            'UserNameTextEdit.EditValue.Clear()
    '                            'PasswordTextEdit.EditValue.Clear()
    '                            UserNameTextEdit.Focus()
    '                        End If
    '                        conn.Dispose()

    '                    Else
    '                        MessageBox.Show("Login Failed...Try again !", "Invalid Login Access Denied", MessageBoxButtons.OK, MessageBoxIcon.[Error])
    '                        'UserNameTextEdit.EditValue.Clear()
    '                        'PasswordTextEdit.EditValue.Clear()
    '                        UserNameTextEdit.Focus()

    '                    End If

    '                Catch ex As Exception
    '                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
    '                End Try
    '                nRetries = 4

    '            Catch ex As Exception
    '                nRetries += 1
    '                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Server connection failed. Attempting retry... (" & nRetries.ToString & " of 3)")
    '            End Try
    '        End While

    '        If nRetries = 3 Then
    '            MsgBox("Error connecting to server, service not available please try again in a few minutes.", MsgBoxStyle.Information, "Server: " & ServerConnection)
    '            'Return False
    '        End If


    '        'Me.Opacity = 0
    '        Try
    '            Me.UserTableAdapter.Fill(Me.SignalTradingDataBaseDataSet.UserTable)
    '            nRetries = 4
    '        Catch ex As Exception
    '            nRetries += 1
    '            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Server connection failed. Attempting retry... (" & nRetries.ToString & " of 3)")
    '        End Try


    '        'Return True

    '    Catch ex As Exception
    '        MsgBox("Error: " & ex.Message, MsgBoxStyle.Critical, "BitBuyBot")
    '        'Return False
    '    Finally
    '        'DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
    '    End Try


    '    '#USERTABLE
    '    Dim dt As DataTable = SignalTradingDataBaseDataSet.Tables("UserTable") ': dtSymbols.Clear() 'New DataTable
    '    Dim itemFound As Integer = UserTableBindingSource.Find("Alias", UserId) ' UserNameTextEdit.EditValue
    '    If itemFound > -1 Then
    '        Dim row As DataRow = dt.Rows(itemFound)
    '        Dim UserAlias As String = CType(row("Alias"), String)
    '        Dim UserPassword As String = CType(row("Password"), String)
    '        IsAdmin = CType(row("IsAdmin"), Boolean)
    '        '#UPDATE
    '        Dim datenow As DateTime = DateTime.Now
    '        Dim TokenId As String = datenow.ToShortTimeString
    '        Dim Host As Net.IPHostEntry = Nothing
    '        Dim HostName As String = String.Empty
    '        Dim IPAddress As String = String.Empty
    '        Try
    '            Host = System.Net.Dns.GetHostEntry(HostName)
    '            HostName = Host.HostName

    '            For Each Ipaddy As Net.IPAddress In Host.AddressList


    '                'RaiseSQLConnect()

    '                ''#CONNECT SERVER LABEL
    '                'ServerConnection = UserTableAdapter.Connection.DataSource
    '                'ServerConnectionLabelControl.Text = ServerConnection

    '                ''TODO - BaseEncrypt
    '                'UserNameTextEdit.Text = UserId
    '                'PasswordTextEdit.Text = password

    '            Next
    '            IPAddress = Host.AddressList(0).ToString()

    '            TokenId = HostName & "." & IPAddress


    '            'Dim constr As String = ServerConnectionLabelControl.Text
    '            'ServerConnectionLabelControl.Text = constr & " "


    '        Catch ex As Exception
    '            MsgBox("Failed to update Host Id, Select Ok to continue...", vbOKOnly, "Server Connection")
    '        End Try
    '        '#UPDATE LOGIN
    '        Try
    '            row.BeginEdit()
    '            row("IsConnected") = True
    '            row("TimeStamp") = datenow
    '            row("TokenId") = TokenId
    '            row.EndEdit()
    '            Me.Validate()
    '            Me.UserTableBindingSource.EndEdit()
    '            Me.TableAdapterManager.UpdateAll(Me.SignalTradingDataBaseDataSet)
    '            'dt.AcceptChanges()
    '        Catch ex As Exception
    '            MsgBox("Failed to update Server, Select Ok to continue...", vbOKOnly, "Server Connection")

    '        End Try
    '    Else
    '        MessageBox.Show("Invalid Credentials:" & vbCrLf & "The User ID and Password you have entered does not match", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
    '        'Return False
    '    End If

    '    Me.Opacity = 100

    '    '****************************************
    '    'RaiseSQLConnect()

    '    'If AuthenticateUser(UserId, Password) Then

    '    '    If RaiseConnectServer() Then



    '    '        '#UPDATE LOGIN



    '    '        'Me.DialogResult = DialogResult.OK
    '    '        'Me.Close()
    '    '    Else
    '    '        'Me.DialogResult = DialogResult.None
    '    '    End If
    '    'End If
    'End Sub

    ''#CANCEL BUTTON
    'Private Sub CancelSimpleButton_Click(sender As Object, e As EventArgs) Handles CancelSimpleButton.Click
    '    Me.Close()
    'End Sub

    'Dim mem As String

    'Private Sub GetIPAddress()

    '    Dim strHostName As String
    '    Dim strIPAddress As String
    '    strHostName = System.Net.Dns.GetHostName()
    '    strIPAddress = System.Net.Dns.GetHostByName(strHostName).AddressList(0).ToString()

    '    mem = strIPAddress
    '    MessageBox.Show("IP Address: " & strIPAddress)

    'End Sub

    'Private Function GetIPv4Address() As String
    '    GetIPv4Address = String.Empty
    '    Dim strHostName As String = System.Net.Dns.GetHostName()
    '    Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(strHostName)

    '    For Each ipheal As System.Net.IPAddress In iphe.AddressList
    '        If ipheal.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
    '            GetIPv4Address = ipheal.ToString()
    '        End If
    '    Next

    'End Function





End Class

