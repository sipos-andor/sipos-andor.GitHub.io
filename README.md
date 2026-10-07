# andor.sipos.io

The online CV of Andor Sípos, founder and technical lead of [operandor](https://operandor.io/), in English, Hungarian,
Croatian and Serbian, with downloadable PDF, DOCX, ATS-friendly, Markdown and JSON Resume versions.

The site is built from the JSON Resume files in [`content/`](content) by
[resume-engine](https://github.com/sipos-andor/resume-engine): prerendered static pages in the operandor design
system (light and dark), the documents, `sitemap.xml`, `robots.txt` and `llms.txt`. GitHub Pages serves the result.

## Layout

| Path | What it is |
|---|---|
| `content/resume.{en,hu,hr,sr-Latn}.json` | The CV in each language; every file a complete JSON Resume with the engine's `x-` extensions. |
| `content/site.json` | The site's origin, default language, language order, download names and storage keys. |
| `src/AndorCv.Build` | The build program: the engine with the operandor theme and the PDF writer. |
| `tests/AndorCv.Content.Tests` | Guards on the content: it loads in four languages, is valid JSON Resume, keeps the agreed facts and holds no e-mail address. |
| `.github/workflows/pages.yml` | Tests, builds and deploys `master` to GitHub Pages; pull requests are only tested and built. |

## Editing the CV

Change the English file and the same item in the three translations: the build compares the languages and fails
when an identifier, a date, a company, a URL, a technology, a level or the number of items differs.

No file may contain an e-mail address. The PDFs show one, drawn as an image, from the `CV_EMAIL` secret; nothing
else does.

## Building locally

The engine and the design system are GitHub Packages, which need a token even to read. Give NuGet a personal access
token with `read:packages` through an environment variable, so it is never written into `nuget.config` or any other
file of the repository (read the token without echoing it, so it stays out of the shell history):

```sh
read -rs GITHUB_PACKAGES_TOKEN
export NuGetPackageSourceCredentials_github="Username=<you>;Password=$GITHUB_PACKAGES_TOKEN"
dotnet test AndorCv.slnx
dotnet publish src/AndorCv.Build -c Release -o build
dotnet build/AndorCv.Build.dll --content content --output site --clean
```

`site/` is then the whole site; serve it with any static file server.

## Settings the deployment needs

- Secrets `CV_EMAIL` (the address the PDFs show as an image) and `CF_WEB_ANALYTICS_TOKEN` (Cloudflare Web Analytics).
- Settings → Pages → Source: GitHub Actions; custom domain `andor.sipos.io`, Enforce HTTPS.
- Read access for this repository on the `Sipos.Resume.*` and `Operandor.*` packages (package settings, Manage
  Actions access).

## Licence

The code is MIT ([LICENSE](LICENSE)). The CV's content in `content/` is not: all rights reserved
([content/LICENSE](content/LICENSE)).
