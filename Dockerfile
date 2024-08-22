# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage
WORKDIR /src

# Copy  everything else and build
COPY soulFoodReport.csproj /src/
COPY . /src/
RUN dotnet restore soulFoodReport.csproj

RUN dotnet publish soulFoodReport.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-stage
COPY --from=build-stage /app /app
COPY soulFoodReport.csproj /app/
COPY ./Properties /app/Properties

EXPOSE 8080

# Runtime image
WORKDIR /app

RUN echo '\n\ 
dotnet soulFoodReport.dll' > /app/start.sh
RUN chmod a+x /app/start.sh
CMD "./start.sh"
