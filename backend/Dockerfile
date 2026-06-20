FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/TicketSystem.Api/TicketSystem.Api.csproj", "src/TicketSystem.Api/"]
COPY ["src/TicketSystem.Application/TicketSystem.Application.csproj", "src/TicketSystem.Application/"]
COPY ["src/TicketSystem.Domain/TicketSystem.Domain.csproj", "src/TicketSystem.Domain/"]
COPY ["src/TicketSystem.Infrastructure/TicketSystem.Infrastructure.csproj", "src/TicketSystem.Infrastructure/"]
RUN dotnet restore "src/TicketSystem.Api/TicketSystem.Api.csproj"

COPY . .
RUN dotnet publish "src/TicketSystem.Api/TicketSystem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TicketSystem.Api.dll"]
