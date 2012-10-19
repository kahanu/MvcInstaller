<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<MvcInstaller.Settings.InstallerConfig>" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>MvcInstaller - Install</title>
    <link href="<%: Url.Content("~/MvcInstaller/Site.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%: Url.Content("~/MvcInstaller/jquery-1.6.1.min.js") %>" type="text/javascript"></script>
<style type="text/css">
.installer-label
{
    float: left; width: 180px; font-weight: bold;
}

.fieldwrapper { margin-bottom: 0.5em; clear: both; }
.fieldwrapperindented { margin-bottom: 0.5em; clear: both; margin-left: 20px; }
.error { color: Red; }
        #update
        {
            width: 90%;
            background-color: #ccc;
            padding: 10px;
            margin: 0 auto;
        }
</style>
</head>

<body>
    <div class="page">

        <div id="header">
            <div id="title">
                <h1>MvcInstaller for ASP.NET MVC 4</h1>
            </div>
              
            <div id="logindisplay">
                &nbsp;      
            </div> 
            
            <div id="menucontainer">
            
                <ul id="menu">              
                    <li><%: Html.ActionLink("Home", "Index", "Home")%></li>
                </ul>
            
            </div>
        </div>

        <div id="main">

    <h2>
        MVC Site Installer</h2>
                    <div id="update">
	                <p><b>Update!</b> - this version of MvcInstaller works with the .Net 4.0 framework in VS 2010 projects and in VS 2012 projects using the .Net 4.5 framework.  <b>BUT</b>, and here's the big but, both implementations use the new Membership system based on the System.Web.Providers namespace!</p>
	                <p>If you want to use the .Net 4.5 SimpleMembershipProviders, I have not yet created a workable version for this new system.  Stay tuned!</p>
            </div>
    <p>For more information on this installer, please visit <a href="http://www.mvccentral.net/s/44" target="_blank">http://www.mvccentral.net/s/44</a>.</p>
    <p>The data below is the information from your installer.config file that will be used to install your application.</p>
    <p style="color:Red;"><b>Important!</b> Make sure this path on the server has <b>write permissions</b>, at least for the installation process.  You can disable it afterward.</p>
    <fieldset>
        <legend>Application</legend>
        <div class="fieldwrapper">
            <div class="installer-label">Name:</div>
            <div class="display-field"><%= Model.ApplicationName %></div>
        </div>
    </fieldset>

    <fieldset>
        <legend>Database settings</legend>
        <p>The database and db user needs to be created prior to running the installer!</p>
        
        <div class="fieldwrapper">
            <div class="installer-label">
                Database:</div>
            <div class="display-field">
                <%= Model.Database.InitialCatalog %></div>
        </div>
        <div class="fieldwrapper">
            <div class="installer-label">
                Data Source:</div>
            <div class="display-field">
                <%= Model.Database.DataSource %></div>
        </div>
        <% if (Model.Database.UseTrustedConnection)
           { %>
        <div class="fieldwrapper">
            <div class="installer-label">Trusted Connection:</div>
            <div class="display-field">True</div>
        </div>
        <%}
           else
           { %>
        <div class="fieldwrapper">
            <div class="installer-label">
                Username:</div>
            <div class="display-field">
                <%= Model.Database.UserName%></div>
        </div>
        <%} %>
    </fieldset>
    <fieldset>
        <legend>Membership settings</legend>
        <% if (Model.Membership.Create)
           { %>
        <p>These are the Roles and Users to be created by the installer.</p>
        <% foreach (var role in Model.RoleManager.Roles)
           {%>
        <div class="fieldwrapper">
            <div class="installer-label">
                Role:</div>
            <div class="display-field">
                <%= role.Name%></div>
        </div>
        <% foreach (var user in role.Users)
           {%>
        <div class="fieldwrapperindented">
            <div class="installer-label">
                User in role:</div>
            <div class="display-field">
                <%= user.UserName%></div>
        </div>
        <%} %>
        <%} %>
        <%}
           else
           { %>
           <p>The Asp.Net Membership system will not be created.</p>
        <%} %>
    </fieldset>
    <div id="response"></div>
    <button id="runprocess">Install</button> 
    <span id="useSecurityGuardSpan">
        <input type="checkbox" id="useSecurityGuard" name="useSecurityGuard" value="true" /> Use <a href="http://www.mvccentral.net/s/43" target="_blank" title="Click here to read an article on SecurityGuard.">SecurityGuard</a> 
        (<span id="qmark" title="Indicates whether to display the LogOn link to point to the SecurityGuard LogOn page. Not to install SecurityGuard." style="cursor: pointer; color: Blue;">?</span>)
    </span>
    <span id="loader" style="display: none;">
        <img src="/MvcInstaller/ajax-loader.gif" alt="Processing... please wait!" />&nbsp;Processing... please wait!
    </span>

    <script type="text/javascript">

        $(function () {
            var loader = $("#loader");
            var button = $("#runprocess");
            var resp = $("#response");
            var SGSpan = $("#useSecurityGuardSpan");
            var SGIsChecked = false;

            $("#runprocess").live("click", function () {
                button.hide();
                SGSpan.hide();
                loader.show();
                SGIsChecked = $(":checkbox").is(":checked");

                $.ajax(
                {
                    url: '<%= Url.Action("Run", "Install") %>',
                    datatype: 'json',
                    type: 'POST',
                    success: OnSuccess,
                    error: OnError
                });
            });

            function OnSuccess(data) {
                loader.hide();
                var msg = data.Message;
                if (data.Success) {
                    if (SGIsChecked) {
                        msg += " <a href=\"/SGAccount/LogOn\">Log On</a>";
                    } else {
                        msg += " <a href=\"/Account/LogOn\">Log On</a>";
                    }

                    resp.removeClass("error").addClass("success");
                } else {
                    button.show();
                    resp.addClass("error");
                }
                resp.html(msg)
            }

            function OnError(data) {
                loader.hide();
                resp.html(data.Message);
                button.show();
                SGSpan.show();
            }
        });
    
    </script>
            <div id="footer">
            </div>
        </div>
    </div>
</body>
</html>
