FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5016

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR "./src/Services/JobPayment/JobPayment.API"
RUN dotnet restore "JobPayment.API.csproj"
RUN dotnet build "JobPayment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobPayment.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobPayment.API.dll"]
