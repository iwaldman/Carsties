FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-net70
WORKDIR /app
EXPOSE 80

# copy all .csproj files and restore as distinct layers.   Use of the same COPY command
# for every dockerfile in the project to take advantage of docker caching
COPY Carsties.sln Carsties.sln
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj

# Restore package deps
RUN dotnet restore src/IdentityService/IdentityService.csproj

# Copy the app folders over
COPY src/IdentityService src/IdentityService
WORKDIR /app/src/IdentityService
RUN dotnet publish -c Release -o /app/src/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime-net70
WORKDIR /app
COPY --from=build-net70 /app/src/out ./
ENTRYPOINT ["dotnet", "IdentityService.dll"]
