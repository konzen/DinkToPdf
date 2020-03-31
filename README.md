# DinkToPdf
.NET Standard 2.0 P/Invoke wrapper for wkhtmltopdf library that uses WebKit rendering engine to convert HTML pages to PDF.

### Install 

Library can be installed through NuGet. Run command bellow from the package manager console:

```
PM> Install-Package DinkToPdf.Standard
```

On Linux or macOS install the wkhtmltopdf package on the OS.

### NuGet package
https://www.nuget.org/packages/DinkToPdf.Standard

### Basic converter
Use this converter in single threaded applications.

Create converter:
```csharp
var converter = new BasicConverter(new PdfTools());
```

### Synchronized converter
Use this converter in multi threaded applications and web servers. Conversion tasks are saved to blocking collection and executed on a single thread.

```csharp
var converter = new SynchronizedConverter(new PdfTools());
```

### Define document to convert
```csharp
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Landscape,
        PaperSize = PaperKind.A4Plus,
    },
    Objects = {
        new ObjectSettings() {
            PagesCount = true,
            HtmlContent = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. In consectetur mauris eget ultrices  iaculis. Ut                               odio viverra, molestie lectus nec, venenatis turpis.",
            WebSettings = { DefaultEncoding = "utf-8" },
            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
        }
    }
};

```

### Convert
If Out property is empty string (defined in GlobalSettings) result is saved in byte array. 
```csharp
byte[] pdf = converter.Convert(doc);
```

If Out property is defined in document then file is saved to disk:
```csharp
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Portrait,
        PaperSize = PaperKind.A4,
        Margins = new MarginSettings() { Top = 10 },
        Out = @"C:\DinkToPdf\src\DinkToPdf.TestThreadSafe\test.pdf",
    },
    Objects = {
        new ObjectSettings()
        {
            Page = "http://google.com/",
        },
    }
};
```
```csharp
converter.Convert(doc);
```

### Dependency injection
Converter must be registered as singleton.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add converter to DI
    services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
}
```
