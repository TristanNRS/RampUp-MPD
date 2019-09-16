using System;
using System.Web.Security;
using System.Data.SqlClient;

using Authentication;
using DbAccess;
using System.Web;

namespace Test2
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void dbLogin_Click(object sender, EventArgs e)
        {
            Auth auth = new Auth();
            string username = auth.getCurrentAdUser();
            try
            {
                /*
                 * Structure of DB
                 * 0    |   1       |   2
                 * ID   |   Name    |   Role
                 * 
                 * Example below:
                 * 1    |   John Smith  |   ADMIN
                 */
                Db db = new Db();
                SqlConnection cnn = db.getConnection();
                cnn.Open();

                string sql = "SELECT [Name], [Group] FROM [dbo].[Users]";
                SqlCommand command = db.getCommand(sql, cnn);
                SqlDataReader dataReader = db.getDataReader(command);

                while (dataReader.Read())
                {
                    if (dataReader.GetValue(0).ToString().Trim().Equals(username) && auth.isCurrentUserInActiveDirectory())
                    {
                        // set cookie 
                        FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddMinutes(30), false, dataReader.GetValue(1).ToString());
                        string cookiestr = FormsAuthentication.Encrypt(tkt);
                        HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
                        ck.Path = FormsAuthentication.FormsCookiePath;
                        Response.Cookies.Add(ck);

                        string strRedirect = Request["ReturnUrl"];
                        if (strRedirect == null)
                            strRedirect = "Default.aspx";
                        Response.Redirect(strRedirect, true);
                        break;
                    }
                }

                // close off connections
                db.closeOff(cnn, command, dataReader);
            } catch (Exception err)
            {
                Response.Write(err);
            }
        }
    }
}