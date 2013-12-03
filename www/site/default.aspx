<%@page language="C#" autoeventwireup="true" inherits="default_page" codebehind="default.aspx.cs" clientidmode="Static" %>
<!doctype html>
<html>
    <head>
        <title>simple oauth</title>
        <link rel="stylesheet" media="all" href="static/site.css" />
    </head>
    <body>
        <h1>Simple OAuth</h1>
        <form id="theForm" runat="server">
            <asp:panel id="errorPanel" runat="server" cssclass="errors" visible="false">
                <pre><asp:literal id="errors" runat="server" /></pre>
            </asp:panel>
            <asp:panel id="setupPanel" runat="server">
                <p>Please review OAuth2 parameters:</p>
                <asp:literal id="configParameters" runat="server" />
                <p class="vtop16">
                    <asp:button id="login" runat="server" onclick="login_Click" cssclass="button" text="OAuth Login" />
                </p>
            </asp:panel>
            <asp:panel id="servicePanel" runat="server" visible="false">
                <h2>Access Token</h2>
                <p>Token: <%= this.Token.Token %></p>
                <p>Token Type: <%= this.Token.TokenType %></p>
                <p>Expiration: <%= this.Token.Expiration.ToString("yyyy-MM-dd HH:mm:ss") %></p>
                <p>Scope: <%= string.Join(" ", this.Token.Scope.ToArray()) %></p>
                <p>Refresh Token: <%= this.Token.RefreshToken %></p>
            </asp:panel>
        </form>
    </body>
</html>
