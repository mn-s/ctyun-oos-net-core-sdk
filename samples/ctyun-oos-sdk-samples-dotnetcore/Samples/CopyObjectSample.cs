using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3.Model;

namespace Ctyun.OOS.Samples
{
    public static class CopyObjectSample
    {
        static string fileToUpload = Config.FileToUpload;
        public static async Task CopyObjects(string sourceBucket, string targetBucket)
        {
            var sourceKey = "ResumableUploadObjectSource";
            var targetKey = "ResumableUploadObjectTarget";
            await CopyObject(sourceBucket, sourceKey, targetBucket, targetKey);
        }
        public static async Task CopyObject(string sourceBucket, string sourceKey, string targetBucket, string targetKey)
        {
            try
            {
                await Sample.Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = sourceBucket,
                    Key = sourceKey,
                    FilePath = fileToUpload
                });
                
                await Sample.Client.CopyObjectAsync(new CopyObjectRequest
                {
                    SourceBucket = sourceBucket,
                    SourceKey = sourceKey,
                    DestinationBucket = targetBucket,
                    DestinationKey = targetKey
                });

                Console.WriteLine("Copy object succeeded");
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