# Problems

## Tiny snippets

- Code duplication should detect larger snippets
- Short snippets detection irritates users

## Domain Rules that look alike but differ subtly

- Some rules are very similar serve different purposes
- Detecting these similarities as duplicates may lead to missing important distinctions

## Performance optimized code

- Extraction of performance-optimized code may lead to inefficiencies

## Boilerplate code defined by frameworks

- DI, ORM and ASP.NET endpoints often have boilerplate code
- These may be necessary and not considered duplicates

## AAA testing pattern

- Arrange, Act, Assert pattern in tests may lead to false positives

