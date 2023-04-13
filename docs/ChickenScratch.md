# ChickenScratch

## Add ChickenScratch to your F# Script

```FSharp
#r "nuget: ChickenScratch"
open ChickenScratch
```

## Creating Html

### Basic Html

```FSharp
module Sample1 =
    open ChickenScratch.HtmlExpressions

    let doc = div {
        _style "border: solid 3px green ; background-color: lightgray ; color: black; padding: 10px"        
        
        h1 { "Welcome to Chicken Scratch" }

        ul {
            li { "Simple DSL for creating HTML" }
            li { "Include" ; strong { "IScratchNodeSource" } ; "types directly in your Html exressions" } 
            li { "Add content from files, strings, streams, or resources" }           
        }        
    }

printfn "%A" Sample1.doc
```

Outputs:

```
<div style="border: solid 3px green ; background-color: lightgray ; color: black; padding: 10px;">
    <h1>Welcome to Chicken Scratch</h1>
    <ul>
        <li>Simple DSL for creating HTML</li>
        <li>
            Include
            <strong>IScratchNodeSource</strong>
            types directly in your HTML
        </li>
    </ul>
</div>
```

Attributes (such `_style`, `_class`, and `_height`) are prefixed with an underscore character.  Tags (such as `div`, `table`, and `h1`) and events (such as `onclick`, `onmouseup`, and `onscroll`) are not.  All are contained within the `HtmlExpressions` module.  It is recommended that you open that module only within a limited scope to prevent common name collisions.

Tags, attributes, and events can be found in the `Tags`, `Attributes`, and `Events` modules respectively.  Those modules are auto-opened with the `HtmlExpressions` module so you generally don't need to specify them, but if you are having trouble remembering exactly what that event you need is called, type `Events.` and let your code completion tool help you find it.

All string content will be encoded.  Use `RawContent` if you want to avoid encoding.

```FSharp
module Sample2 =
    open ChickenScratch.HtmlExpressions

    let doc = div {
        _id "sample2"
        style { """
                #sample2 .mainHeader { color: red }
                #sample2 li { color: green ; text-decoration: underline }
            """ }
        
        h1 { "Look at this" ; _class "mainHeader" }

        ul { 
            yield! 
                seq {1..3} 
                |> Seq.map (fun i -> li { $"Item {i}" ; if i % 2 = 0 then _style "color: yellow" })
        }

        hr { () }

        Tags.p { "This content is encoded & you can see that 1 < 2." }
        p { RawContent "This content is <strong>not</strong> encoded." }
        RawContent "<p>This content is not encoded either.</p>"
    }

Sample2.doc.ToString()
```

Outputs:

```
<div id="sample2">
    <style>
        
                #sample2 .mainHeader { color: red }
                #sample2 li { color: green ; text-decoration: underline }
            
    </style>
    <h1 class="mainHeader">Look at this</h1>
    <ul>
        <li>Item 1</li>
        <li style="color: yellow">Item 2</li>
        <li>Item 3</li>
    </ul>
    <hr/>
    <p>This content is encoded &amp; you can see that 1 &lt; 2.</p>
    <p>This content is <strong>not</strong> encoded.</p>
    <p>This content is not encoded either.</p>
</div>
```

## Including `IScratchNodeSource` Items in Html Expressions

Any type that implements `IScratchNodeSource` can be dropped directly into your Html expressions.

```FSharp
module Sample3 =
    open ChickenScratch.HtmlExpressions

    type EmployeeInfo = {
        Id : int
        Name : string
        Email : string
    }
    with
        interface IScratchNodeSource with
            member this.GetScratchNodes() = [ 
                EncodedContent $"{this.Id}: "
                strong { $"{this.Name} " }
                EncodedContent $"({this.Email})" 
            ]


    type OrgChartEntry = 
    | Manager of EmployeeInfo * (OrgChartEntry list)
    | WorkerBee of EmployeeInfo
    with 
        interface IScratchNodeSource with
            member this.GetScratchNodes() = 
                match this with
                | WorkerBee emp -> [ li { span { emp ; _class "workerBee" } } ]
                | Manager (mgr, emps) -> [ li { span { mgr ; _class "manager"} ; ul { yield! emps } } ]


    type Company = {
        Name : string
        OrgChart : OrgChartEntry list
    }
    with 
        interface IScratchNodeSource with
            member this.GetScratchNodes() = [ h1 { this.Name } ; ul { yield! this.OrgChart }]

    let company = {
        Name = "Samples Incorporated"
        OrgChart = [
            Manager ({ Id = 1 ; Name = "Molly Manager" ; Email = "theboss@Sample.com" }, [
                WorkerBee { Id = 10 ; Name = "Sammy Sampleson" ; Email = "sam@sample.com" }
                WorkerBee { Id = 11 ; Name = "Elise Example" ; Email = "elise@sample.com" }
                WorkerBee { Id = 12 ; Name = "Testy McTesterson" ; Email = "Testy@Test.com" }
            ])
        ]
    }   

    let doc = div { company } 


Sample3.doc.ToString()
```

Outputs:

```
<div>
    <h1>Samples Incorporated</h1>
    <ul>
        <li>
            <span class="manager">
                1: 
                <strong>Molly Manager </strong>
                (theboss@Sample.com)
            </span>
            <ul>
                <li>
                    <span class="workerBee">
                        10: 
                        <strong>Sammy Sampleson </strong>
                        (sam@sample.com)
                    </span>
                </li>
                <li>
                    <span class="workerBee">
                        11: 
                        <strong>Elise Example </strong>
                        (elise@sample.com)
                    </span>
                </li>
                <li>
                    <span class="workerBee">
                        12: 
                        <strong>Testy McTesterson </strong>
                        (Testy@Test.com)
                    </span>
                </li>
            </ul>
        </li>
    </ul>
</div>
```

### Adding Content from Files, Strings, Streams, and Resources

`ScratchStyle`, `ScratchScript`, and `ScratchHtml` each offer the following methods:

- `FromString : String -> ScratchNode`
- `FromStream : Stream -> ScratchNode`
- `FromFile : String -> ScratchNode`
- `FromResource : Assembly -> String -> ScratchNode`

These can be used to load additional content into Html expressions.

```FSharp
module Sample4 = 
    open Sample3
    open ChickenScratch.HtmlExpressions

    let doc = div {
        _id "sample4"
        
        ScratchStyle.FromFile "Sample.css"

        {
            Name = "Samples Incorporated"
            OrgChart = [
                Manager ({ Id = 1 ; Name = "Molly Manager" ; Email = "theboss@Sample.com" }, [
                    WorkerBee { Id = 10 ; Name = "Sammy Sampleson" ; Email = "sam@sample.com" }
                    WorkerBee { Id = 11 ; Name = "Elise Example" ; Email = "elise@sample.com" }
                    WorkerBee { Id = 12 ; Name = "Testy McTesterson" ; Email = "Testy@Test.com" }
                ])
            ]
        }    
    }

printfn "%A" Sample4.doc
```

