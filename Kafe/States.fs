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
  | OrderInProgress ipo, OrderServed (order, _) -> ServedOrder order
  | PlacedOrder order, FoodPrepared (food, _) ->
    {
      PlacedOrder = order
      PreparedFoods = [food]
      ServedDrinks = []
      ServedFoods = []
    } |> OrderInProgress
  | OrderInProgress ipo, FoodPrepared (food, _) ->
    {
      ipo with PreparedFoods = food :: ipo.PreparedFoods
    }
    |> OrderInProgress
  | OrderInProgress ipo, FoodServed (food, _) -> 
    {
      ipo with ServedFoods = food :: ipo.ServedFoods
    } |> OrderInProgress
  | ServedOrder order, TabClosed payment -> ClosedTab (Some payment.Tab.Id)
  | _ -> state