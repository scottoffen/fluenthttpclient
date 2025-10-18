# Contributing to FluentHttpClient

Community feedback is used to inform the direction of the project, and we absolutely want to hear from you! **Your involvement makes the project better** by increasing the usability, quality and adoption of the project. Our goal is to be better stewards by being more responsive and transparent. Following the contribution guidelines outlined below will help us to better help you.

# How To Get Help

Issues are often created that are not bugs or feature requests, but rather questions or discussions - most of which could be answered by anyone in the community; they're not exclusive to the maintainers. For feedback like this, there are several places where people can get all kinds of help, and we would encourage you to use them first.

- Consult the [official documentation](https://scottoffen.github.io/fluenthttpclient).
- Engage in our [community discussions](https://github.com/scottoffen/fluenthttpclient/discussions).

To avoid any misunderstanding, let us state this policy in no uncertain terms: **issues that are created to ask usage questions will be closed.**

# Getting Off To A Good Start

Regardless of whether you are opening a bug report, asking a question on StackOverflow, or getting in on the GitHub discussions, there is information you should always include to ensure you get the results you need. If you don't provide it up front, you will likely be asked for it before you can get any traction on your request.

- Have a descriptive title and a clear description.
- Include the details of the operating system and version, version of .NET and the version of FluentHttpClient you are using.
- If it's an interoperability problem, don't forget to include information about the other "things" you are using, e.g. logging frameworks, dependency injection libraries, IDE, etc.
- Show the minimum amount of code needed to illustrate the problem or demonstrate the behavior.
- Where applicable, consider including a screenshot or a link to your repository where the code can be examined in context.

# Feature Requests

Got an idea on how to make FluentHttpClient better? Start or join a conversation in our [community discussions](https://github.com/scottoffen/fluenthttpclient/discussions) and suggest your change there. **Do not open an issue on GitHub until** you have collected positive feedback about the change. Where it comes to improvements, we want to ensure we are focused on solving problems, not attacking symptoms, while remaining focused on our guiding principles: fast, unopinionated, and minimalist. In a word: simple.

If it is decided that your idea would be a good inclusion to the core FluentHttpClient project, create a feature request issue based on the outcome of the community discussion.

# Issue Management

GitHub issues are reserved for things that can be fixed, added, resolved, or implemented. Here are some guidelines we use in order to manage incoming issues in the most efficient way.

## Issues That Can't or Won't Be Fixed

There are classes of things that get reported that are undefined, indistinct, or out of scope, and as such they are inactionable. When a report comes in that looks like this, we'll ask the original submitter of the issue to clarify what "done" would look like to them. We can and will help with this process if you're unsure. But if the goal remains undefined or is unachievable (e.g. outside the scope or vision of the project), we'll close the issue.

## Abandoned Issues

Occasionally, the maintainers can't get the information they need to resolve something, and as a result an issue just never moves forward. We get it, we're all busy. Maybe the original submitter has moved on or just forgotten. In order to focus our attention in the right places, we will mark issues with the `more-information-needed` label when the maintainers have a question. If we don't receive a response from the original submitter within a week, we'll give a gentle reminder. If we still haven't received a response within 30 days, we will close the issue.

# Coding Conventions

FluentHttpClient is written strictly in C#. The repository includes an [`.editorconfig`](https://editorconfig.org/) file to manage indentation styles as well as line endings, etc.

- As a guiding principle, we aim for [readable code](https://www.amazon.com/Art-Readable-Code-Practical-Techniques/dp/0596802293) above following a convention.
- For C# we ask that you follow the [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) published by Microsoft.
    - We may, at our discretion, override these conventions where we feel it conflicts with readable code.
- Added or updated unit tests are required for any modified or added code.
    - We use [Xunit](https://www.nuget.org/packages/xunit), [Shouldly](https://www.nuget.org/packages/Shouldly) and [Moq](https://www.nuget.org/packages/Moq).
    - For the sake of maintainability, your contributions will be rejected if you do not use them as well.

# Pull Requests

When writing a pull request

- Have a descriptive title
- Include a clear list of what you've done.
- Make sure to include or update test coverage.
- Make sure all of your commits are atomic (one feature per commit).

## Purely Cosmetic Pull Requests

Changes that are cosmetic in nature and do not add anything substantial to the stability, functionality, or testability of the project will generally not be accepted. There are a lot of hidden costs in these kinds of pull requests. These include but are not limited to:

- Someone needs to review those changes
- It creates a lot of notification noise
- It pollutes the git history

> [!IMPORTANT]
> Sometimes, your editor may completely reformat a file when you save it, making numerous white space changes that, while they don't affect the code, create a lot of noise for the reviewers. If this happens, the PR will not be considered until those white space changes are reverted.

# Documentation Changes

Having excellent documentation is crucial to the success of FluentHttpClient. Documentation for FluentHttpClient is written using [Docusaurus](https://docusaurus.io/), and lives in [docs folder](./docs/). Your contribution of clear, concise and accurate documentation is appreciated.