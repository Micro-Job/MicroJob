FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Services/Email"
RUN dotnet restore "EmailService.API.csproj"
RUN dotnet build "EmailService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmailService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmailService.API.dll"]
