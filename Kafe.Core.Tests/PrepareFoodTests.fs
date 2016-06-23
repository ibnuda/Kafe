module PrepareFoodTests

open Domain
open States
open Commands
open Events
open KafeAppTestsDSL
open NUnit.Framework
open TestData
open Errors

[<Test>]
let ``Can prepare foods`` () =
  let order = {order with Foods = [salad]}
  let expected = {
    PlacedOrder = order
    ServedDrinks = []
    PreparedFoods = [salad]
    ServedFoods = []
  }
  Given (PlacedOrder order)
  |> When (PrepareFood (salad, order.Tab.Id))
  |> ThenStateShouldBe (OrderInProgress expected)
  |> WithEvents [FoodPrepared (salad, order.Tab.Id)]

[<Test>]
let ``Can not prepare non ordered food`` () =
  let order = {order with Foods = [pizza]}
  Given (PlacedOrder order)
  |> When (PrepareFood (salad, order.Tab.Id))
  |> ShouldFailWith (CanNotPrepareNonOrderedFood salad)

[<Test>]
let ``Can not prepare served food`` () =
  Given (ServedOrder order)
  |> When (PrepareFood (salad, order.Tab.Id))
  |> ShouldFailWith OrderAlreadyServed

[<Test>]
let ``Can not prepare with closed tab`` () =
  Given (ClosedTab None)
  |> When (PrepareFood (salad, order.Tab.Id))
  |> ShouldFailWith CanNotPrepareWithClosedTab

[<Test>]
let ``Can prepare food during order in progress`` () =
  let order = {order with Foods = [salad; pizza]}
  let orderInProgress = {
    PlacedOrder = order
    ServedFoods = []
    ServedDrinks = []
    PreparedFoods = [pizza]
  }
  let expected = {orderInProgress with PreparedFoods = [salad; pizza]}

  Given (OrderInProgress orderInProgress)
  |> When (PrepareFood (salad, order.Tab.Id))
  |> ThenStateShouldBe (OrderInProgress expected)
  |> WithEvents [FoodPrepared (salad, order.Tab.Id)]

[<Test>]
let ``Can not prepare non ordered food during progress`` () =
  let order = {order with Foods = [salad]}
  let orderInProgress = {
    PlacedOrder = order
    ServedFoods = []
    ServedDrinks = []
    PreparedFoods = []
  }

  Given (OrderInProgress orderInProgress)
  |> When (PrepareFood (pizza, order.Tab.Id))
  |> ShouldFailWith (CanNotPrepareNonOrderedFood pizza)

[<Test>]
let ``Can not prepare already prepared food during progress`` () =
  let order = {order with Foods = [salad]}
  let orderInProgress = {
    PlacedOrder = order
    ServedFoods = []
    ServedDrinks = []
    PreparedFoods = [salad]
  }

  Given (OrderInProgress orderInProgress)
  |> When (PrepareFood (salad, order.Tab.Id))
  |> ShouldFailWith (CanNotPrepareAlreadyPreparedFood salad)