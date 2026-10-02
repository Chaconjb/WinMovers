<#
.SINOPSIS
    Deja el proyecto WinMovers listo para probar en una máquina nueva.

.DESCRIPCIÓN
    Hace, en orden:
      1. Verifica el SDK de .NET, la herramienta dotnet-ef y sqlcmd.
      2. Guarda tu cadena de conexión en user-secrets, NO en
         appsettings.json. Así cada quien apunta a su propio SQL Server
         sin pisarse entre sí ni ensuciar el repositorio.
      3. Compila el proyecto.
      4. Crea la base de datos y aplica las migraciones de EF Core.
      5. Carga los datos de demostración (opcional).
      6. Verifica que todo haya quedado en su lugar.

.PARAMETER Servidor
    Instancia de SQL Server. Ejemplos:
        .\SQLEXPRESS
        MI-PC\MSSQLSERVER01
        localhost

.PARAMETER SinDatos
    Omite la carga de datos de demostración.

.PARAMETER Ejecutar
    Levanta la aplicación al terminar.

.EJEMPLO
    .\preparar-entorno.ps1 -Servidor ".\SQLEXPRESS"
    .\preparar-entorno.ps1 -Servidor "MI-PC\SQLEXPRESS" -Ejecutar
    .\preparar-entorno.ps1 -Servidor "localhost" -SinDatos
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, HelpMessage = "Instancia de SQL Server, p. ej. .\SQLEXPRESS")]
    [string]$Servidor,

    [string]$BaseDatos = "WinMoversDB",

    [switch]$SinDatos,

    [switch]$Ejecutar
)

# 'Continue' y no 'Stop': en Windows PowerShell 5.1, capturar la salida de
# un .exe hace que cada línea de stderr se convierta en un ErrorRecord
# (NativeCommandError). Con 'Stop' el script abortaría por un simple
# warning de NuGet. El control de errores se hace con $LASTEXITCODE.
$ErrorActionPreference = "Continue"
$raiz = $PSScriptRoot
$proyecto = Join-Path $raiz "WinMovers.csproj"
$scriptDatos = Join-Path $raiz "Database\datos_demo.sql"

function Paso  ($n, $t) { Write-Host "`n[$n] $t" -ForegroundColor Cyan }
function Ok    ($t)     { Write-Host "    OK  $t" -ForegroundColor Green }
function Aviso ($t)     { Write-Host "    !   $t" -ForegroundColor Yellow }
function Malo  ($t)     { Write-Host "    X   $t" -ForegroundColor Red }

Write-Host "======================================================" -ForegroundColor White
Write-Host " WinMovers - preparación del entorno de pruebas" -ForegroundColor White
Write-Host "======================================================" -ForegroundColor White
Write-Host " Servidor : $Servidor"
Write-Host " Base     : $BaseDatos"

# ---------------------------------------------------------------------
Paso 1 "Verificando herramientas"
# ---------------------------------------------------------------------
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Malo "No se encontró 'dotnet'. Instalá el SDK de .NET 9 desde https://dotnet.microsoft.com/download"
    exit 1
}
Ok "SDK de .NET $(dotnet --version)"

if (-not (Test-Path $proyecto)) {
    Malo "No se encontró WinMovers.csproj. Ejecutá este script desde la raíz del repositorio."
    exit 1
}
Ok "Proyecto encontrado"

# dotnet-ef es una herramienta aparte del SDK; sin ella no hay migraciones.
$hayEf = $false
try { dotnet ef --version *> $null; $hayEf = $? } catch { $hayEf = $false }

if (-not $hayEf) {
    Aviso "dotnet-ef no está instalado. Instalando..."
    dotnet tool install --global dotnet-ef
    if (-not $?) {
        Malo "No se pudo instalar dotnet-ef. Probá manualmente: dotnet tool install --global dotnet-ef"
        exit 1
    }
    Aviso "Instalado. Si el siguiente paso falla, cerrá y abrí la terminal para refrescar el PATH."
}
Ok "dotnet-ef disponible"

$haySqlcmd = [bool](Get-Command sqlcmd -ErrorAction SilentlyContinue)
if (-not $haySqlcmd -and -not $SinDatos) {
    Aviso "No se encontró 'sqlcmd'; no se podrán cargar los datos de demostración."
    Aviso "Viene con SQL Server Command Line Utilities o con SSMS."
    Aviso "El resto del script continúa igual."
} elseif ($haySqlcmd) {
    Ok "sqlcmd disponible"
}

# ---------------------------------------------------------------------
Paso 2 "Guardando la cadena de conexión en user-secrets"
# ---------------------------------------------------------------------
# Va en user-secrets y no en appsettings.json a propósito: el archivo
# versionado trae el servidor de otra persona, y editarlo genera
# conflictos cada vez que alguien más cambia el suyo. user-secrets vive
# fuera del repositorio y tiene prioridad sobre appsettings.json cuando
# ASPNETCORE_ENVIRONMENT es Development.
$cadena = "Server=$Servidor;Database=$BaseDatos;Trusted_Connection=True;TrustServerCertificate=True"

dotnet user-secrets set "ConnectionStrings:DefaultConnection" $cadena --project $proyecto *> $null
if (-not $?) {
    Malo "No se pudo guardar la cadena de conexión."
    exit 1
}
Ok "Cadena guardada: $cadena"
Aviso "appsettings.json NO se modifica: no lo commitees con tu servidor."

# ---------------------------------------------------------------------
Paso 3 "Compilando"
# ---------------------------------------------------------------------
$salidaBuild = dotnet build $proyecto -v q --nologo
if ($LASTEXITCODE -ne 0) {
    Malo "La compilación falló:"
    $salidaBuild | Select-String -Pattern "error" | Select-Object -First 10
    exit 1
}
Ok "Compilación correcta"

# ---------------------------------------------------------------------
Paso 4 "Creando la base y aplicando migraciones"
# ---------------------------------------------------------------------
# 'database update' crea la base si no existe y aplica sólo lo pendiente,
# así que es seguro correrlo sobre una base ya existente.
$env:ASPNETCORE_ENVIRONMENT = "Development"
$salidaEf = dotnet ef database update --project $proyecto --no-build
if ($LASTEXITCODE -ne 0) {
    Malo "Falló la actualización de la base de datos:"
    $salidaEf | Select-Object -Last 15
    Write-Host ""
    Aviso "Causas típicas:"
    Aviso "  - El nombre de la instancia es incorrecto. Verificá con: sqlcmd -S $Servidor -E -C -Q 'SELECT @@SERVERNAME'"
    Aviso "  - El servicio de SQL Server está detenido."
    Aviso "  - Tu usuario de Windows no tiene permiso para crear bases."
    exit 1
}
$migraciones = ($salidaEf | Select-String -Pattern "Applying migration").Count
if ($migraciones -gt 0) { Ok "$migraciones migración(es) aplicada(s)" } else { Ok "La base ya estaba al día" }

# ---------------------------------------------------------------------
Paso 5 "Cargando datos de demostración"
# ---------------------------------------------------------------------
if ($SinDatos) {
    Aviso "Omitido por -SinDatos"
}
elseif (-not $haySqlcmd) {
    Aviso "Omitido: sqlcmd no está disponible"
}
elseif (-not (Test-Path $scriptDatos)) {
    Aviso "Omitido: no se encontró Database\datos_demo.sql"
}
else {
    # -f 65001 NO es opcional: el archivo está en UTF-8 y sqlcmd por
    # defecto lo lee con la página ANSI de Windows, lo que guardaría
    # "España" como "EspaÃ±a" dentro de la base.
    $salidaDatos = sqlcmd -S $Servidor -d $BaseDatos -E -C -b -I -f 65001 -i $scriptDatos
    if ($LASTEXITCODE -ne 0) {
        Malo "Falló la carga de datos:"
        $salidaDatos | Select-Object -Last 10
        exit 1
    }
    Ok "Datos de demostración cargados"
    $salidaDatos | Select-Object -Last 20 | ForEach-Object { "      $_" }
}

# ---------------------------------------------------------------------
Paso 6 "Verificando"
# ---------------------------------------------------------------------
if ($haySqlcmd) {
    $consulta = @'
SET NOCOUNT ON;
SELECT CASE WHEN COUNT(*) >= 5 THEN 'OK' ELSE 'FALTAN' END + ' migraciones (' + CAST(COUNT(*) AS VARCHAR) + ')'
FROM __EFMigrationsHistory;
SELECT CASE WHEN COUNT(*) > 0 THEN 'OK' ELSE 'VACIO' END + ' catalogo de documentos (' + CAST(COUNT(*) AS VARCHAR) + ')'
FROM Catalogo_Documentos;
SELECT CASE WHEN COUNT(*) = 0 THEN 'OK' ELSE 'HAY MOJIBAKE' END + ' acentos'
FROM Clientes WHERE nombre_cliente LIKE '%Ã%';
'@
    $r = sqlcmd -S $Servidor -d $BaseDatos -E -C -h -1 -W -I -Q $consulta
    $r | Where-Object { $_ -match '\S' } | ForEach-Object {
        if ($_ -match '^OK') { Ok $_ } else { Aviso $_ }
    }
}

Write-Host "`n======================================================" -ForegroundColor White
Write-Host " Listo" -ForegroundColor Green
Write-Host "======================================================" -ForegroundColor White
Write-Host ""
Write-Host " Iniciar la aplicación:   dotnet run"
Write-Host " Abrir en el navegador:   http://localhost:5164"
Write-Host ""
Write-Host " Usuario de prueba:       admin@winmovers.com"
Write-Host " Contraseña:              Admin123!"
Write-Host ""
Write-Host " Qué revisar:"
Write-Host "   /                          panel con contadores"
Write-Host "   /Dashboards/Estadisticas   cajas y kilos, cotizaciones por tipo"
Write-Host "   /Dashboards/Analitica      series mensuales y top de países"
Write-Host "   /Dashboards/Pronostico     proyección a 3 meses (ML.NET)"
Write-Host "   /OrdenTrabajo              16 órdenes; los iconos de cada fila"
Write-Host "                              abren materiales, bienes y lista de embalaje"
Write-Host "   /Inventario                12 materiales, uno bajo el mínimo"
Write-Host "   /Cotizacion                16 cotizaciones en los 5 estados"
Write-Host ""

if ($Ejecutar) {
    Write-Host " Iniciando la aplicación (Ctrl+C para detener)..." -ForegroundColor Cyan
    Write-Host ""
    dotnet run --project $proyecto --no-build
}
