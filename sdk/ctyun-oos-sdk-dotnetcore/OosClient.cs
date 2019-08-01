using Amazon.Runtime;
using Amazon.S3;

namespace Ctyun.OOS
{
    public class OosClient : AmazonS3Client
    {
        public OosClient(string accessKeyId, string accessKeySecret, string endpoint)
            : this(accessKeyId, accessKeySecret, FormatEndpoint(endpoint))
        { }

        protected OosClient(string accessKeyId, string accessKeySecret, OosConfig config)
        : this(new BasicAWSCredentials(accessKeyId, accessKeySecret), config)
        {

        }

        protected OosClient(AWSCredentials credentials, OosConfig config)
        : base(credentials, config)
        { }

        private static OosConfig FormatEndpoint(string endpoint)
        {
            string canonicalizedEndpoint = endpoint.Trim().ToLower();

            if (canonicalizedEndpoint.StartsWith("http://") ||
                canonicalizedEndpoint.StartsWith("https://"))
            {
                endpoint = endpoint.Trim();
            }
            else
            {
                endpoint = "http://" + endpoint.Trim();
            }
            return new OosConfig(endpoint);
        }
    }
}
