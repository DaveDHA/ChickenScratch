namespace ChickenScratch

open System
open Microsoft.FSharp.Reflection
open ChickenScratch.Utility


type TabDataCell = System.Collections.Generic.KeyValuePair<string,obj>
type TabDataRow = TabDataCell seq


type private IsEnum<'t when 't : (new : unit -> 't)
                        and 't : struct 
                        and 't :> ValueType> = 't
    

type TabDataParseException(targetType : Type, ?kvp : TabDataCell, ?innerEx) =
    inherit Exception("", match innerEx with | Some x -> x | _ -> null)

        
    override this.Message = 
        seq {
            yield "Failed to parse tabular data"
                        
            match kvp with
            | Some x ->
                yield $"Tabular data key: {x.Key}"
                let value = if x.Value <> null then x.Value else box "null"
                yield $"Tabular data value: {value}"
            | _ -> ()
                        
            yield $"Target type: {targetType.FullName}"

            match innerEx with
            | Some x -> yield x.Message
            | _ -> ()            
        }
        |> String.concat ". "




module TabDataCell =           
    let inline private try'<'t> (f : TabDataCell -> 't) (kvp : TabDataCell) =
        try f kvp
        with
            | :? TabDataParseException as ex -> reraise()
            | ex -> raise (TabDataParseException(typeof<'t>, kvp, ex))   

    let inline Cast<'t> kvp = kvp |> try' (fun kvp -> kvp.Value :?> 't)
    

    let inline CastOptional<'t> (kvp : TabDataCell) = kvp |> try' (fun kvp ->
        match kvp.Value with
        | null -> None
        | _ -> Some (Cast<'t> kvp)
    )


    let inline ParseEnumWithConvention<'t when IsEnum<'t>> convention (kvp : TabDataCell) = 
        kvp |> try' (fun kvp -> 
            kvp.Value :?> string |> String.toNamingConvention convention |> Enum.Parse<'t>
        )


    let inline ParseEnum<'t when IsEnum<'t>> kvp = kvp |> try' (fun kvp -> 
        ParseEnumWithConvention<'t> PascalCase kvp
    )


    let inline ParseOptionalEnumWithConvention<'t when IsEnum<'t>> convention (kvp : TabDataCell) = 
        kvp |> try' (fun kvp ->
            match kvp.Value with
            | null -> None
            | _ -> Some (ParseEnumWithConvention<'t> convention kvp)
        )


    let inline ParseOptionalEnum<'t when IsEnum<'t>> kvp = kvp |> try' (fun kvp ->
        ParseOptionalEnumWithConvention<'t> PascalCase kvp
    )


    let inline ParseUnion<'t> (kvp : TabDataCell) = kvp |> try' (fun kvp ->
        let case = kvp.Value :?> string |> String.toNamingConvention PascalCase    
        
        FSharpType.GetUnionCases typeof<'t> 
        |> Array.tryFind (fun uc -> uc.Name = case)
        |> function
            | Some uc -> FSharpValue.MakeUnion(uc, [||]) :?> 't
            | _ -> invalidArg "unionCase" (sprintf "Could not find union case %s" case)
    )


    let inline ParseOptionalUnion<'t> (kvp : TabDataCell) = 
        match kvp.Value with
        | null -> None
        | _ -> Some (ParseUnion<'t> kvp)



module TabDataRow =
    //let inline private try'<'t> (f : unit -> 't) (row : TabDataRow) =
    //    try f()
    //    with
    //        | :? TabDataParseException as ex -> reraise()
    //        | ex -> raise (TabDataParseException(typeof<'t>, kvp, ex))   


    let inline Value<'t> name (row : TabDataRow) = 
        row |> Seq.find (fun kvp -> kvp.Key = name) |> TabDataCell.Cast<'t>


    let inline OptionalValue<'t> name (row : TabDataRow) = 
        row 
        |> Seq.find (fun kvp -> kvp.Key = name) 
        |> TabDataCell.CastOptional<'t>            
        

    let inline EnumValueWithConvention<'t when IsEnum<'t>> convention name (row : TabDataRow) = 
        row |> Seq.find (fun kvp -> kvp.Key = name) |> TabDataCell.ParseEnumWithConvention<'t> convention


    let inline EnumValue<'t when IsEnum<'t>> name row = EnumValueWithConvention<'t> PascalCase name row


    let inline OptionalEnumValueWithConvention<'t when IsEnum<'t>> convention name (row : TabDataRow) =
        row 
        |> Seq.find (fun kvp -> kvp.Key = name)
        |> TabDataCell.ParseOptionalEnumWithConvention<'t> convention


    let inline OptionalEnumValue<'t when IsEnum<'t>> name row = OptionalEnumValueWithConvention<'t> PascalCase name row


    let inline UnionValue<'t> name (row : TabDataRow) = 
        row |> Seq.find (fun kvp -> kvp.Key = name) |> TabDataCell.ParseUnion<'t>


    let inline OptionalUnionValue<'t> name (row : TabDataRow) =
        row 
        |> Seq.find (fun kvp -> kvp.Key = name) 
        |> TabDataCell.ParseOptionalUnion<'t>
      