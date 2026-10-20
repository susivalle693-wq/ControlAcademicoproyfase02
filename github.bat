@echo off
cd /d "%~dp0"
set "REPO=https://github.com/susivalle693-wq/ControlAcademioproyfase02.git"
set "TMP=%TEMP%\ca_subir"

if not exist ControlAcademico.sln goto fin

if exist "%TMP%" rmdir /s /q "%TMP%"
git clone %REPO% "%TMP%"
if errorlevel 1 goto fin

robocopy "%~dp0." "%TMP%" /E /XD bin obj .vs .git /XF *.user appsettings.Development.json subir*.bat >nul
if errorlevel 8 goto fin

cd /d "%TMP%"
git add ControlAcademico.sln .gitignore src\ControlAcademico.Entidades src\ControlAcademico.Datos
git commit -m "feat: estructura en capas, entidades y migraciones"
git add src\ControlAcademico.Negocio
git commit -m "feat: servicios de negocio con validaciones"
git add src\ControlAcademico.Web
git commit -m "feat: controladores, vistas y servicio web con Swagger"
git add database docs wireframes src\ControlAcademico.Consola
git commit -m "docs: scripts SQL, diagramas y consola de la Fase 1"
git add -A
git commit -m "docs: README de la Fase 2"
git push origin HEAD

:fin
pause
