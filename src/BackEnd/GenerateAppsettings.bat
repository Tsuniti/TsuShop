@echo off
setlocal enabledelayedexpansion

REM Укажите путь и имя файла для создания
set jsonFile=appsettings.Development.json

REM Функция генерации случайного GUID-строкоподобного значения
set "SecretKey="
for /L %%i in (1,1,32) do (
    set /a rnd=!random!%%16
    set "SecretKey=!SecretKey!!rnd!"
)

REM Формирование JSON содержимого
(
echo {
echo   "Logging": {
echo     "LogLevel": {
echo       "Default": "Information",
echo       "Microsoft.AspNetCore": "Warning"
echo     }
echo   },
echo   "Jwt": {
echo     "SecretKey": "!SecretKey!",
echo     "Issuer": "FirstTodoWebApi",
echo     "Audience": "FirstTodoWebApiClient",
echo     "TtlMinutes": 5
echo   },
echo   "Database": {
echo     "ConnectionString": "Data source=users_todos.db"
echo   },
echo   "FirstUser": {
echo     "Username": "admin",
echo     "Password": "admin"
echo   }
echo }
) > "%jsonFile%"

echo JSON file "%jsonFile%" has been created successfully with a random SecretKey.
pause
