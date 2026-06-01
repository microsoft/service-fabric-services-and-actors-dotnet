---
description: "Guidelines for C# XML documentation comments, NuGet package descriptions, and package README files."
applyTo: "{**/*.cs,src/**/README.md,src/**/*.csproj}"
---

- **Read the `coding.instructions.md` first**.
  Instructions in this file are incomplete without them.

# Documentation guidance

- Write for an intermediate to advanced C# developers.
- Write documentation for the users, not the maintainers.
- Make documentation as concise as possible without losing information.
- Change phrasing to drop redundant words, for example `Returns red apples.` over `Returns apples of red color.`
- Focus on the "why". The "how" should be self-explanatory from the API itself.
- Always try to simplify APIs before resorting to documenting them.

## NuGet packages

### Description (.csproj)

- Start with what the package provides, not "This package contains...".
- Keep it to one sentence — NuGet.org truncates long descriptions in search results.

### README.md

- Use the package ID as the heading.
- Structure: purpose, key types, usage example, documentation links.
- Keep examples minimal — just enough to show how to get started.
- Mark internal packages (`*.Internal`) as not intended for direct consumption.

## C# XML comments

- Document `public` and `protected` members with [XML comments](https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/documentation-comments).
- Document internal members only if they are complex or not self-explanatory.
- Verify XML comments by building the project before committing changes.
- When removing unnecessary XML comments, preserve significant information as regular comments.
- Use `<summary>` to provide a brief, one sentence, description of what the type or member does. Start the summary with a present-tense, third-person verb.
- Use `<remarks>` for additional information, such as usage notes relevant to consumers.
  - Remove `<remarks>` if it restates documented-elsewhere or self-evident information. This is an optional element.
  - Don't document implementation details of the API not relevant to consumers of the library.
  - Don't document inheritor instructions for APIs not meant to be extended by consumers of the library.
- Use `<see langword="..."/>` for language-specific keywords like `null`, `true`, `false`, `int`, `bool`, etc.
- Use `<c>` for inline code snippets.
- Use `<example>` for usage examples on how to use the member.
  - Use `<code>` for code blocks. `<code>` tags should be placed within an `<example>` tag.
    Add the language of the code example using the `language` attribute, for example, `<code language="csharp">`.
- Use `<see cref="..."/>` to reference other types or members inline (in a sentence).
- Use `<seealso cref="..."/>` for standalone (not in a sentence) references to other types or members in the "See also" section of the online docs.
- Reuse documentation from other types and members to keep duplication to a minimum.
  - Use `<inheritdoc/>` for interface implementations and overrides unless there is a major behavior change.
  - Use `<inheritdoc cref="..."/>` to reuse documentation from related types, like the base type, and members, like method overloads.
  - Document differences instead of using `<inheritdoc/>` when there is a major behavior change.
- Use `<para>` when multiple paragraphs are needed to make a documentation section readable.
  - Never use `<para>` in single-paragraph sections.
- For generic overloads, don't add "strongly-typed" or similar qualifiers to distinguish them from non-generic overloads. Let the type parameter references speak for themselves.
- Prefer short type names in the `cref="..."` references. If the type's namespace is not imported, add a `using` directive rather than using a fully-qualified name.

### Methods

- For methods that return a value, the `<summary>` should start with "Returns" and explain the result rather than how it is obtained.
- For void methods, the `<summary>` should describe the action the method performs.
- Use `<returns>` to describe what the method returns, but only if it's not redundant.
  - Remove `<returns>` if it restates documented-elsewhere or self-evident information. This is an optional element.
  - Try improving the method name and return type before documenting it.
  - The description should be a noun phrase that doesn't specify the data type.
  - Begin with an introductory article.
  - If the return type is Boolean, the wording should be of the form "`<see langword="true" />` if ...; otherwise, `<see langword="false" />`.".

### Parameters

- Use `<param name="...">` to describe method parameters, but only if they're not redundant.
  - The description should be a noun phrase that doesn't specify the data type.
  - Begin with an introductory article.
  - If the parameter is a flag enum, start the description with "A bitwise combination of the enumeration values that specifies...".
  - If the parameter is a non-flag enum, start the description with "One of the enumeration values that specifies...".
  - If the parameter is a Boolean, the wording should be of the form "`<see langword="true" />` to ...; otherwise, `<see langword="false" />`.".
  - If the parameter is an "out" parameter, the wording should be of the form
    `When this method returns, contains .... This parameter is treated as uninitialized.`.
- Try improving parameter types and names before documenting them.
- Remove all `<param>` elements if they restate information evident from parameter types and names.
  This is an optional element, but if one parameter is documented, all must be documented as well.
- Use `<paramref name="...">` to reference parameter names in documentation.

### Type Parameters

- Use `<typeparam name="...">` to describe type parameters in generic types or methods, but only if they're not redundant.
  - Remove all `<typeparam>` elements if they restate documented-elsewhere or self-evident information. This is an optional
- Use `<typeparamref name="...">` to reference type parameters in documentation.

### Constructors

- The summary wording should be "Initializes a new instance of the <see cref="..."/> class (or struct).".
- Don't create default constructors just to document them.

### Properties

- The `<summary>` should start with:
  - "Gets or sets..." for a read-write property.
  - "Gets..." for a read-only property.
  - "Sets..." for a set-only property.
  - "Gets [or sets] a value that indicates whether..." for properties that return a Boolean value.
- Use `<value>` to describe the value of the property, but only if it's not redundant.
  - Remove `<value>` if it restates documented-elsewhere or self-evident information. This is an optional element.
  - The description should be a noun phrase that doesn't specify the data type.
  - If the property has a default value, add it in a separate sentence, for example, "The default is `<see langword="false" />`".
  - If the value type is Boolean, the wording should be of the form "`<see langword="true" />` if ...; otherwise, `<see langword="false" />`. The default is ...".

### Events

- The `<summary>` should start with "Occurs when...".

### Exceptions

- Use `<exception cref="...">` to document exceptions thrown by constructors, properties, indexers, methods, operators, and events.
- Document all exceptions thrown directly by the member.
- For exceptions thrown by nested members, document only the exceptions users are most likely to encounter.
- Describe the condition under which it's thrown.
  - Omit "Thrown if ..." or "If ..." at the beginning of the sentence.
