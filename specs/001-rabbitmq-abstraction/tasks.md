# Tasks: RabbitMQ Abstraction

## Phase 1 - Setup

- [X] T001 Create .NET class library project at src/RabbitClientEasyNetQ/RabbitClientEasyNetQ.csproj
- [ ] T002 Create test project at tests/RabbitClientEasyNetQ.Tests/RabbitClientEasyNetQ.Tests.csproj
- [X] T003 Add repository files: README.md, LICENSE, .editorconfig at repo root

## Phase 2 - Foundational (blocking prerequisites)

- [X] T004 [P] [ ] Create Contracts: src/RabbitClientEasyNetQ/Contracts/IMessageBus.cs and src/RabbitClientEasyNetQ/Contracts/RabbitMqOptions.cs and src/RabbitClientEasyNetQ/Contracts/SubscriptionOptions.cs
- [X] T005 Create DI extension skeleton at src/RabbitClientEasyNetQ/Extensions/ServiceCollectionExtensions.cs
- [X] T006 Create internal EasyNetQ adapter skeleton at src/RabbitClientEasyNetQ/Internal/EasyNetQBusAdapter.cs
- [X] T007 [P] Create middleware abstractions at src/RabbitClientEasyNetQ/Middleware/IPublishMiddleware.cs and src/RabbitClientEasyNetQ/Middleware/IConsumeMiddleware.cs and src/RabbitClientEasyNetQ/Middleware/MiddlewarePipeline.cs
- [X] T008 Create observability primitives at src/RabbitClientEasyNetQ/Observability/IMessagingMetrics.cs and src/RabbitClientEasyNetQ/Observability/Correlation.cs
- [ ] T009 Create configuration file type at src/RabbitClientEasyNetQ/Configuration/RabbitMqOptions.cs (if not in Contracts)
- [ ] T010 Create integration test scaffold at tests/RabbitClientEasyNetQ.Tests/Integration/PublishSubscribeIntegrationTests.cs

## Phase 3 - User Story 1 (US1) - Basic publish/subscribe (Priority P1)

- [X] T011 [US1] Implement public interface file src/RabbitClientEasyNetQ/Contracts/IMessageBus.cs
- [X] T012 [US1] Implement MessageBus class (EasyNetQ-backed) at src/RabbitClientEasyNetQ/MessageBus.cs
- [X] T013 [US1] Implement PublishAsync<T> and SubscribeAsync<T> behavior mapping to EasyNetQ adapter at src/RabbitClientEasyNetQ/Internal/EasyNetQBusAdapter.cs
- [ ] T014 [P] [US1] Add example program demonstrating Publish/Subscribe at examples/basic/Program.cs
- [ ] T015 [US1] Add unit tests for IMessageBus behavior at tests/RabbitClientEasyNetQ.Tests/Unit/MessageBusTests.cs
- [ ] T016 [US1] Add integration test verifying end-to-end publish/subscribe at tests/RabbitClientEasyNetQ.Tests/Integration/PublishSubscribeIntegrationTests.cs

## Phase 4 - User Story 2 (US2) - Configuration & DI (Priority P2)

- [ ] T017 [US2] Implement IServiceCollection.AddMessaging(Action<RabbitMqOptions>) and registration wiring at src/RabbitClientEasyNetQ/Extensions/ServiceCollectionExtensions.cs
- [ ] T018 [US2] Implement RabbitMqOptions parsing (connection string + options) and IOptions integration at src/RabbitClientEasyNetQ/Configuration/RabbitMqOptions.cs
- [ ] T019 [US2] Add tests that verify IServiceCollection registration and disposal behavior at tests/RabbitClientEasyNetQ.Tests/Integration/ServiceRegistrationTests.cs

## Phase 5 - User Story 3 (US3) - Observability & Correlation (Priority P3)

- [ ] T020 [US3] Implement correlation ID propagation middleware and logging enrichment at src/RabbitClientEasyNetQ/Middleware/CorrelationMiddleware.cs and src/RabbitClientEasyNetQ/Observability/Correlation.cs
- [ ] T021 [P] [US3] Implement metrics hooks and counters (IMessagingMetrics default and a no-op implementation) at src/RabbitClientEasyNetQ/Observability/MessagingMetrics.cs
- [ ] T022 [US3] Add tests to validate correlation propagation and metrics counters at tests/RabbitClientEasyNetQ.Tests/Unit/ObservabilityTests.cs

## Final Phase - Polish & Cross-cutting concerns

- [ ] T023 Create quickstart documentation at specs/001-rabbitmq-abstraction/quickstart.md
- [ ] T024 Add NuGet packaging config and README at src/RabbitClientEasyNetQ/NuGet.props and README.md
- [ ] T025 Add CI workflow for build and integration tests at .github/workflows/ci.yml

## Dependencies & Execution Order

- Setup (T001-T003) must complete before Foundational tasks.
- Foundational tasks (T004-T010) must complete before User Story implementation tasks.
- US1 (T011-T016) is the recommended MVP and should be implemented first.
- US2 (T017-T019) depends on Foundational tasks and can run in parallel with the latter US1 testing tasks where safe.
- US3 (T020-T022) depends on middleware and observability primitives from Foundational phase.

## Parallel Opportunities

- T004, T007, T014, T021 marked [P] can be done in parallel by different engineers.

## Counts & Summary

- Total tasks: 25
- Tasks for US1: 6
- Tasks for US2: 3
- Tasks for US3: 3
- Setup tasks: 3
- Foundational tasks: 7
- Final tasks: 3

## MVP Suggestion

- Implement only Phase 1-3 (T001-T016) to deliver minimal working publish/subscribe with DI registration and basic tests.

## Next Steps (after tasks.md)

- Start T001-T003 to initialize repository structure.
- Implement contracts (T004) and MessageBus interface (T011) in parallel.
- Run unit tests for T015 and integration tests for T016 using a local RabbitMQ instance.