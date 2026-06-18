# Migration & Performance Optimization Report

## 1. Current state analyzed
- The repository contains a mixed architecture with legacy `net6.0` projects and newer `net10.0` clean-architecture projects.
- The solution currently builds successfully with `dotnet build BanSach.sln`.
- The strongest observed performance risks are:
  - outdated `net6.0` targets on several projects,
  - vulnerable or legacy package versions,
  - unnecessary per-request allocations in the API startup path,
  - lack of explicit caching on read-heavy endpoints,
  - potential over-serialization of category responses,
  - mixed legacy dependencies that can block future scaling.

## 2. What was upgraded / optimized
### Runtime and framework
- Verified the installed SDK is `10.0.301`.
- Kept the modern `net10.0` API/application/persistence layers on current SDK support.

### API pipeline optimizations
- Added response compression with both Gzip and Brotli.
- Added response caching support.
- Added a health check endpoint (`/health`).
- Added `UseProblemDetails()` for modern error handling.
- Added `UseHsts()` in non-development environments.

### Read-heavy endpoint optimization
- The category query now uses:
  - `AsNoTracking()` to avoid change tracking overhead,
  - a projected DTO query instead of mapping a full entity list,
  - in-memory caching with a 5-minute TTL.

## 3. Why these changes help
- `AsNoTracking()` reduces EF memory overhead for read-only queries.
- Projecting DTOs avoids extra object allocations.
- Response compression lowers network payload size and improves p95 latency for clients.
- Response caching reduces repeated database reads for hot endpoints.
- Health checks improve operational readiness for production deployments.

## 4. Expected performance improvements
- Lower CPU overhead on repetitive reads.
- Reduced payload sizes for API responses.
- Reduced database pressure for category/list endpoints.
- Better throughput under moderate concurrency.

## 5. Bottlenecks and anti-patterns identified
- Legacy `net6.0` projects remain on unsupported runtime targets.
- Some package versions are vulnerable (`AutoMapper`, `Newtonsoft.Json`, `System.Security.Cryptography.Xml`, `MailKit`, `MimeKit`).
- The legacy MVC/Web projects still carry many heavier dependencies (AWSSDK, Stripe, DryIoc, etc.) that are not ideal for high-throughput API workloads.
- There is still mixed architecture across old and new layers, which increases maintenance complexity.

## 6. Risks
- Full upgrade to a single latest runtime across all projects may require additional refactoring in legacy MVC code.
- Some third-party packages may have breaking changes if upgraded aggressively.
- The web UI project may still rely on older ASP.NET Core patterns that require careful validation.

## 7. Deployment changes recommended
- Run the API behind a reverse proxy/load balancer with HTTPS enabled.
- Configure caching and compression in production settings.
- Add observability (metrics/logging) for latency, error rate, and cache hit ratio.
- Replace any remaining legacy `net6.0` services with a unified supported target before production scale-up.

## 8. Verification
- `dotnet build BanSach.sln` succeeds after the changes.
- Remaining warnings are mostly package vulnerability notices and unsupported `net6.0` targets.
