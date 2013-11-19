using SynchWebRole.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;

using SynchWebRole.Models;

namespace SynchWebRole.ServiceManager
{
    public partial class SynchDataService : IAccountManager
    {

        public SynchDataService()
        {
        }

        public HttpResponseMessage CreateAccount(SynchAccount newAccount)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                string passwordHash = Encryptor.Generate512Hash(newAccount.password);
                string sessionId = String.Empty;
                Random rand = new Random();
                string sessionValue = string.Format("{0}", rand.Next());
                sessionId = Encryptor.GenerateSimpleHash(sessionValue);

                int accountId = context.CreateAccount(
                    newAccount.businessId,
                    newAccount.login,
                    passwordHash,
                    newAccount.tier,
                    newAccount.firstName,
                    newAccount.lastName,
                    newAccount.email,
                    newAccount.phoneNumber,
                    sessionId,
                    newAccount.deviceId);

                if (accountId < 0)
                    throw new WebFaultException<string>("login already exists", HttpStatusCode.Conflict);

                newAccount.id = accountId;
                newAccount.sessionId = sessionId;

                synchResponse.data = newAccount;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        public HttpResponseMessage Login(SynchAccount account, int deviceType)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                string passwordHash = Encryptor.Generate512Hash(account.password);

                var results = context.GetAccountByLogin(account.login);

                IEnumerator<GetAccountByLoginResult> resultEnum = results.GetEnumerator();
                if (resultEnum.MoveNext())
                {
                    // check if it is the correct password
                    if (resultEnum.Current.password != passwordHash)
                        throw new WebFaultException<string>("unmatched credential", HttpStatusCode.Unauthorized);

                    string sessionId = String.Empty;
                    if (deviceType != (int)DeviceType.website)
                    {
                        Random rand = new Random();
                        string sessionValue = string.Format("{0}", rand.Next());
                        sessionId = Encryptor.GenerateSimpleHash(sessionValue);

                        context.UpdateAccountSession(resultEnum.Current.id, sessionId);
                    }
                    else
                    {
                        sessionId = resultEnum.Current.sessionId;
                    }

                    account.id = resultEnum.Current.id;
                    account.firstName = resultEnum.Current.firstName;
                    account.lastName = resultEnum.Current.lastName;
                    account.businessId = resultEnum.Current.businessId;
                    account.deviceId = resultEnum.Current.deviceId;
                    account.email = resultEnum.Current.email;
                    account.tier = (int)resultEnum.Current.tier;
                    account.phoneNumber = resultEnum.Current.phoneNumber;

                    synchResponse.data = account;
                    synchResponse.status = HttpStatusCode.OK;

                }
                else
                {
                    // login is wrong
                    throw new WebFaultException<string>("unmatched credential", HttpStatusCode.NotFound);
                }
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        public HttpResponseMessage Logout(SynchAccount account)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();


            try
            {
                SessionManager.checkSession(context, account.id, account.sessionId);
                synchResponse.status = HttpStatusCode.OK;
                
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        /*
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
                        integration = (int)business.integration,
                        tier = (int)business.tier,
                        phoneNumber = business.phone_number
                    });
            }

            return retrievedBusiness;
        }

        public List<User> GetAccounts(int aid, int bid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetAllAccounts(bid);

            List<User> retrievedAccounts = new List<User>();

            foreach (var user in results)
            {
                retrievedAccounts.Add(
                    new User()
                    {
                        id = user.id,
                        business = (int)user.business,
                        email = user.email,
                        firstName = user.firstname,
                        lastName = user.lastname,
                        login = user.login,
                        phoneNumber = user.phone_number,
                        sessionId = user.session_id
                    });
            }

            return retrievedAccounts;
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

        

        // log in for the firs time -- create a temp session ID that is 6 char long
        public User firstLogin(LoginUser user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string passwordHash = Encryptor.Generate512Hash(user.pass);

            var results = context.Login(user.login, passwordHash);

            IEnumerator<LoginResult> loggedIn = results.GetEnumerator();
            if (loggedIn.MoveNext())
            {
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
        */

    }
}