FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /artist-awards-dotnet

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /artist-awards-dotnet
COPY --from=build-env /artist-awards-dotnet/out .
ENTRYPOINT ["dotnet", "ArtistAwards.dll"]