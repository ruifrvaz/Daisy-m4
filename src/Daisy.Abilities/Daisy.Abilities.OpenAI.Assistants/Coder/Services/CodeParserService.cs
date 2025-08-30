using Daisy.Abilities.Assistant.Coder.Models;
using Daisy.Resources.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Daisy.Abilities.Assistant.Coder.Services
{
    public class CodeParserService : ICodeParserService
    {
        public CodeParserService(ApplicationSettings settings)
        {
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
        }

        public string ParseCoderFiles(string assistantMessage)
        {
            var match = Regex.Match(assistantMessage, "```json\\s*(.*?)```", RegexOptions.Singleline);
            if (!match.Success)
            {
                return string.Empty;
            }
            var jsonContent = match.Groups[1].Value;

            // try to deserialize to ensure that files are correctly parsed. improve later
            var files = JsonSerializer.Deserialize<List<CoderFileMetadata>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return JsonSerializer.Serialize(files);
        }
    }
}