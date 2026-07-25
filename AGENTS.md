# AGENTS.md

## Purpose

This file defines repository-wide instructions for autonomous coding agents working on the Hotcakes Commerce solution, including its DNN module, commerce libraries, extension providers, view sets, developer samples, tests, and packaging infrastructure.

Agents must follow these standards for analysis, implementation, review, testing, patch preparation, and delivery.

## Agent Operating Rules

- Inspect the repository before proposing architecture.
- Prefer existing code patterns over generic best practices.
- Work from repository-relative paths.
- Keep changes narrowly scoped to the requested task.
- Do not modify generated, packaged, versioned, or unrelated files unless necessary.
- Do not silently make broad refactors.
- Do not claim a build or test succeeded unless the command was actually run and completed successfully.
- If required source or attachments are unreadable, stop and report a fatal error.
- When uncertain, preserve current runtime behavior and data.

## Expected Agent Workflow

1. Identify the affected project, module, library, provider, or packaging surface and its neighboring examples.
2. Review the applicable manifests, project files, SQL providers, settings, services, controllers, resources, and front-end startup code.
3. State material risks early, especially module-loading, authorization, upgrade, data-loss, or packaging risks.
4. Implement the smallest complete change.
5. Review install, upgrade, cache, permission, and uninstall impact.
6. Run available builds or focused validation.
7. Produce a repository-root patch or complete changed files.
8. Summarize actual testing and remaining risks.


# Module Development Standards

These instructions define the expected approach for building, updating, reviewing, packaging, and troubleshooting production-quality DNN CMS modules.

Treat these requirements as project constraints, not suggestions. Preserve established architecture and working runtime behavior unless the task explicitly requires a change.

## Core Priorities

When requirements compete, use this priority order:

1. Security
2. Data integrity
3. Correct runtime behavior
4. Compatibility with the existing project
5. Explicit requirements and supplied mockups
6. Correct store, portal, and module scoping
7. Accessibility
8. Maintainability
9. Performance
10. Visual polish
11. Architectural modernization

Do not sacrifice working runtime behavior, upgrade safety, or data integrity merely to make an implementation appear more modern.

## Before Changing Code

Before implementing anything:

1. Read every provided source file, specification, screenshot, SQL script, and supporting document.
2. Inspect comparable modules in the same repository.
3. Identify existing conventions for module startup, API calls, permissions, localization, settings, caching, SQL, upgrades, packaging, and uninstallation.
4. Preserve those conventions wherever practical.
5. Make the smallest complete set of changes necessary.
6. Avoid unrelated refactoring or formatting churn.
7. Do not introduce a new framework, package, build tool, ORM, state library, or visual system without explicit approval.
8. Preserve backward compatibility unless a breaking change is specifically required.

If a required attachment or source file cannot be opened, read, or understood, stop immediately and report a fatal error. Do not guess.

## Default Platform Assumptions

Unless the repository proves otherwise, assume:

- DNN Platform 9.13.0
- .NET Framework 4.8 for production projects
- .NET Framework 4.7.2 for existing test projects unless their upgrade is explicitly requested
- Microsoft SQL Server
- Visual Studio 2022-compatible MSBuild tooling
- IIS-hosted deployment
- Bootstrap 4 or the version already supplied by the selected Hotcakes view set or DNN surface
- Font Awesome Free
- WebForms for existing module and administrative surfaces
- Razor/MVC for existing Hotcakes view sets
- KnockoutJS for the existing administrative features that already use it
- Windows-compatible build and deployment workflows

Do not upgrade the platform, target framework, JavaScript framework, NuGet packages, npm packages, build tooling, package versions, or assembly versions unless explicitly requested.

The repository permits newer C# syntax through its build configuration, but production code must remain compatible with .NET Framework 4.8. Do not use runtime APIs that are available only in modern .NET.

## Hotcakes Commerce Architecture

This repository is Hotcakes Commerce (HCC-Core), not a generic collection of independent DNN modules.

Preserve these established patterns:

- `HccRequestContext` carries commerce request context.
- `Context.CurrentStore` and `StoreId` are the primary commerce data boundary.
- `Factory.CreateRepo<T>()` and `Factory.CreateService<T>()` create established repositories and services.
- `HccServiceBase`, `HccSimpleRepoBase`, localization repository bases, and neighboring HCC abstractions define the data-access architecture.
- Store configuration belongs in the established `StoreSettings` infrastructure.
- Public extension points for payment, shipping, tax, workflows, integrations, and view sets must remain backward compatible.

Do not replace these patterns with generic DNN DAL2 controllers, Entity Framework, another ORM, or a new dependency-injection architecture unless explicitly requested.

For DNN-facing code, validate the applicable portal, module, tab, permission, and user context in addition to commerce store scope. Do not add `PortalId` or `ModuleId` to commerce records merely because they are common in other DNN modules; determine the established HCC ownership model first.

## Preserve Existing Architecture

The existing module architecture is a primary constraint.

Before adding or changing a view:

- Match proven folder structure.
- Match module bootstrap and loading patterns.
- Match manifest declarations.
- Match namespaces.
- Match service registration.
- Match API request conventions.
- Match localization and resource-file conventions.
- Match settings storage patterns.
- Match packaging conventions.
- Match neighboring module code style.

Treat changes to `View.html`, `View.ascx`, Razor views, SPA startup code, controller initialization, and module bootstrap logic as high risk. Small differences can prevent a module from loading.

When creating a new module from an existing one, copy the proven structure first and change only what the new feature requires.

## Public and Administrative Modules

### Public-facing modules

- Prefer server-rendered WebForms or Razor when that matches the project.
- Do not add a large SPA framework for a simple public view.
- Allow anonymous access only where intentionally required.
- Keep public endpoints narrowly scoped.
- Validate and encode user-provided content.
- Avoid exposing internal IDs or protected configuration.
- Render semantic, accessible HTML.
- Reuse the active website theme.

### Administrative modules

- Preserve the existing WebForms and KnockoutJS administration patterns.
- Do not introduce an SPA framework unless the task explicitly requests it.
- Use Bootstrap and existing components before adding custom CSS.
- Keep management, configuration, imports, templates, and reporting logically separated.
- Enforce permissions in both the interface and server-side API.
- A hidden or login-only page is not automatically secure.

## Security

### Authorization

Every API method must have explicit authorization appropriate to the operation.

Use the project’s established DNN permission and authorization patterns, including:

- Module-level authorization
- Edit permission checks
- Administrator checks
- Superuser checks
- Portal-specific access checks

Never rely only on hidden buttons, hidden pages, JavaScript conditions, navigation visibility, or client-side role checks.

Anonymous access is permitted only for intentionally public endpoints.

### Request context

Browser and API requests should include and validate the appropriate context:

- Store ID or current store context where commerce data is involved
- Module ID
- Tab ID
- Portal ID
- Anti-forgery token
- Current user identity
- Module permissions

Do not trust submitted IDs. Confirm that the current user can access the requested record in the correct portal and module context.

### Input and output

Validate all input on the server:

- Required values
- Length
- Type
- Range
- Enumeration values
- Dates
- Email format where appropriate
- File extension
- MIME type
- File size
- File path
- Rich HTML

Use parameterized SQL or the approved data-access layer. Never concatenate user input into SQL.

Encode output by default. Render unencoded HTML only for intentional HTML fields whose content is trusted or sanitized.

Do not expose stack traces, SQL statements, connection information, secrets, physical paths, or internal implementation details to users.

### Secrets and configuration

Do not store secrets in client-side code.

Do not place portal-specific configuration in `web.config` when it belongs to a portal.

Use the project’s secure server-side configuration pattern. Where portal configuration is stored in `PortalSettings`, continue that pattern.

## Scope and Settings

Determine the correct scope for every setting and data record:

- Store
- Host
- Portal
- Page
- Module
- Tab module
- Role
- User
- Record

Do not save portal-wide behavior at module scope. Do not save portal-specific behavior globally.

When using HCC `StoreSettings`:

- Resolve the correct store through `HccRequestContext`.
- Centralize reads and writes.
- Parse safely.
- Provide sensible defaults.
- Restrict access appropriately.
- Clear the relevant store settings cache after save.
- Confirm the new value is reflected immediately.

Use DNN `PortalSettings`, module settings, tab-module settings, or host settings only when the value genuinely belongs to that DNN scope and the neighboring feature uses that mechanism.

Use `TabModuleSettings` only when the value truly belongs to one module instance.

## Configuration Experience

Preferred approaches:

- `Settings.ascx` for WebForms modules
- An established Hotcakes administration or configuration view
- A purpose-built first-use configuration flow when necessary

Configuration interfaces must:

- Explain each setting clearly.
- Explain whether it affects the module, page, portal, or host.
- Use useful help text instead of vague descriptions.
- Show field-level validation.
- Preserve entered values after recoverable errors.
- Show clear success and failure feedback.
- Prevent unauthorized access.
- Clear relevant caches after saving.
- Refresh affected data after save when appropriate.

If valid initial configuration is missing, administrators may see a configuration prompt. Unauthorized users should see a neutral empty state or nothing.

## Database Standards

### Naming and packaging

Use the established Hotcakes object prefix:

```sql
hcc_
```

In packaged SQL scripts, use:

```sql
{databaseOwner}[{objectQualifier}hcc_ObjectName]
```

Do not hard-code `dbo.` unless the project explicitly requires it.

Use consistent names for tables, procedures, views, functions, indexes, constraints, foreign keys, and defaults.

### Repeatability and safety

When repeatability is requested, scripts must be safe to run more than once.

For schema scripts:

1. Drop dependent objects in safe order when appropriate.
2. Check for existence.
3. Recreate in dependency order.
4. Recreate indexes and constraints.
5. Avoid partial completion.
6. Report meaningful results.

For data correction:

- Include preview behavior where practical.
- Make commit behavior obvious.
- Use transactions where appropriate.
- Prevent duplicate inserts.
- Avoid overwriting good data without explicit approval.
- Include verification queries.
- Report counts using safe aliases such as `[RowsAffected]`, `[MatchedRows]`, or `[UpdatedRows]`.

Do not use ambiguous or problematic unescaped aliases such as `RowCount`.

### Schema design

Design for:

- Multiple stores
- Multiple portals
- Multiple module instances
- Appropriate foreign keys
- Appropriate nullability
- Auditing
- Consistent date storage
- Future growth
- Importability
- Performance
- Clean uninstall

Use fields such as Store ID, Portal ID, Module ID, Created/Modified user and date, status, active flags, and sort order only when they serve a defined purpose. Match the established ownership boundary rather than adding every possible scope field.

### Indexes and data access

Add indexes based on real query patterns, including common joins and filters such as portal, module, user, status, date, foreign keys, and unique business keys.

Do not create excessive indexes.

Preserve the HCC repository and service patterns, including `Factory`, `HccRequestContext`, `HccServiceBase`, `HccSimpleRepoBase`, and the related repository bases used by neighboring code. Do not introduce generic DNN DAL2, Entity Framework, or another data-access architecture without explicit approval.

## Installation, Upgrade, and Uninstallation

### Installation

Confirm that the manifest includes all required:

- Assemblies
- SQL scripts in correct order
- Views and controls
- JavaScript
- CSS
- Resource files
- Images
- Module definitions
- Control keys
- Permissions

### Upgrade

Use the established upgrade mechanism.

For the Hotcakes module, review and preserve:

- `Website/DesktopModules/Hotcakes/Hotcakes.dnn`
- Versioned scripts under `Website/DesktopModules/Hotcakes/Providers/DataProviders/SqlDataProvider`
- The manifest's `UpgradeModule` event and Hotcakes business controller
- `Build.proj` and `WebDeploy/Build.proj` when packaging or deployment is affected

Do not change build numbers, versions, package metadata, or assembly versions unless requested.

Upgrade logic must:

- Be idempotent.
- Detect previously completed work.
- Preserve user data.
- Log useful information.
- Avoid failing when rerun.
- Support older installations appropriately.

### Uninstallation

The uninstall script must remove all module-owned database objects:

- Foreign keys
- Default constraints
- Check constraints
- Indexes
- Views
- Functions
- Stored procedures
- Tables
- Supporting objects

Drop objects in dependency-safe order. Never remove shared objects owned by another feature.

Review install and uninstall SQL together to verify complete coverage.

## API and Service Design

Each endpoint should:

- Have explicit authorization.
- Validate the request.
- Validate portal and module scope.
- Use the project’s service layer where applicable.
- Return an appropriate status code.
- Return a consistent response shape.
- Avoid leaking exception details.
- Log unexpected failures.

Use appropriate HTTP methods. Never use `GET` for state-changing operations.

Prefer focused view models over exposing full database entities.

## Front-End Standards

### Theme integration

Prefer:

- Bootstrap layout, spacing, typography, cards, forms, alerts, modals, tables, and responsive utilities
- Existing theme variables and components
- Existing shared module components

Avoid:

- Material UI
- New CSS frameworks
- Large custom design systems
- Inline styles
- Hard-coded colors
- Hard-coded fonts
- Duplicate Bootstrap, jQuery, Font Awesome, or Vue bundles

Scope custom CSS to the module.

### Responsive behavior

All views must work intentionally on desktop, tablet, and mobile.

Examples:

- Desktop heading actions may use an icon and label.
- Mobile heading actions may use icon-only buttons with accessible labels.
- Grids should collapse predictably.
- Tables should be responsive or transform into a suitable list/card view.
- Modals and forms must remain usable on small screens.
- Touch targets must be large enough.

### Loading and refresh

Data views should provide:

- Loading state
- Empty state
- Error state
- Retry or refresh action when useful

A refresh action should reload server data and ignore unsubmitted form changes unless requirements say otherwise.

On desktop, place heading actions at the right side of the heading area. On mobile, icon-only presentation is acceptable when accessible.

### Grids

Grid interfaces should include the features required by the workflow:

- Search
- Filters
- Sorting
- Paging
- Page-size selection
- Empty state
- Loading state
- Accessible row actions
- Destructive-action confirmation

Where established by the project:

- Persist page size for one year.
- Persist search and current page for 24 hours.
- Restore state safely.
- Reset current page when filters change.

Icon-only row actions require:

- `aria-label`
- `title`
- Keyboard access
- Clear hover and focus states
- Adequate touch target

Use Font Awesome Free unless another licensed set is already included.

### Forms

Forms must:

- Use visible labels.
- Identify required fields.
- Include clear help text.
- Validate on client and server.
- Show field-specific errors.
- Prevent duplicate submission.
- Show progress and success.
- Preserve input after recoverable errors.
- Warn before discarding meaningful unsaved work when appropriate.

Do not use placeholders as the only label.

### Confirmations

Use specific confirmation dialogs for destructive or irreversible actions.

Avoid “Are you sure?”

Prefer wording that identifies the item and consequence, such as:

> Delete the template “Member Welcome”? This cannot be undone.

## Accessibility

Accessibility is required.

Use:

- Semantic headings
- Correct form labels
- Keyboard navigation
- Visible focus
- Adequate contrast
- Accessible modal focus management
- Screen-reader-friendly validation and status messages
- `aria-label` for icon-only actions
- `aria-expanded` and `aria-controls` where appropriate
- Table headers
- Meaningful link and button text
- Useful image alternative text
- Empty alternative text for decorative images

Do not use clickable `div` elements where a button or link is appropriate.

Do not communicate state through color alone.

Tooltips supplement labels. They do not replace accessible names.

## Localization

Localize user-facing text when the project supports localization.

Use resource files for:

- Labels
- Buttons
- Headings
- Help text
- Validation
- Errors
- Empty states
- Confirmations
- Tooltips
- Success messages

Do not scatter hard-coded user-facing strings through JavaScript, markup, and controllers.

## Caching

Use stable cache keys that include the correct store, culture, portal, or module scope.

Every cache must have a clear invalidation strategy.

After settings or data changes:

- Clear the affected cache immediately.
- Avoid clearing the entire site cache when a targeted invalidation is possible.
- Never leak cached data between portals.

When saving portal settings, clear the relevant `PortalSettings` cache.

## Logging and Error Handling

Log useful operational context:

- Store ID
- Portal ID
- Module ID
- Tab ID
- User ID
- Operation
- Record ID
- Exception details

Never log secrets, passwords, full tokens, or unnecessary sensitive data.

User-facing messages should be clear and nontechnical. Log technical details separately.

## Performance

Avoid:

- Unbounded result sets
- N+1 database access
- Loading all records when server-side paging is appropriate
- Repeated settings lookups inside loops
- Returning fields the client does not need
- Filtering large datasets only in memory
- Duplicate front-end libraries

Use server-side paging, filtering, and sorting for potentially large datasets.

Add indexes based on actual access patterns.

## Imports and Templates

### Imports

- Validate files before processing.
- Clearly identify accepted formats.
- Validate required headers and columns.
- Provide preview where practical.
- Show row-level errors.
- Separate warnings from fatal errors.
- Avoid unsafe partial imports.
- Report inserted, updated, skipped, and failed counts.
- Support safe retry.
- Prevent duplicate processing where possible.
- Log enough detail for troubleshooting.

### Templates

- Distinguish system and user-created templates.
- Protect built-in templates from accidental overwrite.
- Validate names.
- Sanitize content.
- Provide preview where useful.
- Confirm destructive actions.
- Preserve existing templates during upgrades unless migration is explicitly required.

## JavaScript

Preserve the existing WebForms, Razor/MVC, KnockoutJS, and Bootstrap 4 implementation used by the affected surface.

- Keep API calls and shared client behavior centralized where the neighboring feature already does so.
- Handle loading, empty, success, and error states explicitly.
- Prevent double submission.
- Ignore or cancel stale requests where practical.
- Do not hide server-side authorization assumptions inside UI logic.

Do not introduce Vue, React, TypeScript, a new CSS framework, or a state library unless the task explicitly requests it.

## Code Quality

Code must be readable, consistent, maintainable, and aligned with the repository.

Avoid:

- Unnecessary duplication
- Deep nesting
- Large controller methods
- Magic strings and numbers
- Swallowed exceptions
- Empty catch blocks
- Commented-out code
- Debug output
- Dead code
- Unused imports
- Premature abstraction

Comments should explain why something is necessary.

## Backward Compatibility

Before changing an existing table, route, API, setting key, template, or client-side model, determine whether it is already used elsewhere.

Prefer additive changes.

For migrations:

- Preserve old values during transition.
- Make migration repeatable.
- Do not silently discard data.
- Document compatibility impact.

## Scheduled Work

When adding scheduled processing:

- Use the project’s established scheduler pattern.
- Register through the existing upgrade mechanism where applicable.
- Prevent duplicate registrations.
- Make processing idempotent.
- Log start, completion, counts, and failures.
- Use reasonable batches.
- Avoid long locks.
- Keep processing portal-aware where required.

Do not change package versions solely to register a scheduler unless requested.

## Testing Expectations

Validation must be proportionate to the requested change and the risks it introduces. The lists below are a review matrix, not a requirement to execute every item for every change. State exactly which checks were applicable, which were performed, and which require a configured DNN/IIS/SQL environment.

Existing test projects use Visual Studio QualityTools/MSTest-era infrastructure and currently target .NET Framework 4.7.2. Match the affected test project. Do not introduce NUnit, Moq, or another test framework without explicit approval.

When applicable, review or test:

### Installation

- Fresh install
- Module placement
- Initial render
- First-use configuration
- Permissions
- SQL object creation
- Resource loading

### Upgrade

- Upgrade from the prior supported version
- Repeat upgrade execution
- Data preservation
- Settings preservation
- Scheduler registration where applicable

### Functionality

- Create
- Read
- Update
- Delete
- Search
- Filters
- Sorting
- Paging
- Validation
- Refresh
- Empty state
- Error state
- Unauthorized access

### Scope

- Multiple stores
- Multiple portals
- Multiple module instances
- Different roles and users
- Administrator
- Superuser
- Anonymous access where applicable

### Responsive and accessible use

- Desktop
- Tablet
- Mobile
- Keyboard-only interaction
- Focus visibility
- Screen-reader labels
- Validation
- Modal behavior
- Icon-only actions

### Uninstallation

- All module-owned objects removed
- No shared objects removed
- No dependency-order failures

State exactly what was actually tested. Never claim a build, installation, or runtime test passed unless it was performed.

## Delivery

Unless the task says otherwise:

- Use repository-relative paths.
- Include every changed, added, and removed file.
- Include manifest, resource, SQL, project, and packaging changes.
- Avoid multiple unnecessary copies of the same source or package.
- Replace superseded generated packages where possible.
- Preserve encoding and line-ending conventions.
- Exclude temporary build output, local environment files, and secrets.
- Exclude unrelated changes.

For patches, ensure they apply from the repository root.

Summarize:

1. What changed
2. Why it changed
3. Files changed
4. Database impact
5. Configuration impact
6. Cache behavior
7. Install or upgrade impact
8. Testing performed
9. Risks or limitations

## Client-Facing Language

In customer-facing or public-facing text:

- Prefer plain business language.
- Avoid internal platform terminology.
- Avoid exposing module IDs, database terms, framework names, or implementation details.
- Prefer terms such as “website,” “site,” “portal,” “management area,” “settings,” and “website administrator.”

## Prohibited Changes Without Explicit Approval

Do not:

- Upgrade DNN or .NET Framework
- Convert the solution to modern .NET
- Replace WebForms merely because it is old
- Replace Vue with React
- Add Material UI
- Add a new CSS framework
- Add a new ORM
- Replace stored procedures with an ORM
- Rewrite module architecture
- Change package or assembly versions
- Rename database objects without migration
- Store portal settings in `web.config`
- Rely only on client-side authorization
- Remove existing compatibility behavior
- Change proven module bootstrap code unnecessarily
- Hard-code portal IDs unless the task is explicitly site-specific
- Use paid icon libraries unless already licensed
- Create unnecessary source copies or duplicate packages
- Claim testing that was not performed
- Guess about unreadable attachments

## Completion Checklist

Before declaring work complete, verify:

- The module follows proven repository patterns.
- The module can load correctly in the target runtime.
- Every API method has appropriate authorization.
- Store, portal, module, user, and record scope are validated server-side where applicable.
- Settings use the correct scope.
- Relevant caches are cleared after writes.
- User-facing strings are localized where required.
- Mobile behavior is intentional.
- Icon-only actions are accessible.
- Destructive actions require clear confirmation.
- SQL is repeatable where requested.
- Upgrade logic is safe and idempotent.
- Uninstall removes all module-owned objects.
- The manifest and package include every required file.
- Existing data and users are protected.
- Errors are logged without exposing secrets.
- Delivery is complete and contains no unrelated changes.

## Repository Confidentiality

This repository is private and contains proprietary source code.

- Do not publish, upload, paste, or transmit repository content to public services, public issues, public pull requests, public snippets, or public repositories.
- Do not change the repository visibility.
- Do not add external integrations or grant repository access without explicit approval.
- Do not expose secrets, credentials, customer information, private URLs, or proprietary source in logs, summaries, patches, screenshots, or generated documentation.
- Do not push, create pull requests, publish packages, deploy, or share artifacts unless explicitly authorized.
- Use only the existing local repository, approved MCP tools, and approved private remotes.
