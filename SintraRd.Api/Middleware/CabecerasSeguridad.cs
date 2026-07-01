// DEVGEP+ | Guillermo Estefan Puello
// Middleware de cabeceras de seguridad HTTP (equivalente a Helmet en Node.js).
// Se agrega a todas las respuestas antes de que lleguen al controlador,
// reduciendo la superficie de ataque en el navegador del cliente.

namespace SintraRd.Api.Middleware;

public class CabecerasSeguridad
{
    private readonly RequestDelegate _siguiente;

    public CabecerasSeguridad(RequestDelegate siguiente)
    {
        _siguiente = siguiente;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        var cabeceras = contexto.Response.Headers;

        // Impide que el navegador infiera el tipo MIME de la respuesta (MIME sniffing)
        cabeceras["X-Content-Type-Options"] = "nosniff";

        // Bloquea la carga de la pagina dentro de un iframe (previene clickjacking)
        cabeceras["X-Frame-Options"] = "DENY";

        // Desactiva el filtro XSS del navegador; en 0 evita vulnerabilidades conocidas en IE/Edge legacy
        cabeceras["X-XSS-Protection"] = "0";

        // Limita la informacion de origen que se envia al navegar a otros dominios
        cabeceras["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Restringe los recursos que puede cargar esta API a su propio origen
        cabeceras["Content-Security-Policy"] = "default-src 'self'";

        // Deshabilita el acceso del navegador a APIs de hardware sensibles
        cabeceras["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

        // Las respuestas de la API no deben almacenarse en cache de proxies ni navegadores
        cabeceras["Cache-Control"] = "no-store";

        await _siguiente(contexto);
    }
}
