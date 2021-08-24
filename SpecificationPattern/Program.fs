open System

type User = {
    Name: string
    IsManager: bool
}

type Team = {
    ContainsUser : User -> bool
    Users: User list
}

type Folder = {
    TeamsWithReadPermission: Team list
}

type Document = {
    Creator: User
    Parent : Folder
}

type Context = {
    Document : Document
    CurrentUser: User
}

type Specification<'T> =
    | Or of Specification<'T> * Specification<'T>
    | And of Specification<'T> * Specification<'T>
    | Not of Specification<'T>
    | Spec of (string * ('T -> bool))

module Specification =
    let rec isSatisfiedBy spec candidate =
        match spec with
        | And (left, right) -> (isSatisfiedBy left candidate) && (isSatisfiedBy right candidate)
        | Or (left, right) -> (isSatisfiedBy left candidate) || (isSatisfiedBy right candidate)
        | Not spec -> not (isSatisfiedBy spec candidate)
        | Spec (_, spec) -> spec candidate
                
    let rec prettyPrint spec candidate =
        match spec with
        | And (left, right) -> sprintf "%A AND %A" (prettyPrint left candidate) (prettyPrint right candidate)
        | Or (left, right) -> sprintf "%A OR %A" (prettyPrint left candidate) (prettyPrint right candidate)
        | Not spec -> sprintf "NOT %A" (prettyPrint spec candidate)
        | Spec spec -> sprintf "%s" (spec |> fst)

[<RequireQualifiedAccess>]
module Rules =
    let isWorkspaceManager (context: Context) =
        context.CurrentUser.IsManager

    let hasReadPermission (context: Context) =
        let folder = context.Document.Parent
        folder.TeamsWithReadPermission
            |> Seq.exists(fun team -> team.ContainsUser(context.CurrentUser) )

    let createdTheDocument (context: Context) =
        context.Document.Creator = context.CurrentUser
        
    let readDocument : Specification<Context> =
        Or (Spec ("isWorkspaceManager", isWorkspaceManager), Spec ("hasReadPermission", hasReadPermission))

    let deleteDocument : Specification<Context> =
        Or (Spec ("isWorkspaceManager", isWorkspaceManager), Spec  ("createdTheDocument", createdTheDocument))

    let renameDocument : Specification<Context> =
        And (readDocument, deleteDocument)
        
[<EntryPoint>]
let main argv =
    let manager = { Name = "Boss person"; IsManager = true }
    let notManager = { Name = "worker"; IsManager = false }
    let users = [ manager; notManager ]
    let team : Team = { Users = users; ContainsUser = fun _ -> true; }
    let folder = { TeamsWithReadPermission = [ team ]  }
    
    let document = { Creator = manager; Parent = folder }
    
    let canDeleteDocument = { Document = document; CurrentUser = manager }
    
    let answer = Specification.isSatisfiedBy Rules.readDocument canDeleteDocument
    Specification.prettyPrint Rules.readDocument canDeleteDocument |> printfn "Answer is %A From %A" answer
    
    
    0