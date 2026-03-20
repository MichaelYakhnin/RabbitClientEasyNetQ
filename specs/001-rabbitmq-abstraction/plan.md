# Implementation Plan: RabbitMQ Abstraction

**Branch**: `001-rabbitmq-abstraction` | **Date**: 2026-03-20 | **Spec**: ../001-rabbitmq-abstraction/spec.md
**Input**: Feature specification from `/specs/001-rabbitmq-abstraction/spec.md`

## Summary

Provide a thin, production-ready .NET 9 class library that exposes a small, strongly-typed messaging surface (IMessageBus) for PublishAsync<T> and SubscribeAsync<T>. Internally the library uses EasyNetQ to manage connections, conventions, and broker interactions. Deliverables include research, data model, interface contracts, quickstart, and agent-context update.

## Technical Context

**Language/Version**: .NET 9
**Primary Dependencies**: EasyNetQ (internal), Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Logging, Microsoft.Extensions.Options, Polly (optional, for retry wrapper)
**Storage**: N/A
**Testing**: xUnit + integration tests using Testcontainers or local RabbitMQ for integration verification (assumption)
**Target Platform**: Server-side .NET apps (Linux/Windows) consuming a class library
**Project Type**: library (NuGet package)
**Performance Goals**: support concurrent publishers; 99% of messages delivered to single subscriber within 5s under normal conditions (aligned with spec SC-003)
**Constraints**: keep public API independent of EasyNetQ; safe concurrent publishing; minimal allocation in hot paths; respect EasyNetQ connection reuse
**Scale/Scope**: designed for typical microservice workloads (tens to hundreds of messages/sec per instance); not a global high-throughput broker client replacement

## Constitution Check

This feature aligns with the constitution principles relevant to libraries used by backend services:

- Asynchronous operations and CancellationToken support: PASS (API includes CancellationToken)
- Dependency injection and use of ILogger<T>: PASS
- No single-page scraping boundary concerns (irrelevant): N/A

No constitutional gates violated. Continue to Phase 0.

## Project Structure

```text
specs/001-rabbitmq-abstraction/
├── spec.md
├── plan.md         # this file
├── research.md     # Phase 0
├── data-model.md   # Phase 1
├── quickstart.md   # Phase 1
├── contracts/
│   ├── IMessageBus.md
│   ├── IMessageSerializer.md
│   ├── RabbitMqOptions.md
│   └── SubscriptionOptions.md
└── checklists/
    └── requirements.md
```

**Structure Decision**: Single library project (class library) with tests and examples in separate test project.

## Phase 0: Research (outline)

Unknowns and decisions captured in research.md. Key topics:
- Correlation ID propagation approach
- Retry and DLQ wiring strategy
- Middleware pipeline shape

## Phase 1: Design outputs

- data-model.md: Entities and validation rules
- contracts/: Public interfaces and option types
- quickstart.md: Minimal adoption guide

## Phase 2: Implementation notes (deferred to tasks.md)

- Create library project and NuGet packaging
- Implement EasyNetQ wrapper and adapter layers
- Implement middleware pipeline and default middleware (correlation, metrics)
- Integration tests using local broker

## Risks & Mitigations

- Dependency on EasyNetQ semantics: mitigate by encapsulating IBus and writing adapter tests
- Serialization incompatibility: mitigate by using EasyNetQ default serializer as the default
- Operational surprises (reconnect behavior): rely on EasyNetQ reconnection and surface health via logs/metrics
