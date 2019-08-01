using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3.Model;

namespace Ctyun.OOS.Samples
{
    public static class GetObjectSample
    {
        static string key = "GetObjectSample";
        static string bigKey = "GetBigObjectSample";
        static string fileToUpload = Config.FileToUpload;
        static string dirToDownload = Config.DirToDownload;
        static string bigFileToUpload = Config.BigFileToUpload;

        public static async Task GetObjects(string bucketName)
        {
            await GetObject(bucketName);

            await GetObjectByRequest(bucketName);

            await GetObjectPartly(bucketName);
        }

        public static async Task GetObject(string bucketName)
        {
            try
            {
                await Sample.Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucketName,
                    FilePath = fileToUpload,
                    Key = key
                });

                var result = await Sample.Client.GetObjectAsync(bucketName, key);

                using (var requestStream = result.ResponseStream)
                {
                    if (!Directory.Exists(dirToDownload))
                    {
                        Directory.CreateDirectory(dirToDownload);
                    }
                    using (var fs = File.Open(dirToDownload + "/sample.data", FileMode.OpenOrCreate))
                    {
                        int length = 4 * 1024;
                        var buf = new byte[length];
                        do
                        {
                            length = requestStream.Read(buf, 0, length);
                            fs.Write(buf, 0, length);
                        } while (length != 0);
                    }
                }

                Console.WriteLine("Get object succeeded");
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

        public static async Task GetObjectByRequest(string bucketName)
        {
            try
            {
                await Sample.Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucketName,
                    FilePath = fileToUpload,
                    Key = key
                });


                var request = new GetObjectRequest();
                request.BucketName = bucketName;
                request.Key = key;
                request.ByteRange = new ByteRange(0, 100);

                var result = await Sample.Client.GetObjectAsync(request);

                Console.WriteLine("Get object succeeded, length:{0}", result.ContentLength);
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

        public static async Task GetObjectPartly(string bucketName)
        {
            await Sample.Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucketName,
                FilePath = bigFileToUpload,
                Key = bigKey
            });

            string localFilePath = dirToDownload + "/big.sample.data";
            if (!Directory.Exists(dirToDownload))
            {
                Directory.CreateDirectory(dirToDownload);
            }
            using (var fileStream = new FileStream(localFilePath, FileMode.OpenOrCreate))
            {
                var bufferedStream = new BufferedStream(fileStream);
                var objectMetadata = await Sample.Client.GetObjectMetadataAsync(bucketName, bigKey);
                var fileLength = objectMetadata.ContentLength;
                Console.WriteLine("Get big object length:{0}", fileLength);
                const int partSize = 1024 * 1024 * 10;

                var partCount = CalPartCount(fileLength, partSize);

                Console.WriteLine("Get big object parts:{0}", partCount);
                for (var i = 0; i < partCount; i++)
                {
                    var startPos = partSize * i;
                    var endPos = partSize * i + (partSize < (fileLength - startPos) ? partSize : (fileLength - startPos)) - 1;
                    await Download(bufferedStream, startPos, endPos, localFilePath, bucketName, key);
                    Console.WriteLine("download big object parts:{0}", i);
                }
                bufferedStream.Flush();
            }
            Console.WriteLine("Get big object succeeded");
        }
        /// <summary>
        /// 计算下载的块数
        /// </summary>
        /// <param name="fileLength"></param>
        /// <param name="partSize"></param>
        /// <returns></returns>
        private static int CalPartCount(long fileLength, long partSize)
        {
            var partCount = (int)(fileLength / partSize);
            if (fileLength % partSize != 0)
            {
                partCount++;
            }
            return partCount;
        }
        /// <summary>
        /// 分块下载
        /// </summary>
        /// <param name="bufferedStream"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="localFilePath"></param>
        /// <param name="bucketName"></param>
        /// <param name="fileKey"></param>
        private static async Task Download(BufferedStream bufferedStream, long startPos, long endPos, String localFilePath, String bucketName, String fileKey)
        {
            Stream contentStream = null;
            try
            {
                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileKey,
                    ByteRange = new ByteRange(startPos, endPos)
                };
                var ossObject = await Sample.Client.GetObjectAsync(getObjectRequest);
                byte[] buffer = new byte[1024 * 1024];
                var bytesRead = 0;
                bufferedStream.Seek(startPos, SeekOrigin.Begin);
                contentStream = ossObject.ResponseStream;
                while ((bytesRead = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bufferedStream.Write(buffer, 0, bytesRead);
                }
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Dispose();
                }
            }
        }
    }
}