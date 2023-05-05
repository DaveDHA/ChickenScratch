namespace Testing.ChickenScratch.TabData

open System
open Xunit
open FsUnit.Xunit
open ChickenScratch
open ChickenScratch.Utility


[<AutoOpen>]
module TestData =
    open System.Collections.Generic

    type TestEnum =
    | camelValue = 1
    | PascalValue = 2
    | snake_value = 3

    type TestUnion = 
    | CaseA
    | CaseB

    let TestRows = [
        [ 
            KeyValuePair("IntValue", box(1)) 
            KeyValuePair("StringValue", box("Test"))
            KeyValuePair("EnumValue", box("PascalValue"))
            KeyValuePair("UnionValue", box("CaseA"))
            KeyValuePair("DateTimeValue", box(DateTime(1955, 11, 5)))
        ]
        [ 
            KeyValuePair("IntValue", box(2)) 
            KeyValuePair("StringValue", box("Test2"))
            KeyValuePair("EnumValue", box("camelValue"))
            KeyValuePair("UnionValue", box("CaseB"))
            KeyValuePair("DateTimeValue", box(DateTime(1985, 11, 5)))
        ]
        [ 
            KeyValuePair("IntValue", box(3)) 
            KeyValuePair("StringValue", box("Test3"))
            KeyValuePair("EnumValue", box("snake_value"))
            KeyValuePair("UnionValue", box("CaseA"))
            KeyValuePair("DateTimeValue", box(DateTime(2015, 10, 21)))
        ]
        [
            KeyValuePair("IntValue", null) 
            KeyValuePair("StringValue", null)
            KeyValuePair("EnumValue", null)
            KeyValuePair("UnionValue", null)
            KeyValuePair("DateTimeValue", null)
        ]                
    ]


module TabDataCellTests =    
    //----------------------------------------------------
    // Cast
    [<Fact>]
    let ``Can Cast``() =
        TestRows[0][0] |> TabDataCell.Cast<int> |> should equal 1
        TestRows[1][0] |> TabDataCell.Cast<int> |> should equal 2
        TestRows[0][0] |> TabDataCell.Cast<TestEnum> |> should equal TestEnum.camelValue
        TestRows[1][0] |> TabDataCell.Cast<TestEnum> |> should equal TestEnum.PascalValue
        TestRows[0][1] |> TabDataCell.Cast<string> |> should equal "Test"
        TestRows[1][1] |> TabDataCell.Cast<string> |> should equal "Test2"        
        TestRows[0][4] |> TabDataCell.Cast<DateTime> |> should equal (DateTime(1955, 11, 5))
        TestRows[1][4] |> TabDataCell.Cast<DateTime> |> should equal (DateTime(1985, 11, 5))
    

    [<Fact>]
    let ``Cast throws InvalidCastException``() =
        (fun () -> TestRows[0][0] |> TabDataCell.Cast<string> |> ignore) |> should throw typeof<InvalidCastException>
        (fun () -> TestRows[0][1] |> TabDataCell.Cast<TestEnum> |> ignore) |> should throw typeof<InvalidCastException>
        (fun () -> TestRows[0][0] |> TabDataCell.Cast<DateTime> |> ignore) |> should throw typeof<InvalidCastException>
        (fun () -> TestRows[0][1] |> TabDataCell.Cast<int> |> ignore) |> should throw typeof<InvalidCastException>


    [<Fact>]
    let ``Cast throws NullReferenceException``() =
        (fun () -> TestRows[3][0] |> TabDataCell.Cast<int> |> ignore) |> should throw typeof<NullReferenceException>        
        (fun () -> TestRows[3][0] |> TabDataCell.Cast<TestEnum> |> ignore) |> should throw typeof<NullReferenceException>        
        (fun () -> TestRows[3][4] |> TabDataCell.Cast<DateTime> |> ignore) |> should throw typeof<NullReferenceException>


    [<Fact>]
    let ``Cast does not throw NullReferenceException when type is nullable``() =
        TestRows[3][1] |> TabDataCell.Cast<string> |> should be null


    //----------------------------------------------------
    // CastOptional    
    [<Fact>]
    let ``Can CastOptional``() =
        TestRows[0][0] |> TabDataCell.CastOptional<int> |> should equal (Some 1)
        TestRows[1][0] |> TabDataCell.CastOptional<int> |> should equal (Some 2)
        TestRows[0][0] |> TabDataCell.CastOptional<TestEnum> |> should equal (Some TestEnum.camelValue)
        TestRows[1][0] |> TabDataCell.CastOptional<TestEnum> |> should equal (Some TestEnum.PascalValue)
        TestRows[0][1] |> TabDataCell.CastOptional<string> |> should equal (Some "Test")
        TestRows[1][1] |> TabDataCell.CastOptional<string> |> should equal (Some "Test2")
        TestRows[0][4] |> TabDataCell.CastOptional<DateTime> |> should equal (Some (DateTime(1955, 11, 5)))
        TestRows[1][4] |> TabDataCell.CastOptional<DateTime> |> should equal (Some (DateTime(1985, 11, 5)))


    [<Fact>]
    let ``CastOptional throws InvalidCastException``() =                
        (fun () -> TestRows[0][0] |> TabDataCell.CastOptional<string> |> ignore) |> should throw typeof<InvalidCastException>
        (fun () -> TestRows[0][1] |> TabDataCell.CastOptional<TestEnum> |> ignore) |> should throw typeof<InvalidCastException>
        (fun () -> TestRows[0][0] |> TabDataCell.CastOptional<DateTime> |> ignore) |> should throw typeof<InvalidCastException>
        (fun () -> TestRows[0][1] |> TabDataCell.CastOptional<int> |> ignore) |> should throw typeof<InvalidCastException>


    [<Fact>]
    let ``CastOptional returns None for null``() =
        TestRows[3][0] |> TabDataCell.CastOptional<int> |> should equal Option<int>.None
        TestRows[3][0] |> TabDataCell.CastOptional<TestEnum> |> should equal Option<TestEnum>.None
        TestRows[3][1] |> TabDataCell.CastOptional<string> |> should equal Option<string>.None
        TestRows[3][4] |> TabDataCell.CastOptional<DateTime> |> should equal Option<DateTime>.None


    [<Fact>]
    let ``Can ParseEnumWithConvention``() =
        TestRows[0][2] |> TabDataCell.ParseEnumWithConvention<TestEnum> PascalCase |> should equal TestEnum.PascalValue
        TestRows[1][2] |> TabDataCell.ParseEnumWithConvention<TestEnum> CamelCase |> should equal TestEnum.camelValue
        TestRows[2][2] |> TabDataCell.ParseEnumWithConvention<TestEnum> SnakeCase |> should equal TestEnum.snake_value
    

    //----------------------------------------------------
    // ParseEnumWithConvention    
    [<Fact>]
    let ``ParseEnumWithConvention throws ArgumentException``() =
        (fun () -> TestRows[0][1] |> TabDataCell.ParseEnumWithConvention<TestEnum> CamelCase |> ignore) 
        |> should throw typeof<ArgumentException>


    [<Fact>]
    let ``ParseEnumWithConvention throws InvalidCastException``() =
        (fun () -> TestRows[0][0] |> TabDataCell.ParseEnumWithConvention<TestEnum> PascalCase |> ignore) 
        |> should throw typeof<InvalidCastException>

        (fun () -> TestRows[0][4] |> TabDataCell.ParseEnumWithConvention<TestEnum> SnakeCase |> ignore) 
        |> should throw typeof<InvalidCastException>


    [<Fact>]
    let ``ParseEnumWithConvention throws ArgumentNullException``() =
        (fun () -> TestRows[3][2] |> TabDataCell.ParseEnumWithConvention<TestEnum> PascalCase |> ignore) 
        |> should throw typeof<ArgumentNullException>        


    //----------------------------------------------------
    // ParseEnum    
    [<Fact>]
    let ``Can ParseEnum``() =
        TestRows[0][2] |> TabDataCell.ParseEnum<TestEnum> |> should equal TestEnum.PascalValue
        

    [<Fact>]
    let ``ParseEnum throws ArgumentException``() =
        (fun () -> TestRows[0][1] |> TabDataCell.ParseEnum<TestEnum> |> ignore) 
        |> should throw typeof<ArgumentException>


    [<Fact>]
    let ``ParseEnum throws InvalidCastException``() =
        (fun () -> TestRows[0][0] |> TabDataCell.ParseEnum<TestEnum> |> ignore) 
        |> should throw typeof<InvalidCastException>

        (fun () -> TestRows[0][4] |> TabDataCell.ParseEnum<TestEnum> |> ignore) 
        |> should throw typeof<InvalidCastException>


    [<Fact>]
    let ``ParseEnum throws ArgumentNullException``() =
        (fun () -> TestRows[3][2] |> TabDataCell.ParseEnum<TestEnum> |> ignore) 
        |> should throw typeof<ArgumentNullException>        
        

    //----------------------------------------------------
    // ParseOptionalEnumWithConvention
    [<Fact>]
    let ``Can ParseOptionalEnumWithConvention``() =
        TestRows[0][2] |> TabDataCell.ParseOptionalEnumWithConvention<TestEnum> PascalCase 
        |> should equal (Some TestEnum.PascalValue)

        TestRows[1][2] |> TabDataCell.ParseOptionalEnumWithConvention<TestEnum> CamelCase 
        |> should equal (Some TestEnum.camelValue)

        TestRows[2][2] |> TabDataCell.ParseOptionalEnumWithConvention<TestEnum> SnakeCase 
        |> should equal (Some TestEnum.snake_value)
    

    [<Fact>]
    let ``ParseOptionalEnumWithConvention returns None for null``() =
        TestRows[3][2] |> TabDataCell.ParseOptionalEnumWithConvention<TestEnum> PascalCase 
        |> should equal Option<TestEnum>.None


    [<Fact>]
    let ``ParseOptionalEnumWithConvention throws ArgumentException``() =
        (fun () -> TestRows[0][1] |> TabDataCell.ParseOptionalEnumWithConvention<TestEnum> CamelCase |> ignore) 
        |> should throw typeof<ArgumentException>


    [<Fact>]
    let ``ParseOptionalEnumWithConvention throws InvalidCastException``() =
        (fun () -> TestRows[0][0] |> TabDataCell.ParseOptionalEnumWithConvention<TestEnum> PascalCase |> ignore) 
        |> should throw typeof<InvalidCastException>

        (fun () -> TestRows[0][4] |> TabDataCell.ParseOptionalEnumWithConvention<TestEnum> SnakeCase |> ignore) 
        |> should throw typeof<InvalidCastException>


    //----------------------------------------------------
    // ParseOptionalEnum
    [<Fact>]
    let ``Can ParseOptionalEnum``() =
        TestRows[0][2] |> TabDataCell.ParseOptionalEnum<TestEnum> |> should equal (Some TestEnum.PascalValue)
        

    [<Fact>]
    let ``ParseOptionalEnum returns None for null``() =
        TestRows[3][2] |> TabDataCell.ParseOptionalEnum<TestEnum>|> should equal Option<TestEnum>.None


    [<Fact>]
    let ``ParseOptionalEnum throws ArgumentException``() =
        (fun () -> TestRows[0][1] |> TabDataCell.ParseOptionalEnum<TestEnum> |> ignore) 
        |> should throw typeof<ArgumentException>


    [<Fact>]
    let ``ParseOptionalEnum throws InvalidCastException``() =
        (fun () -> TestRows[0][0] |> TabDataCell.ParseOptionalEnum<TestEnum> |> ignore) 
        |> should throw typeof<InvalidCastException>

        (fun () -> TestRows[0][4] |> TabDataCell.ParseOptionalEnum<TestEnum> |> ignore) 
        |> should throw typeof<InvalidCastException>
    
        
    //----------------------------------------------------
    // ParseUnion
    [<Fact>]
    let ``Can ParseUnion``() = 
        TestRows[0][3] |> TabDataCell.ParseUnion<TestUnion> |> should equal CaseA
        TestRows[1][3] |> TabDataCell.ParseUnion<TestUnion> |> should equal CaseB


    [<Fact>]
    let ``ParseUnion throws ArgumentException``() =
        (fun () -> TestRows[0][1] |> TabDataCell.ParseUnion<TestUnion> |> ignore)
        |> should throw typeof<ArgumentException>

        (fun () -> TestRows[0][2] |> TabDataCell.ParseUnion<TestUnion> |> ignore)
        |> should throw typeof<ArgumentException>


    [<Fact>]
    let ``ParseUnion throws InvalidCastException``() =
        (fun () -> TestRows[0][0] |> TabDataCell.ParseUnion<TestUnion> |> ignore)
        |> should throw typeof<InvalidCastException>

        (fun () -> TestRows[0][4] |> TabDataCell.ParseUnion<TestUnion> |> ignore)
        |> should throw typeof<InvalidCastException>


    [<Fact>]
    let ``ParseUnion throws ArgumentNullException``() =
        (fun () -> TestRows[3][3] |> TabDataCell.ParseUnion<TestUnion> |> ignore)
        |> should throw typeof<ArgumentNullException>


    //----------------------------------------------------
    // ParseOptionalUnion
    [<Fact>]
    let ``Can ParseOptionalUnion``() = 
        TestRows[0][3] |> TabDataCell.ParseOptionalUnion<TestUnion> |> should equal (Some CaseA)
        TestRows[1][3] |> TabDataCell.ParseOptionalUnion<TestUnion> |> should equal (Some CaseB)


    [<Fact>]
    let ``ParseOptionalUnion returns None for null``() =
        TestRows[3][3] |> TabDataCell.ParseOptionalUnion<TestUnion> |> should equal Option<TestUnion>.None

    
    [<Fact>]
    let ``ParseOptionalUnion throws ArgumentException``() =
        (fun () -> TestRows[0][1] |> TabDataCell.ParseOptionalUnion<TestUnion> |> ignore)
        |> should throw typeof<ArgumentException>

        (fun () -> TestRows[0][2] |> TabDataCell.ParseOptionalUnion<TestUnion> |> ignore)
        |> should throw typeof<ArgumentException>


    [<Fact>]
    let ``ParseOptionalUnion throws InvalidCastException``() =
        (fun () -> TestRows[0][0] |> TabDataCell.ParseOptionalUnion<TestUnion> |> ignore)
        |> should throw typeof<InvalidCastException>

        (fun () -> TestRows[0][4] |> TabDataCell.ParseOptionalUnion<TestUnion> |> ignore)
        |> should throw typeof<InvalidCastException>    