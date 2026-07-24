# CLAUDE.md

Before working in this repository, read and follow `AGENTS.md` as the canonical
repository instruction file.

## Repository Context

- This is Hotcakes Commerce (HCC-Core), not a generic DNN module or an Active
  Forums project.
- The supported platform is DNN 9.13.0 on .NET Framework 4.8.
- Existing test projects currently remain on .NET Framework 4.7.2.
- Commerce behavior is primarily scoped through `HccRequestContext`,
  `Context.CurrentStore`, and `StoreId`.
- Preserve the existing HCC service, repository, factory, caching, manifest,
  SQL-provider, view-set, and packaging patterns.
- The established UI includes WebForms, Razor/MVC view sets, Knockout
  administration code, and Bootstrap 4.
- Do not introduce a new framework, ORM, package, testing stack, or architecture
  without explicit approval.

## Working Expectations

- Inspect neighboring code and the closest working implementation before
  proposing architecture or editing files.
- Keep changes narrowly scoped and preserve public extension points and runtime
  behavior.
- Treat payment data, customer data, orders, credentials, API keys, store
  settings, and cross-store isolation as high-risk areas.
- Use the existing `{databaseOwner}[{objectQualifier}hcc_*]` SQL conventions.
- Match the affected Visual Studio QualityTools/MSTest project when tests are in
  scope.
- Report exactly what was changed and actually validated, including database,
  configuration, cache, upgrade, packaging, and compatibility impact.
- Do not push, publish, deploy, create pull requests, or share proprietary
  repository content unless explicitly authorized.
