namespace ChickenScratch

open System
open System.Net

type IScratchHtmlSource =
    abstract member GetHtmlString : unit -> string


type IScratchNodeSource =
    inherit IScratchHtmlSource

    abstract member GetScratchNodes : unit -> ScratchNode list
               

and 
    [<StructuredFormatDisplay("{AsHtmlString}")>] 
    ScratchNode =
    | Element of ScratchElement
    | Content of string
    | HtmlSource of IScratchHtmlSource
    | Raw of string
    with
        member this.Render indent = 
            match this with
            | Element el -> el.Render indent
            | Content str -> WebUtility.HtmlEncode str
            | HtmlSource src -> src.GetHtmlString()
            | Raw str -> str
        
        member this.AsHtmlString = this.Render 0
        
        interface IScratchNodeSource with
            member this.GetHtmlString() = this.AsHtmlString
            member this.GetScratchNodes() = [ this ]
            

and 
    [<StructuredFormatDisplay("{AsHtmlString}")>] 
    ScratchElement = {
        Tag : string
        Attributes : Map<string, string>
        Children : ScratchNode list
        AllowChildren : bool
    }
        with
        member this.Render indent = 
            let indentLine level = sprintf "%s%s" (String.replicate (level * 4) " ")

            let openTag = 
                seq {
                    yield $"<{this.Tag}"
                    for kvp in this.Attributes do
                        yield $" {kvp.Key}=\"{kvp.Value}\""
                    if not this.AllowChildren then yield "/>" else yield ">"
                } 
                |> String.concat ""

            let content = this.Children |> List.map (fun c -> c.Render (indent + 1))

            let closeTag = if this.AllowChildren then $"</{this.Tag}>" else ""

            let singleLine = Seq.length content < 2 && (content |> Seq.sumBy(fun str -> str.Length)) <= 80

            seq {
                yield openTag
                yield! if singleLine then content else content |> List.map (indentLine (indent + 1))
                yield if singleLine then closeTag else closeTag |> indentLine indent
            }
            |> String.concat (if singleLine then "" else "\n")

        member this.AsHtmlString = this.Render 0

        interface IScratchNodeSource with
            member this.GetHtmlString() = this.AsHtmlString
            member this.GetScratchNodes() = [ Element this ]

