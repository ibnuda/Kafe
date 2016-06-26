module CommandHandlers

open Queries
open Commands
open CommandHandlers
open Errors
open EventStore
open Chessie.ErrorHandling

type Commander<'a, 'b> = {
  Validate : 'a -> Async<Choice<'b, string>>
  ToCommand : 'b -> Command
}

type ErrorResponse = {
  Message : string
}

let messageError message = {Message = message}

let getTabIdFromCommand = function
| OpenTab tab -> tab.Id
| PlaceOrder order -> order.Tab.Id
| ServeDrink (drink, tabId) -> tabId
| PrepareFood (food, tabId) -> tabId
| ServeFood (food, tabId) -> tabId
| CloseTab payment -> payment.Tab.Id

let handleCommand eventStore commandData commander = async {
  let! validationResult = commander.Validate commandData
  match validationResult with
  | Choice1Of2 validatedCommandData ->
    let command = commander.ToCommand validatedCommandData
    let! state = eventStore.GetState (getTabIdFromCommand command)
    match evolve state command with
    | Ok((newState, events), _) -> return (newState, events) |> ok
    | Bad (error) -> return error.Head |> toErrorString |> messageError |> fail
  | Choice2Of2 errorMessage ->
    return errorMessage |> messageError |> fail
}