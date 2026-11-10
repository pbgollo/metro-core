# MetroIsJustABus — Product Vision

## Value Proposition

MetroIsJustABus is a distributed system that supports an urban bus network and passenger services: network and schedule information, tickets and validations, occupancy, disruptions, and notifications.

## The Problem We Solve

1. Passengers need up-to-date information about lines, stops, schedules, and disruptions.
2. Operational staff need to maintain the network and schedules and register day-to-day changes.
3. Ticket purchase, validation, and occupancy must stay consistent across the system.
4. Relevant changes must reach subscribed passengers without making unrelated features unavailable when notifications fail.

## Solution

- A single web application for passengers and operational staff (role-based access).
- A core backend as the source of truth for the domain.
- An independently deployable notification service across a separate application boundary.
- Synchronous APIs for the web app and asynchronous messaging for cross-component effects (e.g. notifications).

## What This Product Is NOT

- Not a single monolithic backend that exposes all functionality through one REST API.
- Not a real payment gateway (ticket purchase may be simulated without an external payment provider).
- Not a physical ticket-validation device or hardware integration (validation is modeled as a domain operation).
- Not a separate operational desktop/mobile client in the initial architecture (ops use the same web app with different permissions).
- Not a full enterprise transit suite (no fleet telemetry, driver apps, or advanced analytics unless later required).

## Audience

1. **Passengers**: consult the network, manage accounts and tickets, subscribe to alerts.
2. **Operational staff**: maintain network and schedules, manage ticket types, register disruptions, consult validations and occupancy.
3. **Course / engineering team**: build and operate the system with AI-assisted Spec-Driven Development, CI/CDE/CD, and evidence of agent use.

## Core Capabilities (from Milestone 1)

1. Network consultation (lines, stops, schedules, scheduled trips), reflecting known disruptions.
2. Ticket types, purchase, account tickets, validation, and occupancy updates.
3. User accounts, subscriptions, and notification preferences.
4. Operational maintenance of network, schedules, ticket types, and disruptions.
5. Partial-failure tolerance: notification outages must not block network consultation or unrelated functions.

## Scope Notes for This Repository (`metro-core`)

This repository hosts the **core domain backend** and, by design, the **notification service** as a separate project/process/container in the same repo. The passenger/ops UI lives in `metro-web`.

Canonical project statement: `docs/spec/metro-is-just-a-bus.md` (PDF in the same folder prevails on conflict).

## Success Criteria (project)

- Requirements and architecture remain traceable to the project statement.
- Distributed decomposition is respected (web, core, supporting service(s), independent notifications).
- AI-generated artifacts are validated; ambiguities and assumptions stay documented.
