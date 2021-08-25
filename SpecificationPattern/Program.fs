module Program

type User = { Name: string; IsManager: bool }

type Team =
    { ContainsUser: User -> bool
      Users: User list }

type Folder = { TeamsWithReadPermission: Team list }

type Document = { Creator: User; Parent: Folder }

type Context =
    { Document: Document
      CurrentUser: User }

type Specification<'T> =
    | Or of  Specification<'T> * Specification<'T>
    | And of Specification<'T> * Specification<'T>
    | Not of Specification<'T>
    | Rule of ('T -> bool)

module Specification =
    let rec isSatisfiedBy spec candidate =
        match spec with
        | And (left, right) ->
            (isSatisfiedBy left candidate)
            && (isSatisfiedBy right candidate)
        | Or (left, right) ->
            (isSatisfiedBy left candidate)
            || (isSatisfiedBy right candidate)
        | Not spec -> not (isSatisfiedBy spec candidate)
        | Rule isSatisfiedBy -> isSatisfiedBy candidate

    let rec prettyPrint spec =
        match spec with
        | And (left, right) -> $"(%s{prettyPrint left} AND %s{prettyPrint right})"
        | Or (left, right) -> $"(%s{prettyPrint left} OR %s{prettyPrint right})"
        | Not spec -> $"NOT(%s{prettyPrint spec})"
        | Rule rule -> $"%A{rule}"

[<RequireQualifiedAccess>]
module Rules =
    
    let private isWorkspaceManager (context: Context) = context.CurrentUser.IsManager

    let private hasReadPermission (context: Context) =
        let folder = context.Document.Parent

        folder.TeamsWithReadPermission
        |> Seq.exists (fun team -> team.ContainsUser(context.CurrentUser))

    let private createdTheDocument (context: Context) =
        context.Document.Creator = context.CurrentUser

    let canReadDocument =
        Or(Rule isWorkspaceManager, Rule hasReadPermission)

    let canDeleteDocument =
        Or(Rule isWorkspaceManager, Rule createdTheDocument)

    let canRenameDocument =
        And(canReadDocument, canDeleteDocument)

open Specification

[<EntryPoint>]
let main argv =

    let manager =
        { Name = "Boss person"
          IsManager = true }

    let notManager = { Name = "worker"; IsManager = false }
    let users = [ manager; notManager ]

    let team: Team =
        { Users = users
          ContainsUser = fun _ -> true }

    let folder = { TeamsWithReadPermission = [ team ] }

    let document = { Creator = manager; Parent = folder }

    let canDeleteDocument =
        { Document = document
          CurrentUser = manager }

    let canNotDeleteDocument =
        { Document = document
          CurrentUser = notManager }

    let rules =
        [ Rules.canDeleteDocument
          Rules.canReadDocument
          Rules.canRenameDocument ]

    let contexts =
        [ canDeleteDocument
          canNotDeleteDocument ]

    for rule in rules do
        for context in contexts do
            let answer = isSatisfiedBy rule context
            prettyPrint rule
            |> printfn "Answer is %A from %A" answer 

    0
