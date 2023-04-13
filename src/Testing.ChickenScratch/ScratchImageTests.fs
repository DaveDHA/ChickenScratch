module Testing.ChickenScratch.ScratchImage.ScratchImageTests

open Xunit
open FsUnit.Xunit
open ChickenScratch
open ChickenScratch.HtmlExpressions
open System.Xml
open System
open System.IO
open System.Reflection


type TempFile(extension) =
    let path = sprintf "%s%s" (Path.GetTempFileName()) extension
    
    member this.Path = path

    interface IDisposable with      
        member _.Dispose() = File.Delete(path)



[<Fact>]
let ``ScratchImage FromBytes encodes image data``() =
    let bytes = [| 0x00uy; 0x01uy; 0x02uy; 0x03uy |]
    let image = ScratchImage.FromBytes ".jpg" bytes
    
    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)

    doc.SelectSingleNode("/img").Attributes["src"].Value 
    |> should endWith (Convert.ToBase64String(bytes))

    
[<Theory>]
[<InlineData(".jpg", "image/jpeg")>]
[<InlineData(".jpeg", "image/jpeg")>]
[<InlineData(".png", "image/png")>]
[<InlineData(".gif", "image/gif")>]
let ``ScratchImage FromBytes includes correct mime type``(fileExtension, expectedMimeType) =
    let bytes = [| 0x00uy; 0x01uy; 0x02uy; 0x03uy |]
    let image = ScratchImage.FromBytes fileExtension bytes
    
    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)
    
    doc.SelectSingleNode("/img").Attributes["src"].Value 
    |> should startWith (sprintf "data:%s;base64," expectedMimeType)
    

[<Fact>]
let ``ScratchImage FromFile encodes image data``() =
    let bytes = [| 0x00uy; 0x01uy; 0x02uy; 0x03uy |]
    use tempFile = new TempFile(".jpg")
    File.WriteAllBytes(tempFile.Path, bytes)
    
    let image = ScratchImage.FromFile tempFile.Path
    
    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)

    doc.SelectSingleNode("/img").Attributes["src"].Value 
    |> should endWith (Convert.ToBase64String(bytes))
    

[<Theory>]
[<InlineData(".jpg", "image/jpeg")>]
[<InlineData(".jpeg", "image/jpeg")>]
[<InlineData(".png", "image/png")>]
[<InlineData(".gif", "image/gif")>]
let ``ScratchImage FromFile includes correct mime type``(fileExtension, expectedMimeType) =
    let bytes = [| 0x00uy; 0x01uy; 0x02uy; 0x03uy |]
    use tempFile = new TempFile(fileExtension)
    File.WriteAllBytes(tempFile.Path, bytes)
    
    let image = ScratchImage.FromFile tempFile.Path
    
    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)
    
    doc.SelectSingleNode("/img").Attributes["src"].Value 
    |> should startWith (sprintf "data:%s;base64," expectedMimeType)
    

[<Fact>]
let ``ScratchImage FromStream encodes image data``() =
    let bytes = [| 0x00uy; 0x01uy; 0x02uy; 0x03uy |]
    
    let image = using (new MemoryStream(bytes)) (ScratchImage.FromStream ".jpg")
    
    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)

    doc.SelectSingleNode("/img").Attributes["src"].Value 
    |> should endWith (Convert.ToBase64String(bytes))
    

[<Theory>]
[<InlineData(".jpg", "image/jpeg")>]
[<InlineData(".jpeg", "image/jpeg")>]
[<InlineData(".png", "image/png")>]
[<InlineData(".gif", "image/gif")>]
let ``ScratchImage FromStream includes correct mime type``(fileExtension, expectedMimeType) =
    let bytes = [| 0x00uy; 0x01uy; 0x02uy; 0x03uy |]
    
    let image = using (new MemoryStream(bytes)) (ScratchImage.FromStream fileExtension)
    
    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)
    
    doc.SelectSingleNode("/img").Attributes["src"].Value 
    |> should startWith (sprintf "data:%s;base64," expectedMimeType)
    

[<Fact>]
let ``ScratchImage FromResource encodes image data``() =
    let image = ScratchImage.FromResource (Assembly.GetExecutingAssembly()) "Testing.ChickenScratch.resources.ScratchImageTest.jpg"
    
    use stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Testing.ChickenScratch.resources.ScratchImageTest.jpg")
    use memStream = new MemoryStream()
    stream.CopyTo(memStream)

    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)

    doc.SelectSingleNode("/img").Attributes["src"].Value 
    |> should endWith (Convert.ToBase64String(memStream.ToArray()))
    

[<Fact>]
let ``Can Add Attributes via Prototype``() =
    let image = 
        [| 0x00uy |]
        |> ScratchImage.FromBytes ".jpg"
        |> ScratchImage.WithPrototype (img { _width "100px" ; _height "200px" })

    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)

    doc.SelectSingleNode("/img").Attributes["width"].Value |> should equal "100px"
    doc.SelectSingleNode("/img").Attributes["height"].Value |> should equal "200px"


[<Fact>]
let ``Can Add Class via Prototype and still keep unique class``() =
    let image = 
        [| 0x00uy |]
        |> ScratchImage.FromBytes ".jpg"
        |> ScratchImage.WithPrototype (img { _class "test" })
    
    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)

    doc.SelectSingleNode("/img").Attributes["class"].Value |> should haveSubstring "test"
    doc.SelectSingleNode("/img").Attributes["class"].Value |> should haveSubstring image.UniqueClass


[<Fact>]
let ``Prototype children are discarded``() =
    let image = 
        [| 0x00uy |]
        |> ScratchImage.FromBytes ".jpg"
        |> ScratchImage.WithPrototype (img { "child1" ; "child2" })
    
    let doc = XmlDocument()
    doc.LoadXml(image.AsHtmlString)

    doc.SelectSingleNode("/img").ChildNodes.Count |> should equal 0


[<Fact>]
let ``Every ScratchImage has a unique class``() =
    seq { 1..1000 }
    |> Seq.map (fun _ -> [| 0x00uy |] |> ScratchImage.FromBytes ".jpg")
    |> Seq.distinctBy (fun x -> x.UniqueClass)
    |> Seq.length
    |> should equal 1000 
    

[<Fact>]
let ``ScratchImage As Style``() =
    let image = 
        [| 0x00uy |]
        |> ScratchImage.FromBytes ".jpg"
        
    image |> ScratchImage.AsStyle |> should equal image.Style

    let doc = XmlDocument()
    doc.LoadXml(image.Style.AsHtmlString)

    doc.SelectSingleNode("/style").InnerText.Trim()
    |> should equal (sprintf ".%s { content: url(\"data:image/jpeg;base64,AA==\") }" image.UniqueClass)


[<Fact>]
let ``ScratchImage as Instance``() =
    let image = 
        [| 0x00uy |]
        |> ScratchImage.FromBytes ".jpg"
        
    image |> ScratchImage.AsInstance |> should equal image.Instance
    
    let doc = XmlDocument()
    doc.LoadXml(image.Instance.AsHtmlString)

    doc.SelectSingleNode("/img").Attributes["class"].Value |> should equal image.UniqueClass
    doc.SelectSingleNode("/img").Attributes["src"] |> should be null