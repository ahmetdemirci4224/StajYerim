@echo off
echo ==========================================
echo StajYerim Projesi Baslatiliyor...
echo ==========================================
echo.

echo [1/3] Veritabani (PostgreSQL) baslatiliyor...
cd Backend
start "PostgreSQL Server" /MIN cmd /c ".\pgsql\pgsql\bin\postgres.exe -D .\pg_data"
cd ..
timeout /t 3 /nobreak > nul

echo [2/3] Backend (.NET API) baslatiliyor...
cd Backend
start "StajYerim Backend API" cmd /k "set ASPNETCORE_ENVIRONMENT=Development&& dotnet run --project StajYerim.API --urls http://localhost:5249"
cd ..
timeout /t 2 /nobreak > nul

echo [3/3] Frontend (React/Vite) baslatiliyor...
cd frontend
start "StajYerim Frontend" cmd /k "npm run dev -- --host"
cd ..

echo.
echo ==========================================
echo Sistem basariyla baslatildi!
echo.
echo Frontend Arayuzu: http://localhost:5173
echo Backend Swagger: http://localhost:5249/swagger
echo.
echo Kapatmak isterseniz acilan siyah pencereleri (CMD)
echo carpidan kapatmaniz yeterlidir.
echo ==========================================
pause