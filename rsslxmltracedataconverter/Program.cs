using System;
using System.Xml;
using RsslTraceXmlFileUtil;
using CommandLine;
namespace RSSLTraceConverter
{
    class Program
    {
        class Options
        {

            [Option('s', "source", Required = true, HelpText = "Absolution path to RSSL XML Trace file")]
            public string SourceFile { get; set; }

            [Option('o', "output", Required = false,Default = "output.xml", HelpText = "Absolute path to Xml output file, default is output.xml generate under running directory.")]
            public string DestinationFile { get; set; }

            [Option('r', "rdmdict", Required = false, Default = @"./Dict/RDMFieldDictionary", HelpText = "Absolution path to RDMFieldDictionary file. Default reading dictionary form /Dict/RDMFieldDictionary")]
            public string RdmFieldDictionaryFile { get; set; }

            [Option('e', "enumdict", Required = false, Default = @"./Dict/enumtype.def", HelpText = "Absolution path to enumtype.def file. Default reading dictionary form /Dict/enumtype.def")]
            public string EnumTypeDefFile { get; set; }

            // Omitting long name, defaults to name of property, ie "--verbose"
            [Option(Default = false, HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }
        }
        public class Config
        {
            public string RdmFieldDictPath { get; set; }
            public string Enumtypedefpath { get; set; }
            public string Sourcexmlfile { get; set; }
            public string DestinationFile { get; set; }
            public bool Verbose { get; set; }
        }
        static void Main(string[] args)
        {
            Config config = null;
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts, out config));

            if (result.Tag == ParserResultType.NotParsed)
            {
                return;
            }

            try
            {
                if(!config.Verbose) Console.WriteLine($"Start loading and processing Xml file {config.Sourcexmlfile} please wait");
                if (!RsslTraceFileUtil.Convert(config.Sourcexmlfile, config.DestinationFile, out var errorMsg,
                    config.RdmFieldDictPath, config.Enumtypedefpath,config.Verbose))
                {
                    Console.WriteLine("Data conversion failed");
                    Console.WriteLine($"{errorMsg}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Detect!!: {ex.Message}");
                return;
            }

            Console.WriteLine($"Finished reading and processing Xml data, please find output from {config.DestinationFile}");
        }
        static int RunOptionsAndReturnExitCode(Options options, out Config config)
        {
            config = new Config();
            config.Verbose = options.Verbose;
            config.DestinationFile = options.DestinationFile;
            config.Sourcexmlfile = options.SourceFile;
            config.Enumtypedefpath = options.EnumTypeDefFile;
            config.RdmFieldDictPath = options.RdmFieldDictionaryFile;
            return 0;
        }


    }

      
}
