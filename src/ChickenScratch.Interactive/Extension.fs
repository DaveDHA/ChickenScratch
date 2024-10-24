module ChickenScratch.Interactive.Extension

open ChickenScratch
open Microsoft.DotNet.Interactive
open Microsoft.DotNet.Interactive.Formatting
open System.Reflection


let private registerScratchNodeFormatter() =
    Formatter.Register((fun (n : ScratchNode) -> n.AsHtmlString), mimeType = "text/html")


let private registerScratchElementFormatter() =
    Formatter.Register((fun (e : ScratchElement) -> e.AsHtmlString), mimeType = "text/html")


let private registerScratchNodeSourceFormatter() = 
    let formatScratchNodeSource (source : IScratchNodeSource) = 
        source.GetScratchNodes() 
        |> List.map(fun node -> node.AsHtmlString) 
        |> String.concat "\n"

    Formatter.Register<IScratchNodeSource>(formatScratchNodeSource, mimeType = "text/html")


let private registerScratchNodeSourceSeqFormatter() =
    let fmt (source : IScratchNodeSource seq) = 
        source 
        |> Seq.collect(fun nodeSource -> nodeSource.GetScratchNodes()) 
        |> Seq.map(fun node -> node.AsHtmlString) 
        |> String.concat "\n"

    Formatter.Register<IScratchNodeSource seq>(fmt, mimeType = "text/html")


let Load() = 
    registerScratchNodeFormatter()
    registerScratchElementFormatter()
    registerScratchNodeSourceFormatter()    
    registerScratchNodeSourceSeqFormatter()

    try
        let style = ScratchStyle.FromResource (Assembly.GetExecutingAssembly()) "ChickenScratch.Interactive.resources.ExtensionStyle.css"
        KernelInvocationContext.Current.Display(style) |> ignore
    with
    | _ -> KernelInvocationContext.Current.Display("Failed to load resource ExtensionStyle.css") |> ignore