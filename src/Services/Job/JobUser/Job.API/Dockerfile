FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR "./src/Services/Job/JobUser/Job.API"
RUN dotnet restore "Job.API.csproj"
RUN dotnet build "Job.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Job.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Job.API.dll"]
