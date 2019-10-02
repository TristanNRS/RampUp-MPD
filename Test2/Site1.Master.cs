using System;
using Authentication;


namespace Test2
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Auth auth = new Auth();
            string username;

            // set signed in user name
            if ((username = auth.getUsername()) != null)
            {
                authResult.Text = username;
            }
            else
            {
                authResult.Text = "ACCESS DENIED";
            }


        }

    }
}