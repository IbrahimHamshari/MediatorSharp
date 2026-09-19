# Library namespace and public surface

Every public type lives in the flat `MediatorSharp` namespace, the assembly and package id
are `MediatorSharp`, and the projects/folders are `MediatorSharp`,
`MediatorSharp.Sample`, and `MediatorSharp.Tests`. We chose one short, product-name
namespace over matching the original `Ibrashira.*` prefix, because consumers write the
`using` in every file and the package id is discovered once. The `IMediator`, `IResult`,
`IResult<T>`, and `IError` interfaces are part of the public surface: they are the
contract that `Result`, `Result<T>`, and `Mediator` implement, so consumers must be able
to name them.

Because the root namespace is `MediatorSharp`, no first-party project may itself be
rooted in a `MediatorSharp.*` namespace that would shadow the library's types for a
different project — the compiler resolves `using MediatorSharp;` against the enclosing
namespace first. The sample and test projects are therefore rooted at
`MediatorSharp.Sample` and `MediatorSharp.Tests` and reference the library with `using
MediatorSharp;`.

## Considered Options

- **Keep the `Ibrashira.*` prefix on projects and package id.** Rejected: it forces a
  long `using` and a mismatched product name in every consumer file.
- **Root `MediatorSharp` with `.Interfaces` and `.Models` sub-namespaces.** Rejected: the
  library's whole surface is a handful of types; the sub-namespaces add friction without
  organizing anything.

## Consequences

- This is a breaking change to the previously shipped `MediatorSharp.lib.*` namespace and
  `Ibrashira.MediatorSharp` package id.
- The package is published to GitHub Packages (`https://nuget.pkg.github.com/IbrahimHamshari/index.json`)
  using the workflow's `GITHUB_TOKEN`; consumers must authenticate to that feed to restore it.
