﻿module States

open Domain
open Events
open System

type State =
| ClosedTab of Guid option
| OpenedTab of Tab
| PlacedOrder of Order
| OrderInProgress of InProgressOrder
| ServedOrder of Order

let apply state event =
  match state, event with
  | ClosedTab _, TabOpened tab -> OpenedTab tab
  | OpenedTab _, OrderPlaced order -> PlacedOrder order
  | PlacedOrder order, DrinkServed (item, _) ->
    {
      PlacedOrder = order
      ServedDrinks = [item]
      ServedFoods = []
      PreparedFoods = []
    } |> OrderInProgress
  | _ -> state