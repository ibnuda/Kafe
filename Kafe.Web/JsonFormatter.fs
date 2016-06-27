module JsonFormatter

open Newtonsoft.Json.Linq
open Domain
open States
open Events
open CommandHandlers
open ReadModels
open Suave
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors

let (.=) key (value : obj) = new JProperty(key, value)

let jobj jProperties =
  let jObject = new JObject()
  jProperties |> List.iter jObject.Add
  jObject

let jArray jObjects =
  let jArray = new JArray()
  jObjects |> List.iter jArray.Add
  jArray

let tabIdJObj tabId =
  jobj [
    "tabId" .= tabId
  ]

let tabJObj tab =
  jobj [
    "id" .= tab.Id
    "tableNumber" .= tab.TableNumber
  ]

let itemJObj item =
  jobj [
    "menuNumber" .= item.MenuNumber
    "name" .= item.Name
  ]

let foodJObj (Food food) = itemJObj food

let drinkJObj (Drink drink) = itemJObj drink

let foodJArray foods = foods |> List.map foodJObj |> jArray

let drinkJArray drinks = drinks |> List.map drinkJObj |> jArray

let orderJObj (order : Order) =
  jobj [
    "tabId" .= order.Tab.Id
    "tableNumber" .= order.Tab.TableNumber
    "foods" .= order.Foods
    "drinks" .= order.Drinks
  ]

let orderInProgressJObj (orderInProgress : InProgressOrder) =
  jobj [
    "tabId" .= orderInProgress.PlacedOrder.Tab.Id
    "tableNumber" .= orderInProgress.PlacedOrder.Tab.TableNumber
    "preparedFoods" .= foodJArray orderInProgress.PreparedFoods
    "servedFoods" .= foodJArray orderInProgress.ServedFoods
    "servedDrinks" .= drinkJArray orderInProgress.ServedDrinks
  ]

let stateJObj = function
| ClosedTab tabId ->
  let state = "state" .= "ClosedTab"
  match tabId with
  | Some id -> jobj [state; "data" .= tabIdJObj id]
  | None -> jobj [state]
| OpenedTab tab ->
  jobj [
    "state" .= "OpenedTab"
    "data" .= tabJObj tab
  ]
| PlacedOrder order ->
  jobj [
    "state" .= "PlacedOrder"
    "data" .= orderJObj order
  ]
| OrderInProgress inOrderProgress ->
  jobj [
    "state" .= "OrderInProgress"
    "data" .= orderInProgressJObj inOrderProgress
  ]
| ServedOrder order ->
  jobj [
    "state" .= "ServedOrder"
    "data" .= orderJObj order
  ]

let statusJObj = function
| Open tabId ->
  "status" .= jobj [
    "open" .= tabId.ToString()
  ]
| InService tabId ->
  "status" .= jobj [
    "inService" .= tabId.ToString()
  ]
| Closed -> "status" .= "closed"

let eventJObj = function
| TabOpened tab ->
  jobj [
    "event" .= "TabOpened"
    "data" .= tabJObj tab
  ]
| OrderPlaced order ->
  jobj [
    "event" .= "OrderPlaced"
    "data" .= jobj [
      "order" .= orderJObj order
    ]
  ]
| DrinkServed (item, tabId) ->
  jobj [
    "event" .= "DrinkServed"
    "data" .= jobj [
      "drink" .= drinkJObj item
      "tabId" .= tabId
    ]
  ]
| FoodPrepared (item, tabId) ->
  jobj [
    "event" .= "FoodPrepared"
    "data" .= jobj [
      "food" .= foodJObj item
      "tabId" .= tabId
    ]
  ]
| FoodServed (item, tabId) ->
  jobj [
    "event" .= "FoodServed"
    "data" .= jobj [
      "food" .= foodJObj item
      "tabId" .= tabId
    ]
  ]

| OrderServed (order, payment) ->
  jobj [
    "event" .= "OrderServed"
    "data" .= jobj [
      "order" .= orderJObj order
      "tabId" .= payment.Tab.Id
      "tableNumber" .= payment.Tab.TableNumber
      "amount" .= payment.Amount
    ]
  ]
| TabClosed payment ->
  jobj [
    "event" .= "TabClosed"
    "data" .= jobj [
      "amountPaid" .= payment.Amount
      "tabId" .= payment.Tab.Id
      "tableNumber" .= payment.Tab.TableNumber
    ]
  ]

let tableJObj table =
  jobj [
    "number" .= table.Number
    "waiter" .= table.Waiter
    statusJObj table.Status
  ]

let chefToDoJObj (toDo : ChefToDo) =
  jobj [
    "tabId" .= toDo.Tab.Id.ToString
    "tableNumber" .= toDo.Tab.TableNumber
    "foods" .= toDo.Foods
  ]

let waiterToDoJObj (toDo : WaiterToDo) =
  jobj [
    "tabId" .= toDo.Tab.Id
    "tableNumber" .= toDo.Tab.TableNumber
    "foods" .= foodJArray toDo.Foods
    "drinks" .= drinkJArray toDo.Drinks
  ]

let cashierToDoJObj (payment : Payment) =
  jobj [
    "tabId" .= payment.Tab.Id
    "tableNumber" .= payment.Tab.TableNumber
    "paymentAmount" .= payment.Amount
  ]

let JSON webPart jsonString (context : HttpContext) = async {
  let wp = webPart jsonString >=> Writers.setMimeType "application/json; charset=utf-8"
  return! wp context
}

let toStateJson state =
  state |> stateJObj |> string |> JSON OK

let toErrorJson error =
  jobj [
    "error" .= error.Message
  ]
  |> string
  |> JSON BAD_REQUEST

let toReadModelsJson toJObj key models =
  models
  |> List.map toJObj
  |> JArray
  |> (.=) key
  |> List.singleton
  |> jobj
  |> string
  |> JSON OK

let toTablesJson = toReadModelsJson tableJObj "tables"
let toChefToDosJson = toReadModelsJson chefToDoJObj "chefToDos"
let toWaiterToDosJson = toReadModelsJson waiterToDoJObj "waiterToDos"
let toCashierToDosJson = toReadModelsJson cashierToDoJObj "cashierToDos"
let toFoodsJson = toReadModelsJson foodJObj "foods"
let toDrinksJson = toReadModelsJson drinkJObj "drinks"
