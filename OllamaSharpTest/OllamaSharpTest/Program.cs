using OllamaSharp;
using Whisper.net;
using NAudio.Wave;
using System.Speech.Synthesis;
using Whisper.net.Logger;
using ChromaDB.Client;
using System.Diagnostics;

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

            string prompt = string.Empty;

            // Delete the audio file
            try
            {
                using (var fileStream = File.OpenRead(wavFileName))
                {
                    // This section processes the audio file and prints the results (start time, end time and text) to the console.
                    await foreach (var result in processor.ProcessAsync(fileStream))
                    {
                        Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
                        if (!result.Text.Contains("[BLANK_AUDIO]"))
                        {
                            prompt += result.Text;
                        }
                    }
                }

                File.Delete(wavFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return prompt;
        }

        static string InputPrompt()
        {
            string prompt = Console.ReadLine() ?? string.Empty;
            return prompt;
        }

        static async Task<string> Chat(string prompt, string context = "")
        {
            string generated = string.Empty;

            Console.WriteLine("");
            Console.Write("AI: ");

            string request = 
                $"This is a context information: {context}\r\n" +
                $"This is a question. Please answer it based on context information: {prompt}";

            // Start generation
            await foreach (var answerToken in new Chat(OllamaClient).SendAsync(request))
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
            List<float[]>? embeddings = null;

            try
            {
                OllamaClient.SelectedModel = "mxbai-embed-large";
                var result = await OllamaClient.EmbedAsync(prompt);
                embeddings = result.Embeddings;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                OllamaClient.SelectedModel = "phi3";

                if (embeddings == null)
                {
                    embeddings = new List<float[]>();
                }
            }

            return embeddings;
        }

        static async Task<string> Query(float[] queryEmbedding)
        {
            const string processName = "chroma";
            const string collectionName = "docs";

            string context = string.Empty;

            bool isRunning = Process.GetProcessesByName(processName).Length != 0;
            if (!isRunning)
            {
                Console.WriteLine("ChromaDB is not running.");
                return context;
            }

            // Connect to ChromaDB
            var configOptions = new ChromaConfigurationOptions(uri: "http://localhost:8000/api/v1/");
            using var httpClient = new HttpClient();
            var client = new ChromaClient(configOptions, httpClient);

            // Create or Get a collection
            var collection = await client.GetOrCreateCollection(collectionName);
            var collectionClient = new ChromaCollectionClient(collection, configOptions, httpClient);

            // Query the database
            var queryData = await collectionClient.Query([new(queryEmbedding)]);

            foreach (var item in queryData)
            {
                foreach (var entry in item)
                {
                    context += $"{entry.Document}\r\n";
                }
            }

            return context;
        }

        static async Task Main()
        {
            // Initialize Ollama
            var uri = new Uri("http://localhost:11434");
            OllamaClient = new OllamaApiClient(uri) { SelectedModel = "phi3" };

            // Pulling a model and reporting progress
            // If the model doesn't exist, download it.
            await foreach (var status in OllamaClient.PullModelAsync("phi3"))
            {
                Console.WriteLine($"{status.Percent}% {status.Status}");
            }

            Console.WriteLine();

            string prompt = string.Empty;
            string context = string.Empty;

#if true
            while (true)
            {
                if (true)
                {
                    // Record voice
                    prompt = await Record();
                }

                if (prompt == string.Empty)
                {
                    // Enter the prompt
                    Console.Write($"You: {prompt}");
                    prompt = InputPrompt();
                }
                else
                {
                    Console.Write($"You: {prompt}");
                }

                // Convert prompt to numeric vector
                List<float[]> queryEmbeddings = await Embed(prompt);

                if (queryEmbeddings.Count != 0)
                {
                    // Searching the database and extracting context
                    foreach (var queryEmbedding in queryEmbeddings)
                    {
                        context += await Query(queryEmbedding);
                    }
                }

                // Chat with AI
                string script = await Chat(prompt, context);

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