# Feature Specification: RabbitMQ Abstraction

**Feature Branch**: `1-rabbitmq-abstraction`  
**Created**: 2026-03-20  
**Status**: Draft  
**Input**: User description: "Design a production-grade .NET 9 class library that provides a clean abstraction layer over RabbitMQ messaging, implemented internally using EasyNetQ. The goal is to expose a minimal, strongly-typed API focused on Publish and Subscribe, while delegating low-level messaging concerns to EasyNetQ. Core Requirements: Abstraction Layer, EasyNetQ Integration, Configuration, Serialization, Resilience, Message Handling, Dependency Injection, Observability, Extensibility, Thread Safety & Performance. Deliverables include architecture, interfaces, wrapper design, flows, error strategy, examples and project structure." 

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Basic publish/subscribe (Priority: P1)

A library consumer (service developer) needs to publish strongly-typed events and subscribe to typed handlers without depending on EasyNetQ or RabbitMQ types.

**Why this priority**: Core value—enables message-based integration across services with minimal developer friction.

**Independent Test**: Configure the library with valid connection settings, publish a test message of type TestEvent, and verify a subscribed handler receives and processes it asynchronously.

**Acceptance Scenarios**:

1. Given a running RabbitMQ instance and library configured with correct connection details, When service A calls PublishAsync<TestEvent>(msg), Then service B subscribed via SubscribeAsync<TestEvent>(handler) receives the message within a configurable timeout and handler completes successfully.
2. Given a handler that throws an exception, When message processing fails, Then the library logs the failure, applies at-least-once semantics (message redelivery or retry), and does not crash the subscriber process.

---

### User Story 2 - Configuration & DI (Priority: P2)

A developer must register the messaging library with dependency injection and provide configuration via options or connection string.

**Why this priority**: Makes the library easy to adopt and manage in real applications.

**Independent Test**: Register the library via IServiceCollection.AddMessaging and verify that IMessageBus is resolvable and disposes on host shutdown.

**Acceptance Scenarios**:

1. Given a host configured with AddMessaging, When the host starts, Then an internal connection to the broker is established (auto-declared exchanges/queues) and IMessageBus is available via DI.
2. Given the host stops, When shutdown occurs, Then IBus and other resources are disposed gracefully.

---

### User Story 3 - Observability & Correlation (Priority: P3)

A developer needs correlation IDs and basic metrics (publish/consume counters) integrated into logging so that messages can be traced across services.

**Why this priority**: Improves operability and debugging in production.

**Independent Test**: Publish a message with an explicit CorrelationId in metadata; verify it appears in logs produced by the consumer and that publish/consume counters increment.

**Acceptance Scenarios**:

1. Given a published message with correlation metadata, When it is consumed, Then logs from the consumer include the same correlation ID.
2. Given active message traffic, When telemetry hooks are enabled, Then publish and consume counters reflect observed activity.

---

### Edge Cases

- What happens when the broker is temporarily unreachable? The library must surface errors, retry per policy, and recover when the broker returns.
- What happens on schema mismatch (consumer expects different shape)? The library must fail the handler gracefully and route message to retry/dead-letter as configured.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Library MUST expose an interface IMessageBus with at least:
  - Task PublishAsync<T>(T message, CancellationToken ct = default)
  - Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken ct = default)
- **FR-002**: Library MUST NOT expose EasyNetQ types (e.g., IBus) in its public surface; EasyNetQ is an internal implementation detail.
- **FR-003**: Library MUST support configuration via RabbitMqOptions and connection-string style input, and integrate with IOptions<T>.
- **FR-004**: Library MUST allow customization points for serialization via a pluggable IMessageSerializer while remaining compatible with EasyNetQ serialization pipeline.
- **FR-005**: Library MUST leverage EasyNetQ features (auto-declare exchanges/queues, conventions, pub/sub model) internally.
- **FR-006**: Library MUST provide at-least-once delivery semantics; asynchronous handlers are supported and exceptions in handlers are captured and logged without crashing the subscriber.
- **FR-007**: Library MUST provide DI extension IServiceCollection.AddMessaging(Action<RabbitMqOptions> configure) that registers IMessageBus and manages IBus lifecycle and disposal.
- **FR-008**: Library MUST integrate with Microsoft.Extensions.Logging and expose hooks for metrics (publish/consume counters) and correlation ID propagation.
- **FR-009**: Library MUST provide optional resilience controls: retry policy (configurable), and dead-letter queue configuration (opt-in).
- **FR-010**: Library MUST support a simple middleware pipeline for publish and consume flows enabling enrichment and custom behaviors.
- **FR-011**: Library MUST allow advanced subscription options (subscription id, durability) as optional parameters to SubscribeAsync or via a subscription options type.
- **FR-012**: Library MUST be safe for concurrent publishing from multiple threads and respect EasyNetQ connection/channel reuse semantics.

### Key Entities

- **Message (T)**: Arbitrary strongly-typed payload published and consumed via generic API; metadata (e.g., CorrelationId) may be attached.
- **IMessageBus**: Public abstraction that exposes Publish/Subscribe operations and hides implementation details.
- **RabbitMqOptions**: Configuration bag containing host, virtual host, username, password, connection string, retry/dead-letter options, and convention hooks.
- **IMessageSerializer**: Optional serializer abstraction allowing replacement of the default serializer while remaining compatible with message headers and format expected by the broker.
- **SubscriptionOptions**: Describes subscription-specific settings: subscription id, durability, concurrency, prefetch, dead-letter behavior.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Developers can integrate the library and implement a basic publish-and-subscribe flow in under 15 minutes using documented examples.
- **SC-002**: The library does not expose EasyNetQ types in the public API; automated API surface checks pass (e.g., public surface scan yields no EasyNetQ symbols).
- **SC-003**: Under normal operating conditions, 99% of published messages are delivered to a single subscribed handler within 5 seconds.
- **SC-004**: When a consumer handler throws, the library logs the failure and applies configured retry/dead-letter behavior; 95% of transient errors are retried and resolved without manual intervention.
- **SC-005**: Publish and consume counters are available to telemetry and increment on each successful publish/consume operation.

## Assumptions

- The implementation will use EasyNetQ internally on .NET 9, but public API must be independent of EasyNetQ types.
- The target consumers are .NET workloads using Microsoft DI and logging.
- Default serializer is acceptable but a pluggable serializer interface is required for special cases.
- Operational metrics export and advanced telemetry integration will be provided via hooks, not opinionated exporters.

## Architecture (high-level)

- Public surface: IMessageBus, RabbitMqOptions, IMessageSerializer, SubscriptionOptions and DI extension AddMessaging.
- Internal: EasyNetQ IBus wrapper implementation that translates between public interfaces and EasyNetQ calls, manages lifecycle, conventions, and serialization adapters.
- Cross-cutting: Middleware pipeline for publish and consume, logging/correlation propagation, and telemetry hooks.

## Wrapper & Flow (Publish/Subscribe)

- Publish flow: Caller -> IMessageBus.PublishAsync<T>(T) -> publish middleware pipeline -> serializer -> EasyNetQ wrapper -> IBus.Publish/PublishAsync -> broker.
- Subscribe flow: EasyNetQ subscription -> wrapper normalizes incoming message and metadata -> consumer middleware pipeline -> deserializer -> user handler invoked asynchronously -> on success ack; on exception apply retry/dlq per config and log.

## Error Handling Strategy

- Use EasyNetQ’s automatic reconnection and built-in error handling as primary mechanism.
- Surface handler exceptions via logging and metrics; apply configurable wrapper-level retry policies for transient failures.
- Support optional dead-letter queue wiring where persistent failures are routed to a DLQ with reason metadata.
- Ensure exceptions in handlers do not crash the host process; they are observed, logged, and handled according to subscription policy.

## Extensibility Points

- IMessageSerializer to replace or wrap default serialization.
- Convention hooks in RabbitMqOptions to customize naming.
- Middleware pipeline for publish and consume allowing enrichment, validation, and correlation propagation.
- Hooks for metrics (counters and timers) and custom logging enrichers.

## Example usage (developer-facing)

- Registering (conceptual):

  - IServiceCollection.AddMessaging(opts => { opts.ConnectionString = "amqp://..."; opts.EnableDeadLetter = true; });
  - var bus = provider.GetRequiredService<IMessageBus>();
  - await bus.PublishAsync(new OrderCreated { OrderId = 123 });
  - await bus.SubscribeAsync<OrderCreated>(async order => { /* handle */ });

(Implementation code examples belong in design docs, not in this spec.)

## Non-Goals

- Re-implementing RabbitMQ client logic handled by EasyNetQ.
- Exposing EasyNetQ types in the public API.
- Over-engineering beyond a thin, clean abstraction layer.

## Deliverables

- High-level architecture description (this spec).
- Public interface definitions and wrapper design guidance.
- Publish/Subscribe flow and error handling strategy.
- Example usage and recommended project structure.

