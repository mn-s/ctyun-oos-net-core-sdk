using System;
using System.Threading.Tasks;
using Amazon.Runtime;

namespace Ctyun.OOS.Samples
{
    public class Program
    {
        public static async Task Main(string[] args)
		{
			Console.WriteLine("Ctyun SDK for .NET Samples!");

            const string bucketName = "<your bucket name>";

            try
            {
                await ListBucketsSample.ListBuckets();

                await PutObjectSample.PutObject(bucketName);

                await ListObjectsSample.ListObjects(bucketName);

                await GetObjectSample.GetObjects(bucketName);

                await DeleteObjectsSample.DeleteObjects(bucketName);

                await CopyObjectSample.CopyObjects(bucketName, bucketName);

                await MultipartUploadSample.UploadMultipart(bucketName);

                await MultipartUploadSample.UploadMultipartCopy(bucketName, bucketName);

                await MultipartUploadSample.ListMultipartUploads(bucketName);

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

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}
