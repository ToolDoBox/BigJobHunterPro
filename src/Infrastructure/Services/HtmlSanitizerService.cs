using Application.Interfaces;
using HtmlAgilityPack;

namespace Infrastructure.Services;

public class HtmlSanitizerService : IHtmlSanitizer
{
    public string Sanitize(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var document = new HtmlDocument();
        document.LoadHtml(html);

        RemoveDangerousNodes(document);
        RemoveDangerousAttributes(document);

        return document.DocumentNode.OuterHtml;
    }

    private static void RemoveDangerousNodes(HtmlDocument document)
    {
        var nodesToRemove = document.DocumentNode.SelectNodes("//script|//iframe|//object|//embed");
        if (nodesToRemove == null)
        {
            return;
        }

        foreach (var node in nodesToRemove)
        {
            node.Remove();
        }
    }

    private static void RemoveDangerousAttributes(HtmlDocument document)
    {
        foreach (var node in document.DocumentNode.Descendants())
        {
            if (node.NodeType != HtmlNodeType.Element || node.Attributes.Count == 0)
            {
                continue;
            }

            for (var i = node.Attributes.Count - 1; i >= 0; i--)
            {
                var attribute = node.Attributes[i];
                if (attribute.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase))
                {
                    node.Attributes.Remove(attribute);
                    continue;
                }

                if (IsJavascriptUrlAttribute(attribute))
                {
                    node.Attributes.Remove(attribute);
                }
            }
        }
    }

    private static bool IsJavascriptUrlAttribute(HtmlAttribute attribute)
    {
        if (!attribute.Name.Equals("href", StringComparison.OrdinalIgnoreCase)
            && !attribute.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var value = attribute.Value?.TrimStart();
        return value != null && value.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase);
    }
}
