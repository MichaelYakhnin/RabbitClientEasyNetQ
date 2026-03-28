Multi-service demo: order-api (Minimal Web API) + order-worker (subscriber) using RabbitMQ and RabbitClientEasyNetQ library.

Run with docker-compose:

```bash
docker-compose up --build
```

POST an order:

```bash
curl -X POST http://localhost:5000/orders -H "Content-Type: application/json" -d '{"CustomerId":"CUST-1","Items":[{"ProductName":"Widget","Quantity":2,"UnitPrice":9.95}] }'
```

Verify worker logs show received message.

Files added:
- [`src/RabbitClientEasyNetQ/Contracts/OrderCreated.cs`](src/RabbitClientEasyNetQ/Contracts/OrderCreated.cs:1)
- [`services/order-api/Program.cs`](services/order-api/Program.cs:1)
- [`services/order-api/order-api.csproj`](services/order-api/order-api.csproj:1)
- [`services/order-api/Dockerfile`](services/order-api/Dockerfile:1)
- [`services/order-worker/Program.cs`](services/order-worker/Program.cs:1)
- [`services/order-worker/order-worker.csproj`](services/order-worker/order-worker.csproj:1)
- [`services/order-worker/Dockerfile`](services/order-worker/Dockerfile:1)
- [`docker-compose.yml`](docker-compose.yml:1)
