# Aspose.Words FOSS for .NET

Open-source .NET library for Word documents, built from the genuine Aspose.Words codebase. Create, read, and edit DOCX, convert to Markdown and plain text. No Microsoft Word required.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

## What it can do

- **Create documents from scratch** with the full document object model or the high-level `DocumentBuilder`.
- **Read and edit existing DOCX files**: text, formatting, styles, tables, lists, sections, headers and footers, bookmarks, comments, footnotes, shapes.
- **Convert** between DOCX, Markdown, and plain text.
- **Combine and reorganize documents**: append, clone, and import content between documents.
- **Find and replace** text with regular expressions and formatting-aware options.
- **Work with tracked changes**: inspect, accept, or reject revisions.
- **Update fields**: the full field evaluation engine is included, though values that depend on page layout (such as page numbers in a TOC) come out as placeholders (see [what's not included](#whats-not-included-and-where-to-get-it)).
- **Protect documents** and round-trip macro-enabled files (DOCM/DOTM) with their VBA projects intact.
- **Verify digital signatures**: check whether a DOCX is signed and untampered, inspect the certificates, or remove signatures. (Creating new signatures is not included in this edition.)

The library is pure managed C# targeting **.NET Standard 2.0**, so it runs on .NET Framework 4.6.2+, .NET 6/8/10, Windows, Linux, and macOS. No native dependencies, no Office automation.

## Supported formats

| Format | Load | Save |
|---|:---:|:---:|
| DOCX / DOCM / DOTX / DOTM | ✅ | ✅ |
| Flat OPC (all variants) | ✅ | ✅ |
| Markdown | ✅ | ✅ |
| Plain text | ✅ | ✅ |

The engine underneath already knows how to handle far more (see [the story](#the-story-behind-this-code) below), and more formats may be opened over time. If you need a format that is not in the table, you have two options: [open an issue](../../issues) to request it, and we will consider open-sourcing it, or get it right away in the commercial [Aspose.Words for .NET](https://products.aspose.com/words/net/), which reads and writes all of them today.

## Getting started

A NuGet package is coming very soon. Until then, build from source:

```bash
git clone https://github.com/aspose-words-foss/Aspose.Words-FOSS-for-.NET.git
cd Aspose.Words-FOSS-for-.NET
dotnet build Aspose.Words.sln -c Release
```

Then add a project reference to `Aspose.Words.csproj` from your application.

### A quick example

```csharp
using Aspose.Words;

// Create a document from scratch.
Document doc = new Document();
DocumentBuilder builder = new DocumentBuilder(doc);

builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
builder.Writeln("Hello from Aspose.Words FOSS!");

builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.BodyText;
builder.Writeln("This document was created entirely in code, no Word installed.");

doc.Save("Hello.docx");

// Convert an existing document to Markdown.
Document report = new Document("Report.docx");
report.Save("Report.md");
```

The API is the same as in the commercial Aspose.Words for .NET, so the [official documentation and examples](https://docs.aspose.com/words/net/) largely apply here too, within the supported feature set.

## What's not included (and where to get it)

To keep this edition free, the subsystems that power Aspose's commercial offering were removed. To be upfront about it:

- **Page layout and rendering.** No saving to PDF, XPS, or images, and no printing. This also means layout-dependent field values (such as page numbers in a TOC or `NUMPAGES`) evaluate to `0` rather than being computed.
- **Additional formats.** DOC, RTF, ODT, HTML, EPUB, and others are not read or written in this edition.
- **Mail merge, LINQ Reporting, document comparison, document signing, and embedded-font subsetting** are not included.

One important thing to know: what's included today is the *minimum*, the commercial codebase reduced as far as it would go. It is not the final shape of the project, and the boundary is not set in stone. If a feature you need sits on the wrong side of it, [open an issue](../../issues) and tell us about your use case. Where there is real demand, we will consider opening up more of the code.

If your project grows into needing any of the above, the commercial [Aspose.Words for .NET](https://products.aspose.com/words/net/) has all of it. And because this library *is* that codebase, upgrading is essentially swapping the package reference. Your code carries over.

## The story behind this code

Most open-source Word libraries start from zero and work their way up the OOXML spec. This one went the other way. It is not a rewrite or a wrapper: it is the actual Aspose.Words for .NET source code, the same document engine that has processed Word documents in production since 2003.

Aspose.Words has been in continuous development for over two decades, and it is older than DOCX itself. The library began with manual reverse engineering of the binary DOC format, at a time when no public specification existed. Then came WordML, the Word 2003 XML dialect, then OOXML when Word 2007 arrived, and one format after another in the years since. To create this FOSS edition, we started from the full commercial source and carefully reduced it: the commercial subsystems (page layout, rendering, the long tail of format converters, and the licensing machinery) were removed, along with internal infrastructure and materials not meant for publication. What remains is the genuine core: the document model, the DOCX reader and writer, and thousands of small fixes for real-world documents that only accumulate after twenty years of production use.

The reduction itself was an interesting engineering project. To carry it out, we built an internal tool, the Full2Foss agent, developed with the help of [Claude](https://claude.com), Anthropic's AI assistant. Full2Foss traces which subsystems each piece of code depends on, cuts what has to go, and keeps thousands of tests honest along the way.

That is also why the source is worth a read. This is production code, not a demonstration. If you have ever wondered what it actually takes to handle Word documents correctly, it is all in here.

## Enjoying it? Star it ⭐

This project is brand new, and stars are how GitHub decides whether anyone else gets to discover it. If Aspose.Words FOSS looks useful to you:

- **Star** the repo. It takes one click and helps more than anything else.
- **Watch** it (Releases only is fine) to hear about new features first. The NuGet package and new formats are landing soon.
- **Tell someone.** A mention in a blog post, a team chat, or an answer on Stack Overflow means a lot to a young project.

## Contributing

Bug reports and feature requests are very welcome, please [open an issue](../../issues). Pull requests are considered case by case; for anything non-trivial, it is best to open an issue first so we can discuss the approach.

## License

[MIT](LICENSE). Free for commercial and personal use.
