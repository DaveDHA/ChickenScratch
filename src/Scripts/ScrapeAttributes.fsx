#r "nuget: HtmlAgilityPack"

open HtmlAgilityPack

let globalAttributes = 
    let doc = HtmlWeb().Load("https://www.w3schools.com/TAGs/ref_standardattributes.asp")

    let table = doc.DocumentNode.Descendants("table") |> Seq.find (fun t -> t.GetClasses() |> Seq.contains "ws-table-all")

    table.Descendants("tr") 
    |> Seq.choose (fun tr -> tr.Descendants("td") |> Seq.tryHead)
    |> Seq.map (fun td -> td.InnerText)        
    |> Seq.toList



let tagData =
    let doc = HtmlWeb().Load("https://www.w3schools.com/TAGs/default.asp")

    let table = doc.DocumentNode.Descendants("table") |> Seq.find (fun t -> t.HasClass "ws-table-all")
    table.Descendants("tr")
    |> Seq.choose (fun tr -> tr.Descendants("td") |> Seq.tryHead)
    |> Seq.map (fun td -> td.Descendants("a") |> Seq.head)
    |> Seq.filter (fun a -> not (a.HasClass "notsupported" ))    
    |> Seq.map (fun a -> ((System.Net.WebUtility.HtmlDecode a.InnerText), (sprintf "https://www.w3schools.com/TAGs/%s" (a.GetAttributeValue("href", "")))))
    |> Seq.filter (fun (x,_) -> x <> "<!-->" && x <> "<!DOCTYPE>")
    |> Seq.toList

let tags = tagData |> Seq.map fst |> Seq.map (fun str -> str.Replace("<", "").Replace(">", "")) |> Seq.toList
let urls = tagData |> List.map snd

tags, urls


let getAttributesFromTagUrl (url : string) =
    let rec nextTable (node : HtmlNode) =
        if node.Name = "table" then node else nextTable node.NextSibling
    
    let doc = HtmlWeb().Load(url)
    
    doc.DocumentNode.Descendants("h2") |> Seq.filter (fun d -> d.InnerHtml = "Attributes") |> Seq.tryHead
    |> function
        | Some header ->
            let table = header |> nextTable
        
            table.Descendants("tr") 
            |> Seq.choose (fun tr -> tr.Descendants("td") |> Seq.tryHead)
            |> Seq.map (fun td -> td.InnerText)        
            |> Seq.toList
        | _ -> List.empty
        
   
let reserved = [ "id" ; "class" ; "type" ; "data-*" ; "for" ; "list" ; "open" ]
let replacements = [ "``id``" ; "``class``" ; "``type``" ; "``for``" ; "``list``" ; "``open``" ]
let selfclosing = [ "area" ; "base" ; "br" ; "col" ; "embed" ; "hr" ; "img" ; "input" ; "link" ; "meta" ; "param" ; "source" ; "track" ; "wbr" ]

let tagCode tag = 
    let method = if selfclosing |> Seq.contains tag then "selfClosingTag" else "tag"
    sprintf "        let %s = %s \"%s\"" tag method tag


let allAttrs = 
    [ globalAttributes ; replacements ] 
    @ (urls |> List.map getAttributesFromTagUrl)
    |> Seq.collect id
    |> Seq.filter (fun x -> not (Seq.contains x reserved))
    |> Set.ofSeq


printfn "--------------------------------------------------"
printfn "    [<AutoOpen>]"
printfn "    module Tags ="
tags |> Seq.iter (tagCode >> (printfn "%s"))
printfn ""
printfn "    [<AutoOpen>]"
printfn "    module Attributes ="
allAttrs |> Seq.iter (fun e -> printfn "        let %s = attr \"%s\"" e e)
printfn "--------------------------------------------------"
