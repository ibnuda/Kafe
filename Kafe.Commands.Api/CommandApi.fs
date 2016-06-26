module CommandApi

open System.Text
open CommandHandlers
open OpenTab
open Queries
open Chessie.ErrorHandling

let handleCommandRequest validationQueries eventStore = function
| OpenTabRequest tab ->
  validationQueries.Table.GetTableByTableNumber
  |> openTabCommander
  |> handleCommand eventStore tab
| _ -> messageError "Invalid command" |> fail |> async.Return

