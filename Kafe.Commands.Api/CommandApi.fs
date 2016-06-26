module CommandApi

open System.Text
open CommandHandlers
open OpenTab
open PlaceOrder
open ServeDrink
open PrepareFood
open ServeFood
open CloseTab
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
| PrepareFoodRequest (tabId, foodMenuNumber) ->
  validationQueries.Food.GetFoodByMenuNumber
  |> prepareFoodCommander validationQueries.Table.GetTableByTabId
  |> handleCommand eventStore (tabId, foodMenuNumber)
| ServeFoodRequest (tabId, foodMenuNumber) ->
  validationQueries.Food.GetFoodByMenuNumber
  |> serveFoodCommander validationQueries.Table.GetTableByTabId
  |> handleCommand eventStore (tabId, foodMenuNumber)
| CloseTabRequest (tabId, amount) ->
  closeTabCommander validationQueries.Table.GetTableByTabId
  |> handleCommand eventStore (tabId, amount)
| _ -> messageError "Invalid command" |> fail |> async.Return