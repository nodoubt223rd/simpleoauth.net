using System;
using System.Web.UI;

using SimpleOAuth.OAuth2;

public partial class default_page : Page
{
    #region Properties
    protected AccessToken Token
    {
        get { return (AccessToken)ViewState["Token"]; }
        set { ViewState["Token"] = value; }
    }

    protected string State
    {
        get { return (string)Session["State"]; }
        set { Session["State"] = value; }
    }
    #endregion Properties

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        this.errorPanel.Visible = false;
        this.errors.Text = string.Empty;

        if (this.IsPostBack)
            return;

        ClientConfiguration config = OAuth2Configuration.Current.Clients[0];

        if (config.AuthorizationStateRequired && string.IsNullOrWhiteSpace(this.State))
            this.State = Guid.NewGuid().ToString("N");

        OAuth2Client.AuthorizationReturn ar = OAuth2Client.GetAuthorizationReturn(this.State);

        if (ar != null)
        {
            this.State = ar.State;

            try
            {
                OAuth2Client c = new OAuth2Client(config);

                this.Token = c.GetAccessToken(ar.Code, this.State);

                this.setupPanel.Visible = false;
                this.servicePanel.Visible = true;
            }
            catch (Exception ex)
            {
                _ShowError(ex);
            }
        }
        else
        {
            this.configParameters.Text =
                "<p>Name <code>" + config.Name + "</code></p>"
                + "<p>ClientId <code>" + config.ClientId + "</code></p>"
                + "<p>ClientSecret <code>" + config.ClientSecret + "</code></p>"
                + "<p>AuthorizationStateRequired <code>" + config.AuthorizationStateRequired + "</code></p>"
                + "<p>AuthorizationUrl <code>" + config.AuthorizationUrl + "</code></p>"
                + "<p>TokenUrl <code>" + config.TokenUrl + "</code></p>"
                + "<p>RedirectUrl <code>" + config.RedirectUrl + "</code></p>"
                + "<p>Scopes <code>" + config.Scope + "</code></p>";
        }
    }

    protected void login_Click(object sender, EventArgs e)
    {
        try
        {
            OAuth2Client c = new OAuth2Client(OAuth2Configuration.Current.Clients[0]);

            Response.Redirect(c.GetAuthorizationUrl(this.State));
        }
        catch (System.Threading.ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            _ShowError(ex);
        }
    }
    #endregion Event Handlers

    #region Implementation
    private void _ShowError(Exception e)
    {
        this.errorPanel.Visible = true;
        this.errors.Text = e.ToString();
    }

    private void _ShowError(string e)
    {
        this.errorPanel.Visible = true;
        this.errors.Text = e;
    }
    #endregion Implementation
}
