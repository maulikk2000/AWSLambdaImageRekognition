using System;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.S3;
using Amazon.S3.Model;

namespace ImageOrNotClient
{
    public class ImageDetector
    {
        private readonly string _accessKey;
        private readonly string _secretKey;

        public ImageDetector(string accessKey, string secretKey)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
        }

        private async Task<Boolean> IsImage(string filePath, string fileName)
        {
            await UploadToS3(filePath, fileName);

            var amazonLambdaClient = new AmazonLambdaClient(_accessKey, _secretKey, Amazon.RegionEndpoint.APSouth1);

            InvokeRequest ir = new InvokeRequest
            {
                InvocationType = InvocationType.RequestResponse,
                FunctionName = "ImageOrNot", //name of the function which we created when we deployed the func.
                Payload = "\"" + fileName + "\""
            };

            var result = await amazonLambdaClient.InvokeAsync(ir);

            var strResponse = Encoding.ASCII.GetString(result.Payload.ToArray());

            if(bool.TryParse(strResponse, out bool retval))
            {
                return retval;
            }

            return false;
        }

        private async Task UploadToS3(string filePath, string fileName)
        {
            var client = new AmazonS3Client(_accessKey, _secretKey, Amazon.RegionEndpoint.APSouth1);

            var putRequest = new PutObjectRequest
            {
                BucketName = "somebucket",
                Key = fileName,
                FilePath = filePath,
                ContentType = "text/plain"
            };

            var response = await client.PutObjectAsync(putRequest);
        }
    }
}
