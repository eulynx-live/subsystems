FROM mcr.microsoft.com/dotnet/aspnet:7.0

COPY src/Point/bin/Release/net7.0/publish/ app/

WORKDIR /app

ENTRYPOINT ["dotnet", "Point.dll"]
