# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj e restaurar
COPY *.csproj ./
RUN dotnet restore

# Copiar todo o conteúdo e compilar
COPY . ./
RUN dotnet publish -c Release -o /out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Expõe a porta HTTP
EXPOSE 80

# Inicia a app
ENTRYPOINT ["dotnet", "otw.chatbot.lecafc.api.dll"]