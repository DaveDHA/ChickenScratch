namespace Testing.ChickenScratch.OtherContent

open System.IO
open System.Reflection
open System.Text
open Xunit
open FsUnit.Xunit
open ChickenScratch


module ScratchStyleTests =
    
    [<Literal>]
    let styleResource = "Testing.ChickenScratch.resources.TestStyle.css"
    
    
    [<Fact>]
    let ``ScratchStyle.FromString`` () =
        let css = "body { background-color: red; }"
        let style = ScratchStyle.FromString css
        style.ToString() |> should equal $"<style>{css}</style>"
    
    
    [<Fact>]
    let ``ScratchStyle.FromStream`` () =
        let css = ".table { color: green }"
        use stream = new MemoryStream (Encoding.UTF8.GetBytes css)
        let style = ScratchStyle.FromStream stream
        style.ToString() |> should equal $"<style>{css}</style>"
    
    
    [<Fact>]
    let ``ScratchStyle.FromFile`` () =
        if File.Exists "TestStyle.css" then File.Delete "TestStyle.css"
        
        use stream = Assembly.GetExecutingAssembly().GetManifestResourceStream styleResource
        use reader = new StreamReader(stream)
        let text = reader.ReadToEnd()        
        File.WriteAllText("TestStyle.css", text)

        let style = ScratchStyle.FromFile "TestStyle.css"
        style.ToString() |> should equal $"<style>\n    {text}\n</style>"
    
    
    [<Fact>]
    let ``ScratchStyle.FromResource`` () =
        use stream = Assembly.GetExecutingAssembly().GetManifestResourceStream styleResource
        use reader = new StreamReader(stream)
        let text = reader.ReadToEnd()

        let style = ScratchStyle.FromResource (Assembly.GetExecutingAssembly()) styleResource
        style.ToString() |> should equal $"<style>\n    {text}\n</style>"
           

    
module ScratchScriptTests =
    
    [<Literal>]
    let scriptResource = "Testing.ChickenScratch.resources.TestScript.js"
    
    
    [<Fact>]
    let ``ScratchScript.FromString`` () =
        let js = "console.log('Hello, world!');"
        let script = ScratchScript.FromString js
        script.ToString() |> should equal $"<script>{js}</script>"
    
    
    [<Fact>]
    let ``ScratchScript.FromStream`` () =
        let js = "console.log('Hello, world!');"
        use stream = new MemoryStream (Encoding.UTF8.GetBytes js)
        let script = ScratchScript.FromStream stream
        script.ToString() |> should equal $"<script>{js}</script>"
    
    
    [<Fact>]
    let ``ScratchScript.FromFile`` () =
        if File.Exists "TestScript.js" then File.Delete "TestScript.js"
        
        use stream = Assembly.GetExecutingAssembly().GetManifestResourceStream scriptResource
        use reader = new StreamReader(stream)
        let text = reader.ReadToEnd()        
        File.WriteAllText("TestScript.js", text)

        let script = ScratchScript.FromFile "TestScript.js"
        script.ToString() |> should equal $"<script>{text}</script>"
    
    
    [<Fact>]
    let ``ScratchScript.FromResource`` () =
        use stream = Assembly.GetExecutingAssembly().GetManifestResourceStream scriptResource
        use reader = new StreamReader(stream)
        let text = reader.ReadToEnd()

        let script = ScratchScript.FromResource (Assembly.GetExecutingAssembly()) scriptResource
        script.ToString() |> should equal $"<script>{text}</script>"



    module ScratchHtmlTests =
        
        [<Literal>]
        let htmlResource = "Testing.ChickenScratch.resources.TestDoc.html"
        
        
        [<Fact>]
        let ``ScratchHtml.FromString`` () =
            let html = "<h1>Hello, world!</h1>"
            let content = ScratchHtml.FromString html
            content.ToString() |> should equal html
        
        
        [<Fact>]
        let ``ScratchHtml.FromStream`` () =
            let html = "<h1>Hello, world!</h1>"
            use stream = new MemoryStream (Encoding.UTF8.GetBytes html)
            let content = ScratchHtml.FromStream stream
            content.ToString() |> should equal html
        
        
        [<Fact>]
        let ``ScratchHtml.FromFile`` () =
            if File.Exists "TestDoc.html" then File.Delete "TestDoc.html"
            
            use stream = Assembly.GetExecutingAssembly().GetManifestResourceStream htmlResource
            use reader = new StreamReader(stream)
            let text = reader.ReadToEnd()        
            File.WriteAllText("TestDoc.html", text)

            let content = ScratchHtml.FromFile "TestDoc.html"
            content.ToString() |> should equal text
        
        
        [<Fact>]
        let ``ScratchHtml.FromResource`` () =
            use stream = Assembly.GetExecutingAssembly().GetManifestResourceStream htmlResource
            use reader = new StreamReader(stream)
            let text = reader.ReadToEnd()

            let content = ScratchHtml.FromResource (Assembly.GetExecutingAssembly()) htmlResource
            content.ToString() |> should equal text
