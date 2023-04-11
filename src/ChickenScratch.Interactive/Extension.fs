module ChickenScratch.Interactive.Extension

open ChickenScratch
open Microsoft.DotNet.Interactive.Formatting


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


let Load() = 
    registerScratchNodeFormatter()
    registerScratchElementFormatter()
    registerScratchNodeSourceFormatter()    