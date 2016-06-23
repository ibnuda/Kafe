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

[<Test>]
let ``Cannot serve non ordered drinks`` () =
  let order = {order with Drinks = [coke]}
  Given (PlacedOrder order)
  |> When (ServeDrink (lemonade, order.Tab.Id))
  |> ShouldFailWith (CanNotServeNonOrderedDrink lemonade)

[<Test>]
let ``Cannot serve already served drink`` () =
  Given (ServedOrder order)
  |> When (ServeDrink (coke, order.Tab.Id))
  |> ShouldFailWith OrderAlreadyServed

[<Test>]
let ``Cannot serve drink for non placed order`` () =
  Given (OpenedTab tab)
  |> When (ServeDrink (coke, tab.Id))
  |> ShouldFailWith CanNotServeForNonPlacedOrder

[<Test>]
let ``Cannot serve with closed tab`` () =
  Given (ClosedTab None)
  |> When (ServeDrink (coke, tab.Id))
  |> ShouldFailWith CanNotServeWithClosedTab

[<Test>]
let ``Can serve drink for order containing only one drink`` () =
  let order = {order with Drinks = [coke]}
  let payment = {Tab = order.Tab; Amount = drinkPrice coke}
  Given (PlacedOrder order)
  |> When (ServeDrink (coke, order.Tab.Id))
  |> ThenStateShouldBe (ServedOrder order)
  |> WithEvents [
    DrinkServed (coke, order.Tab.Id)
    OrderServed (order, payment)
  ]

[<Test>]
let ``Remain in order in progress while serving drink`` () =
  let order = {order with Drinks = [coke; lemonade; appleJuice]}
  let orderInProgress = {
    PlacedOrder = order
    ServedDrinks = [coke]
    PreparedFoods = []
    ServedFoods = []
  }
  let expected = {
    orderInProgress with ServedDrinks = orderInProgress.ServedDrinks
  }

  Given (OrderInProgress orderInProgress)
  |> When (ServeDrink (lemonade, order.Tab.Id))
  |> ThenStateShouldBe (OrderInProgress expected)
  |> WithEvents [DrinkServed (lemonade, order.Tab.Id)]

[<Test>]
let ``Can not serve non ordered drinks during order in progress`` () =
  let order = {order with Drinks = [coke; lemonade]}
  let orderInProgress = {
    PlacedOrder = order
    ServedDrinks = [coke]
    PreparedFoods = []
    ServedFoods = []
  }

  Given (OrderInProgress orderInProgress)
  |> When (ServeDrink (appleJuice, order.Tab.Id))
  |> ShouldFailWith (CanNotServeNonOrderedDrink appleJuice)

[<Test>]
let ``Can not serve an already served drinks`` () =
  let order = {order with Drinks = [coke; lemonade]}
  let orderInProgress = {
    PlacedOrder = order
    ServedDrinks = [coke]
    PreparedFoods = []
    ServedFoods = []
  }

  Given (OrderInProgress orderInProgress)
  |> When (ServeDrink (coke, order.Tab.Id))
  |> ShouldFailWith (CanNotServeAlreadyServedDrink coke)
