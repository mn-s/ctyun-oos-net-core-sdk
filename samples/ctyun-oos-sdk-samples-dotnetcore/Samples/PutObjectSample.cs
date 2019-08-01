using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model;

namespace Ctyun.OOS.Samples
{
    public static class PutObjectSample
    {
        static string fileToUpload = Config.FileToUpload;

        public static async Task PutObject(string bucketName)
        {
            await PutObjectFromFile(bucketName);

            await PutObjectFromString(bucketName);

            await PutObjectWithDir(bucketName, 1);
            await PutObjectWithDir(bucketName, 2);
            await PutObjectWithDir(bucketName, 3);
            
            await PutObjectWithMd5(bucketName);
        }

        public static async Task PutObjectFromFile(string bucketName)
        {
            const string key = "PutObjectFromFile";
            try
            {
                var req = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    FilePath = fileToUpload
                };
                await Sample.Client.PutObjectAsync(req);
                Console.WriteLine("Put object:{0} succeeded", key);
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

        public static async Task PutObjectFromString(string bucketName)
        {
            const string key = "PutObjectFromString";
            const string str = "Ctyun OOS SDK for C#";

            try
            {
                byte[] binaryData = Encoding.ASCII.GetBytes(str);
                var stream = new MemoryStream(binaryData);

                var req = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    InputStream = stream
                };
                await Sample.Client.PutObjectAsync(req);
                Console.WriteLine("Put object:{0} succeeded", key);
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

        public static async Task PutObjectWithDir(string bucketName, int numIndex)
        {
            string key = "folder/sub_folder/PutObjectFromFile" + numIndex;

            try
            {
                var req = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    FilePath = fileToUpload
                };
                await Sample.Client.PutObjectAsync(req);
                Console.WriteLine("Put object:{0} succeeded", key);
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

        public static async Task PutObjectWithMd5(string bucketName)
        {
            const string key = "PutObjectWithMd5";

            try
            {
                string md5;
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    using (var md5Calculator = MD5.Create())
                    {
                        long position = fs.Position;
                        var partialStream = new PartialWrapperStream(fs, fs.Length);
                        var md5Value = md5Calculator.ComputeHash(partialStream);
                        fs.Seek(position, SeekOrigin.Begin);
                        md5 = Convert.ToBase64String(md5Value);
                    }
                }

                var req = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    FilePath = fileToUpload,
                    MD5Digest = md5
                };
                await Sample.Client.PutObjectAsync(req);
                Console.WriteLine("Put object:{0} succeeded", key);
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