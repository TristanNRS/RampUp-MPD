using System.Collections.Generic;
using System.DirectoryServices;
using System.Web;

namespace Authentication
{
    public class Auth
    {
        public bool isAuthenticated()
        {
            return ((HttpContext.Current.User != null) && HttpContext.Current.User.Identity.IsAuthenticated);
        }

        public string getUsername()
        {
            // Returns current signed in user's username
            if (this.isAuthenticated())
                return HttpContext.Current.User.Identity.Name;
            return null;
        }

        public bool isCurrentUserInActiveDirectory()
        {
            // Searches the AD for the current user's username
            string username = this.getCurrentUser();
            DirectorySearcher search = new DirectorySearcher();
            search.Filter = $"(SAMAccountName={username})";
            search.PropertiesToLoad.Add("cn");
            SearchResult result = search.FindOne();

            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string getCurrentUser()
        {
            // Gets current signed in user 
            string loginName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return loginName.Replace("PLANNING\\", "");
        }

        public bool isAuthorized(string role, List<string> authorizationNeeded)
        {
            // Checks if passed role is contained in list of authorized roles

            if(role != null || role != string.Empty)
                return authorizationNeeded.Contains(role);
            return false;
        }


    }
}
