# Contributing to Cats Compression

Thanks for helping out! Bug reports, ideas and pull requests are all welcome.

## Reporting a bug or suggesting a feature

Open an [issue](https://github.com/Ushmayne/File-Compression-Utility/issues/new/choose) and pick **Bug report** or **Feature request**. For bugs, the most useful things to include are your Windows version, the app version (shown in the exe's *Properties > Details*), and the exact error message.

Found a security problem? Please don't open a public issue. See [SECURITY.md](SECURITY.md) instead.

## Setting up

You need Windows 10 or 11 and the [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

```bash
git clone https://github.com/Ushmayne/File-Compression-Utility.git
cd File-Compression-Utility
dotnet run --project CatsCompression   # start the app
dotnet test                            # run the tests
```

You can also open `CatsCompression.sln` in Visual Studio 2022 or VS Code.

## Where things are

| Path | What it is |
|---|---|
| `CatsCompression/FileCompUtility.cs` | All the compress and extract logic. No UI code. |
| `CatsCompression/MainForm.cs` | The window: buttons, file pickers and messages. |
| `tests/CatsCompression.Tests/` | Automated tests for `FileCompUtility`. |
| `.github/workflows/` | CI (tests on every push) and the release build. |

## Making a change

1. Fork the repo and create a branch from `main`.
2. Make your change. Keep compression logic in `FileCompUtility` and UI code in `MainForm`, so the logic stays testable.
3. Add or update tests for anything that changes behavior, then run `dotnet test`. All tests must pass.
4. If you changed something users see, update the README too.
5. Open a pull request that explains what changed and why. The tests run automatically on every pull request.

Keep pull requests focused on one thing. Small ones get reviewed faster.

## Releases

Maintainers publish releases by pushing a version tag (for example `git tag v1.1.0` then `git push origin v1.1.0`). GitHub Actions then builds both exes and posts them on the Releases page.

## License

By contributing, you agree that your contributions are released under the project's [MIT License](LICENSE).
