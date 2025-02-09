﻿using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using VGMToolbox.format;

namespace UsmToolkit
{
    [Command(Description = "Extract audio and video")]
    public class ExtractCommand
    {
        [Required]
        [FileOrDirectoryExists]
        [Argument(0, Description = "File or folder containing usm files")]
        public string InputPath { get; set; }

        protected int OnExecute(CommandLineApplication app)
        {
            FileAttributes attr = File.GetAttributes(InputPath);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                foreach (var file in Directory.GetFiles(InputPath, "*.usm"))
                    Process(file);
            }
            else
                Process(InputPath);

            return 0;
        }

        private void Process(string fileName)
        {
            Console.WriteLine($"File: {fileName}");
            var usmStream = new CriUsmStream(fileName);

            Console.WriteLine("Demuxing...");
            usmStream.DemultiplexStreams(new MpegStream.DemuxOptionsStruct()
            {
                AddHeader = false,
                AddPlaybackHacks = false,
                ExtractAudio = true,
                ExtractVideo = true,
                SplitAudioStreams = false
            });
        }
    }
    
    [Command(Description = "Convert according to the parameters in config.json")]
    public class ConvertCommand
    {
        [Required]
        [FileOrDirectoryExists]
        [Argument(0, Description = "File or folder containing usm files")]
        public string InputPath { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "Specify output directory", ShortName = "o", LongName = "output-dir")]
        public string OutputDir { get; set; }

        [Option(CommandOptionType.NoValue, Description = "Remove temporary m2v and audio after converting", ShortName = "c", LongName = "clean")]
        public bool CleanTempFiles { get; set; }

        protected int OnExecute(CommandLineApplication app)
        {
            FileAttributes attr = File.GetAttributes(InputPath);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                foreach (var file in Directory.GetFiles(InputPath, "*.usm"))
                    Process(file);
            }
            else
                Process(InputPath);

            return 0;
        }

        private void Process(string fileName)
        {
            Console.WriteLine($"File: {fileName}");
            var usmStream = new CriUsmStream(fileName);

            Console.WriteLine("Demuxing...");
            usmStream.DemultiplexStreams(new MpegStream.DemuxOptionsStruct()
            {
                AddHeader = false,
                AddPlaybackHacks = false,
                ExtractAudio = true,
                ExtractVideo = true,
                SplitAudioStreams = false
            });

            if (!string.IsNullOrEmpty(OutputDir) && !Directory.Exists(OutputDir))
                Directory.CreateDirectory(OutputDir);

            JoinOutputFile(usmStream);
        }

        private void JoinOutputFile(CriUsmStream usmStream)
        {
            if (!File.Exists("config.json"))
            {
                Console.WriteLine("ERROR: config.json not found!");
                return;
            }

            var audioFormat = usmStream.FinalAudioExtension;
            var pureFileName = Path.GetFileNameWithoutExtension(usmStream.FilePath);

            Helpers.ExecuteProcess("ffmpeg", Helpers.CreateFFmpegParameters(usmStream, pureFileName, OutputDir));

            if (CleanTempFiles)
            {
                Console.WriteLine($"Cleaning up temporary files from {pureFileName}");

                File.Delete(Path.ChangeExtension(usmStream.FilePath, "wav"));
                File.Delete(Path.ChangeExtension(usmStream.FilePath, "adx"));
                File.Delete(Path.ChangeExtension(usmStream.FilePath, "hca"));
                File.Delete(Path.ChangeExtension(usmStream.FilePath, "m2v"));
            }
        }
    }
}
