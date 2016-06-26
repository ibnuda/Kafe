module CommandApi

open System.Text
open CommandHandlers
open OpenTab
open PlaceOrder
open ServeDrink
open Queries
open Chessie.ErrorHandling

let handleCommandRequest validationQueries eventStore = function
| OpenTabRequest tab ->
  validationQueries.Table.GetTableByTableNumber
  |> openTabCommander
  |> handleCommand eventStore tab
| PlaceOrderRequest placeOrder ->
  placeOrderCommander validationQueries
  |> handleCommand eventStore placeOrder
| ServeDrinkRequest (tabId, drinkMenuNumber) ->
  validationQueries.Drink.GetDrinkByMenuNumber
  |> serveDrinkCommander validationQueries.Table.GetTableByTabId
  |> handleCommand eventStore (tabId, drinkMenuNumber)
| _ -> messageError "Invalid command" |> fail |> async.Return

