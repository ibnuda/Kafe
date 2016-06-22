module ServeDrinkTests

open Domain
open System
open States
open Commands
open Events
open KafeAppTestsDSL
open NUnit.Framework
open TestData
open Errors

[<Test>]
let ``Can Serve Drink`` () =
  let order = {order with Drinks = [coke; lemonade]}
  let expected = {
    PlacedOrder = order
    ServedDrinks = [coke]
    PreparedFoods = []
    ServedFoods = []
  }
  Given (PlacedOrder order)
  |> When (ServeDrink (coke, order.Tab.Id))
  |> ThenStateShouldBe (OrderInProgress expected)
  |> WithEvents [DrinkServed (coke, order.Tab.Id)]

let ``Cannot serve non ordered drinks`` () =
  let order = {order with Drinks = [coke]}
  Given (PlacedOrder order)
  |> When (ServeDrink (lemonade, order.Tab.Id))
  |> ShouldFailWith (CanNotServeNonOrderedDrink lemonade)

let ``Cannot serve already served drink`` () =
  Given (ServedOrder order)
  |> When (ServeDrink (coke, order.Tab.Id))
  |> ShouldFailWith OrderAlreadyServed

let ``Cannot serve drink for non placed order`` () =
  Given (OpenedTab tab)
  |> When (ServeDrink (coke, tab.Id))
  |> ShouldFailWith CanNotServeForNonPlacedOrder

let ``Cannot serve with closed tab`` () =
  Given (ClosedTab None)
  |> When (ServeDrink (coke, tab.Id))
  |> ShouldFailWith CanNotServeWithClosedTab