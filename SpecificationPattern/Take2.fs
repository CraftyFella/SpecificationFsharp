module SpecificationPattern.Take2



type Rule<'T> = 'T -> bool 

type Rules<'T> =
    | IsWorkspaceManagerRule of Rule<'T>
    | HasReadPermission of Rule<'T>
    | CreatedTheDocument of Rule<'T>
    | CanReadDocument
