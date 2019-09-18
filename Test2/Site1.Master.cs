﻿using System;
using Authentication;


namespace Test2
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Auth auth = new Auth();
            string username;
            if ((username = auth.getUsername()) != null)
            {
                authResult.Text =  username;
                logout.Style.Add("display", "inline");
            }
            else
            {
                authResult.Text = "ACCESS DENIED";
                logout.Style.Add("display", "none");
            }


        }

        protected void logout_Click(object sender, EventArgs e)
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("Login.aspx", true);
        }
    }
}