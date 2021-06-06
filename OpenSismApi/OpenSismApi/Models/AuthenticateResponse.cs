using System.Text.Json.Serialization;


namespace OpenSismApi.Models
{
    public class AuthenticateResponse
    {
        
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(  string jwtToken, string refreshToken)
        {
           
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}