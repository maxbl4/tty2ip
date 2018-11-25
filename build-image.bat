dotnet publish -o bin/_build -c Release
docker build --pull --platform armhf -t maxbl4/tty2ip:arm .
docker push maxbl4/tty2ip:arm