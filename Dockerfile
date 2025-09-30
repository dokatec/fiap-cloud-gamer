# Etapa de Build - Compila a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Fcg.Api/Fcg.Api.csproj", "Fcg.Api/"]
COPY ["Fcg.Application/Fcg.Application.csproj", "Fcg.Application/"]
COPY ["Fcg.Domain/Fcg.Domain.csproj", "Fcg.Domain/"]
COPY ["Fcg.Infrastructure/Fcg.Infrastructure.csproj", "Fcg.Infrastructure/"]
RUN dotnet restore "Fcg.Api/Fcg.Api.csproj"
COPY . .

WORKDIR "/src/Fcg.Api"
RUN dotnet build "Fcg.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fcg.Api.csproj" -c Release -o /app/publish

# Etapa Final - Configura o ambiente de execução com a aplicação e o banco de dados
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

# Instala dependências do SO e do dotnet
USER root
RUN apt-get update && apt-get install -y postgresql postgresql-client && rm -rf /var/lib/apt/lists/*
RUN dotnet tool install --global dotnet-ef

# Copia o código fonte para um diretório separado para uso do 'dotnet ef'
WORKDIR /source
COPY --from=build /src .

# Define o diretório de trabalho principal e copia a aplicação compilada
WORKDIR /app
COPY --from=publish /app/publish .

# Copia e dá permissão de execução para o script de entrypoint
COPY entrypoint.sh .
RUN chmod +x ./entrypoint.sh

# Expõe a porta da aplicação
EXPOSE 5000

# Define o script como ponto de entrada do contêiner
ENTRYPOINT ["tail", "-f", "/dev/null"]
