Module mainModule

	'Luno
	'ja9p3dd39b3hz
	'glnQxJecFjMEkDZHAIKIJ5EAtCmnJbrPYaiMOwfDlQs

	'Binance
	'Key CpnHEP1VNRaRKGktL6r5bxz5uVOHyFi949uphkNWhekyEhPK9YEQBLXYd83aORHQ
	'Code 8V1pNl2GlCN2J2ed5GKJJGlgPfhjgJ2Bv5zwewNyR6LQ1Z1rQaa4PfcKsekDFOtC
	'March 25

	'Coinigy

	'Coinigy Websocket API Private channel
	'651CD393-726A-F6DC-43EA-213DCA3845FB

	'DESKTOP-59PABLF\MSSQLSERVER2019

	'BinanceAIMdev
	'FfD4OoYqD1UF8RR8H67bAtLoXj1WgprlS9fBwYwgEa8ho7Bbfkf7tWZnFY1eoItn
	'Secret Key
	'v0VhCkrEZx4eBi38JCp4Jbi3rQ9hZQEMLC8yQS8jKm0XIFevQcYkmeXy2G95RpFw

	'BinanceAIMdevA
	'FfD4OoYqD1UF8RR8H67bAtLoXj1WgprlS9fBwYwgEa8ho7Bbfkf7tWZnFY1eoItn
	'Secret Key
	'v0VhCkrEZx4eBi38JCp4Jbi3rQ9hZQEMLC8yQS8jKm0XIFevQcYkmeXy2G95RpFw

	'BinanceAIMDevB
	'Hd5UAqrkhOAZ0jHHLjQd87aCCqJWjUI8kfhHd5ToDWqnJMNCjhnyZ9xtz59QwNzw
	'Secret Key
	't5JKhw1Sb4peX5Y1wnUdgd6r6NTBqCjVUcIoFNxTvUbivU5ygVYf9v5dAVpdh8fK

	'BinanceAIMdevC
	'API Key
	'LTQ6us3ltGI9Wv4kuKwIItYYpjXu7CsDMQygf6pZm8awQdUgcpFFZdVUXiEtydRh
	'Secret Key
	'wfwC8roU5rF7bvLmVKN1HNnifLedrGcghELW1AGigHBNZ5HO9nQr0qFf2P2EjuK1

	'BINANCEDEV
	'API Key
	'A53tMngpyWc18du15IBsAhY8mqihjhIJt8U1zUN1NTrLexEv30iVMkwRYwVRGVrC
	'Secret Key
	'QoYKHocp3byZyiMwEamB2lB00nNSPSE798L3Co3IwiULtHM3UyzJtmf1HHSsa4Yx


	'#TOPROPERCASE EXTENSION
	<System.Runtime.CompilerServices.Extension()>
	Function ToProperCase(ByVal str As String) As String
		Dim myTI As System.Globalization.TextInfo
		myTI = New System.Globalization.CultureInfo("en-US", False).TextInfo
		str = str.ToLower
		str = myTI.ToTitleCase(str)
		Return str
	End Function

	'#LOCAL FOLDERS
	Public localversion As String = "1.0"
	'#FOLDERS
	Public LocalFolderEnv As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\" & "aimzacom\" & localversion '& "\" '& baseparams.Exchange
	'TODO SYNC with MAINMODULE
	Public Function LocalFolder(ByVal filestring As String, Optional subfolder As String = "") As String
		'%localappdata%
		'%userprofile%\documents - mydocuments
		'%systemdrive%\ProgramData
		'%appdata%

		Dim result As String = LocalFolderEnv
		Dim resultstring As String = String.Empty
		Try
			If Not String.IsNullOrEmpty(subfolder) Then
				'#APPLICATION FOLDER
				result = LocalFolderEnv & "\" & subfolder
			End If
			If Not IO.Directory.Exists(result) Then
				IO.Directory.CreateDirectory(result)
			End If
			resultstring = result & "\" & filestring
		Catch ex As Exception ': exerr = ex
			'Throw New ExceptionExt(ex.Message)

		End Try

		Return resultstring
	End Function


End Module
