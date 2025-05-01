@echo off
setlocal enabledelayedexpansion

:: Project Paths
set MCPROJECTPATH=D:\Self\Github\deneshiqua\Practical.CleanArchitecture\src\Microservices
set IDPROJECTPATH=D:\Self\Github\deneshiqua\Practical.CleanArchitecture\src\IdentityServer

:: Install dotnet-ef cli
:: dotnet tool install --global dotnet-ef --version="8.0"

:: Menü
echo.
echo Hangi migrationi calistirmak istiyorsunuz?
echo --------------------------------------------
echo 1. Configuration.Api
echo 2. Identity.Api
echo 3. IdentityServer
echo 4. Notification.Api
echo 5. Product.Api
echo 6. Storage.Api
echo 7. TUMUNU CALISTIR
echo 0. IPTAL / CIKIS
echo --------------------------------------------
set /p choice="Seciminizi girin (0-7): "

:: Seçim kontrolü
if "%choice%"=="0" (
    echo Islemi iptal ettiniz. Cikis yapiliyor...
    goto End
)

if "%choice%"=="1" goto Configuration
if "%choice%"=="2" goto Identity
if "%choice%"=="3" goto IdentityServer
if "%choice%"=="4" goto Notification
if "%choice%"=="5" goto Product
if "%choice%"=="6" goto Storage
if "%choice%"=="7" (
    call :Configuration
    call :Identity
    call :IdentityServer
    call :Notification
    call :Product
    call :Storage
    goto End
)

:: Hatalı seçim
echo Hatali secim yaptiniz.
goto End

:Configuration
echo [Configuration.Api] Migration Baslatiliyor
cd %MCPROJECTPATH%\Services.Configuration\ClassifiedAds.Services.Configuration.Api || exit /b
dotnet ef migrations add Init --context ConfigurationDbContext -o Migrations/ConfigurationDb
dotnet ef database update --context ConfigurationDbContext
goto :eof

:Identity
echo [Identity.Api] Migration Baslatiliyor
cd %MCPROJECTPATH%\Services.Identity\ClassifiedAds.Services.Identity.Api || exit /b
dotnet ef migrations add Init --context IdentityDbContext -o Migrations/IdentityDb
dotnet ef database update --context IdentityDbContext
goto :eof

:IdentityServer
echo [IdentityServer] Migration Baslatiliyor
:: cd %IDPROJECTPATH%\OpenIddict\ClassifiedAds.Migrator || exit /b
:: dotnet ef migrations add Init --context ConfigurationDbContext -o Migrations/ConfigurationDb
:: dotnet ef migrations add Init --context PersistedGrantDbContext -o Migrations/PersistedGrantDb
:: dotnet ef database update --context ConfigurationDbContext
:: dotnet ef database update --context PersistedGrantDbContext
goto :eof

:Notification
echo [Notification.Api] Migration Baslatiliyor
cd %MCPROJECTPATH%\Services.Notification\ClassifiedAds.Services.Notification.Api || exit /b
dotnet ef migrations add Init --context NotificationDbContext -o Migrations/NotificationDb
dotnet ef database update --context NotificationDbContext
goto :eof

:Product
echo [Product.Api] Migration Baslatiliyor
cd %MCPROJECTPATH%\Services.Product\ClassifiedAds.Services.Product.Api || exit /b
dotnet ef migrations add Init --context ProductDbContext -o Migrations/ProductDb
dotnet ef database update --context ProductDbContext
goto :eof

:Storage
echo [Storage.Api] Migration Baslatiliyor
cd %MCPROJECTPATH%\Services.Storage\ClassifiedAds.Services.Storage.Api || exit /b
dotnet ef migrations add Init --context StorageDbContext -o Migrations/StorageDb
dotnet ef database update --context StorageDbContext
goto :eof

:End
echo.
echo Islemler tamamlandi veya iptal edildi.
pause
