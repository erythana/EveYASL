
# YASL
Yet another Swagger Library

YASL is a .NET library which supports you with the oAuth2 Authentication (with PKCE Implementation).
  - Provides an easy to use interface to retrieve the access_code which allows you to authenticate yourself at an endpoint.
  
# How to use the library
There are a few things to set up before you can use this library. The basic usage is like the following:
  - Initialize the Library
  Required to get the code_challenge which is used in the next step.
  This will look like this:
 

			YASL.Initialize()

  - Authenticate yourself with your credentials on the website  
  This is the website that will create the Authentication-Code and send it to the URI you supplied. Build the URL to your needs...  
  It can look like this (note: you need to pass the code_challenge from the DLL to this call):  

			https://login.eveonline.com/v2/oauth/authorize?response_type=code&redirect_uri=___CALLBACK-URIfromDevManagement___&client_id=___CLIENT-IDfromDevManagement___&scope=___SCOPESfromDevManagement___&code_challenge=" & YASL.codechallenge & "&code_challenge_method=S256"

  The scopes have to be seperated by an single space (url-encoded), split them with "%20", for example: scope1%20scope2
 - Get the Auth-Code - this depends on your layout
  There are different ways to obtain an Auth-Code. Read up on that topic. As i made an native application, i made the following:  
  In your application, right-click on your project, select properties and tick "single instance application" in your application-tab.  
  Then, press the "application" events at the bottom and change your Application-Events file to something like this:
  

    		Private Sub MyApplication_StartupNextInstance(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) Handles Me.StartupNextInstance

			If TypeOf Me.MainForm Is Form1 Then
				Dim s As String = ""
				If e.CommandLine.Count > 0 Then
					s = e.CommandLine.Item(0).ToString()
				End If
				DirectCast(Me.MainForm, Form1).ProcessCallback(s)
			End If
		End Sub

This will call the "ProcessCallback" Function in Form 1 and submit the string "s" (wich is the full URI).
Process it so you can get the auth code.  
Now, create the following registry keys (according to https://msdn.microsoft.com/en-us/windows/desktop/aa767914#MainContent)


    HKEY_CLASSES_ROOT
       <URI-protocolname>
          DefaultIcon
             (Default) = "<pathtoyourapplication.exe,1"
          shell
             open
                command
                   (Default) = "<pathtoyourapplication.exe" "%1"


 - Pass the variables to the Library  
  Yasl.Settings(AuthServer, Client-ID, authcode)
  The call cann look like this:  

			YASL.Settings("https://login.eveonline.com/v2/oauth/token", "f04e32dfasdfcc5a857dafa09127c", authcode)

  The token will update automatically before expiring. If you've set everything up you can retrieve the Access-Token via
  
			YASL.access_token

# ToDo
  There isn't really a error handling in this dll. Probably can crash when something doesn't work out. Going to look at this at another time or if i feel the need to do so...
  
# Bugs
  No known bugs so far...
  
# Support
I hope this library or the source code helps you working with the Swagger Interface. I created this for my own use for the Eve-Online ESI. If you found it usefull, feel free to support me with an nice ingame message or an tip to "Christian Gaterau"

