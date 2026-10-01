FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o ficheiro do projeto e restaura dependências
COPY ["BilheticaAeronauticaWeb/BilheticaAeronauticaWeb.csproj", "BilheticaAeronauticaWeb/"]
RUN dotnet restore "BilheticaAeronauticaWeb/BilheticaAeronauticaWeb.csproj"

# Copia todo o código e compila a aplicação
COPY . .
WORKDIR "/src/BilheticaAeronauticaWeb"

RUN dotnet build "BilheticaAeronauticaWeb.csproj" -c Release -o /app/build

RUN dotnet publish "BilheticaAeronauticaWeb.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Imagem final de execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BilheticaAeronauticaWeb.dll"]