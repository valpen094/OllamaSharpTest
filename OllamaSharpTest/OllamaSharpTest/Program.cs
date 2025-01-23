﻿using OllamaSharp;
using Whisper.net;
using NAudio.Wave;
using System.Speech.Synthesis;
using Whisper.net.Logger;

namespace OllamaSharpTest
{
    internal class Program
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