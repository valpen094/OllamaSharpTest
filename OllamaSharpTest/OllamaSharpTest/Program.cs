using OllamaSharp;
using Whisper.net;
using NAudio.Wave;
using System.Speech.Synthesis;

namespace OllamaSharpTest
{
    class Program
    {
        static async Task Main()
        {
            // set up the client
            var uri = new Uri("http://localhost:11434");
            var ollama = new OllamaApiClient(uri);

            // select a model which should be used for further operations
            ollama.SelectedModel = "phi3";

            // Listing all models that are available locally
            var models = await ollama.ListLocalModelsAsync();
            Console.WriteLine($"Connecting to {uri} ...");

            // Pulling a model and reporting progress
            // Send/receive log
            await foreach (var status in ollama.PullModelAsync("phi3"))
            {
                Console.WriteLine($"{status.Percent}% {status.Status}");
            }

            // Generating a completion directly into the console
            // Details of generated text. Commented out because the log is long.
            /* 
            await foreach (var stream in ollama.GenerateAsync("How are you today?"))
            {
                Console.Write(stream.Response);
            }
            */

            var chat = new Chat(ollama);
            while (true)
            {
                string generated = string.Empty;

                Console.Write("You: ");
                var prompt = Console.ReadLine();

                Console.Write("AI: ");

                // Start generation
                await foreach (var answerToken in chat.SendAsync(prompt))
                {
                    generated += answerToken;
                    Console.Write(answerToken);
                }

                using var synthesizer = new SpeechSynthesizer() { Volume = 100, Rate = 0 };

                // Start reading aloud
                synthesizer.Speak(generated);

                // For line breaks
                Console.WriteLine();
            }
        }
    }
}