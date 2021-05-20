#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TransferWorker/TransferWorker.csproj", "TransferWorker/"]
COPY ["../../NetCore/NetCore3/Netcore.Utils/Netcore.Utils/NetCore.Utils.csproj", "../../NetCore/NetCore3/Netcore.Utils/Netcore.Utils/"]
RUN dotnet restore "TransferWorker/TransferWorker.csproj"
COPY . .
WORKDIR "/src/TransferWorker"
RUN dotnet  build "TransferWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransferWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransferWorker.dll"]