# Principles — MetroIsJustABus (`metro-core`)

1. The system is distributed. Do not put all functionality behind a single REST process. Notifications live across an independent application boundary (separate process and container).

2. `metro-core` is the source of truth for the domain. Other components react to it; they do not redefine network, ticket, or schedule truth.

3. Domain and Application must not depend on ASP.NET, EF, or other infrastructure frameworks. Depend on abstractions; implement them in Infrastructure.

4. Controllers stay thin. Business outcomes are expressed as `ApiResult`; HTTP status mapping happens only in Presentation (`FromResult`).

5. Writes go through EF Core + Unit of Work; reads use Dapper query repositories. Do not mix those paths without a documented reason.

6. Cross-component effects (especially notifications) are asynchronous. Persist locally first, publish the event, consume idempotently — no duplicate tickets, validations, or notifications.

7. Partial failure is expected. A down notification service or broker must not block network consultation or other unrelated core features.

8. Authorize on the server. Roles and claims from the client are not authority by themselves.

9. Do not invent requirements that contradict `docs/spec/metro-is-just-a-bus.md` or `.ai/steering/`. If scope is unclear, it is out of scope until decided and documented.

10. Prefer extending existing module patterns over introducing a parallel architecture inside this repo.
