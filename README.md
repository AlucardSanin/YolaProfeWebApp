# Website de Fonética I · UNEFM

Plataforma informativa de entrenamiento fonético (EVA) para estudiantes del Programa de
Educación en Lengua Extranjera Mención Inglés de la UNEFM, municipalización Carirubana.
Desarrollada como parte del Trabajo de Grado de **Br. Yolanny Gómez** (tutora: Lcda. Karibel Velasco).

- **Sin backend:** Blazor WebAssembly standalone. Es un sitio estático que se puede alojar en
  GitHub Pages, Netlify, Azure Static Web Apps o cualquier hosting.
- **Sin cuentas de usuario:** no hay registro ni base de datos. Las grabaciones de los
  *Speaking Challenges* se envían por un formulario de Google por módulo. Las métricas de uso
  (Log Data de la tesis) se registran con Google Analytics 4 si se configura
  `googleAnalyticsId` en `sitio.json`.
- **Sin salir del sitio:** los videos de YouTube se reproducen en la página y los recursos que
  lo permiten (`"incrustar": true`) se abren en una ventana interna.
- **Contenido editable sin programar:** textos en Markdown y datos en JSON dentro de
  `wwwroot/contenido`. Ver [GUIA-CONTENIDO.md](GUIA-CONTENIDO.md).
- **Pensado para conexiones lentas:** PWA con caché offline de la aplicación, videos de
  YouTube con carga diferida (miniatura → reproductor al pulsar), sin fuentes web.

## Requisitos

- .NET SDK 10

## Ejecutar en local

```bash
dotnet run --urls http://localhost:5280
```

## Publicar

```bash
dotnet publish -c Release -o salida
```

El sitio queda en `salida/wwwroot`. Si se aloja en una subcarpeta (ej. `https://dominio/fonetica/`),
cambie `<base href="/" />` en `index.html` por `<base href="/fonetica/" />`.

### GitHub Pages (automático)

El flujo [.github/workflows/publicar.yml](.github/workflows/publicar.yml) compila y publica en
cada `push` a `main`. Ajusta la ruta base al nombre del repositorio y crea `404.html` para las rutas.
En el repositorio: **Settings → Pages → Source: GitHub Actions**.

## Estructura

| Ruta                                   | Qué contiene                                              |
|----------------------------------------|-----------------------------------------------------------|
| `wwwroot/configuracion.json`           | `contenidoUrlBase`: carpeta o URL de donde se lee el contenido |
| `wwwroot/contenido/`                   | Todo el contenido editable (ver la guía)                  |
| `Services/ContenidoService.cs`         | Lectura de JSON/Markdown (admite comentarios y comas finales) |
| `Services/MarkdownService.cs`          | Markdown → HTML (Markdig); `código` = AFI, `==texto==` = resaltado |
| `Pages/`                               | Inicio, Módulo (`/modulo/{id}`), Acerca de                |
| `Components/VideoYoutube.razor`        | Video con carga diferida (youtube-nocookie)               |
| `wwwroot/service-worker.published.js`  | Caché: app *cache-first*; `contenido/` *network-first*    |

## Contenido alojado fuera del sitio

`configuracion.json` permite leer el contenido desde otra URL (debe terminar en `/` y el
servidor debe permitir CORS), por ejemplo:

```json
{ "contenidoUrlBase": "https://cdn.jsdelivr.net/gh/USUARIO/REPO-CONTENIDO@main/" }
```

> Google Drive **no** sirve archivos con CORS, así que no funciona como origen directo del
> contenido. Para que la autora edite sin programador, la opción recomendada es editar en
> github.com (ver la guía, opción A).
