using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace FoneticaUNEFM.Services;

/// <summary>
/// Convierte Markdown a HTML. Convenciones para quien edita el contenido:
///  - `símbolo`  (entre comillas invertidas) se muestra con la fuente del AFI.
///  - ==texto==  se resalta en naranja/amarillo (sílabas acentuadas, enlaces entre palabras).
///  - Las imágenes y enlaces relativos se buscan en la misma carpeta del archivo .md.
/// </summary>
public static class MarkdownService
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public static string AHtml(string? markdown, Uri? carpeta = null)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return "";

        var documento = Markdown.Parse(markdown, Pipeline);

        foreach (var enlace in documento.Descendants<LinkInline>())
        {
            if (string.IsNullOrEmpty(enlace.Url)) continue;

            if (carpeta is not null && EsRelativa(enlace.Url))
                enlace.Url = new Uri(carpeta, enlace.Url).ToString();

            var atributos = enlace.GetAttributes();
            if (enlace.IsImage)
            {
                atributos.AddPropertyIfNotExist("loading", "lazy");
                atributos.AddPropertyIfNotExist("decoding", "async");
            }
            else if (enlace.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                atributos.AddPropertyIfNotExist("target", "_blank");
                atributos.AddPropertyIfNotExist("rel", "noopener");
            }
        }

        foreach (var codigo in documento.Descendants<CodeInline>())
            codigo.GetAttributes().AddClass("ipa");

        using var escritor = new StringWriter();
        var renderizador = new HtmlRenderer(escritor);
        Pipeline.Setup(renderizador);
        renderizador.Render(documento);
        escritor.Flush();
        return escritor.ToString();
    }

    private static bool EsRelativa(string url) =>
        !url.StartsWith('#') && !url.StartsWith('/') && !Uri.IsWellFormedUriString(url, UriKind.Absolute);
}
