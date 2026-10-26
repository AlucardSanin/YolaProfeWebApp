namespace FoneticaUNEFM.Models;

// Estas clases reflejan los archivos JSON de la carpeta wwwroot/contenido.
// Los nombres de propiedad se leen sin distinguir mayúsculas (ej. "nombrePlataforma").

public class Configuracion
{
    public string ContenidoUrlBase { get; set; } = "contenido/";
}

public class Sitio
{
    public string NombrePlataforma { get; set; } = "";
    public string Lema { get; set; } = "";
    public string Universidad { get; set; } = "";
    public string SiglasUniversidad { get; set; } = "";
    public string Programa { get; set; } = "";
    public string Sede { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public string TituloTesis { get; set; } = "";
    public string Autora { get; set; } = "";
    public string Tutora { get; set; } = "";
    public string Anio { get; set; } = "";
    public string LogoPrincipal { get; set; } = "";
    public string LogoSecundario { get; set; } = "";
    public string GoogleAnalyticsId { get; set; } = "";
    public string FormularioUrl { get; set; } = "";
    public string FormularioCampoModulo { get; set; } = "";
    public List<string> Modulos { get; set; } = [];
}

public class Modulo
{
    // Se asigna con el nombre de la carpeta, no viene del JSON.
    public string Id { get; set; } = "";
    public int Numero { get; set; }
    public string Titulo { get; set; } = "";
    public string Objetivo { get; set; } = "";
    public string PorQueImporta { get; set; } = "";
    public List<Video> Videos { get; set; } = [];
    public List<Enlace> Enlaces { get; set; } = [];
    public Reto Reto { get; set; } = new();
}

public class Video
{
    public string Titulo { get; set; } = "";
    public string Fuente { get; set; } = "";
    public string YoutubeId { get; set; } = "";
    public string Busqueda { get; set; } = "";
}

public class Enlace
{
    // "lectura", "practica" o "quiz": define en qué grupo se muestra.
    public string Tipo { get; set; } = "lectura";
    public string Titulo { get; set; } = "";
    public string Url { get; set; } = "";
    public string Descripcion { get; set; } = "";
    // true: se puede abrir en una ventana dentro de la plataforma (el sitio lo permite).
    public bool Incrustar { get; set; }
    // Dirección alternativa para mostrarlo dentro (vacío = se usa Url).
    public string UrlIncrustada { get; set; } = "";

    public string UrlVisor => string.IsNullOrWhiteSpace(UrlIncrustada) ? Url : UrlIncrustada;

    public string Dominio =>
        Uri.TryCreate(Url, UriKind.Absolute, out var uri) ? uri.Host.Replace("www.", "") : "";
}

public class Reto
{
    public string Titulo { get; set; } = "";
    public string MensajeFinal { get; set; } = "";
    public string FormularioUrl { get; set; } = "";
}
