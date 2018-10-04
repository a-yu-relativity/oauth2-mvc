# OAuth 2 MVC
MVC App demonstrating OAuth 2.0 code flow with Relativity

## How to Use
First, create an OAuth2 Client in the Relativity UI. You may name it whatever you'd like, but make sure you select "Code" for the authorization flow. Also, make sure to white-list `http://localhost:49203/home/authorize` as a Redirect URL.

[Creating an OAuth2 Client](https://help.relativity.com/9.6/Content/Relativity/Authentication/OAuth2_clients.htm#Creating)

Inside [HomeController.cs](/MVC%20App/Controllers/HomeController.cs#L13), replace the constants `BASE_URL`, `CLIENT_ID`, and `CLIENT_SECRET` with your actual credentials:

```C#
private const string BASE_URL = "https://instance";
// ...
private const string CLIENT_ID = "MY CLIENT ID";
private const string CLIENT_SECRET = "MY CLIENT SECRET";
// ...
```

### Note
You may have to change the port number for this web app, depending on which ports are available. In that case, you should be able to follow the instructions [here](https://stackoverflow.com/a/21202917) and update the REDIRECT_URI accordingly.
