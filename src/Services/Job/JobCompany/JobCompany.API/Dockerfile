FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR "./src/Services/Job/JobCompany/JobCompany.API"
RUN dotnet restore "JobCompany.API.csproj"
RUN dotnet build "JobCompany.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobCompany.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobCompany.API.dll"]
