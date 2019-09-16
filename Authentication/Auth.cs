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
            if (this.isAuthenticated())
                return HttpContext.Current.User.Identity.Name;
            return null;
        }

        public bool isCurrentUserInActiveDirectory()
        {
            string username = this.getCurrentAdUser();
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

        public string getCurrentAdUser()
        {
            string loginName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return loginName.Replace("PLANNING\\", "");
        }

    }
}
