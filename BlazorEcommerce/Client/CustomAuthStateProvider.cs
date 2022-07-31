using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorEcommerce.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _http;

        public CustomAuthStateProvider(ILocalStorageService localStorageService, HttpClient http)
        {
            _localStorageService = localStorageService;
            _http = http;
        }

        // get auth token from local storage.
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string authToken = await _localStorageService.GetItemAsStringAsync("authToken");

            var identity = new ClaimsIdentity(); // 토큰 수혜자를 지정하는 속성

            // set a default request header for the authorizaition header
            // need http client 
                         
            _http.DefaultRequestHeaders.Authorization = null;
            // currently in this state, the user is unauthorized.

            // check auth token and parse the claims and then set the authorization header to the new auth token we found in the local storage.

            // first
            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");

                    //header에 token을 담는다
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
                }
                catch(Exception ex)
                {
                    await _localStorageService.RemoveItemAsync("authToken");
                    identity = new ClaimsIdentity();
                }
            }

            // claimsPrincipal : supports multiple claims based identities
            // 의미 : user 정보가 있는 identity object가있고, Claims는 identity안에 있는 여러개의 정보들
            var user = new ClaimsPrincipal(identity);

            // AuthenticationState에 claims 정보들을 담는다.
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;

        }

        // parse the payload of the token
        // jwt token이 base64 형태인듯
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch(base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            // 8비트 부호 없는 정수 배열로 인코딩
            return Convert.FromBase64String(base64);
        }


        // get payload
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            // payload : header나 meta data같은 것들을 제외한 순수하게 보내려는 data
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);

            // object 형태로 만들때 Dictionary key,value 형태로 변환한다.
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            // claims : identity에 들어있는 데이터들
            var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));

            return claims;
        }
        
    }
}
