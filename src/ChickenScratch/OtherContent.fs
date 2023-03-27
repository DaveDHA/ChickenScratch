namespace ChickenScratch

open System.IO
open System.Reflection
open ScratchTagExpressions

module ScratchStyle =
    let FromCssString css = Tags.style { raw css }


    let FromStream (stream : Stream) =
        use reader = new StreamReader(stream)
        reader.ReadToEnd() |> FromCssString


    let FromCssFile path = File.OpenRead(path) |> FromStream


    let FromResource (assembly : Assembly) resourcePath =
        resourcePath
        |> assembly.GetManifestResourceStream
        |> FromStream
    
    
    let FromScratchResource = FromResource (Assembly.GetExecutingAssembly())



module ScratchScript =
    let FromJsString js = script { raw js }


    let FromStream (stream : Stream) =
        use reader = new StreamReader(stream)
        reader.ReadToEnd() |> FromJsString


    let FromCssFile path = File.OpenRead(path) |> FromStream


    let FromResource (assembly : Assembly) resourcePath =
        resourcePath
        |> assembly.GetManifestResourceStream
        |> FromStream
    
    
    let FromScratchResource = FromResource (Assembly.GetExecutingAssembly())
