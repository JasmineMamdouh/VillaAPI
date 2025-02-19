using System.Net;

namespace VillaAPI.Models
{
    public class APIResponse
    {
        /*
         since all APIs return one standard response with multiple properties
        so, we made a class for API responses to use it and let things be organized
         */
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true; //set it to be true by default, in case of errors set it to false
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }

    }
}
