# Étape 1 : build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# ✅ Installer dotnet-ef dans l'image
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY . ./
RUN dotnet publish backend.csproj -c Release -o /out

# Étape 2 : runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out .

# 👇 Déclare que l'app écoute sur le port 5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "backend.dll"]

# # Dockerfile (temporairement pour les migrations)
# FROM mcr.microsoft.com/dotnet/sdk:8.0

# WORKDIR /app

# # ✅ Installer dotnet-ef globalement
# RUN dotnet tool install --global dotnet-ef
# ENV PATH="$PATH:/root/.dotnet/tools"

# COPY . ./
# RUN dotnet restore
