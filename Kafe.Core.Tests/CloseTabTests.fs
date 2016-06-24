module CloseTabTests

open Domain
open States
open Commands
open Events
open KafeAppTestsDSL
open NUnit.Framework
open TestData
open Errors

[<Test>]
let ``Can close the tab by paying full amount`` () =
  let order = {order with Foods = [salad; pizza; pecel]; Drinks = [appleJuice]}
  let payment = {Tab = tab; Amount = 17.4m}

  Given (ServedOrder order)
  |> When (CloseTab payment)
  |> ThenStateShouldBe (ClosedTab (Some tab.Id))
  |> WithEvents [TabClosed payment]

[<Test>]
let ``Can not close tab with invalid payment amount`` () =
  let order = {order with Foods = [salad; pizza; pecel]; Drinks = [appleJuice]}
  let payment = {Tab = tab; Amount = 18.4m}

  Given (ServedOrder order)
  |> When (CloseTab payment)
  |> ShouldFailWith (InvalidPayment (17.4m, 18.4m))