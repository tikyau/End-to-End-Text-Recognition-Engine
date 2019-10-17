using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;
using System.Configuration;
using System.Threading;

namespace AzureCustomVisionService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter source image file path: ");
            string sourceImagesFilePath = Console.ReadLine();
            Console.Write("Enter crop image file path: ");
            string outputImageFilePath = Console.ReadLine();
            ManualResetEvent[] manualEvents;
            State stateInfo;

            // Create the crop image directory if not exists
            if (!Directory.Exists(outputImageFilePath))
                Directory.CreateDirectory(outputImageFilePath);
            // Check whether user has specified single file as input
            if (Path.GetExtension(sourceImagesFilePath).Length > 0)
            {
                manualEvents = new ManualResetEvent[1];
                manualEvents[0] = new ManualResetEvent(false);
                stateInfo = new State(sourceImagesFilePath, outputImageFilePath, 0, manualEvents[0]);
                ThreadPool.QueueUserWorkItem(
                          new WaitCallback(MakePredictionRequest), stateInfo);
            }
            else
            {
                // Process all files under directory
                manualEvents = new ManualResetEvent[Directory.GetFiles(sourceImagesFilePath).Length];
                // Run for all images
                int threadCounter = 0;
                foreach (var item in Directory.GetFiles(sourceImagesFilePath))
                {
                    manualEvents[threadCounter] = new ManualResetEvent(false);
                    stateInfo = new State(item, outputImageFilePath, threadCounter, manualEvents[threadCounter++]);
                    ThreadPool.QueueUserWorkItem(
                          new WaitCallback(MakePredictionRequest), stateInfo);
                }
            }
            if (WaitHandle.WaitAll(manualEvents, -1, false))
                Console.WriteLine("\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        public static async void MakePredictionRequest(object stateInfo)
        {
            State state = (State)stateInfo;
            Console.WriteLine("Starting work item {0}.", state.workItemcount.ToString());

            var inputImageFilePath = state.sourceImagesFilePath;
            var outputImageFilePath = state.outputImageFilePath;
            var client = new HttpClient();

            // Request headers - replace this example key with your valid Prediction-Key.
            client.DefaultRequestHeaders.Add("Prediction-Key", ConfigurationManager.AppSettings["Prediction-Key"]);

            // Prediction URL - replace this example URL with your valid Prediction URL.
            string url = ConfigurationManager.AppSettings["Prediction-URL"];

            try
            {
                HttpResponseMessage response;

                // Request body. 
                byte[] byteData = Util.GetImageAsByteArray(inputImageFilePath);
                VisionAPIResponse apiResponse = new VisionAPIResponse();
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(url, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<VisionAPIResponse>(responseString);
                }

                // Find out max probability for barcode
                var maxProbability = apiResponse.predictions.Where(obj => obj.tagName == ConfigurationManager.AppSettings["TagName"]).Max(obj => obj.probability);
                var result = apiResponse.predictions.Where(obj => obj.probability == maxProbability && obj.tagName == ConfigurationManager.AppSettings["TagName"]).Single();

                Rectangle cropRect = new Rectangle();
                Bitmap src = Image.FromFile(inputImageFilePath) as Bitmap;

                int width = src.Width;
                int height = src.Height;

                // Set Crop image coordinates 
                cropRect.X = Convert.ToInt32(result.boundingBox.left * width);
                cropRect.Y = Convert.ToInt32(result.boundingBox.top * height);
                cropRect.Width = Convert.ToInt32(result.boundingBox.width * width);
                cropRect.Height = Convert.ToInt32(result.boundingBox.height * height);

                var newImage = src.Clone(cropRect, src.PixelFormat);
                outputImageFilePath = Path.Combine(outputImageFilePath, Path.GetFileName(inputImageFilePath));

                // Delete the file if it exists
                if (File.Exists(outputImageFilePath))
                    File.Delete(outputImageFilePath);
                newImage.Save(outputImageFilePath);

                // Wait for 1 second to finish saving the file
                Thread.Sleep(1000);
                await RecognizeText.GetTextFromImage(outputImageFilePath);
            }
            finally
            {
                state.manualEvent.Set();
            }
        }


    }
}

