# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Get the source code from GitHub
RUN git clone https://github.com/Soul-food-code/soulFoodReport.git .

# Restore dependencies
RUN dotnet restore soulFoodReport.csproj 

# Build the application
RUN dotnet publish soulFoodReport.csproj -c Release -o /tmp/out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app
COPY --from=build /tmp/out .
ENTRYPOINT ["dotnet", "soulFoodReport.dll"]
