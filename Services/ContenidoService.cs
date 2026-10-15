using System.Net;
using System.Text.Json;
using FoneticaUNEFM.Models;

namespace FoneticaUNEFM.Services;

/// <summary>
/// Lee el contenido de la plataforma (JSON y Markdown) desde la carpeta configurada
/// en wwwroot/configuracion.json. No hay backend: todo son archivos estáticos.
/// </summary>
public class ContenidoService(HttpClient http)
{
    // Comentarios y comas finales permitidos para que editar a mano sea más tolerante.
    private static readonly JsonSerializerOptions OpcionesJson = new(JsonSerializerDefaults.Web)
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private Task<Uri>? _base;
    private Task<Sitio>? _sitio;
    private readonly Dictionary<string, Task<Modulo?>> _modulos = [];

    public Task<Sitio> ObtenerSitioAsync() => _sitio ??= CargarSitioAsync();

    public Task<Modulo?> ObtenerModuloAsync(string id)
    {
        if (!_modulos.TryGetValue(id, out var tarea))
        {
            tarea = CargarModuloAsync(id);
            _modulos[id] = tarea;
        }
        return tarea;
    }

    public async Task<List<Modulo>> ObtenerModulosAsync()
    {
        var sitio = await ObtenerSitioAsync();
        var modulos = await Task.WhenAll(sitio.Modulos.Select(ObtenerModuloAsync));
        return modulos.OfType<Modulo>().ToList();
    }

    /// <summary>Dirección absoluta de un archivo dentro de la carpeta de contenido.</summary>
    public async Task<Uri> UrlAsync(string rutaRelativa) => new(await ObtenerBaseAsync(), rutaRelativa);

    /// <summary>Devuelve el texto Markdown o null si el archivo no existe.</summary>
    public async Task<string?> ObtenerTextoAsync(string rutaRelativa)
    {
        using var respuesta = await http.GetAsync(await UrlAsync(rutaRelativa));
        if (respuesta.StatusCode == HttpStatusCode.NotFound) return null;
        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadAsStringAsync();
    }

    /// <summary>Enlace al formulario de Google, con el módulo preseleccionado si está configurado.</summary>
    public static string? UrlFormulario(Sitio sitio, Modulo modulo)
    {
        var url = string.IsNullOrWhiteSpace(modulo.Reto.FormularioUrl) ? sitio.FormularioUrl : modulo.Reto.FormularioUrl;
        if (string.IsNullOrWhiteSpace(url)) return null;
        if (string.IsNullOrWhiteSpace(sitio.FormularioCampoModulo)) return url;

        var separador = url.Contains('?') ? "&" : "?";
        var valor = Uri.EscapeDataString($"Módulo {modulo.Numero}");
        return $"{url}{separador}usp=pp_url&{sitio.FormularioCampoModulo}={valor}";
    }

    private Task<Uri> ObtenerBaseAsync() => _base ??= CargarBaseAsync();

    private async Task<Uri> CargarBaseAsync()
    {
        var config = await LeerJsonAsync<Configuracion>(new Uri(http.BaseAddress!, "configuracion.json")) ?? new();
        var ruta = config.ContenidoUrlBase.Trim();
        if (!ruta.EndsWith('/')) ruta += "/";
        return new Uri(http.BaseAddress!, ruta);
    }

    private async Task<Sitio> CargarSitioAsync()
        => await LeerJsonAsync<Sitio>(await UrlAsync("sitio.json"))
           ?? throw new InvalidOperationException("No se encontró el archivo sitio.json en la carpeta de contenido.");

    private async Task<Modulo?> CargarModuloAsync(string id)
    {
        var modulo = await LeerJsonAsync<Modulo>(await UrlAsync($"modulos/{id}/modulo.json"));
        if (modulo is not null) modulo.Id = id;
        return modulo;
    }

    private async Task<T?> LeerJsonAsync<T>(Uri url)
    {
        using var respuesta = await http.GetAsync(url);
        if (respuesta.StatusCode == HttpStatusCode.NotFound) return default;
        respuesta.EnsureSuccessStatusCode();
        await using var flujo = await respuesta.Content.ReadAsStreamAsync();
        try
        {
            return await JsonSerializer.DeserializeAsync<T>(flujo, OpcionesJson);
        }
        catch (JsonException ex)
        {
            // Mensaje pensado para quien edita el contenido: indica archivo y línea.
            throw new InvalidOperationException(
                $"El archivo {url.AbsolutePath} tiene un error de formato cerca de la línea {ex.LineNumber + 1}. " +
                "Revise comillas y comas.", ex);
        }
    }
}
