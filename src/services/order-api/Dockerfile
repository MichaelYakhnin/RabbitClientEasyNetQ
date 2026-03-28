FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the order-api service and the RabbitClientEasyNetQ library source
COPY ./src/services/order-api ./services/order-api
COPY ./src/RabbitClientEasyNetQ ./RabbitClientEasyNetQ

WORKDIR /src/services/order-api
RUN dotnet restore "order-api.csproj"
RUN dotnet publish "order-api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Order.Api.dll"]
