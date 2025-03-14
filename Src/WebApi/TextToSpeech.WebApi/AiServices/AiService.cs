using System.Text.Json;
using Betalgo.Ranul.OpenAI;
using Betalgo.Ranul.OpenAI.Managers;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using RestSharp;

namespace TextToSpeech.WebApi.AiServices;

public class AiService(IConfiguration configuration)
{
    public async Task<MemoryStream> TextToSpeech(string text)
    {
        #region Configuration
        string baseUrl = "https://api.avalai.ir/v1";
        string apiKey = configuration.GetSection("AvalAi").GetSection("ApiKey").Value;
        #endregion


        var openAiService = new OpenAIService(new OpenAIOptions()
        {
            ApiKey = apiKey, BaseDomain = baseUrl
        });


        var audioResult = (await openAiService.Audio.CreateSpeech<byte[]>
        (new AudioCreateSpeechRequest()
        {
            Model = "tts-1-hd", Voice = "echo", Input = text
        }));

        string outputFilePath = $"output_{DateTime.Now.Ticks}.mp3";

        try
        {
            await File.WriteAllBytesAsync(outputFilePath, audioResult.Data);
            MemoryStream memoryStream = new MemoryStream(audioResult.Data);
            return memoryStream;

        }
        catch (Exception e)
        {
            Console.WriteLine("خطایی پیش آمده");
            throw;
        }


    }

    public async Task<RootObject> TextToSpeechAvanegar(string text)
    {
        try
        {


            var url = "https://partai.gw.isahab.ir/TextToSpeech/v1/speech-synthesys";
            var client = new RestClient(url);
            var request = new RestRequest(new Uri(url), Method.Post);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("gateway-token", configuration.GetSection("Avanegar").GetSection("ApiKey").Value.ToString());


            var body = @"{" + "\n" +
                       @"	""data"": ""(text)""," + "\n" +
                       @"    ""filePath"":""true""," + "\n" +
                       @"    ""base64"":""0""," + "\n" +
                       @"    ""checksum"":""1"" ," + "\n" +
                       @"    ""speaker"":""1""" + "\n" +
                       @"}";

            body = body.Replace("(text)", text);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);

            RootObject newObject = JsonSerializer.Deserialize<RootObject>(response.Content);

            return newObject;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

    }
}

public class RootObject
{
    public Data data { get; set; }
    public Meta meta { get; set; }
}

public class Data
{
    public string status { get; set; }
    public Data1 data { get; set; }
    public object error { get; set; }
}

public class Data1
{
    public string filePath { get; set; }
    public string checksum { get; set; }
}

public class Meta
{
    public string shamsiDate { get; set; }
    public string requestId { get; set; }
}