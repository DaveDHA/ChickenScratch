namespace Testing.ChickenScratch.Interactive.TabData

open System
open Xunit
open FsUnit.Xunit
open ChickenScratch
open Microsoft.DotNet.Interactive.Formatting
open Microsoft.DotNet.Interactive.Formatting.TabularData


module TabularDataResourceTests =

    [<Fact>]
    let ``toMsDataFrame creates a DataFrame`` () =
        let data = [
                {| Key = "test1" ; Value = 2 |}
                {| Key = "test3" ; Value = 4 |}
            ]
        
        let df = data.ToTabularDataResource() |> TabularDataResource.toMsDataFrame
        
        df.Columns.Count |> should equal 2
        df.Rows.Count |> should equal 2L
        df.Rows[0]["Key"] |> string |> should equal "test1"
        df.Rows[0]["Value"] :?> int |> should equal 2
        df.Rows[1]["Key"] |> string |> should equal "test3"
        df.Rows[1]["Value"] :?> int |> should equal 4


    [<Fact>]
    let ``toMsDataFrame can work with integer types``() =
        let data = [
                {| Int16Val = 2s ; Int32Val = 3 ; Int64Val = 4L |}                
            ]
        
        let df = data.ToTabularDataResource() |> TabularDataResource.toMsDataFrame
        
        df.Columns["Int16Val"].DataType |> should equal typeof<int16>
        df.Columns["Int32Val"].DataType |> should equal typeof<int32>
        df.Columns["Int64Val"].DataType |> should equal typeof<int64>
        df.Rows[0]["Int16Val"] :?> int16 |> should equal 2s
        df.Rows[0]["Int32Val"] :?> int32 |> should equal 3
        df.Rows[0]["Int64Val"] :?> int64 |> should equal 4L


    [<Fact>]
    let ``toMsDataFrame can work with unsigned integer types``() =
        let data = [
                {| UInt16Val = 2us ; UInt32Val = 3u ; UInt64Val = 4UL |}                
            ]
        
        let df = data.ToTabularDataResource() |> TabularDataResource.toMsDataFrame
        
        df.Columns["UInt16Val"].DataType |> should equal typeof<uint16>
        df.Columns["UInt32Val"].DataType |> should equal typeof<uint32>
        df.Columns["UInt64Val"].DataType |> should equal typeof<uint64>
        df.Rows[0]["UInt16Val"] :?> uint16 |> should equal 2us
        df.Rows[0]["UInt32Val"] :?> uint32 |> should equal 3u
        df.Rows[0]["UInt64Val"] :?> uint64 |> should equal 4UL


    [<Fact>]
    let ``toMsDataFrame can work with floating point types``() =
        let data = [
                {| SingleVal = 2.0f ; DoubleVal = 3.0 ; DecimalVal = 4.0M |}                
            ]
        
        let df = data.ToTabularDataResource() |> TabularDataResource.toMsDataFrame
        
        df.Columns["SingleVal"].DataType |> should equal typeof<float32>
        df.Columns["DoubleVal"].DataType |> should equal typeof<float>
        df.Columns["DecimalVal"].DataType |> should equal typeof<decimal>
        df.Rows[0]["SingleVal"] :?> float32 |> should equal 2.0f
        df.Rows[0]["DoubleVal"] :?> float |> should equal 3.0
        df.Rows[0]["DecimalVal"] :?> decimal |> should equal 4.0M

    
    [<Fact>]
    let ``toMsDataFrame can work with byte types``() =
        let data = [
                {| ByteVal = 2uy ; SByteVal = -3y |}                
            ]
        
        let df = data.ToTabularDataResource() |> TabularDataResource.toMsDataFrame
        
        df.Columns["ByteVal"].DataType |> should equal typeof<byte>
        df.Columns["SByteVal"].DataType |> should equal typeof<sbyte>
        df.Rows[0]["ByteVal"] :?> byte |> should equal 2uy
        df.Rows[0]["SByteVal"] :?> sbyte |> should equal -3y


    [<Fact>]
    let ``toMsDataFrame can work with string and char types``() =
        let data = [
                {| StringVal = "test" ; CharVal = 'c' |}                
            ]
        
        let df = data.ToTabularDataResource() |> TabularDataResource.toMsDataFrame
        
        df.Columns["StringVal"].DataType |> should equal typeof<string>
        df.Columns["CharVal"].DataType |> should equal typeof<char>
        df.Rows[0]["StringVal"] :?> string |> should equal "test"
        df.Rows[0]["CharVal"] :?> char |> should equal 'c'


    [<Fact>]
    let ``toMsDataFrame can work with DateTime types``() =        
        let data = [
                {| DateTimeVal = DateTime(2021, 1, 1) |}                
            ]
        
        let df = data.ToTabularDataResource() |> TabularDataResource.toMsDataFrame
        
        df.Columns["DateTimeVal"].DataType |> should equal typeof<DateTime>
        df.Rows[0]["DateTimeVal"] :?> DateTime |> should equal (data[0].DateTimeVal)
        

    [<Fact>]
    let ``toMsDataFrame can work with nullable types``() =
        let data = [
            {| IntVal = Nullable<int>(32) ; DoubleVal = Nullable<float>(3.14) ; StringVal = "Str" |}
            {| IntVal = Nullable() ; DoubleVal = Nullable() ; StringVal = null |}
        ]

        let df = data.ToTabularDataResource() |> TabularDataResource.toMsDataFrame

        df.Columns["IntVal"].DataType |> should equal typeof<int>
        df.Columns["DoubleVal"].DataType |> should equal typeof<float>
        df.Columns["StringVal"].DataType |> should equal typeof<string>
        df.Rows[0]["IntVal"] :?> int |> should equal 32
        df.Rows[0]["DoubleVal"] :?> float |> should equal 3.14
        df.Rows[0]["StringVal"] :?> string |> should equal "Str"
        df.Rows[1]["IntVal"] |> should equal null
        df.Rows[1]["DoubleVal"] |> should equal null
        df.Rows[1]["StringVal"] |> should equal null


    [<Fact>]
    let ``toMsDataFrame converts Options to nullables``() =
        let data = [
            {| IntVal = Some 32 ; DoubleVal = Some 3.14 ; StringVal = Some "Str" |}
            {| IntVal = None ; DoubleVal = None ; StringVal = None |}
        ]

        let df = data.ToTabularDataResource() |> TabularDataResource.toMsDataFrame
        
        df.Columns["IntVal"].DataType |> should equal typeof<int>
        df.Columns["DoubleVal"].DataType |> should equal typeof<float>
        df.Columns["StringVal"].DataType |> should equal typeof<string>
        df.Rows[0]["IntVal"] :?> int |> should equal 32
        df.Rows[0]["DoubleVal"] :?> float |> should equal 3.14
        df.Rows[0]["StringVal"] :?> string |> should equal "Str"
        df.Rows[1]["IntVal"] |> should equal null
        df.Rows[1]["DoubleVal"] |> should equal null
        df.Rows[1]["StringVal"] |> should equal null
    