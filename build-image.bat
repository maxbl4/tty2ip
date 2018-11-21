dotnet publish -o bin/_build
docker build --pull --platform armhf -t maxbl4/tty2ip:arm .
docker push maxbl4/tty2ip:arm