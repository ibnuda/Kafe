module QueriesApi

open Queries
open Suave
open Suave.Filters
open Suave.Operators
open JsonFormatter
open CommandHandlers

let readModels getReadModels wp (context : HttpContext) = async {
  let! model = getReadModels ()
  return! wp model context
}

let queriesApi queries eventStore =
  GET >=> choose [
    path "/tables" >=> readModels queries.Table.GetTables toTablesJson
  ]