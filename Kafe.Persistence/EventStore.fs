module EventStore

open States
open System
open NEventStore
open Events

let getTabIdFromState = function
| ClosedTab None -> None
| OpenedTab tab -> Some tab.Id
| PlacedOrder order -> Some order.Tab.Id
| OrderInProgress order -> Some order.PlacedOrder.Tab.Id
| ServedOrder order -> Some order.Tab.Id
| ClosedTab (Some tab) -> Some tab

let saveEvent (storeEvents : IStoreEvents) state event =
  match getTabIdFromState state with
  | Some tabId ->
    use stream = storeEvents.OpenStream(tabId.ToString())
    stream.Add(new EventMessage(Body = event))
    stream.CommitChanges(Guid.NewGuid())
  | None -> ()

let saveEvents (storeEvents : IStoreEvents) state events =
  events
  |> List.iter (saveEvent storeEvents state)
  |> async.Return

let getEvents (storeEvents : IStoreEvents) (tabId : Guid) =
  use stream = storeEvents.OpenStream(tabId.ToString())
  stream.CommittedEvents
  |> Seq.map (fun message -> message.Body)
  |> Seq.cast<Event>

let getState storeEvents tabId =
  getEvents storeEvents tabId
  |> Seq.fold apply (ClosedTab None)
  |> async.Return

type EventStore = {
  GetState : Guid -> Async<State>
  SaveEvents : State -> Event list -> Async<unit>
}