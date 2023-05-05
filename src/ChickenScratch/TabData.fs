namespace ChickenScratch

open System
open Microsoft.FSharp.Reflection
open ChickenScratch.Utility


type TabDataCell = System.Collections.Generic.KeyValuePair<string,obj>
type TabDataRow = TabDataCell seq


type private IsEnum<'t when 't : (new : unit -> 't)
                        and 't : struct 
                        and 't :> ValueType> = 't
    

module TabDataCell =           
    let inline Cast<'t> (kvp : TabDataCell) = kvp.Value :?> 't


    let inline CastOptional<'t> (kvp : TabDataCell) = 
        match kvp.Value with
        | null -> None
        | _ -> Some (Cast<'t> kvp)


    let inline ParseEnumWithConvention<'t when IsEnum<'t>> convention (kvp : TabDataCell) = 
        kvp.Value :?> string |> String.toNamingConvention convention |> Enum.Parse<'t>


    let inline ParseEnum<'t when IsEnum<'t>> kvp = ParseEnumWithConvention<'t> PascalCase kvp


    let inline ParseOptionalEnumWithConvention<'t when IsEnum<'t>> convention (kvp : TabDataCell) = 
        match kvp.Value with
        | null -> None
        | _ -> Some (ParseEnumWithConvention<'t> convention kvp)


    let inline ParseOptionalEnum<'t when IsEnum<'t>> kvp = 
        ParseOptionalEnumWithConvention<'t> PascalCase kvp


    let inline ParseUnion<'t> (kvp : TabDataCell) = 
        let case = kvp.Value :?> string |> String.toNamingConvention PascalCase
        
        FSharpType.GetUnionCases typeof<'t> 
        |> Array.tryFind (fun uc -> uc.Name = case)
        |> function
            | Some uc -> FSharpValue.MakeUnion(uc, [||]) :?> 't
            | _ -> invalidArg "unionCase" (sprintf "Could not find union case %s" case)


    let inline ParseOptionalUnion<'t> (kvp : TabDataCell) = 
        match kvp.Value with
        | null -> None
        | _ -> Some (ParseUnion<'t> kvp)



module TabDataRow =
    let inline Value<'t> name (row : TabDataRow) = 
        row |> Seq.find (fun kvp -> kvp.Key = name) |> TabDataCell.Cast<'t>


    let inline OptionalValue<'t> name (row : TabDataRow) = 
        row 
        |> Seq.tryFind (fun kvp -> kvp.Key = name) 
        |> function
            | Some kvp -> TabDataCell.CastOptional<'t> kvp
            | _ -> None
        

    let inline EnumValueWithConvention<'t when IsEnum<'t>> convention name (row : TabDataRow) = 
        row |> Seq.find (fun kvp -> kvp.Key = name) |> TabDataCell.ParseEnumWithConvention<'t> convention


    let inline EnumValue<'t when IsEnum<'t>> name row = EnumValueWithConvention<'t> PascalCase name row


    let inline OptionalEnumValueWithConvention<'t when IsEnum<'t>> convention name (row : TabDataRow) =
        row 
        |> Seq.tryFind (fun kvp -> kvp.Key = name)
        |> Option.bind (TabDataCell.ParseOptionalEnumWithConvention<'t> convention)


    let inline OptionalEnumValue<'t when IsEnum<'t>> name row = OptionalEnumValueWithConvention<'t> PascalCase name row


    let inline UnionValue<'t> name (row : TabDataRow) = 
        row |> Seq.find (fun kvp -> kvp.Key = name) |> TabDataCell.ParseUnion<'t>


    let inline OptionalUnionValue<'t> name (row : TabDataRow) =
        row 
        |> Seq.tryFind (fun kvp -> kvp.Key = name) 
        |> Option.bind (TabDataCell.ParseOptionalUnion<'t>)
      