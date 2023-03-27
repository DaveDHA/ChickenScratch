open ChickenScratch
open System.Reflection

module TestDocument =
    open ScratchTagExpressions
    
    let getResourceList() = 
        ul {
            yield!
                Assembly.GetAssembly(typeof<ScratchNode>).GetManifestResourceNames()
                |> Seq.map (fun resource -> li { raw resource })
        }


    let build() =
        html { 
            head { 
                ScratchStyle.FromScratchResource "ChickenScratch.Resources.TreeTable.css"
                ScratchScript.FromScratchResource "ChickenScratch.Resources.TreeTable.js"
            }
            body {
                h1 { "This is a test document for Chicken Scratch"}
                h2 { "Embedded Resources" }
                
                getResourceList()                
            }
        }

let doc = TestDocument.build()

printfn "%A" doc

System.IO.File.WriteAllText("ManualTest.html", sprintf "%A" doc)

