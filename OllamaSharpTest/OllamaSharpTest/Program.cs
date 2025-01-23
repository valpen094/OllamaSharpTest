using OllamaSharp;
using Whisper.net;
using NAudio.Wave;
using System.Speech.Synthesis;
using Whisper.net.Logger;
using ChromaDB.Client;

namespace OllamaSharpTest
{
    class Program
    {
        private static OllamaApiClient OllamaClient;

        static async Task<string> Record()
        {
            string modelFileName = $"{Directory.GetCurrentDirectory()}\\ggml-base.bin";
            const string wavFileName = "voice.wav";

            // Optional logging from the native library
            using var whisperLogger = LogProvider.AddConsoleLogging(WhisperLogLevel.None);

            // This section creates the whisperFactory object which is used to create the processor object.
            using var whisperFactory = WhisperFactory.FromPath("ggml-base.bin");

            // This section creates the processor object which is used to process the audio file, it uses language `auto` to detect the language of the audio file.
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("auto")
                .Build();

            using (var waveIn = new WaveInEvent())
            {
                waveIn.WaveFormat = new WaveFormat(16000, 1);

                using (var waveWriter = new WaveFileWriter(wavFileName, waveIn.WaveFormat))
                {
                    waveIn.DataAvailable += (sender, e) =>
                    {
                        waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                    };

                    // Start recording
                    waveIn.StartRecording();

                    Console.WriteLine("Please speak into the microphone.\r\n* recording... [Enter]");
                    Console.ReadLine();

                    // Stop recording
                    waveIn.StopRecording();
                }
            }

            var fileStream = File.OpenRead(wavFileName);

            string prompt = string.Empty;

            // This section processes the audio file and prints the results (start time, end time and text) to the console.
            await foreach (var result in processor.ProcessAsync(fileStream))
            {
                Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
                prompt += result.Text;
            }

            fileStream.Dispose();
            fileStream.Close();

            // Delete the audio file
            try
            {
                File.Delete(wavFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return prompt;
        }

        static async Task<string> Chat(string prompt)
        {
            string generated = string.Empty;

            Console.Write("You: ");

            if (prompt == string.Empty)
            {
                // Input prompt
                prompt = Console.ReadLine() ?? string.Empty;
            }
            else
            {
                // Output voice recognition data
                Console.WriteLine(prompt);
            }

            Console.Write("AI: ");

            // Start generation
            await foreach (var answerToken in new Chat(OllamaClient).SendAsync(prompt))
            {
                generated += answerToken;
                Console.Write(answerToken);
            }

            return generated;
        }

        static void Ondoku(string script)
        {
            // Start reading aloud
            using var synthesizer = new SpeechSynthesizer() { Volume = 100, Rate = 0 };
            synthesizer.Speak(script);

            // For line breaks
            Console.WriteLine();
        }

        static async Task<List<float[]>> Embed(string prompt)
        {
            var result = await OllamaClient.EmbedAsync(prompt);
            List<float[]> vectors = result.Embeddings;

            // Commented out due to large number of outputs

            /*
            // Display numeric vector
            Console.WriteLine("Embedding Vector:");
            foreach (var embedding in result.Embeddings)
            {
                foreach(var value in embedding)
                {

                    Console.Write($"{value:F6} ");
                }
            }

            Console.WriteLine();
            */

            return vectors;
        }

        static async Task hoge()
        {
            var configOptions = new ChromaConfigurationOptions(uri: "http://localhost:8000/api/v1/");
            using var httpClient = new HttpClient();
            var client = new ChromaClient(configOptions, httpClient);

            var string5Collection = client.GetOrCreateCollection("string5");
            var string5Client = new ChromaCollectionClient(await string5Collection, configOptions, httpClient);

            string5Client.Add(["340a36ad-c38a-406c-be38-250174aee5a4"], embeddings: [new([1f, 0.5f, 0f, -0.5f, -1f])]);

            var getResult = string5Client.Get("340a36ad-c38a-406c-be38-250174aee5a4", include: ChromaGetInclude.Metadatas | ChromaGetInclude.Documents | ChromaGetInclude.Embeddings);
            Console.WriteLine($"ID: {getResult!.Id}");
        }

        static async Task Main()
        {
            // Initialize Ollama
            var uri = new Uri("http://localhost:11434");
            OllamaClient = new OllamaApiClient(uri);

            // select a model which should be used for further operations
            OllamaClient.SelectedModel = "phi3";

            // Listing all models that are available locally
            var models = await OllamaClient.ListLocalModelsAsync();
            Console.WriteLine($"Connecting to {uri} ...");

            // Pulling a model and reporting progress
            // Send/receive log
            await foreach (var status in OllamaClient.PullModelAsync("phi3"))
            {
                Console.WriteLine($"{status.Percent}% {status.Status}");
            }

            Console.WriteLine();

            string prompt = string.Empty;

            // Chromadb.Client サンプルコード動作確認用（削除予定）
            await hoge();

#if true
            while (true)
            {
                if (true)
                {
                    // Record voice
                    prompt = await Record();
                }

                // Chat with AI
                string script = await Chat(prompt);

                // Convert prompt to numeric vector
                await Embed(prompt);

                if (true)
                {
                    // Reading aloud
                    Ondoku(script);
                }
            }
#endif
        }
    }
}