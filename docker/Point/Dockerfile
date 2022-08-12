FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY src/Point/bin/Release/net6.0/publish/ app/

WORKDIR /app

ENTRYPOINT ["dotnet", "Point.dll"]
