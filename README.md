PokemonApp

Prueba técnica · Aplicación web ASP.NET Core 8 (MVC) que consume PokeAPI

.NETC#ExcelEmail
Contenido

    Funcionalidades
    Stack técnico
    Puesta en marcha
    Configuración SMTP
    Decisiones técnicas
    Supuestos
    Estructura del proyecto
    Posibles mejoras

Funcionalidades

    Listado paginado — grid con imagen y nombre, paginación manual (Skip/Take)
    Filtros — búsqueda por nombre y dropdown por especie
    Detalle en modal — descripción de la especie vía /pokemon-species/{id}
    Exportar a Excel — .xlsx de PokemonApp — Prueba técnica

Aplicación web ASP.NET Core 8 (MVC) que consume PokeAPI.
Funcionalidades

    Listado paginado de Pokémon en grid con imagen, nombre y tipos
    Filtros por nombre (texto) y por especie (dropdown)
    Detalle en modal, con descripción de la especie (/pokemon-species/{id})
    Exportación a Excel de la página actualmente visible
    Envío de correo con el Excel adjunto (individual o general)

Requisitos

    .NET SDK 8.0 (fijado vía global.json)
    Acceso a internet (consume PokeAPI en vivo)

Ejecución

dotnet run --project src/PokemonApp.Web

Configuración de correo (SMTP)PokemonApp — Prueba técnica

Aplicación web ASP.NET Core 8 (MVC) que consume PokeAPI.
Funcionalidades

    Listado paginado de Pokémon en grid con imagen, nombre y tipos
    Filtros por nombre (texto) y por especie (dropdown)
    Detalle en modal, con descripción de la especie (/pokemon-species/{id})
    Exportación a Excel de la página actualmente visible
    Envío de correo con el Excel adjunto (individual o general)

Requisitos

    .NET SDK 8.0 (fijado vía global.json)
    Acceso a internet (consume PokeAPI en vivo)

Ejecución

dotnet run --project src/PokemonApp.Web

Configuración de correo (SMTP)

Las credenciales no van en appsettings.json; se configuran con dotnet user-secrets:

cd src/PokemonApp.Web                                                                                                        
dotnet user-secrets init
dotnet user-secrets set "SmtpSettings:Host" "TuHost.com"
dotnet user-secrets set "SmtpSettings:Port" "Puerto"
dotnet user-secrets set "SmtpSettings:Username" "Usuario"
dotnet user-secrets set "SmtpSettings:Password" "Contraseña"

Probado con Mailtrap como sandbox SMTP.
Decisiones técnicas
Área	Elección	Justificación
Cache	IMemoryCache (in-process), expiración 6h	Cachea listado, especies y detalle; reduce latencia y llamadas a PokeAPI. Para este alcance no se justifica Redis.
Excel	ClosedXML	Licencia MIT. EPPlus v5+ cambió a licencia comercial (Polyform Noncommercial).
Correo	MailKit	Biblioteca recomendada por Microsoft para SMTP en .NET. Config vía IOptions<SmtpSettings> + user-secrets.
Timeouts/errores	HttpClient.Timeout (30s) + manejo explícito de HttpRequestException/TaskCanceledException	Sin dependencias extra (Polly): para este alcance es más simple y suficiente. Logging con ILogger (Warning = fallo externo esperable, Error = inesperado).
Paginación	Manual con Skip/Take	PokeAPI entrega el listado completo; se pagina en memoria.
Export/envío	Solo la página visible

Las credenciales no van en appsettings.json; se configuran con dotnet user-secrets:

cd src/PokemonApp.Web                                                                                                        
dotnet user-secrets init
dotnet user-secrets set "SmtpSettings:Host" "TuHost.com"
dotnet user-secrets set "SmtpSettings:Port" "Puerto"
dotnet user-secrets set "SmtpSettings:Username" "Usuario"
dotnet user-secrets set "SmtpSettings:Password" "Contraseña"

Probado con Mailtrap como sandbox SMTP.
Decisiones técnicas
Área	Elección	Justificación
Cache	IMemoryCache (in-process), expiración 6h	Cachea listado, especies y detalle; reduce latencia y llamadas a PokeAPI. Para este alcance no se justifica Redis.
Excel	ClosedXML	Licencia MIT. EPPlus v5+ cambió a licencia comercial (Polyform Noncommercial).
Correo	MailKit	Biblioteca recomendada por Microsoft para SMTP en .NET. Config vía IOptions<SmtpSettings> + user-secrets.
Timeouts/errores	HttpClient.Timeout (30s) + manejo explícito de HttpRequestException/TaskCanceledException	Sin dependencias extra (Polly): para este alcance es más simple y suficiente. Logging con ILogger (Warning = fallo externo esperable, Error = inesperado).
Paginación	Manual con Skip/Take	PokeAPI entrega el listado completo; se pagina en memoria.
Export/envío	Solo la página visiblela página visible (ClosedXML)
    Envío de correo — con el Excel adjunto, individual o general (MailKit + SMTP)
    Cache en memoria — listado, especies y detalle con expiración de 6 h

Stack técnico
Área	Tecnología
Framework	ASP.NET Core 8 (MVC)
API	PokeAPI vía HttpClient tipado
Cache	IMemoryCache (in-process)
Excel	ClosedXML — licencia MIT
Correo	MailKit
Secretos	dotnet user-secrets
Puesta en marcha

Requisitos

    .NET SDK 8.0 (fijado por global.json)
    Conexión a internet (la app consume PokeAPI en vivo)

git clone https://github.com/<usuario>/PokemonApp.gitcd PokemonAppdotnet run --project src/PokemonApp.Web

Abre la URL que muestra la consola (por defecto https://localhost:7xxx).
Configuración SMTP

    [!IMPORTANT]Las credenciales nunca van en appsettings.json. Se registran como secretosde usuario, que viven fuera del repositorio.

cd src/PokemonApp.Webdotnet user-secrets initdotnet user-secrets set "SmtpSettings:Host"     "sandbox.smtp.mailtrap.io"dotnet user-secrets set "SmtpSettings:Port"     "587"dotnet user-secrets set "SmtpSettings:User"     "<tu-usuario>"dotnet user-secrets set "SmtpSettings:Password" "<tu-password>"dotnet user-secrets set "SmtpSettings:From"     "no-reply@ejemplo.com"

Envíos probados con Mailtrap como sandbox SMTP.
Decisiones técnicas
Cache en memoria (IMemoryCache)

    El listado completo, las especies y el detalle de cada Pokémon se cachean con expiración de 6 h.
    Reduce latencia y llamadas a PokeAPI al navegar y paginar.
    Al ser una app de una sola instancia, no se justifica un cache distribuido (Redis).

Excel: ClosedXML (en vez de EPPlus)

    ClosedXML usa licencia MIT, sin restricciones de uso.
    EPPlus v5+ cambió a licencia comercial (Polyform Noncommercial): exigiría comprar licencia para uso empresarial.

Correo: MailKit

    Biblioteca SMTP recomendada por Microsoft para .NET.
    Configuración tipada vía IOptions<SmtpSettings> y credenciales en dotnet user-secrets.

Errores y timeouts

    HttpClient.Timeout de 30 s, configurado en un único lugar (Program.cs).
    Captura explícita de HttpRequestException (fallo de red) y TaskCanceledException con TimeoutException interna (timeout desde .NET 5).
    Logging con ILogger: Warning para fallos externos esperables, Error para lo inesperado. El usuario nunca ve el texto de la excepción.
    Sin Polly ni reintentos: para este alcance, try/catch + timeout es suficiente y evita dependencias adicionales.

Paginación manual

    PokeAPI devuelve el listado completo; se pagina en memoria con Skip/Take propios, sin componentes de grid automáticos.

Supuestos

    Filtro por especie: compara por nombre exacto, asumiendo correspondencia 1:1 entre el nombre del Pokémon y el de su especie. PokeAPI no garantiza esto para formas alternas (p. ej., las formas de Deoxys comparten especie).
    Exportar y enviar correo: operan sobre la página visible en el momento de la acción, no sobre el resultado filtrado completo (interpretación literal del enunciado).