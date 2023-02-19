FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY src/LevelCrossing/bin/Release/net6.0/publish/ app/

WORKDIR /app

ENTRYPOINT ["dotnet", "LevelCrossing.dll"]
