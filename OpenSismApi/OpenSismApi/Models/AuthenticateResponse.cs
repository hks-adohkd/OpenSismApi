using System;
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


    public class TimeResponse
    {

        public DateTime DateTimeNow { get; set; }

       

        public TimeResponse(DateTime TimeNow)
        {

            DateTimeNow = TimeNow;
            
        }
    }

}