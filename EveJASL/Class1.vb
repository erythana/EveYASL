Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json
Imports System.ComponentModel

Public Class EveYASL
	Public Shared Property access_token
	Public Shared Property codechallenge As String
	Private Shared refresh_token As String

	Private Shared oAuthServer As String
	Private Shared clientID As String
	Private Shared verifier As String


	Public Shared Sub Initialize()
		'TODO: Implement check wheter to expose the access_code or not (and to checker wheter everything submitted is correct)
		CreatePKCE()



	End Sub

	Public Shared Sub Settings(ByVal oAuthServer As String, ByVal clientID As String, ByVal authorizationcode As String)
		If oAuthServer <> "" And clientID <> "" And authorizationcode <> "" Then
			CreatePKCE()
			oAuthServer = oAuthServer
			clientID = clientID
			Dim requesttype As String = "authorization_code"
			Dim modifiedcode = "&code=" & authorizationcode
			Dim modifiedVerifier = "&code_verifier=" & verifier

			Dim json_result As JSON_result = ReturnAccessCode(oAuthServer, requesttype, modifiedcode, clientID, modifiedVerifier)
			If Not json_result Is Nothing Then
				access_token = json_result.access_token
				refresh_token = json_result.refresh_token

				'set up timer for refresh_token:
				Dim timerDelegate As System.Threading.TimerCallback = AddressOf GetRefreshToken
				Dim refreshTimer As New System.Threading.Timer(timerDelegate, Nothing, 0, 20000)

			End If
		End If
	End Sub

	Private Shared Sub CreatePKCE()
		Dim buffer = New Byte(32) {}
		Dim rng = New RNGCryptoServiceProvider()
		rng.GetBytes(buffer)
		verifier = Convert.ToBase64String(buffer).Replace("=", "").Replace("+", "-").Replace("/", "_")

		Dim sha = New SHA256Managed()
		sha.ComputeHash(Encoding.UTF8.GetBytes(verifier))
		codechallenge = Convert.ToBase64String(sha.Hash).Replace("=", "").Replace("+", "-").Replace("/", "_")
	End Sub

	Private Shared Function ReturnAccessCode(ByVal authServer As String, ByVal requesttype As String, ByVal modifiedcode As String, ByVal client_id As String, Optional ByVal verifier As String = "")
		Try
			Dim request As WebRequest = WebRequest.Create(authServer)
			Dim postdata As String = "grant_type=" & requesttype & modifiedcode & "&client_id=" & client_id & verifier
			Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postdata)
			request.Method = "POST"
			request.ContentType = "application/x-www-form-urlencoded"
			request.ContentLength = byteArray.Length
			Dim datastream As System.IO.Stream = request.GetRequestStream
			datastream.Write(byteArray, 0, byteArray.Length)
			datastream.Close()
			Dim response As WebResponse = request.GetResponse
			If Not response Is Nothing Then
				Dim pagecontent As String = New StreamReader(response.GetResponseStream()).ReadToEnd
				Dim jsonresult As JSON_result = JsonConvert.DeserializeObject(Of JSON_result)(pagecontent)
				Return jsonresult
			Else
			End If

		Catch ex As WebException
			'TODO: Where to pipe error?
			Dim pagecontent = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd

		End Try
		Return Nothing
	End Function

	Private Shared Sub GetRefreshToken()

		Dim requesttype As String = "refresh_token"
		Dim modifiedcode As String = "&refresh_token=" & refresh_token

		Dim json_result As JSON_result = ReturnAccessCode(oAuthServer, requesttype, modifiedcode, clientID)
		access_token = json_result.access_token
		refresh_token = json_result.refresh_token
	End Sub



	Private Class JSON_result
		Public Property access_token As String
		Public Property expires_in As Integer
		Public Property token_type As String
		Public Property refresh_token As String
	End Class
End Class

