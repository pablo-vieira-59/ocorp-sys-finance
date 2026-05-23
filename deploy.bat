@echo off

echo =========================
echo Atualizando repositorio
echo =========================
git checkout main
git pull

echo =========================
echo Buildando imagem Docker
echo =========================

docker build -t sys-finance-app .

echo =========================
echo Removendo container antigo
echo =========================

docker rm -f sys-finance-app

echo =========================
echo Subindo novo container
echo =========================

docker run -d ^
-p 8080:8080 ^
-v "C:/Secrets/SysFinance/appsettings.Production.json:/app/appsettings.Production.json" ^
--name sys-finance-app ^
--restart unless-stopped ^
sys-finance-app

echo =========================
echo Deploy finalizado
echo =========================

pause
