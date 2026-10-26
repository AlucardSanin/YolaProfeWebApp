// Funciones pequeñas que Blazor llama desde C# (JS interop).
window.fonetica = {
    // ----- Google Analytics 4 -----
    // Se carga solo si sitio.json tiene "googleAnalyticsId". Las páginas vistas se envían a mano
    // porque la plataforma es una SPA (no recarga la página al cambiar de módulo).
    iniciarAnalitica(id) {
        if (!id || window.gtag) return;
        window.dataLayer = window.dataLayer || [];
        window.gtag = function () { window.dataLayer.push(arguments); };
        window.gtag('js', new Date());
        window.gtag('config', id, { send_page_view: false });
        const script = document.createElement('script');
        script.async = true;
        script.src = 'https://www.googletagmanager.com/gtag/js?id=' + encodeURIComponent(id);
        document.head.appendChild(script);
    },

    paginaVista() {
        if (!window.gtag) return;
        // Espera a que <PageTitle> actualice el título de la pestaña.
        setTimeout(() => window.gtag('event', 'page_view', {
            page_location: location.href,
            page_path: location.pathname,
            page_title: document.title
        }), 300);
    },

    evento(nombre, parametros) {
        if (window.gtag) window.gtag('event', nombre, parametros || {});
    },

    // ----- Visor de recursos (ventana dentro de la plataforma) -----
    bloquearDesplazamiento(bloquear) {
        document.documentElement.classList.toggle('sin-desplazamiento', bloquear);
    },

    enfocar(elemento) {
        if (elemento) elemento.focus();
    },

    // Índice del módulo: desplazarse a una sección sin cambiar de página.
    irA(id) {
        const destino = document.getElementById(id);
        if (destino) destino.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};
