using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CoreWCFWebClient1.Extensions
{
    public static class BasicHttpBindingClientFactory
    {

        /// <summary>
        /// Creates a basic auth client with credentials set in header Authorization formatted as 'Basic [base64encoded username:password]'
        /// Makes it easier to perform basic auth in Asp.NET Core for WCF
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static TServiceImplementation WithBasicAuth<TServiceContract, TServiceImplementation>(this TServiceImplementation client, string username, string password)
              where TServiceContract : class
                where TServiceImplementation : ClientBase<TServiceContract>, new()
        {
            string clientUrl = client.Endpoint.Address.Uri.ToString();

            var binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            string basicHeaderValue = "Basic " + Base64Encode($"{username}:{password}");
            var eab = new EndpointAddressBuilder(new EndpointAddress(clientUrl));
            eab.Headers.Add(AddressHeader.CreateAddressHeader("Authorization",  // Header Name
                string.Empty,           // Namespace
                basicHeaderValue));  // Header Value
            var endpointAddress = eab.ToEndpointAddress();

            var clientWithConfiguredBasicAuth = (TServiceImplementation)Activator.CreateInstance(typeof(TServiceImplementation), binding, endpointAddress)!;
            clientWithConfiguredBasicAuth.ClientCredentials.UserName.UserName = username;
            clientWithConfiguredBasicAuth.ClientCredentials.UserName.Password = username;

            return clientWithConfiguredBasicAuth;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

    }
}
