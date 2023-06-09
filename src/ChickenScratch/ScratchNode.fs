﻿namespace ChickenScratch

open System.Net


type IScratchNodeSource =
    abstract member GetScratchNodes : unit -> ScratchNode list
               

and 
    [<StructuredFormatDisplay("{AsHtmlString}")>] 
    ScratchNode =
    | Element of ScratchElement
    | EncodedContent of string
    | RawContent of string
    with
        member internal this.Render indent = 
            match this with
            | Element el -> el.Render indent
            | EncodedContent str -> WebUtility.HtmlEncode str
            | RawContent str -> str
        
        member this.AsHtmlString = this.Render 0
        
and 
    [<StructuredFormatDisplay("{AsHtmlString}")>] 
    ScratchElement = {
        Tag : string
        Attributes : Map<string, string>
        Children : ScratchNode list
        AllowChildren : bool
    }
        with
        member internal this.Render indent = 
            let indentLine level = sprintf "%s%s" (String.replicate (level * 4) " ")

            let openTag = 
                seq {
                    $"<{this.Tag}"
                    for kvp in this.Attributes do
                        $" {kvp.Key}=\"{kvp.Value}\""
                    if not this.AllowChildren then "/>" else ">"
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
            