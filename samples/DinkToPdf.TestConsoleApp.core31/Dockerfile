#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
RUN apt-get update && apt-get install -y -q --no-install-recommends \
    wkhtmltopdf \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["samples/DinkToPdf.TestConsoleApp.x64.core31/DinkToPdf.TestConsoleApp.x64.core31.csproj", "samples/DinkToPdf.TestConsoleApp.x64.core31/"]
RUN dotnet restore "samples/DinkToPdf.TestConsoleApp.x64.core31/DinkToPdf.TestConsoleApp.x64.core31.csproj"
COPY . .
WORKDIR "/src/samples/DinkToPdf.TestConsoleApp.x64.core31"
RUN dotnet build "DinkToPdf.TestConsoleApp.x64.core31.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DinkToPdf.TestConsoleApp.x64.core31.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DinkToPdf.TestConsoleApp.x64.core31.dll"]