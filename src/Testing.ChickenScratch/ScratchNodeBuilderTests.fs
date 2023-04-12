namespace Testing.ChickenScratch.ScratchNodeBuilder

open Xunit
open FsUnit.Xunit
open ChickenScratch
open ChickenScratch.HtmlExpressions

module ScratchNodeBuilderTests =
    
    [<Fact>]
    let ``Can build empty node`` () =
        let expected =
            Element {
                Tag = "hr"
                Attributes = Map.empty
                Children = []
                AllowChildren = false
            }

        let actual = hr { () }
        
        actual |> should equal expected


    [<Fact>]
    let ``Can build node with attributes`` () =
        let expected =
            Element {
                Tag = "img"
                Attributes = [ ("height", "100px") ; ("src", "http://example.com/image") ] |> Map.ofSeq
                Children = []
                AllowChildren = false
            }
    
        let actual = img { 
            _height "100px"
            _src "http://example.com/image"
        }
    
        actual |> should equal expected
    

    [<Fact>]
    let ``Can build node with element child`` () =
        let expected =
            Element {
                Tag = "div"
                Attributes = Map.empty
                Children = [
                    Element {
                        Tag = "span"
                        Attributes = Map.empty
                        Children = []
                        AllowChildren = true
                    }
                ]
                AllowChildren = true
            }
    
        let actual = div { 
            span { () }
        }
    
        actual |> should equal expected
    
    
    [<Fact>]
    let ``Can build node with encoded content child`` () =
        let expected =
            Element {
                Tag = "p"
                Attributes = Map.empty
                Children = [ EncodedContent "1 & 2 < 4" ]
                AllowChildren = true
            }
    
        let actual = p { 
            "1 & 2 < 4"
        }
    
        actual |> should equal expected
    
    
    [<Fact>]
    let ``Can build node with raw content child`` () =
        let expected =
            Element {
                Tag = "p"
                Attributes = Map.empty
                Children = [ RawContent "<hr/>" ]
                AllowChildren = true
            }
    
        let actual = p { 
            RawContent "<hr/>"
        }
    
        actual |> should equal expected
    
    
    [<Fact>]
    let ``Can build node with multiple children`` () =
        let expected =
            Element {
                Tag = "div"
                Attributes = Map.empty
                Children = [
                    Element {
                        Tag = "span"
                        Attributes = Map.empty
                        Children = []
                        AllowChildren = true
                    }
                    EncodedContent "1 & 2 < 4"
                    RawContent "<hr/>"
                ]
                AllowChildren = true
            }
    
        let actual = div { 
            span { () }
            "1 & 2 < 4"
            RawContent "<hr/>"
        }
    
        actual |> should equal expected


    [<Fact>]
    let ``Can mix children and attributes`` () =
        let expected =
            Element {
                Tag = "div"
                Attributes = [ ("class", "container") ; ("width", "100px") ] |> Map.ofSeq
                Children = [
                    Element {
                        Tag = "span"
                        Attributes = [ ("class", "text") ; ("style", "color: red") ] |> Map.ofSeq
                        Children = [ EncodedContent "hello" ; EncodedContent "world"]
                        AllowChildren = true
                    }                    
                    Element {
                        Tag = "span"
                        Attributes = [ ("class", "text") ; ("style", "color: red") ] |> Map.ofSeq
                        Children = [ EncodedContent "hello" ; EncodedContent "world"]
                        AllowChildren = true
                    }                    
                ]
                AllowChildren = true
            }
    
        let actual = div { 
            _class "container"
            span { _class "text" ; "hello" ; _style "color: red" ; "world" }
            span { "hello" ; _class "text" ; "world" ; _style "color: red" }
            _width "100px"            
        }
    
        actual |> should equal expected


    [<Fact>]
    let ``Can build node with multiple Element children via yield!``() =
        let expected =
            Element {
                Tag = "div"
                Attributes = Map.empty
                Children = 
                    Element {
                        Tag = "span"
                        Attributes = Map.empty
                        Children = []
                        AllowChildren = true
                    }
                    |> List.replicate 3                    
                AllowChildren = true
            }
    
        let actual = div { yield! seq {1..3} |> Seq.map (fun _ -> span { () }) }
    
        actual |> should equal expected


    [<Fact>]
    let ``Can build node with multiple EncodedContent children via yield!``() =
        let expected =
            Element {
                Tag = "div"
                Attributes = Map.empty
                Children = EncodedContent "hello" |> List.replicate 3                    
                AllowChildren = true
            }
    
        let actual = div { yield! seq {1..3} |> Seq.map (fun _ -> "hello") }
    
        actual |> should equal expected
    
    
    [<Fact>]
    let ``Can build node with multiple RawContent children via yield!``() =
        let expected =
            Element {
                Tag = "ul"
                Attributes = Map.empty
                Children = RawContent "<li>hi</li>" |> List.replicate 3                    
                AllowChildren = true
            }
    
        let actual = ul { yield! seq {1..3} |> Seq.map (fun _ -> RawContent "<li>hi</li>") }
    
        actual |> should equal expected


    [<Fact>]
    let ``Can include IScratchNodeSource in children``() =
        let source =
            { new IScratchNodeSource with
                member _.GetScratchNodes() = [ EncodedContent "hello" ; EncodedContent "world" ] }

        let expected =
            Element {
                Tag = "div"
                Attributes = Map.empty
                Children = source.GetScratchNodes()
                AllowChildren = true
            }
    
        let actual = div { source }
    
        actual |> should equal expected
    
    
    [<Fact>]
    let ``Can include IScratchNodeSource in children via yield!``() =
        let source =
            { new IScratchNodeSource with
                member _.GetScratchNodes() = [ EncodedContent "hello" ; EncodedContent "world" ] }
        let expected =
            Element {
                Tag = "div"
                Attributes = Map.empty
                Children = source.GetScratchNodes() @ source.GetScratchNodes()
                AllowChildren = true
            }
    
        let actual = div { yield! [ source ; source ] }
    
        actual |> should equal expected


    [<Fact>]
    let ``Can build complex tree``() =
        let expected =
            Element {
                Tag = "div"
                Attributes = [ ("class", "container") ] |> Map.ofSeq
                Children = [
                    Element {
                        Tag = "h1"
                        Attributes = [ ("class", "title") ] |> Map.ofSeq
                        Children = [ EncodedContent "Hello" ]
                        AllowChildren = true
                    }
                    Element {
                        Tag = "ul"
                        Attributes = [ ("class", "list") ] |> Map.ofSeq
                        Children = [
                            Element {
                                Tag = "li"
                                Attributes = [ ("class", "item") ] |> Map.ofSeq
                                Children = [ EncodedContent "1" ]
                                AllowChildren = true
                            }
                            Element {
                                Tag = "li"
                                Attributes = [ ("class", "item") ] |> Map.ofSeq
                                Children = [ EncodedContent "2" ]
                                AllowChildren = true
                            }
                            Element {
                                Tag = "li"
                                Attributes = [ ("class", "item") ] |> Map.ofSeq
                                Children = [ EncodedContent "3" ]
                                AllowChildren = true
                            }
                        ]
                        AllowChildren = true
                    }
                ]
                AllowChildren = true
            }
    
        let actual = div { 
            _class "container"
            h1 { _class "title" ; "Hello" }
            ul { 
                _class "list"
                li { _class "item" ; "1" }
                li { _class "item" ; "2" }
                li { _class "item" ; "3" }
            }
        }
    
        actual |> should equal expected
    