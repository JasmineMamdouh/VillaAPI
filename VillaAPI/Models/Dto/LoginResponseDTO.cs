namespace VillaAPI.Models.Dto
{
    /*
     if login is successful we need to respond with user details and token that validates his identity
     thus, approves he is an authenticated user
     */
    public class LoginResponseDTO
    {
        public LocalUser User { get; set; }
        public string Token { get; set; }

    }
}
