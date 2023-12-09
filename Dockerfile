
# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj file to the container
COPY *.csproj ./

# Restore dependencies
RUN dotnet restore

# Copy the rest of the application code
COPY . ./

# Publish the application
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory in the container
WORKDIR /app

# Copy the published output from build-env to the final image
COPY --from=build-env /app/out .

# Expose the port that the application will run on
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "PlatformService.dll"]
