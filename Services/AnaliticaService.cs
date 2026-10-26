using Microsoft.JSInterop;

namespace FoneticaUNEFM.Services;

/// <summary>
/// Registro de uso con Google Analytics 4 (métricas de Log Data de la tesis: frecuencia de
/// acceso, tiempo de permanencia y recursos multimedia visualizados). Si no hay ID configurado
/// en sitio.json, todas las llamadas son inofensivas.
/// </summary>
public class AnaliticaService(IJSRuntime js)
{
    public async Task IniciarAsync(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        await js.InvokeVoidAsync("fonetica.iniciarAnalitica", id.Trim());
    }

    public ValueTask PaginaVistaAsync() => js.InvokeVoidAsync("fonetica.paginaVista");

    public ValueTask EventoAsync(string nombre, object? parametros = null) =>
        js.InvokeVoidAsync("fonetica.evento", nombre, parametros);
}
