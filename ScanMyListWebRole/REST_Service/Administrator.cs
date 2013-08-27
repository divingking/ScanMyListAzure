using SynchWebRole.Library_Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;

namespace SynchWebRole.REST_Service
{
    public partial class SynchDataService : IAdministrator
    {
        public SynchDataService() {
        }

        public Business FetchBusiness(ScanMyListDatabaseDataContext context, int bid)
        {
            var result = context.GetBusiness(bid);
            IEnumerator<GetBusinessResult> businessEnumerator = result.GetEnumerator();

            if (businessEnumerator.MoveNext())
            {
                GetBusinessResult retrievedBusiness = businessEnumerator.Current;

                return new Business()
                {
                    id = retrievedBusiness.id,
                    name = retrievedBusiness.name,
                    address = retrievedBusiness.address,
                    zip = (int)retrievedBusiness.zip,
                    category = retrievedBusiness.category,
                    email = retrievedBusiness.email
                };
            }
            else
            {
                throw new FaultException(
                    string.Format("Failed to fetch information for Business {0}", bid));
            }
        }

        public string FetchAccountEmail(ScanMyListDatabaseDataContext context, int aid)
        {
            var result = context.GetAccountEmail(aid);
            IEnumerator<GetAccountEmailResult> enumerator = result.GetEnumerator();

            if (enumerator.MoveNext())
            {
                return enumerator.Current.email;
            }
            else
            {
                throw new FaultException(
                    string.Format("Account with id {0} not found! ", aid));
            }
        }

        public int RegisterBusiness(Business business)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            int bid = context.CreateBusiness(
                business.name, business.address, business.zip, business.email, business.category, business.integration, business.tier);

            return bid;
        }

        public User RegisterAccount(User user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var result = context.GetAccountByLogin(user.login);
            if (result.GetEnumerator().MoveNext())
            {
                throw new WebFaultException(HttpStatusCode.Conflict);
            }
            else
            {
                string passwordHash = Encryptor.Generate512Hash(user.pass);

                context.CreateAccount(
                    user.login,
                    passwordHash,
                    user.email,
                    1,
                    user.tier);

                LoginUser loginUser = new LoginUser();
                loginUser.login = user.login;
                loginUser.pass = user.pass;
                loginUser.device = (int)DeviceType.website;

                User newUser = this.Login(loginUser);

                return newUser;
            }
        }

        public List<Business> SearchBusinessByName(string name, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string searchString = "%" + name + "%";
            var results = context.SearchBusinessByName(searchString);

            List<Business> retrievedBusiness = new List<Business>();

            foreach (var business in results)
            {
                retrievedBusiness.Add(
                    new Business()
                    {
                        id = business.id,
                        name = business.name,
                        address = business.address,
                        zip = (int)business.zip,
                        email = business.email,
                        category = business.category,
                        integration = (int)business.integration
                    });
            }

            return retrievedBusiness;
        }

        public string LinkAccountToBusiness(int bid, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var acountBusinessResult = context.GetAccountBusiness(aid);
            IEnumerator<GetAccountBusinessResult> accountBusinessEnumerator = acountBusinessResult.GetEnumerator();

            if (accountBusinessEnumerator.MoveNext())
            {
                if (accountBusinessEnumerator.Current.business == 1)
                {
                    var requestBusinessResult = context.GetBusiness(bid);
                    IEnumerator<GetBusinessResult> requestBusinessEnumerator = requestBusinessResult.GetEnumerator();

                    if (requestBusinessEnumerator.MoveNext())
                    {
                        context.LinkAccountToBusiness(aid, bid);
                        return string.Format("Account {0} successfully linked to Business {1}", aid, bid);
                    }
                    else
                    {
                        throw new FaultException("Business requested to link does not exist! ");
                    }
                }
                else
                {
                    throw new FaultException("This account is already linked to a business! ");
                }
            }
            else
            {
                throw new FaultException("Account does not exist! ");
            }
        }

        public User Login(LoginUser user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string passwordHash = Encryptor.Generate512Hash(user.pass);

            var results = context.Login(user.login, passwordHash);

            IEnumerator<LoginResult> loggedIn = results.GetEnumerator();
            if (loggedIn.MoveNext())
            {
                /*
                if (string.IsNullOrEmpty(loggedIn.Current.session_id))
                {
                    Random rand = new Random();
                    string sessionValue = string.Format("{0}", rand.Next());
                    string sessionId = Encryptor.GenerateHash(sessionValue);

                    context.UpdateSessionId(loggedIn.Current.id, sessionId);

                    return new User()
                    {
                        id = loggedIn.Current.id, 
                        business = (int)loggedIn.Current.business,
                        tier = (int)loggedIn.Current.tier,
                        sessionId = sessionId
                    };
                }
                else
                {
                    throw new FaultException("The account has already logged in! ");
                }*/
                // Changed logic to allow multiple logins into same account (not Singleton anymore)
                // by CH
                string sessionId = "";
                if (user.device != (int)DeviceType.website || loggedIn.Current.session_id == null)
                {
                    Random rand = new Random();
                    string sessionValue = string.Format("{0}", rand.Next());
                    sessionId = Encryptor.GenerateSimpleHash(sessionValue);

                    context.UpdateSessionId(loggedIn.Current.id, sessionId);
                }
                else
                {
                    sessionId = loggedIn.Current.session_id;
                }

                return new User()
                {
                    id = loggedIn.Current.id,
                    business = (int)loggedIn.Current.business,
                    tier = (int)loggedIn.Current.tier,
                    sessionId = sessionId
                };

            }
            else
            {
                throw new FaultException("Invalid username or password. ");
            }
        }

        public string Logout(int aid, string sessionId)
        {
            try
            {
                SessionManager.checkSession(aid, sessionId);
            }
            catch (FaultException)
            {

                throw new FaultException("The user is not logged in yet! ");
            }

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            context.Logout(aid);

            return string.Format("User {0} logged out. ", aid);
        }
    }
}