namespace Testing.ChickenScratch.Interactive.OtherContent

open System
open System.IO
open Xunit
open FsUnit.Xunit
open ChickenScratch
open Microsoft.DotNet.Interactive.Formatting
open Microsoft.DotNet.Interactive.Formatting.TabularData

module ScratchHtmlTests =
    type Bookend (f : unit -> unit) =
        do f()

        interface IDisposable with
            member this.Dispose() = f()


    type TestDummy = | DummyInstance


    [<Fact>]
    let ``UseFormatter uses registered formatter`` () =
        use _ = new Bookend(fun () -> Formatter.ResetToDefault())

        let formatter = fun (dummy : TestDummy) (tw : TextWriter) -> tw.Write "<p>TestDummy</p>"
        Formatter.Register<TestDummy>(formatter, mimeType = "text/html")

        DummyInstance |> ScratchHtml.UseFormatter |> should equal (RawContent "<p>TestDummy</p>")


    [<Fact>]    
    let ``UseFormatter prefers html formatter`` () =
        use _ = new Bookend(fun () -> Formatter.ResetToDefault())

        let htmlFormatter = fun (dummy : TestDummy) (tw : TextWriter) -> tw.Write "<p>html</p>"
        let textFormatter = fun (dummy : TestDummy) (tw : TextWriter) -> tw.Write "text"

        Formatter.Register<TestDummy>(textFormatter, mimeType = "text/plain")
        Formatter.Register<TestDummy>(htmlFormatter, mimeType = "text/html")
        Formatter.Register<TestDummy>(textFormatter, mimeType = "text/markdown")

        DummyInstance |> ScratchHtml.UseFormatter |> should equal (RawContent "<p>html</p>")        


    [<Fact>]
    let ``UseTabularFormatter uses registered formatter`` () =
        let dummyRecords = [ 
            {| Key = "test1" ; Value = "test2" |}
            {| Key = "test3" ; Value = "test4" |}
        ]

        let expected = dummyRecords.ToTabularDataResource()
        
        dummyRecords |> ScratchHtml.UseTabularFormatter 
        |> function
            | RawContent s -> 
                // Not gonna tie to specific format, just check for presence of values
                s |> should haveSubstring "Key"
                s |> should haveSubstring "Value"
                s |> should haveSubstring "test1"
                s |> should haveSubstring "test4"

            | _ -> failwith "Expected RawContent"
