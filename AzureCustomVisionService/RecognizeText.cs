using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureCustomVisionService
{
    class RecognizeText
    {
        public static async Task GetTextFromImage(string imagePath)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Ocp-Apim-Subscription-Key"]);

            // API URL
            string url = ConfigurationManager.AppSettings["RecognizeTextAPI-URL"];
            try
            {
                HttpResponseMessage recognizeTextAPIResponse;

                // Request body. 
                byte[] byteData = Util.GetImageAsByteArray(imagePath);
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    recognizeTextAPIResponse = await client.PostAsync(url, content);
                    IEnumerable<string> values;
                    string operation_location_url = "";
                    if (recognizeTextAPIResponse.Headers.TryGetValues("operation-location", out values))
                        operation_location_url = values.First();

                    Console.WriteLine(imagePath + ": " + operation_location_url);
                    RecognitionAPIResponse recognitionAPIResponse = new RecognitionAPIResponse();
                    if (operation_location_url != string.Empty)
                    {
                        while(true)
                        {
                            var operationLocationResponse = await client.GetAsync(operation_location_url);

                            var responseString = await operationLocationResponse.Content.ReadAsStringAsync();
                            recognitionAPIResponse = JsonConvert.DeserializeObject<RecognitionAPIResponse>(responseString);
                            if (recognitionAPIResponse.status == "Succeeded")
                            {
                                Console.WriteLine(imagePath + ": " + recognitionAPIResponse.recognitionResult.lines[0].text);
                                break;
                            }
                            else if (recognitionAPIResponse.status == "Failed")
                            {
                                Console.WriteLine(imagePath + ": " + recognitionAPIResponse);
                                break;
                            }
                        }                       
                        
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: " + ex.Message);
                Console.WriteLine("Error occured: " + ex.StackTrace);
            }
        }
    }
}
