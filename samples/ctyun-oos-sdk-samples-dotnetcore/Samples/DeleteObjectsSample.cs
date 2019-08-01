using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3.Model;

namespace Ctyun.OOS.Samples
{
    public static class DeleteObjectsSample
    {
        public static async Task DeleteObjects(string bucketName)
        {
            await DeleteObject(bucketName);

            await BatchDeleteObjects(bucketName);
        }

        public static async Task DeleteObject(string bucketName)
        {
            try
            {
                var result = await Sample.Client.ListObjectsAsync(bucketName);
                var model = result.S3Objects.FirstOrDefault();
                if (model != null)
                {
                    await Sample.Client.DeleteObjectAsync(bucketName, model.Key);
                    Console.WriteLine("Delete object:{0}", model.Key);
                }
                Console.WriteLine("Delete object succeeded");

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

        public static async Task BatchDeleteObjects(string bucketName)
        {
            try
            {
                var listObjectsRequest = new ListObjectsRequest()
                {
                    BucketName = bucketName,
                    Prefix = "folder/sub_folder"
                };
                var result = await Sample.Client.ListObjectsAsync(listObjectsRequest);

                var response = await Sample.Client.DeleteObjectsAsync(new DeleteObjectsRequest
                {
                    BucketName = bucketName,
                    Objects = result.S3Objects
                                .Select(o => new KeyVersion
                                {
                                    Key = o.Key
                                }).ToList(),
                    Quiet = false
                });
                response.DeletedObjects.ForEach(o =>
                {
                    Console.WriteLine("Batch Delete object:{0}", o.Key);
                });

                Console.WriteLine("Batch Delete objects succeeded");
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