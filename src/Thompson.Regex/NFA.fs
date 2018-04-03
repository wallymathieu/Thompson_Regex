module NFA

/// Represents an NFA state plus zero or one or two arrows exiting.
/// if c == Match, no arrows out; matching state.
/// If c == Split, unlabeled arrows to out and out1 (if != NULL).
/// If c < 256, labeled arrow with character c to out.
type NFAState<'t> when 't :equality=
    | Value of 't
    | Match //=256
    | Split //=257
    with
        member self.matches c=
            match self with
            |Value c'-> c'=c
            |_ -> false
        override self.ToString()=
            match self with
            | Value c-> sprintf "'%O'" c
            | Match -> "Match"
            | Split -> "Split"

        static member isSplit state=
            match state with
            | Split -> true
            | _ -> false
        static member isMatch state=
            match state with
            | Match -> true
            | _ -> false

type State<'t> when 't :equality ={
    c:NFAState<'t>
    mutable out:State<'t> option
    mutable out1:State<'t> option
    id:int
}

type internal RewriteState<'t> when 't : equality=
    | State_out1 of State<'t>
    | State_out of State<'t>

/// A partially built NFA without the matching state filled in.
/// Frag.start points at the start state.
/// Frag.out is a list of places that need to be set to the
/// next state for this fragment.
type internal Frag<'t when 't : equality>(start:State<'t>, out:RewriteState<'t> list)=
    member val start=start
    member val out=out

type OperationType=
    | Catenate
    | Alternate
    | ZeroOrOne
    | ZeroOrMore
    | OneOrMore
    with
        static member fromChar c=
            match c with
            | '.' -> Some Catenate
            | '|' -> Some Alternate
            | '?' -> Some ZeroOrOne
            | '*' -> Some ZeroOrMore
            | '+' -> Some OneOrMore
            | _ -> None

type Match<'t>=
    | Operation of OperationType
    | Value of 't

/// Convert postfix regular expression to NFA.
///  Return start state.
let post2nfa (postfix:Match<'t> list):State<'t> option when 't : equality=

    /// get ids for states
    let IdGenerator()=
        let id= ref 1
        ( fun unit->
            id := !id+1
            id
        )

    /// Allocate and initialize State
    let state c id out out1 : State<_>=
        { c=c; id=id; out=out; out1=out1 }

    /// Patch the list of states at out to point to start.
    let patch (list, s) =
        list |> List.iter (fun next->
            match next with
            | State_out s'-> s'.out <- Some s
            | State_out1 s'-> s'.out1 <- Some s
        )

    /// Initialize Frag struct.
    let frag(start, out)=
        new Frag<_>(start=start, out=out)

    let idgen = IdGenerator()
    let stack = new System.Collections.Generic.Stack<Frag<'t>>()
    let pop()=
        stack.Pop()
    let push v=
        stack.Push v
    for p in postfix do
        match p with
        | Operation Catenate -> //catenate
            let e2 = pop()
            let e1 = pop()
            patch(e1.out, e2.start)
            push(frag(e1.start, e2.out))
        | Operation Alternate -> //alternate
            let e2 = pop()
            let e1 = pop()
            let s = state Split (!idgen()) (Some(e1.start)) (Some(e2.start))
            push(frag(s, e1.out @ e2.out))
        | Operation ZeroOrOne -> //zero or one
            let e =pop()
            let s = state Split (!idgen()) (Some(e.start)) None
            push(frag(s, e.out @ [State_out1(s)]))
        | Operation ZeroOrMore ->
            let e = pop()
            let s = state Split (!idgen()) (Some(e.start)) None
            patch(e.out, s)
            push(frag(s, [State_out1(s)]))
        | Operation OneOrMore -> //one or more
            let e = pop()
            let s = state Split (!idgen()) (Some(e.start)) None
            patch(e.out, s)
            push( frag(e.start, [State_out1(s)]) )
        | Value c -> //default
            let s = state (NFAState.Value c) (!idgen()) None None
            push( frag(s, [State_out(s)]) )

    let e = pop()
    if (stack.Count>0) then
        None
    else
        let matchstate = {c=Match;out=None;out1=None;id=0}
        patch(e.out, matchstate)

        Some(e.start)
