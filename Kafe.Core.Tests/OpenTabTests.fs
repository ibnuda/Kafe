module OpenTabTests

open NUnit.Framework
open KafeAppTestsDSL
open Domain
open Events
open Commands
open States
open System
open Errors

[<Test>]
let ``Can Open a new Tab`` () =
  let tab = {Id = Guid.NewGuid(); TableNumber = 1}

  Given (ClosedTab None)
  |> When (OpenTab tab)
  |> ThenStateShouldBe (OpenedTab tab)
  |> WithEvents [TabOpened tab]

[<Test>]
let ``Cannot open an already Opened Tab`` () =
  let tab = {Id = Guid.NewGuid(); TableNumber = 1}

  Given (OpenedTab tab)
  |> When (OpenTab tab)
  |> ShouldFailWith TabAlreadyOpened