namespace Ctyun.OOS.Samples
{
    public static class Sample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        public static OosClient Client = new OosClient(accessKeyId, accessKeySecret, endpoint);
    }
}