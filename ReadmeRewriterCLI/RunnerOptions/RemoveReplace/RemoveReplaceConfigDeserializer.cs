using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ReadmeRewriterCLI.RunnerOptions.RemoveReplace
{
    [ExcludeFromCodeCoverage]
    internal sealed class RemoveReplaceConfigDeserializer : IRemoveReplaceConfigDeserializer
    {

        private static readonly JsonSerializerOptions s_cachedJsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        public RemoveReplaceConfig? LoadAndParseJson(string configPath, List<string> loadErrors)
        {
            RemoveReplaceConfig? config;
            try
            {
                string json = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<RemoveReplaceConfig>(json, s_cachedJsonSerializerOptions);
            }
            catch (Exception exception)
            {
                loadErrors.Add($"Failed to read config '{configPath}': {exception.Message}");
                return null;
            }

            if (config == null)
            {
                loadErrors.Add($"Config file '{configPath}' is empty or invalid.");
                return null;
            }

            return config;
        }
    }
}
