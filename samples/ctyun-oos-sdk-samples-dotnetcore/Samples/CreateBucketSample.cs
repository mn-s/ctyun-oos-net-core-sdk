//using System;
//using System.Threading.Tasks;
//using Amazon.Runtime;
//using Amazon.S3;
//using Amazon.S3.Model;

//namespace Ctyun.OOS.Samples
//{
//    public static class CreateBucketSample
//    {
//        public static async Task CreateBucket(string bucketName)
//        {
//            try
//            {
//                var bucketReq = new PutBucketRequest();
//                bucketReq.BucketName = bucketName;
//                bucketReq.BucketRegion = new S3Region("hbwh");
//                bucketReq.CannedACL = new S3CannedACL(S3CannedACL.PublicRead);
//                //await Sample.Client.PutBucketAsync(bucketName);
//                await Sample.Client.PutBucketAsync(bucketReq);
//                Console.WriteLine("Created bucket name:{0} succeeded ", bucketName);
//            }
//            catch (AmazonServiceException ex)
//            {
//                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}",
//                                ex.ErrorCode, ex.Message, ex.RequestId);
//            }
//        }
//    }
//}