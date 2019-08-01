using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3.Model;

namespace Ctyun.OOS.Samples
{
    public static class ListObjectsSample
    {
        public static async Task ListObjects(string bucketName)
        {
            await SimpleListObjects(bucketName);

            await SimpleListObjectsWithPrefix(bucketName);

            await ListObjectsWithRequest(bucketName);
        }

        public static async Task SimpleListObjects(string bucketName)
        {
            try
            {
                var result = await Sample.Client.ListObjectsAsync(bucketName);

                Console.WriteLine("List objects of bucket:{0} succeeded ", bucketName);
                foreach (var summary in result.S3Objects)
                {
                    Console.WriteLine(summary.Key);
                }

                Console.WriteLine("List objects of bucket:{0} succeeded, is list all objects ? {1}", bucketName, !result.IsTruncated);
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

        public static async Task SimpleListObjectsWithPrefix(string bucketName)
        {
            try
            {
                var listObjectsRequest = new ListObjectsRequest()
                {
                    BucketName = bucketName,
                    Prefix = "folder/sub_folder"
                };
                var result = await Sample.Client.ListObjectsAsync(listObjectsRequest);

                Console.WriteLine("List objects of bucket:{0}/folder/sub_folder succeeded ", bucketName);
                foreach (var summary in result.S3Objects)
                {
                    Console.WriteLine(summary.Key);
                }

                Console.WriteLine("List objects of bucket:{0} succeeded, is list all objects ? {1}", bucketName, !result.IsTruncated);
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

        public static async Task ListObjectsWithRequest(string bucketName)
        {
            try
            {
                var keys = new List<string>();
                ListObjectsResponse result = null;
                string nextMarker = string.Empty;
                do
                {
                    var listObjectsRequest = new ListObjectsRequest()
                    {
                        BucketName = bucketName,
                        Marker = nextMarker,
                        MaxKeys = 2
                    };
                    result = await Sample.Client.ListObjectsAsync(listObjectsRequest);

                    foreach (var summary in result.S3Objects)
                    {
                        Console.WriteLine(summary.Key);
                        keys.Add(summary.Key);
                    }

                    nextMarker = result.NextMarker;
                } while (result.IsTruncated);

                Console.WriteLine("List objects of bucket:{0} succeeded ", bucketName);
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