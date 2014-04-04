# Contributing
The following highlight some guidelines for contributing to Crichton.NET and submitting pull requests.

## Background
At the heart of Crichton is the [ALPS specification](http://alps.io/spec/index.html). It is important that
functionality that impacts Crichton's resource descriptors conforms to and preserves the existing ALPS-related
implementations so that profiles can be properly generated and referenced.

## Guidelines
Crichton.NET aspires follow ideas set out by Bob Martin in [Clean Code](http://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882).
As such, the following are some guidelines to think about as you code:

### Code should follow S.O.L.I.D principles
* See [http://en.wikipedia.org/wiki/SOLID_%28object-oriented_design%29]

### Every file should be easy to read.
* Use pronounceable, meaningful names that reveal intentions.
* Code should read like a top-down narrative starting with required modules to make things easy to find.
* Use three or less method arguments.
* Stay DRY and keep methods, classes and modules sizes small.

### Only add comments that actually add clarification.
* If you are explaining bad code, fix the code.
* Aspire to self-documenting variable and method names.

### Only do one thing.
* Methods should call other methods vs. writing larger methods that violate SRP.
* Every method should be followed by any methods it calls (as the next level of abstraction).

### Unit Tests and Specs should be F.I.R.S.T.
* Fast - run quickly.
* Independent - not rely on previous tests.
* Repeatable - work in any environment.
* Self-validating - examples are written to document what passes.
* Timely - Follow TDD/BDD principles.

## Pull Requests
(.NET specific Pull Request guide coming soon)

