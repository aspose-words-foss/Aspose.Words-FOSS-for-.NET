# Aspose.Words FOSS for .NET

Open-source .NET library for Word documents, built from the genuine Aspose.Words codebase — the same document engine that has processed Word documents in production since 2003, carefully reduced to a free edition. Create, read, and edit DOCX, convert to Markdown and plain text. No Microsoft Word required.

Licensed under [MIT](https://github.com/aspose-words-foss/Aspose.Words-FOSS-for-.NET/blob/master/LICENSE): free for commercial and personal use, no license keys, no evaluation watermarks.

## Installation

```bash
dotnet add package Aspose.Words.FOSS
```

Or via the NuGet Package Manager console:

```powershell
Install-Package Aspose.Words.FOSS
```

## Quick example

```csharp
using Aspose.Words;

// Create a document from scratch.
Document doc = new Document();
DocumentBuilder builder = new DocumentBuilder(doc);
builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
builder.Writeln("Hello from Aspose.Words FOSS!");
doc.Save("Hello.docx");

// Convert an existing document to Markdown.
new Document("Report.docx").Save("Report.md");
```

The API is source-compatible with the commercial [Aspose.Words for .NET](https://products.aspose.com/words/net/), so the [official documentation](https://docs.aspose.com/words/net/) largely applies within the supported feature set.

## What it can do

- Create documents from scratch with the full document object model or the high-level `DocumentBuilder`.
- Read and edit existing DOCX files: text, formatting, styles, tables, lists, sections, headers/footers, bookmarks, comments, footnotes, shapes.
- Convert between DOCX, Markdown, and plain text.
- Append, clone, and import content between documents; find and replace with regex; inspect, accept, or reject tracked changes; update fields; protect documents; round-trip DOCM/DOTM with VBA intact; verify digital signatures.

**Supported formats:** DOCX / DOCM / DOTX / DOTM / Flat OPC (load and save), Markdown (load and save), plain text (load and save).

## Targets and dependencies

| Target | Runs on | Dependencies |
|---|---|---|
| `net462` | .NET Framework 4.6.2+ (Windows) | none — uses GDI+ |
| `netstandard2.0` | .NET Framework 4.6.2+, .NET 6+, Mono | SkiaSharp, BitMiracle.LibTiff.NET |
| `net8.0` | .NET 8, 9, 10 | SkiaSharp, BitMiracle.LibTiff.NET |

On **Linux**, also add the [SkiaSharp.NativeAssets.Linux](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Linux) package (standard SkiaSharp practice — native binaries for Linux ship separately).

## What's not included

To keep this edition free, the subsystems that power Aspose's commercial offering were removed. To be upfront about it:

- **Page layout and rendering**: no saving to PDF, XPS, or images, and no printing. Layout-dependent field values (page numbers in a TOC, `NUMPAGES`) evaluate to `0`.
- **Additional formats**: DOC, RTF, ODT, HTML, EPUB and others are not read or written.
- **Mail merge, LINQ Reporting, document comparison, creating digital signatures, embedded-font subsetting.**
- On the cross-platform targets, WMF/EMF metafiles cannot be rasterized to bitmaps (the `net462` GDI+ path handles this).

The boundary is not set in stone — if a feature you need sits on the wrong side of it, [open an issue](https://github.com/aspose-words-foss/Aspose.Words-FOSS-for-.NET/issues). Where there is real demand, we will consider opening up more of the code. If you need everything today, the commercial [Aspose.Words for .NET](https://products.aspose.com/words/net/) has it all, and because this library *is* that codebase, upgrading is essentially swapping the package reference.

## Build from source

The full source, including the test suite, is on GitHub:

```bash
git clone https://github.com/aspose-words-foss/Aspose.Words-FOSS-for-.NET.git
cd Aspose.Words-FOSS-for-.NET
dotnet build Aspose.Words.sln -c Release
```

This produces the same assemblies the package ships (plus per-assembly outputs for debugging). Run the test suite with `dotnet test Aspose.Words.Tests/Aspose.Words.Tests.csproj -c Debug`.

## Issues and support

Found a bug or need a feature? [Open an issue](https://github.com/aspose-words-foss/Aspose.Words-FOSS-for-.NET/issues) — bug reports with a reproducing document are especially appreciated. Pull requests are considered case by case; for anything non-trivial, open an issue first to discuss the approach.

If the library is useful to you, a [star on GitHub](https://github.com/aspose-words-foss/Aspose.Words-FOSS-for-.NET) helps others discover it.

## License

[MIT](https://github.com/aspose-words-foss/Aspose.Words-FOSS-for-.NET/blob/master/LICENSE). Free for commercial and personal use.
