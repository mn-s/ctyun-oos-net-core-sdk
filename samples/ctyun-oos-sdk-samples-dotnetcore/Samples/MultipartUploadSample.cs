using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3.Model;

namespace Ctyun.OOS.Samples
{
    public static class MultipartUploadSample
    {
        static readonly string bigFileToUpload = Config.BigFileToUpload;

        static readonly int partSize = 5 * 1024 * 1024;

        public static async Task UploadMultipart(string bucketName)
        {
            try
            {
                var key = "UploadMultipartObject";
                var uploadId = await InitiateMultipartUpload(bucketName, key);

                var partETags = await UploadParts(bucketName, key, bigFileToUpload, uploadId);
                var result = await CompleteUploadPart(bucketName, key, uploadId, partETags);
                Console.WriteLine(@"Upload multipart result :{0},{1} ", result.HttpStatusCode, result.Location);
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

        public static async Task UploadMultipartCopy(string sourceBucket, string targetBucket)
        {
            try
            {
                var sourceKey = "UploadMultipartObject";
                var targetKey = "UploadMultipartTargetObject";
                var uploadId = await InitiateMultipartUpload(targetBucket, targetKey);

                var partETags = await UploadPartCopys(targetBucket, targetKey, sourceBucket, sourceKey, uploadId);
                var result = await CompleteUploadPart(targetBucket, targetKey, uploadId, partETags);
                Console.WriteLine(@"Upload multipart copy result :{0},{1} ", result.HttpStatusCode, result.Location);
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


        /// <summary>
        /// 列出所有执行中的Multipart Upload事件
        /// </summary>
        /// <param name="bucketName">目标bucket名称</param>
        public static async Task ListMultipartUploads(String bucketName)
        {
            var result = await Sample.Client.ListMultipartUploadsAsync(new ListMultipartUploadsRequest
            {
                BucketName = bucketName
            });
            // Console.WriteLine("Bucket name:" + result.BucketName);
            // Console.WriteLine("Key marker:" + result.KeyMarker);
            // Console.WriteLine("Delimiter:" + result.Delimiter);
            // Console.WriteLine("Prefix:" + result.Prefix);
            // Console.WriteLine("UploadIdMarker:" + result.UploadIdMarker);

            foreach (var part in result.MultipartUploads)
            {
                Console.WriteLine("{0},UploadId:{1}", part.Key, part.UploadId);
                await Sample.Client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
                {
                    BucketName = result.BucketName,
                    Key = part.Key,
                    UploadId = part.UploadId
                });
                Console.WriteLine("remove {0},UploadId:{1}", part.Key, part.UploadId);
            }
        }

        private static async Task<string> InitiateMultipartUpload(String bucketName, String key)
        {
            var result = await Sample.Client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
            {
                BucketName = bucketName,
                Key = key
            });
            return result.UploadId;
        }

        private static async Task<List<PartETag>> UploadParts(string bucketName, string key, string fileToUpload, string uploadId)
        {
            var fi = new FileInfo(fileToUpload);
            var fileSize = fi.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            var partETags = new List<PartETag>();
            using (var fs = File.Open(fileToUpload, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    var skipBytes = (long)partSize * i;
                    fs.Seek(skipBytes, 0);
                    var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);

                    var result = await Sample.Client.UploadPartAsync(new UploadPartRequest
                    {
                        BucketName = bucketName,
                        Key = key,
                        UploadId = uploadId,
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = i + 1
                    });

                    partETags.Add(new PartETag
                    {
                        ETag = result.ETag,
                        PartNumber = result.PartNumber
                    });
                    Console.WriteLine("finish {0}/{1},{2}/{3}/{4},uploadId:{5}", partETags.Count, partCount, size, size + skipBytes, fileSize, uploadId);
                }
            }
            Console.WriteLine("finished upload");
            return partETags;
        }

        private static async Task<CompleteMultipartUploadResponse> CompleteUploadPart(string bucketName, string key, string uploadId, List<PartETag> partETags)
        {
            var request = new CompleteMultipartUploadRequest
            {
                BucketName = bucketName,
                Key = key,
                UploadId = uploadId
            };
            foreach (var etag in partETags)
            {
                request.AddPartETags(etag);
            }
            return await Sample.Client.CompleteMultipartUploadAsync(request);
        }


        private static async Task<List<PartETag>> UploadPartCopys(string targetBucket, string targetKey, string sourceBucket, string sourceKey, string uploadId)
        {
            var metadata = await Sample.Client.GetObjectMetadataAsync(sourceBucket, sourceKey);
            var fileSize = metadata.ContentLength;

            var partCount = (int)fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            var partETags = new List<PartETag>();
            for (var i = 0; i < partCount; i++)
            {
                var skipBytes = (long)partSize * i;
                var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                var result = await Sample.Client.CopyPartAsync(new CopyPartRequest
                {
                    SourceBucket = sourceBucket,
                    SourceKey = sourceKey,
                    DestinationBucket = targetBucket,
                    DestinationKey = targetKey,
                    UploadId = uploadId,
                    PartNumber = i + 1,
                    FirstByte = skipBytes,
                    LastByte = skipBytes + size
                });
                partETags.Add(new PartETag
                {
                    PartNumber = result.PartNumber,
                    ETag = result.ETag
                });
                Console.WriteLine("finish copy {0}/{1},{2}/{3}/{4},uploadId:{5}", partETags.Count, partCount, size, size + skipBytes, fileSize, uploadId);
            }
            Console.WriteLine("finished copy");

            return partETags;
        }
    }
}
