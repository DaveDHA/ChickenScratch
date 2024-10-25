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
        

let private makeOptCol<'t, 'c when DataFrameable<'t> and 'c :> DataFrameColumn> 
    (name : string) 
    (ctor : (string * (Nullable<'t> seq) -> 'c)) 
    (data : obj seq) =
        ctor(name, data |> Seq.cast<Option<'t>> |> Seq.map Option.toNullable) :> DataFrameColumn


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
    | :? Option<bool> -> makeOptCol field.Name BooleanDataFrameColumn data
    | :? Option<byte> -> makeOptCol field.Name ByteDataFrameColumn data
    | :? Option<char> -> makeOptCol field.Name CharDataFrameColumn data
    | :? Option<DateTime> -> makeOptCol field.Name DateTimeDataFrameColumn data
    | :? Option<decimal> -> makeOptCol field.Name DecimalDataFrameColumn data
    | :? Option<double> -> makeOptCol field.Name DoubleDataFrameColumn data        
    | :? Option<int16> -> makeOptCol field.Name Int16DataFrameColumn data
    | :? Option<int> -> makeOptCol field.Name Int32DataFrameColumn data        
    | :? Option<int64> -> makeOptCol field.Name Int64DataFrameColumn data
    | :? Option<sbyte> -> makeOptCol field.Name SByteDataFrameColumn data
    | :? Option<single> -> makeOptCol field.Name SingleDataFrameColumn data
    | :? Option<uint16> -> makeOptCol field.Name UInt16DataFrameColumn data
    | :? Option<uint> -> makeOptCol field.Name UInt32DataFrameColumn data
    | :? Option<uint64> -> makeOptCol field.Name UInt64DataFrameColumn data
    | :? Option<string> -> 
        let data' = data |> Seq.cast<Option<string>> |> Seq.map (function Some s -> s | _ -> null)
        StringDataFrameColumn(field.Name, data') :> DataFrameColumn
    | _ -> StringDataFrameColumn(field.Name, (data |> Seq.map (fun x -> if x = null then null else x.ToString()))) :> DataFrameColumn


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
    

let toMsDataFrame (tabData : TabularDataResource) =
    tabData.Schema.Fields |> Seq.map (toDataFrameColumn tabData) |> DataFrame 

