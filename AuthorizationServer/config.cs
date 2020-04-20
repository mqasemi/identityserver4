using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace AuthorizationServer
{
    public class config
    {
        public static IEnumerable<ApiResource> getApiResource(){
            return new List<ApiResource>{
                new ApiResource("BankApiDotnet", "Bank Acounting API")
            };
        }
        public static IEnumerable<Client> GetClients(){
            return new List<Client> {
                new Client {
    ClientId = "angular_spa",
    ClientName = "Angular 4 Client",
    AllowedGrantTypes = GrantTypes.Implicit,
    AllowedScopes = new List<string> { "openid", "profile", "BankApiDotnet" },
    RedirectUris = new List<string> { "http://localhost:4200/auth-callback" },
    PostLogoutRedirectUris = new List<string> { "http://localhost:4200/" },
    AllowedCorsOrigins = new List<string> { "http://localhost:4200" },
    AllowAccessTokensViaBrowser = true,
    SlidingRefreshTokenLifetime=10
},
                new Client{ ClientId="bank",
                ClientSecrets={new Secret("secret".Sha256())},
                AllowedGrantTypes=GrantTypes.ClientCredentials,
                AllowedScopes =
                    {
                        "BankApiDotnet"
                    }
                },
                 new Client{ ClientId="AuthCodeBank",
                ClientSecrets={new Secret("secret".Sha256())},
                AllowedGrantTypes=GrantTypes.Code,
                RedirectUris = { "http://localhost:5002/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "BankApiDotnet",
                        
                    }
                }
                ,
                 new Client{ ClientId="AuthCodeBankHybrid",
                ClientSecrets={new Secret("secret".Sha256())},
                AllowedGrantTypes=GrantTypes.Code,
                RedirectUris = { "http://localhost:5002/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "BankApiDotnet",
                        
                    },
                    AllowOfflineAccess=true
                }
                
            };
        }
        public static List<IdentityResource> GetIdentityResources()
{
    return new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile() // <-- usefull
    };
}
    }
}