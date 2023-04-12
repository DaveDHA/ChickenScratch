module Testing.ChickenScratch.ScratchNode

open System
open Xunit
open FsUnit.Xunit
open ChickenScratch


module ScratchElementTests =
    [<Theory>]
    [<InlineData("tag")>]
    [<InlineData("test")>]
    [<InlineData("hello")>]
    let ``Includes tag`` tag =
        let result = 
            {
                Tag = tag
                Attributes = Map.empty
                Children = []
                AllowChildren = false
            }.AsHtmlString

        result |> should startWith $"<{tag}"


    [<Fact>]
    let ``AllowChildren false`` () =
        let result = 
            {
                Tag = "tag"
                Attributes = Map.empty
                Children = []
                AllowChildren = false
            }.AsHtmlString
        
        result |> should endWith "/>"


    [<Fact>]
    let ``AllowChildren true`` () =
        let result = 
            {
                Tag = "tag"
                Attributes = Map.empty
                Children = []
                AllowChildren = true
            }.AsHtmlString
        
        result |> should endWith $"</tag>"

    
    [<Fact>]
    let ``Includes attributes`` () =
        let attrs = [ ("key", "value") ; ("key2", "value2") ; ("key3", "value3") ]
        for attrCount in {1..3} do
            let expected = attrs |> Seq.take attrCount

            let result =                 
                {
                    Tag = "tag"
                    Attributes = Map.ofSeq expected
                    Children = []
                    AllowChildren = false
                }.AsHtmlString

            expected |> Seq.iter (fun (key, value) -> result |> should haveSubstring $"{key}=\"{value}\"")
    

    [<Fact>]
    let ``Includes children`` () =
        let children = [ "child1" ; "child2" ; "child3" ]
        for childCount in {1..3} do
            let expected = children |> List.take childCount
            let result =                 
                {
                    Tag = "tag"
                    Attributes = Map.empty
                    Children = expected |> List.map RawContent
                    AllowChildren = false
                }.AsHtmlString

            expected |> Seq.iter (fun str -> result |> should haveSubstring str)
    

    [<Fact>]
    let ``2 or more children: multiple lines, indented`` () =
        let children = [ "child1" ; "child2" ; "child3" ]
        for childCount in {2..3} do
            let expected = children |> List.take childCount
            let result =                 
                {
                    Tag = "tag"
                    Attributes = Map.empty
                    Children = expected |> List.map RawContent
                    AllowChildren = false
                }.AsHtmlString

            expected |> Seq.iter (fun str -> result |> should haveSubstring $"\n    {str}")
    
    
    [<Fact>]
    let ``Less than 2 children, less than 80 chars: one line, no indent`` () =
        let result =                 
            {
                Tag = "tag"
                Attributes = Map.empty
                Children = [ RawContent "child" ]
                AllowChildren = false
            }.AsHtmlString

        result |> should not' (haveSubstring "\n")
        result |> should not' (startWith "    ")
    

    [<Fact>]
    let ``Less then 2 children, more than 80 chars: multiple lines, indented`` () =
        let result =                 
            {
                Tag = "tag"
                Attributes = Map.empty
                Children = [ RawContent (String.replicate 81 "x") ]
                AllowChildren = false
            }.AsHtmlString

        result |> should haveSubstring "\n    "
    
    

module ScratchNodeTests =
    [<Fact>]
    let ``RawContent is unchanged`` () =
        let result = (RawContent "<test attr=\"&!<>\"/>").AsHtmlString
        result |> should equal "<test attr=\"&!<>\"/>"
    

    [<Fact>]
    let ``EncodedContent is encoded`` () =
        let result = (EncodedContent "<test attr=\"&!<>\"/>").AsHtmlString
        result |> should equal "&lt;test attr=&quot;&amp;!&lt;&gt;&quot;/&gt;"
    

    [<Fact>]
    let ``Element is rendered`` () = 
        let elmt = 
            {
                Tag = "tag"
                Attributes = [ ("attr1", "value1") ; ("attr2", "value2") ] |> Map.ofList
                Children = [ RawContent "child" ; EncodedContent "2 < 3"]
                AllowChildren = true
            }
            
        (Element elmt).AsHtmlString |> should equal elmt.AsHtmlString
        
    
    [<Fact>]
    let ``Can render a complex tree`` () =
        let expected = """<table>
    <tr class="row1">
        <td class="firstCol">1</td>
        <td class="sndCol" style="color: red">1 &lt; 2</td>
    </tr>
    <tr class="row2">
        <td class="firstCol">1</td>
        <td class="sndCol" style="color: red">1 &lt; 2</td>
    </tr>
    <tr class="row3">
        <td class="firstCol">1</td>
        <td class="sndCol" style="color: red">1 &lt; 2</td>
    </tr>
</table>"""

        let tds = [
            {
                Tag = "td"
                Attributes = [ ("class", "firstCol") ] |> Map.ofList
                Children = [ RawContent "1" ]
                AllowChildren = true
            }
            {
                Tag = "td"
                Attributes = [ ("class", "sndCol") ; ("style", "color: red") ] |> Map.ofList
                Children = [ EncodedContent "1 < 2" ]
                AllowChildren = true
            }
        ]
        let trs = [
            for i in {1..3} do
                {
                    Tag = "tr"
                    Attributes = [ ("class", $"row{i}") ] |> Map.ofList
                    Children = tds |> List.map (fun td -> Element td)
                    AllowChildren = true
                }            
        ]

        let table = 
            {
                Tag = "table"
                Attributes = Map.empty
                Children = trs |> List.map (fun tr -> Element tr)
                AllowChildren = true
            }

        let expectedNormalized = expected |> Seq.filter (fun c -> c <> '\r') |> Seq.toArray |> String
        (Element table).AsHtmlString |> should equal expectedNormalized

                    