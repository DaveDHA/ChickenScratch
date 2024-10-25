module ChickenScratch.TabularDataResource

open ChickenScratch
open Microsoft.Data.Analysis
open Microsoft.DotNet.Interactive.Formatting.TabularData
open System


type private DataFrameable<'t
    when 't : (new : unit -> 't) 
    and 't : struct 
    and 't :> ValueType> = 't

let private makeCol<'t, 'c when DataFrameable<'t> and 'c :> DataFrameColumn> 
    (name : string) 
    (ctor : (string * (Nullable<'t> seq) -> 'c)) 
    (data : obj seq) =
        ctor(name, data |> Seq.cast<Nullable<'t>>) :> DataFrameColumn
        

let private colForValue (field : TableSchemaFieldDescriptor) data (value : obj) =
    match value with
    | :? bool -> makeCol field.Name BooleanDataFrameColumn data
    | :? byte -> makeCol field.Name ByteDataFrameColumn data
    | :? char -> makeCol field.Name CharDataFrameColumn data
    | :? DateTime -> makeCol field.Name DateTimeDataFrameColumn data
    | :? decimal -> makeCol field.Name DecimalDataFrameColumn data
    | :? double -> makeCol field.Name DoubleDataFrameColumn data
    | :? int16 -> makeCol field.Name Int16DataFrameColumn data
    | :? int -> makeCol field.Name Int32DataFrameColumn data
    | :? int64 -> makeCol field.Name Int64DataFrameColumn data
    | :? sbyte -> makeCol field.Name SByteDataFrameColumn data
    | :? single -> makeCol field.Name SingleDataFrameColumn data
    | :? uint16 -> makeCol field.Name UInt16DataFrameColumn data
    | :? uint -> makeCol field.Name UInt32DataFrameColumn data
    | :? uint64 -> makeCol field.Name UInt64DataFrameColumn data
    | :? string -> StringDataFrameColumn(field.Name, (data |> Seq.cast<string>)) :> DataFrameColumn
    | _ -> StringDataFrameColumn(field.Name, (data |> Seq.map (fun x -> x.ToString()))) :> DataFrameColumn


let private colForAllNulls (field : TableSchemaFieldDescriptor) (dataLen : int) =
    match field.Type with
    | TableSchemaFieldType.Number -> SingleDataFrameColumn(field.Name, dataLen) :> DataFrameColumn
    | TableSchemaFieldType.Integer -> Int32DataFrameColumn(field.Name, dataLen) :> DataFrameColumn
    | TableSchemaFieldType.Boolean -> BooleanDataFrameColumn(field.Name, dataLen) :> DataFrameColumn
    | TableSchemaFieldType.DateTime -> DateTimeDataFrameColumn(field.Name, dataLen) :> DataFrameColumn
    | _ -> StringDataFrameColumn(field.Name, dataLen) :> DataFrameColumn
    

let toDataFrameColumn (tabData : TabularDataResource) (field : TableSchemaFieldDescriptor) =
    try
        let data = tabData.Data |> Seq.map (TabDataRow.Value<obj> field.Name)

        data
        |> Seq.tryFind ((<>) null)
        |> function
            | Some x -> colForValue field data x
            | None -> colForAllNulls field (data |> Seq.length)                        
        
    with exn ->  
        raise (System.Exception(sprintf "Exception occurred creating DataFrameColumn '%s': %s" field.Name exn.Message, exn))
    

let toMsdaDataFrame (tabData : TabularDataResource) =
    tabData.Schema.Fields |> Seq.map (toDataFrameColumn tabData) |> DataFrame 

