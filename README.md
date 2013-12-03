# Simple OAuth for .NET

`Version 0.1.0.0`

A bare-bones .NET library to assist you with integrating OAuth2.

Please refer to the [OAuth2] final specification.

### Requirements ###

* .NET 4
* Visual Studio .NET 2010
* Newtonsoft Json 5+ (included)

### Using ###

Build the solution and add the simpleoauth.dll as a reference to your project.

Create client configurations in your configuration (see the
[sample site web.config]).

Get an access token from the configured site:

    OAuth2Client c = new OAuth2Client(OAuth2Configuration.Current.Clients["config name"]);

    Response.Redirect(c.GetAuthorizationUrl());

Some services, LinkedIn for example, require state to be passed with the
authorization request.

If the target service gets approved or rejected it will redirect back to your
configured page and you can detect it during page load:

    OAuth2Client.AuthorizationReturn ar = OAuth2Client.GetAuthorizationReturn();

    if (ar != null)
    {
        // got an authorization code, get a token
        OAuth2Client c = new OAuth2Client(OAuth2Configuration.Current.Clients["config name"]);
        AccessToken token = c.GetAccessToken(ar.Code);
    }

An `OAuth2Exception` will be thrown if there are problems.  It's up to you to
do something with the `AccessToken` once you have it, e.g. persist it in
association with a user, call some APIs using the `AccessToken.Token`, etc.

### Running the Sample Site ###

Modify the `www/site/web.config` to configure an OAuth2 service.

### Roadmap ###

* Expanded samples, including MVC
* .NET 4.5
* AccessToken storage paradigms
* Support for
  * Framework for implementing an OAuth 2 provider
  * Client password
  * Password credentials grant
  * Client credentials grant
  * Extension grants 
  * [OAuth 1.0] (client/provider) because some people don't do OAuth 2

### License ###

This software is released under the [Simple OAuth Project License]

[OAuth2]: http://tools.ietf.org/html/rfc6749
[sample site web.config]: www/site/web.config
[OAuth 1.0]: http://tools.ietf.org/html/rfc5849
[Simple OAuth Project License]: LICENSE

