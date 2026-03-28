FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the order-worker service and the RabbitClientEasyNetQ library source
COPY ./src/services/order-worker ./services/order-worker
COPY ./src/RabbitClientEasyNetQ ./RabbitClientEasyNetQ

WORKDIR /src/services/order-worker
RUN dotnet restore "order-worker.csproj"
RUN dotnet publish "order-worker.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Order.Worker.dll"]
