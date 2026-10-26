# Guía para editar el contenido de la plataforma

Esta guía es para la autora de la plataforma. **No hace falta saber programar**: todo el
contenido (textos, videos, enlaces, imágenes y el formulario) vive en archivos de texto dentro
de la carpeta `wwwroot/contenido`. Al cambiar esos archivos, la página cambia.

## 1. Dónde está cada cosa

```
wwwroot/
├── configuracion.json        ← dónde buscar el contenido (normalmente no se toca)
└── contenido/
    ├── sitio.json            ← nombre, datos de la tesis, logos, enlace del formulario, orden de módulos
    ├── inicio.md             ← texto de bienvenida de la página de inicio
    ├── imagenes/             ← logos
    └── modulos/
        ├── modulo-1/
        │   ├── modulo.json   ← título, objetivo, videos, enlaces y datos del Speaking Challenge
        │   ├── teoria.md     ← "Puntos clave" (texto de teoría, imágenes, tablas)
        │   ├── reto.md       ← instrucciones y texto a leer del Speaking Challenge
        │   └── (imágenes del módulo)
        ├── modulo-2/ …
        ├── modulo-3/ …
        └── modulo-4/ …
```

## 2. Archivos `.md` (textos)

Son textos normales con algunos símbolos para dar formato (formato *Markdown*):

| Escribo…                        | Se ve como…                                         |
|---------------------------------|-----------------------------------------------------|
| `**negrita**`                   | **negrita**                                         |
| `*cursiva*`                     | *cursiva*                                           |
| `- elemento`                    | una lista con viñetas                               |
| `1. elemento`                   | una lista numerada                                  |
| `### Subtítulo`                 | un subtítulo                                        |
| `> texto`                       | un recuadro destacado (se usa para el texto a leer) |
| `` `/θ/` ``                     | símbolo fonético con la fuente del AFI              |
| `==PHO==-to-graph`              | sílaba resaltada en naranja/amarillo                |
| `[texto](https://…)`            | un enlace                                           |
| `![descripción](imagen.jpg)`    | una imagen guardada en la misma carpeta             |

**Símbolos fonéticos:** escríbalos siempre entre comillas invertidas (la tecla junto al `+`
en teclados en español, o `Alt Gr` + `}`), por ejemplo `` `/iː/` ``. Así se muestran con una
fuente que dibuja bien todos los símbolos del AFI.

## 3. Archivos `.json` (datos)

Son listas de datos entre comillas. Reglas importantes:

- Cambie **solo el texto entre comillas** a la derecha de los dos puntos.
- No borre comillas `"`, comas `,` ni llaves `{ }` / corchetes `[ ]`.
- Las líneas que empiezan con `//` son notas explicativas; no afectan la página.
- Si se equivoca, la página mostrará un mensaje indicando el archivo y la línea con el error.

### Agregar el video de YouTube

1. Abra el video en YouTube. La dirección se ve así: `https://www.youtube.com/watch?v=AbC123xyz`
2. Copie el código que va después de `v=` (en el ejemplo, `AbC123xyz`).
3. Péguelo en `"youtubeId"` dentro del `modulo.json` del módulo:

```json
{ "titulo": "Speech Organs Overview", "fuente": "Keys to English", "youtubeId": "AbC123xyz" }
```

Los videos se reproducen dentro de la plataforma, sin salir del sitio. Para ahorrar datos, solo
se descarga el video cuando el estudiante pulsa "Reproducir". Si un módulo no tiene videos,
deje la lista vacía: `"videos": []` y la sección no aparece.

### Agregar otro video o enlace

Copie una línea completa `{ … }`, péguela debajo y separe ambas con una coma:

```json
"videos": [
  { "titulo": "Video 1", "fuente": "…", "youtubeId": "…" },
  { "titulo": "Video 2", "fuente": "…", "youtubeId": "…" }
],
```

### Lecturas, prácticas y quizzes (`"enlaces"`)

Cada recurso externo lleva un `"tipo"` que decide en qué sección aparece:

| `"tipo"`      | Sección                   |
|---------------|---------------------------|
| `"lectura"`   | 📖 Lecturas de apoyo      |
| `"practica"`  | 🎧 Prácticas interactivas |
| `"quiz"`      | ✅ Quizzes y juegos       |

```json
{
  "tipo": "practica",
  "titulo": "Ejercicios con símbolos fonéticos",
  "url": "https://agendaweb.org/exercises/phonetic/phonetic-symbols-1.htm",
  "incrustar": true,
  "descripcion": "Relaciona cada símbolo del AFI con su sonido (Agendaweb)."
}
```

- `"incrustar": true` muestra el botón **Abrir aquí**, que abre el sitio en una ventana dentro
  de la plataforma. **Úselo solo si el sitio lo permite**: muchos sitios (EnglishClub,
  LearnEnglishSounds, PurposeGames) bloquean esto y la ventana quedaría en blanco. Si no está
  segura, no lo ponga: el recurso se abrirá en una pestaña nueva.
- `"urlIncrustada"` (opcional): una dirección distinta para la ventana interna. Por ejemplo,
  en Educaplay la versión para insertar es `…/juego/…` en lugar de `…/recursos-educativos/…`.
- Si `"url"` está vacío, el recurso aparece como recomendación sin botón (ej. *Shadowing*).

## 4. Formulario de Google para las grabaciones

Cada módulo tiene su propio formulario en su `modulo.json` → `"reto"` → `"formularioUrl"`.
El botón abre el formulario en una **pestaña nueva**: como los formularios piden iniciar sesión
con Google para adjuntar el audio, Google no permite completarlos dentro de otra página.

Para crear un formulario nuevo:

1. Cree un formulario en [forms.google.com](https://forms.google.com) con:
   - Nombre y apellido, Cédula, Sección (respuesta corta).
   - **Módulo** (desplegable: Módulo 1, Módulo 2, Módulo 3, Módulo 4).
   - **Grabación** (pregunta tipo *Subir archivo* → permitir solo *Audio*; tamaño máximo 10 MB).
2. Pulse **Enviar** → ícono de enlace 🔗 → copie el enlace.
3. Péguelo en el `"formularioUrl"` del `"reto"` del módulo (o en `sitio.json` para usar
   uno general en todos los módulos).

> Nota: Google exige que el estudiante inicie sesión con una cuenta de Google para subir
> archivos. Las grabaciones quedan en su Google Drive, en una carpeta del formulario.

**Opcional – módulo preseleccionado:** en el formulario, menú ⋮ → *Obtener enlace
prellenado*, elija "Módulo 1" y pulse *Obtener enlace*. En el enlace aparecerá algo como
`entry.123456789=Módulo+1`. Copie solo `entry.123456789` en `"formularioCampoModulo"` de
`sitio.json`. Así, cada botón abre el formulario con su módulo ya marcado.


## 5. Imágenes

- Guarde la imagen dentro de la carpeta del módulo (ej. `modulos/modulo-1/tracto-vocal.jpg`).
- Úsela en `teoria.md` con `![Descripción de la imagen](tracto-vocal.jpg)`.
- Use imágenes livianas (menos de 300 KB). Puede reducirlas en [squoosh.app](https://squoosh.app).

## 6. Google Analytics 4 (métricas de la tesis)

La plataforma envía a Google Analytics las páginas visitadas, el tiempo de permanencia y estos
eventos: `reproducir_video`, `abrir_recurso` (lecturas, prácticas, quizzes) y `abrir_formulario`.

1. En [analytics.google.com](https://analytics.google.com) cree una propiedad GA4 y un flujo
   de datos *Web* con la dirección de la plataforma.
2. Copie el **ID de medición** (empieza por `G-`).
3. Péguelo en `sitio.json`, en `"googleAnalyticsId"`.

Para ver los eventos como columnas en los informes, en GA4 vaya a *Administrar → Definiciones
personalizadas* y cree dimensiones para los parámetros `modulo`, `video`, `recurso` y `tipo`.

## 7. Agregar un módulo nuevo

1. Copie la carpeta `modulo-4` y cámbiele el nombre (ej. `modulo-5`).
2. Edite sus archivos `modulo.json`, `teoria.md` y `reto.md`.
3. En `sitio.json`, agregue `"modulo-5"` a la lista `"modulos"`.

## 8. Cómo publicar los cambios

### Opción A (recomendada): editar directamente en GitHub

Si la plataforma está en GitHub con la publicación automática configurada:

1. Entre al repositorio en github.com y abra el archivo (ej. `wwwroot/contenido/modulos/modulo-1/teoria.md`).
2. Pulse el lápiz ✏️ (*Edit*), haga el cambio y pulse **Commit changes**.
3. Para subir imágenes: abra la carpeta → **Add file** → **Upload files**.
4. En 2–3 minutos la página publicada se actualiza sola.

### Opción B: servidor propio (hosting, FTP)

1. El programador sube una vez la carpeta publicada (`wwwroot`) al servidor.
2. Para cambiar contenido, basta con reemplazar por FTP los archivos dentro de `contenido/`
   en el servidor. No hace falta volver a compilar.
3. Los estudiantes verán los cambios la próxima vez que abran la página con conexión
   (el contenido siempre se pide primero a internet y, si no hay conexión, se muestra la
   última versión guardada).
