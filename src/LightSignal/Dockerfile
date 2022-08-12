FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN apt-get update \
&& apt-get install -y --no-install-recommends curl \
&& rm -rf /etc/apt/lists.d

RUN curl -sSL https://bit.ly/install-xq | bash

COPY bin/Release/net6.0/publish/ app/

WORKDIR /app

COPY entrypoint.sh .

ENTRYPOINT ["./entrypoint.sh"]
