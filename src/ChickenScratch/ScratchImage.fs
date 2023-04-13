namespace ChickenScratch

open System
open System.IO
open System.Reflection
open ChickenScratch.HtmlExpressions


type ScratchImage = {
    DataUrl : string
    UniqueClass : string
    Prototype : ScratchElement option
}
    with
        member this.Merge includeSrc =
            let protoAttrs = 
                this.Prototype 
                |> Option.map (fun p -> p.Attributes) 
                |> Option.defaultValue Map.empty
            
            let classAttr = 
                match protoAttrs.TryFind "class" with
                | Some classes -> sprintf "%s %s" classes this.UniqueClass
                | _ -> this.UniqueClass
            
            let finalAttrs = 
                protoAttrs
                |> Map.add "class" classAttr
                |> (if includeSrc then (Map.add "src" this.DataUrl) else Map.remove "src")
                    
            {
                Tag = "img"
                Attributes = finalAttrs
                Children = []
                AllowChildren = false
            }            


        member this.Instance = Element (this.Merge false)

        member this.Style = 
            style { RawContent $".{this.UniqueClass} {{ content: url(\"{this.DataUrl}\") }}" }


        member this.WithPrototype prototype = 
            match prototype with
            | Element e -> { this with Prototype = Some e }
            | _ -> failwith "ScratchImage.WithPrototype: prototype must be an Element ScratchNode"
        

        member this.WithUniqueClass uniqueClass = { this with UniqueClass = uniqueClass }

        interface IScratchNodeSource with member this.GetScratchNodes() = [ Element (this.Merge true) ]
            


module ScratchImage =
    let private create() = {
        DataUrl = String.Empty
        UniqueClass = sprintf "scratch-image-%s" (Guid.NewGuid().ToString())
        Prototype = None
    }

    let private formatDataUrl bytes fileExtension =
        let mime = 
            match fileExtension with
            | ".jpg" | ".jpeg" -> "image/jpeg"
            | ".png" -> "image/png"
            | ".gif" -> "image/gif"
            | _ -> failwithf "Unknown image type: %s. Acceptable values: .jpg ; .jpeg ; .png ; .gif" fileExtension
        
        sprintf "data:%s;base64,%s" mime (Convert.ToBase64String bytes)

    
    let FromBytes bytes fileExtension =
        { create() with DataUrl = formatDataUrl bytes fileExtension }


    let FromFile (path : string) = FromBytes (File.ReadAllBytes path) (Path.GetExtension path)


    let FromStream (stream : Stream) fileExtension =
        use memStream = new MemoryStream()
        stream.CopyTo(memStream)
        FromBytes (memStream.ToArray()) fileExtension
        

    let FromResource (assembly : Assembly) (resourcePath : string) =
        FromStream (assembly.GetManifestResourceStream resourcePath) (Path.GetExtension resourcePath)
        

    let AsStyle (img : ScratchImage) = img.Style        


    let AsInstance (img : ScratchImage) = img.Instance


    let WithPrototype prototype (img : ScratchImage) = img.WithPrototype prototype
        

    let WithUniqueClass uniqueClass (img : ScratchImage) = img.WithUniqueClass uniqueClass

