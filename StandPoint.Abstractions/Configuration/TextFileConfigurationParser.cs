using System;
using System.Collections.Generic;
using System.IO;

namespace StandPoint.Abstractions.Configuration
{
    internal class TextFileConfigurationParser
    {
        private readonly IDictionary<string, string> _data;

        public TextFileConfigurationParser()
        {
            _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, string> Parse(Stream input)
        {
            _data.Clear();
            using (var sr = new StreamReader(input))
            {
                var lineCount = -1;
                string lineRead;
                while ((lineRead = sr.ReadLine()) != null)
                {
                    lineCount++;
                    var line = lineRead.Trim();
                    if(string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                    var split = line.Split('=');
                    if(split.Length == 0) continue;
                    if(split.Length == 1)
                        throw new FormatException($"Line {lineCount}: No value are set");

                    var key = split[0];
                    _data[key] = split[1];
                }
            }

            return _data;
        }
    }
}
