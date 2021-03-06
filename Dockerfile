FROM microsoft/dotnet:2.1-sdk
COPY lib/armhf/* /usr/lib/
COPY bin/_build /app
WORKDIR /app
ENTRYPOINT ["dotnet", "tty2ip.dll"]