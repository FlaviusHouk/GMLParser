using System;
using System.IO;
using System.Linq;

using GMLParser.Model;

namespace GMLParser.CLI
{
    public class CommandLine
    {
        private string[] _files;
        private Parser _parser;
        public CommandLine(string[] fileNames)
        {
            if(fileNames.Any(file => string.Compare(Path.GetExtension(file), ".gml") != 0 ))
                throw new ArgumentException("Some files in wrong format");

            _files = fileNames;
            _parser = new Parser();
        }

        public void ProcessFiles()
        {
            foreach(string file in _files)
            {
                using(FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    using(StreamReader reader = new StreamReader(fileStream))
                    {
                        string CCode = _parser.ParseGML(reader.ReadToEnd());
                    }
                }
            }
        }
    }
}