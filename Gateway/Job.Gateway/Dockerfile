FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Gateway/Job.Gateway"
RUN dotnet restore "Job.Gateway.csproj"
RUN dotnet build "Job.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Job.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Job.Gateway.dll"]
