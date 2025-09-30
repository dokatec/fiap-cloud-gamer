# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all the project files
COPY Fcg.Api.sln .
COPY Fcg.Api/Fcg.Api.csproj Fcg.Api/
COPY Fcg.Application/Fcg.Application.csproj Fcg.Application/
COPY Fcg.Domain/Fcg.Domain.csproj Fcg.Domain/
COPY Fcg.Infrastructure/Fcg.Infrastructure.csproj Fcg.Infrastructure/
# Note: Tests are not needed for the final image, but restore might need the project file.
COPY Fcg.Tests/Fcg.Tests.csproj Fcg.Tests/

# Restore dependencies
RUN dotnet restore Fcg.Api.sln

# Copy the rest of the source code
COPY . .

# Publish the API project
WORKDIR /src/Fcg.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the app will listen on
EXPOSE 5000

# Define environment variables
ENV ASPNETCORE_URLS=http://+:5000

# Set the entrypoint for the container
ENTRYPOINT ["dotnet", "Fcg.Api.dll"]
