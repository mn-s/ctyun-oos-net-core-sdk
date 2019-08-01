using Amazon.S3;

namespace Ctyun.OOS
{
    public class OosConfig : AmazonS3Config
    {
        public string Endpoint
        {
            get
            {
                return ServiceURL;
            }
            set
            {
                ServiceURL = value;
            }
        }

        public new string SignatureVersion
        {
            get
            {
                return SignatureVersion;
            }
            // 禁止修改
            // set { }
        }

        public OosConfig(string endpoint)
        {
            Endpoint = endpoint;
            base.SignatureVersion = "2";
        }
    }
}