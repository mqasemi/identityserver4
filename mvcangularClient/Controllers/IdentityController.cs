using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;
using static IdentityModel.OidcConstants;
using mvcangularClient.model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace mvcangularClient.Controllers
{

    public class IdentityController : Controller
    {

        private async Task<HttpResponseMessage> CallApiUsingUserAccessToken()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            return await client.GetAsync("http://localhost:5001/identity");
        }
        private async Task<HttpResponseMessage> GetUserClaimsFromApiWithClientCredentials()
        {
            var client = new HttpClient();

            var oidcDiscoveryResult = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (oidcDiscoveryResult.IsError) throw new Exception(oidcDiscoveryResult.Error);

            var tokenResponse = await client.RequestTokenAsync(new IdentityModel.Client.TokenRequest
            {
                Address = oidcDiscoveryResult.TokenEndpoint,
                GrantType = GrantTypes.ClientCredentials,

                ClientId = "bank",
                ClientSecret = "secret",

                Parameters =
    {
        { "scope", "BankApiDotnet" }
    }
            });
            // request token

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                throw new HttpRequestException(tokenResponse.Error);
            }
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");
            // call api

            client.SetBearerToken(tokenResponse.AccessToken);
            var response = await client.GetAsync("http://localhost:5001/identity");
            return response;
        }

        [Authorize]
        public async Task<ViewResult> Index()
        {
            var userClaimsVM = new UserClaimsVM();
             var userClaimsWithClientCredentials = await GetUserClaimsFromApiWithClientCredentials();
            userClaimsVM.UserClaimsWithClientCredentials =
            userClaimsWithClientCredentials.IsSuccessStatusCode ? await
            userClaimsWithClientCredentials.Content.ReadAsStringAsync() :
            userClaimsWithClientCredentials.StatusCode.ToString();

            var userClaimsWithAccessToken = await CallApiUsingUserAccessToken();
            userClaimsVM.UserClaimsWithAccessToken = userClaimsWithAccessToken.IsSuccessStatusCode ? await userClaimsWithAccessToken.Content.ReadAsStringAsync() : userClaimsWithAccessToken.StatusCode.ToString();

            return View(userClaimsVM);
        }
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }


    }

}