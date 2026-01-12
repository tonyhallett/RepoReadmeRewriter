using System.Diagnostics.CodeAnalysis;
using System.Text;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help;
using Spectre.Console;

namespace ReadmeRewriterCLI.ConsoleWriting
{
    [ExcludeFromCodeCoverage]
    internal sealed class SpectreConsoleWriter : IConsoleWriter
    {
        private readonly IAnsiConsole _ansiConsole;
        private SpectreConsoleWriter(IAnsiConsole ansiConsole) => _ansiConsole = ansiConsole;
        private static IConsoleWriter? s_instance;
        public static IConsoleWriter Instance()
        {
            if (s_instance != null)
            {
                return s_instance;
            }

            Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            s_instance = new SpectreConsoleWriter(AnsiConsole.Console);

            return s_instance;
        }

        public void WriteLine(string message) => _ansiConsole.WriteLine(message);

        public void WriteErrorLine(string message) => _ansiConsole.MarkupLine($"[red]{message}[/]");

        public void WriteWarningLine(string message) => _ansiConsole.MarkupLine($"[yellow]{message}[/]");

        private sealed class OptionsLayout(
            List<IOptionInfo> requiredOptions,
            List<IOptionInfo> optionalOptions,
            bool hasDefaultOrCompletions,
            bool hasAliases
        )
        {
            public List<IOptionInfo> RequiredOptions { get; } = requiredOptions;
            public List<IOptionInfo> OptionalOptions { get; } = optionalOptions;
            public bool HasDefaultsOrCompletions { get; } = hasDefaultOrCompletions;
            public bool HasAliases { get; } = hasAliases;

            public static OptionsLayout Build(List<IOptionInfo> options)
            {
                List<IOptionInfo> requiredOptions = [];
                List<IOptionInfo> optionalOptions = [];
                bool hasAliases = false;
                bool hasDefaultOrCompletions = false;

                foreach (IOptionInfo option in options)
                {
                    if (!hasAliases && option.Aliases.Count > 0)
                    {
                        hasAliases = true;
                    }

                    if (!hasDefaultOrCompletions && (option.DefaultValue != null || option.CompletionLines.Count > 0))
                    {
                        hasDefaultOrCompletions = true;
                    }

                    List<IOptionInfo> list = option.Required ? requiredOptions : optionalOptions;
                    list.Add(option);
                }

                return new OptionsLayout(requiredOptions, optionalOptions, hasDefaultOrCompletions, hasAliases);
            }
        }

        public void WriteHelp(IArgumentsOptionsInfo helpOutput)
        {
            WriteDescription();
            WriteUsage();
            WriteArguments();
            WriteOptions();

            void WriteDescription()
            {
                WriteHeader("About");
                _ansiConsole.WriteLine("A CLI tool to help you rewrite your GitHub or GitLab relative README assets to absolute.");
                // todo need to add readme for cli
                // string readmePath = "https://github.com/tonyhallett/NugetRepoReadme/blob/master/README.md";
                // _ansiConsole.MarkupLine($"See [link={readmePath}]readme[/] for full details");
                _ansiConsole.WriteLine();
            }

            void WriteUsage()
            {
                WriteHeader("Usage");
                string exeName = "ReadmeRewriterCLI";
                var usageSb = new StringBuilder(exeName + " ");
                foreach (IArgumentInfo argument in helpOutput.Arguments)
                {
                    _ = usageSb.Append($"<{argument.Name}> ");
                }

                if (helpOutput.Options.Count > 0)
                {
                    _ = usageSb.Append("[options]");
                }

                // note that MarkupLine throws when no [markup]...[/]
                _ansiConsole.WriteLine(usageSb.ToString());
                _ansiConsole.WriteLine();
            }

            void WriteArguments()
            {
                if (helpOutput.Arguments.Count == 0)
                {
                    return;
                }

                TableColumn tc1 = new("");
                TableColumn defaultColumn = new("Default");
                TableColumn descriptionColumn = new("");
                Table table = new Table()
                {
                    Expand = true
                }.AddColumns(tc1, defaultColumn, descriptionColumn);

                bool hasDefaultValue = false;

                foreach (IArgumentInfo argument in helpOutput.Arguments)
                {
                    string defaultValue = argument.DefaultValue ?? "";
                    if (!hasDefaultValue)
                    {
                        hasDefaultValue = argument.DefaultValue != null;
                    }

                    _ = table.AddRow($"<{argument.Name}>", defaultValue, argument.Description ?? "");
                }

                if (!hasDefaultValue)
                {
                    // this does not work as expected
                    defaultColumn.Width = 0;
                }

                WriteHeader("Arguments");
                _ansiConsole.Write(table);
            }

            void WriteOptions()
            {
                OptionsLayout optionsLayout = OptionsLayout.Build(helpOutput.Options);

                TableColumn tc1 = new("");
                TableColumn aliasesColumn = new("Aliases");
                TableColumn valuesColumn = new("Values");
                TableColumn descriptionColumn = new("");

                Table table = new()
                {
                    Expand = true
                };
                AddColumns();

                // exception if add rows before columns
                AddRequiredOptions();
                AddOptionalOptions();

                void AddColumns() => GetColumns().ForEach((col) => table.AddColumn(col));

                List<string> GetColumns()
                {
                    var columns = new List<string> { "" };
                    if (optionsLayout.HasAliases)
                    {
                        columns.Add("Aliases");
                    }

                    if (optionsLayout.HasDefaultsOrCompletions)
                    {
                        columns.Add("Values");
                    }

                    columns.Add("Description");
                    return columns;
                }

                void AddOptionalOptions()
                {
                    if (optionsLayout.OptionalOptions.Count > 0)
                    {
                        if (optionsLayout.RequiredOptions.Count > 0)
                        {
                            _ = table.AddEmptyRow();
                        }

                        _ = table.AddRow("[bold]Optional[/]");

                        AddRows(optionsLayout.OptionalOptions);
                    }
                }

                void AddRequiredOptions()
                {
                    if (optionsLayout.RequiredOptions.Count > 0)
                    {
                        _ = table.AddRow("[bold]Required[/]");
                        AddRows(optionsLayout.RequiredOptions);
                    }
                }

                void AddRows(List<IOptionInfo> options)
                {
                    foreach (IOptionInfo option in options)
                    {
                        bool addedMainRow = false;
                        if (option.DefaultValue != null)
                        {
                            AddMainRow($"Default : {option.DefaultValue}");
                        }

                        option.CompletionLines.ForEach(AddRow);
                        if (!addedMainRow)
                        {
                            AddMainRow("");
                        }

                        void AddRow(string value)
                        {
                            if (addedMainRow)
                            {
                                _ = table.AddRow("", "", value, "");
                            }
                            else
                            {
                                AddMainRow(value);
                            }
                        }

                        void AddMainRow(string value)
                        {
                            string aliases = string.Join(", ", option.Aliases);
                            _ = table.AddRow(option.Name, aliases, value, option.Description ?? "");
                            addedMainRow = true;
                        }
                    }
                }

                WriteHeader("Options");
                _ansiConsole.Write(table);
            }

            string GetHeader(string header) => $"[bold]{header}:[/]";

            void WriteHeader(string header) => _ansiConsole.MarkupLine(GetHeader(header));
        }
    }
}
