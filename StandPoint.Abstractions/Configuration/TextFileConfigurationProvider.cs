using System.IO;
using Microsoft.Extensions.Configuration;

namespace StandPoint.Abstractions.Configuration
{
    public class TextFileConfigurationSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            this.EnsureDefaults(builder);
            return (IConfigurationProvider)new TextFileConfigurationProvider(this); ;
        }
    }

    public class TextFileConfigurationProvider : FileConfigurationProvider
    {
        public TextFileConfigurationProvider(FileConfigurationSource source) : base(source){}

        public override void Load(Stream stream)
        {
            var configurationFileParser = new TextFileConfigurationParser();
            this.Data = configurationFileParser.Parse(stream);
        }
    }
}
