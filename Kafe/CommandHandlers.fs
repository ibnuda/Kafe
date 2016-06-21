module CommandHandlers

open States
open Events
open System
open Commands
open Domain

let execute state command =
  match command with
  | OpenTab tab -> TabOpened tab
  | _ -> failwith "TODO" // [TabOpened { Id = Guid.NewGuid(); TableNumber = 1}]

let evolve state command =
  let events = execute state command
  let newState = apply state events
  (newState, events)