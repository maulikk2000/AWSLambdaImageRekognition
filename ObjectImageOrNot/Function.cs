using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ObjectImageOrNot
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes an image name as an input and returns whether it's abc or not?
        /// </summary>
        /// <param name="input">name of the image we want to detect</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<bool> FunctionHandler(string input, ILambdaContext context)
        {
            var rekognitionClient = new AmazonRekognitionClient();

            var response = await rekognitionClient.DetectLabelsAsync(
                new DetectLabelsRequest
                {
                    Image = new Image
                    {
                        S3Object = new S3Object
                        {
                            Bucket = "somebucket",
                            Name = input //the name of the image we are detecting
                        }
                    }
                });

            //iterate through all the labels for the real-world object detected
            foreach (var label in response.Labels)
            {
                if(label.Confidence > 50) //here if confidence is more than 50, we make the call that it's the image we are looking for
                {
                    if(label.Name == "Fried Chicken" || label.Name == "Nuggets")
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}
