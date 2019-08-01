using System;
using System.Threading.Tasks;
using Amazon.Runtime;

namespace Ctyun.OOS.Samples
{
    public static class ListBucketsSample
    {
        public static async Task ListBuckets()
        {
            await ListAllBuckets();
        }
        public static async Task ListAllBuckets()
        {
            try
            {
                var buckets = await Sample.Client.ListBucketsAsync();
                Console.WriteLine("List all buckets: ");
                foreach (var bucket in buckets.Buckets)
                {
                    Console.WriteLine("Name:{0},CreateTime:{1}", bucket.BucketName, bucket.CreationDate.ToString("R"));
                }
            }
            catch (AmazonServiceException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}",
                                ex.ErrorCode, ex.Message, ex.RequestId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }
    }
}