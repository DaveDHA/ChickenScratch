#r "nuget: HtmlAgilityPack"

open HtmlAgilityPack

let doc = HtmlWeb().Load("https://www.w3schools.com/TAGs/ref_eventattributes.asp")
let headers = doc.DocumentNode.Descendants("h2") |> Seq.filter (fun d -> d.InnerHtml.Contains("Events"))

let rec nextTable (node : HtmlNode) =
    if node.Name = "table" then node else nextTable node.NextSibling

let getEventsFromHeader (header : HtmlNode) =
    let table = header |> nextTable
    
    table.Descendants("tr") 
    |> Seq.choose (fun tr -> tr.Descendants("td") |> Seq.tryHead)
    |> Seq.map (fun td -> td.InnerText)        
    |> Seq.toList

let events = headers |> Seq.collect getEventsFromHeader |> Set.ofSeq

printfn "--------------------------------------------------"
printfn "    [<AutoOpen>]"
printfn "    module Events ="
events |> Seq.iter (fun e -> printfn "        let %s = attr \"%s\"" e e)
printfn "--------------------------------------------------"
