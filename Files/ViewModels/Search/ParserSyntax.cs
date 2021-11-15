using Files.Filesystem.Search;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Files.ViewModels.Search
{
    public interface IParserSyntax
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<IParserSyntaxItem> Items { get; }
    }

    public interface IParserSyntaxItem
    {
    }

    public interface ITextSyntaxItem : IParserSyntaxItem
    {
        string Text { get; }
    }

    public interface IParameterSyntaxItem : IParserSyntaxItem
    {
        string Key { get; }
        string Parameter { get; }
        string Description { get; }
    }

    public class ParserSyntax : IParserSyntax
    {
        public string Name { get; }
        public string Description { get; }
        public IEnumerable<IParserSyntaxItem> Items { get; }

        public ParserSyntax(IParserKey key)
        {
            Name = key.Name;
            Description = key.Description;
            Items = GetItems(key).ToList();
        }

        private IEnumerable<IParserSyntaxItem> GetItems(IParserKey key)
        {
            Regex parameterRegex = new(@":([^:\[\s]+)(\[([^\[\]]+)])?");

            string syntax = key.Syntax;
            while (syntax.Length > 0)
            {
                int newLineIndex = syntax.IndexOf('\n');
                Match parameterMatch = parameterRegex.Match(syntax);

                if (0 <= newLineIndex && (!parameterMatch.Success || newLineIndex < parameterMatch.Index))
                {
                    string text = syntax.Substring(0, newLineIndex);
                    syntax = syntax.Substring(newLineIndex + 1);

                    yield return new TextItem(text);
                }
                else if (parameterMatch.Success)
                {
                    string text = syntax.Substring(0, parameterMatch.Index);
                    string parameter = parameterMatch.Groups[1].Value;
                    string description = parameterMatch.Groups[3].Value;
                    syntax = syntax.Substring(parameterMatch.Index + parameterMatch.Length);

                    if (!string.IsNullOrEmpty(text))
                    {
                        yield return new TextItem(text);
                    }
                    yield return new ParameterItem(key.Name, parameter, description);
                }
                else
                {
                    var text = syntax;
                    syntax = string.Empty;

                    yield return new TextItem(text);
                }
            }
        }

        private class TextItem : ITextSyntaxItem
        {
            public string Text { get; }

            public TextItem(string text) => Text = text;
        }

        private class ParameterItem : IParameterSyntaxItem
        {
            public string Key { get; }
            public string Parameter { get; }
            public string Description { get; }

            public ParameterItem(string key, string parameter, string description)
                => (Key, Parameter, Description) = (key, parameter, description);
        }
    }
}
