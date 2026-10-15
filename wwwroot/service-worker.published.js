// Service worker de producción: guarda la aplicación en caché para que cargue rápido
// (y funcione sin conexión) después de la primera visita.
// See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const contentCacheName = 'contenido-cache';
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/, /\.webmanifest$/ ];
// El contenido editable NO se fija en la instalación: la autora puede cambiar esos archivos
// directamente en el servidor sin recompilar, así que se pide siempre a la red primero.
const offlineAssetsExclude = [ /^service-worker\.js$/, /^configuracion\.json$/, /^contenido\// ];

// Funciona tanto en la raíz del dominio como en una subcarpeta (ej. GitHub Pages).
const baseUrl = new URL(self.registration.scope);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

function isEditableContent(url) {
    const relative = url.startsWith(baseUrl.href) ? url.substring(baseUrl.href.length) : null;
    return relative !== null && offlineAssetsExclude.slice(1).some(pattern => pattern.test(relative));
}

async function onInstall(event) {
    console.info('Service worker: Install');

    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        // index.html se ajusta al publicar (ruta base de GitHub Pages), por eso no se verifica su hash.
        .map(asset => new Request(asset.url, asset.url.endsWith('.html')
            ? { cache: 'no-cache' }
            : { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    if (event.request.method !== 'GET') {
        return fetch(event.request);
    }

    // Contenido editable: red primero; si no hay conexión, la última copia guardada.
    if (isEditableContent(event.request.url)) {
        const cache = await caches.open(contentCacheName);
        try {
            const response = await fetch(event.request, { cache: 'no-cache' });
            if (response.ok) {
                cache.put(event.request, response.clone());
            }
            return response;
        } catch {
            const cached = await cache.match(event.request);
            if (cached) return cached;
            throw new Error('Sin conexión y sin copia guardada de ' + event.request.url);
        }
    }

    // Aplicación: caché primero. Las navegaciones sirven index.html.
    const shouldServeIndexHtml = event.request.mode === 'navigate'
        && !manifestUrlList.some(url => url === event.request.url);

    const request = shouldServeIndexHtml ? 'index.html' : event.request;
    const cache = await caches.open(cacheName);
    const cachedResponse = await cache.match(request);
    return cachedResponse || fetch(event.request);
}
