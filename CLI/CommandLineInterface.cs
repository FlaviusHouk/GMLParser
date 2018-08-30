using System;
using System.IO;
using System.Linq;

using GMLParser.Model;

namespace GMLParser.CLI
{
    public class CommandLine
    {
        private string[] _files;
        private string _pathToHeaders; 
        private Parser _parser;
        public CommandLine(string[] fileNames)
        {
            _pathToHeaders = fileNames.First();

            _files = fileNames.Skip(1).ToArray();

            if(_files.Any(file => string.Compare(Path.GetExtension(file), ".gml") != 0 ))
                throw new ArgumentException("Some files in wrong format");
            
            _parser = new Parser();
        }

        public void ProcessFiles()
        {
            foreach(string file in _files)
            {
                string CCode = string.Empty;
                string loc = Path.GetDirectoryName(Path.GetFullPath(file));
                string name = Path.GetFileNameWithoutExtension(file);
                string outFile = $"{loc}/{name}.g.c"; 
                string outHeader = $"{_pathToHeaders}/{name}.g.h"; 

                using(FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    using(StreamReader reader = new StreamReader(fileStream))
                    {
                        CCode = _parser.ParseGML(reader.ReadToEnd(), name);
                    }
                }

                using(FileStream fileStream = File.Open(outFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fileStream.SetLength(0);
                    using(StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.Write(CCode);
                    }
                }

                using(FileStream fileStream = File.Open(outHeader, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fileStream.SetLength(0);
                    using(StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.Write(_parser.GenerateFileHeader(name));
                    }
                }
            }
        }
    }
}