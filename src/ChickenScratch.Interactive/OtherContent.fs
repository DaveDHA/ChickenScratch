namespace ChickenScratch

open System.IO
open Microsoft.DotNet.Interactive.Formatting
open Microsoft.DotNet.Interactive.Formatting.TabularData


module ScratchHtml =
    let UseFormatter<'t> (target : 't) =
        use stringWriter = new StringWriter()
        use fmtContext = new FormatContext(stringWriter)
        Formatter.FormatTo(target, fmtContext, "text/html") |> ignore
        stringWriter.ToString() |> RawContent


    let UseTabularFormatter<'t> (target : 't seq) =
        target.ToTabularDataResource() |> UseFormatter

