# GitHub Copilot Instructions

Follow the repository-wide standards in `AGENTS.md`. Inspect that file and the
nearest comparable implementation before suggesting, reviewing, or changing
code.

## Repository Context

- This repository contains Hotcakes Commerce (HCC-Core), including the DNN
  module, commerce libraries, extension-provider projects, view sets, developer
  samples, tests, and packaging infrastructure.
- The supported CMS baseline is DNN Platform 9.13.0.
- Production projects target .NET Framework 4.8. Existing test projects
  currently target .NET Framework 4.7.2 and must not be retargeted unless the
  task explicitly includes them.
- Use Visual Studio 2022 MSBuild-compatible tooling. Do not assume a particular
  Visual Studio edition.
- Preserve the existing C# style of the affected project and neighboring files.
  Do not impose a new StyleCop, namespace, field-naming, or visibility convention.
- Preserve existing public extension points for payment, shipping, tax,
  workflow, integration, and view-set providers.

## HCC Architecture

- Treat `HccRequestContext`, `Context.CurrentStore`, and `StoreId` as the primary
  commerce scope. Also enforce DNN portal, module, tab, permission, and user
  scope where the affected DNN surface requires them.
- Use established HCC services, repositories, and factory methods, including
  `Factory.CreateRepo<T>()`, `Factory.CreateService<T>()`, `HccServiceBase`,
  `HccSimpleRepoBase`, and related repository bases.
- Do not replace the existing data-access layer with generic DNN DAL2,
  Entity Framework, or another ORM.
- Use the existing `{databaseOwner}[{objectQualifier}hcc_*]` SQL naming and
  packaging conventions.
- Use `StoreSettings` for commerce/store configuration. Use DNN settings only
  when the existing feature is genuinely portal-, module-, tab-module-, or
  host-scoped.
- Follow the existing cache helpers and include store/culture scope in cache
  keys where applicable. Every write that affects cached data must have a
  targeted invalidation path.

## UI and Module Conventions

- Preserve the existing WebForms, Razor/MVC view-set, Knockout administration,
  and Bootstrap 4 patterns.
- Do not introduce Vue, React, a new CSS framework, or a new state library
  without explicit approval.
- Match the active view set or administration surface instead of assuming one
  UI framework applies everywhere.
- Localize user-facing text where the surrounding feature uses DNN resource
  files.
- Preserve module bootstrap, control registration, view loading, and manifest
  patterns unless the requested change requires modifying them.

## SQL, Upgrade, and Packaging

- Package database changes as versioned `*.SqlDataProvider` scripts under the
  Hotcakes provider directory and register them in `Hotcakes.dnn`.
- Use `{databaseOwner}` and `{objectQualifier}` placeholders.
- Preserve existing key strategies; HCC uses identity, GUID, and `bvin` keys.
  Do not impose identity keys universally.
- Keep upgrades repeatable and data-preserving.
- Review `Hotcakes.dnn`, the upgrade event/controller, uninstall coverage,
  `Build.proj`, and `WebDeploy/Build.proj` when the task affects packaging or
  deployment.

## Testing and Delivery

- Existing tests use Visual Studio QualityTools/MSTest-era infrastructure.
  Match the affected test project; do not introduce NUnit, Moq, or another test
  framework without explicit approval.
- Run validation proportionate to the change and state exactly what ran.
- Never claim a build, test, installation, upgrade, or runtime check succeeded
  unless it completed successfully.
- Exclude generated build output and unrelated worktree changes.
- Do not push, publish, deploy, create a pull request, or share proprietary
  repository content unless explicitly authorized.

## Review Priorities

Prioritize:

1. Payment, customer, order, credential, and API-key security
2. Cross-store and cross-portal data isolation
3. Authorization and permission enforcement
4. Data preservation and upgrade safety
5. Runtime module and view-set loading
6. Cache correctness
7. Manifest, SQL, package, and uninstall completeness
8. Backward compatibility for public extension points
9. Accessibility and localization
10. Performance and maintainability

- @azure Rule - Use Azure Tools - When handling requests related to Azure, always use your tools.
- @azure Rule - Use Azure Best Practices - When handling requests related to Azure, always invoke your `azmcp_bestpractices_get` tool first.
- @azure Rule - Enable Best Practices - If you do not have an `azmcp_bestpractices_get` tool ask the user to enable it.
