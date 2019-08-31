using Newtonsoft.Json;

namespace Lanina.Public.Web.Api.ThirdPartyClients.Google
{
    public class GoogleReCaptchaClient
    {
        private class ValidateRecapthcaResponse
        {
            public bool Success { get; set; }
        }

        private readonly GoogleReCaptchaSettings _settings;
        public GoogleReCaptchaClient(GoogleReCaptchaSettings settings)
        {
            _settings = settings;

        }

        public bool IsHuman(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;

            var client = new System.Net.WebClient();
            var secret = _settings.SecretKey;

            var responseString = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, token));

            var responseObject = JsonConvert.DeserializeObject<ValidateRecapthcaResponse>(responseString);

            return responseObject.Success;
        }
    }
}
