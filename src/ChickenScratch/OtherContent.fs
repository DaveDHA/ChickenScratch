namespace ChickenScratch

open System.IO
open System.Reflection
open HtmlExpressions

module ScratchStyle =
    let FromString css = Tags.style { RawContent css }


    let FromStream (stream : Stream) =
        using (new StreamReader(stream)) (fun r -> r.ReadToEnd() |> FromString)
        

    let FromFile path = 
        using (File.OpenRead path) FromStream
        

    let FromResource (assembly : Assembly) resourcePath =
        using (assembly.GetManifestResourceStream resourcePath) FromStream
        
        
    let FromScratchResource = FromResource (Assembly.GetExecutingAssembly())



module ScratchScript =
    let FromString js = Tags.script { RawContent js }


    let FromStream (stream : Stream) =
        using (new StreamReader(stream)) (fun r -> r.ReadToEnd() |> FromString)
        

    let FromFile path = using (File.OpenRead path) FromStream


    let FromResource (assembly : Assembly) resourcePath =
        using (assembly.GetManifestResourceStream resourcePath) FromStream
        
        
    let FromScratchResource = FromResource (Assembly.GetExecutingAssembly())



module ScratchHtml =
    let FromString html = RawContent html


    let FromStream (stream : Stream) =
        using (new StreamReader(stream)) (fun r -> r.ReadToEnd() |> FromString)
        

    let FromFile path = using (File.OpenRead path) FromStream


    let FromResource (assembly : Assembly) resourcePath =
        using (assembly.GetManifestResourceStream resourcePath) FromStream
        
        
    let FromScratchResource = FromResource (Assembly.GetExecutingAssembly())