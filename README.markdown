Mvc Installer
by King Wilder
https://github.com/kahanu/MvcInstaller
Created on: 11/21/2010

*************************
Update on October 20, 2012

MvcInstaller.Mvc4 has been created for the .Net 4.0 and .Net 4.5 Frameworks for both VS 2010 and VS 2012 projects.  This version uses the System.Web.Providers namespace for both IDE's.
*************************


Updated on November 14, 2011

A small, easy to use ASP.NET MVC 2 application installer.  This pluggable application easily installs your database tables, views, etc, and creates your Roles and Users for your Membership system.

This is primarily targeted toward site administrators to quickly and easily install their ASP.NET MVC application on their staging or production server with little effort.

RESTRICTIONS

This version only works with the following:

-- SQL Server 2005 and higher
-- ASP.NET Membership provider
-- ASP.NET MVC 2 or higher

THE WAY MVC INSTALLER WORKS

The Mvc Installer gets all the information needed to install the database and Membership system from the installer.config file.  There is no UI interaction other then simply clicking a button to run the process.

This allows you to easily plug this assembly into any new Mvc application to quickly and easily install it.

By using this simple installer.config file, it allows you to easy set initial Membership administrators or other users at the time of installation.  So once the installer is finished, your database and membership system is ready to go.

Also, once the installer has finished successfully, it will remove the "AppInstalled" key in your appSettings section of the web.config file.  This will prevent others from trying to manually navigate to the installer after your application is up and running.


REQUIREMENTS

You MUST create the database PRIOR to running MvcInstaller.  You should be able to do this with your web host through their control panel.  MvcInstaller does NOT create the database, it simply runs the sql scripts for your schema and any seed data, but it does not create the database.



MODIFY YOUR INSTALLER.CONFIG FILE

This section will define the elements of the xml file so you can update it for your application.

Path
   RelativeSqlPath - enter the name of the folder where you copied your sql scripts.  By default its the App_Data folder.  Since this is a relative path, you DO NOT need to prefix the path with a slash (\).
   
Database
   UseTrustedConnection = a boolean whether you are using a trusted connection or not.  A trusted connection will ignore any username or password set in the Database element.
   EntityFrameworkEntitiesName - the name of the Entity Framework entities name if Entity Framework is used.  If not, leave blank and a standard connection string will be created.
   
   ConnectionStringName - the installer will create the connection string for you, so include the name here.  It will 	need to be manually set in the "membership", "profile", and "rolemanager" sections of your web.config file.
   DataSource - this is the name of the server.  localhost is set by default, but you can change it to whatever your 	server instance is.
   InitialCatalog - this is the name of the database.
   UserName - this is the username used as the login credentials for this database.
   Password - this is the password used as the login credentials for this database.
   
Membership
   Create - a boolean value to tell the installer whether it is to create the Membership system or not.
   Roles - a collection of roles to create
       Role - the name of a role to create
           Users - a collection of users to create for this role
               User - a user to create for this role.  It should include the following:
                   -- UserName
                   -- Password
                   -- Email
                   -- SecretQuestion
                   -- SecretAnswer
                   
That's all that's needed to setup your installer.

RUN THE INSTALLER

To run the installer, simply launch your application and in the address bar of your browser, navigate to the "Install" path.  Such as: http://www.mynewdomain.com/install

You will be shown the Mvc Installer page that displays all the information you've entered into your installer.config file.  You can check it to make sure it's correct.  When you confirm that everything is correct, then click the "Install" button.

New as of November, 1, 2011 - a checkbox now appears next to the "Install" button that will tell MvcInstaller whether you have already installed "SecurityGuard" and will be logging on using "SecurityGuard".  If so, check the box and it will simply change the resulting "LogOn" link to point to the "/SGAccount/LogOn" controller and action instead of the default "/Account/Login" path.

It will initiate an Ajax call to the InstallController and the Run action to install your database and membership system.

If it succeeds, it will display a message saying so, and a "Log On" link.

If there is an error, it will display that too.  Any errors should be pretty specific to tell you what you may have to change to make this work.


SUMMARY

I hope you find this helpful.  If you have any questions or problems, please enter them into the Discussion on CodePlex.

TROUBLESHOOTING

There are times when it doesn't install.  Here are some things to look for.

1) RolesManager = enabled - if you are installing the AspNet Membership system, make sure the RoleManager section in the web.config file is set to "true".
2) Write Permissions - for the installer to work, you must have Write Permissions set on the server.  After installation you can disable write permissions.
3) ConnectionString names - make sure your membership sections in the web.config file has the correct connectionStringName.

If you like the project, I always accept [Donations](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=CPTJFSQBBFSWN).

Thanks,

King Wilder


   	