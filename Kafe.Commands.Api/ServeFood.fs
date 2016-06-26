module ServeFood

open FSharp.Data
open Commands
open Queries
open CommandHandlers
open PrepareFood

[<Literal>]
let ServeFoodJson = """{
  "serveFood" : {
    "tabId" : "b29fd982-d980-4b94-8c3f-36558e228e5f",
    "menuNumber" : 1
  }
}"""

type ServeFoodReq = JsonProvider<ServeFoodJson>

let (|ServeFoodRequest|_|) payload =
  try
    let request = ServeFoodReq.Parse(payload).ServeFood
    (request.TabId, request.MenuNumber) |> Some
  with
  | _ -> None

let serveFoodCommander getTableByTabId getFoodByMenuNumber = {
  Validate = validateFood getTableByTabId getFoodByMenuNumber
  ToCommand = ServeFood
}