module NFA

/// Represents an NFA state plus zero or one or two arrows exiting.
/// if c == Match, no arrows out; matching state.
/// If c == Split, unlabeled arrows to out and out1 (if != NULL).
/// If c < 256, labeled arrow with character c to out.
type NFAState=
    | Char of char
    | Match //=256
    | Split //=257
    with
        member self.matches c=
            match self with
            |Char c'-> c'=c
            |_ -> false
        override self.ToString()=
            match self with
            | Char c-> sprintf "'%c'" c
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


//[<Struct>]
type State={
    c:NFAState
    mutable out:State option
    mutable out1:State option
    /// it would be nice if this was 
    mutable lastList:int
}
type RegexState(matchstate:State,nstate:int)=
    member val matchstate=matchstate
    member val nstate=nstate with get,set
    member self.nstate_plus_plus()=
        self.nstate <- self.nstate+1
        //new RegexState(matchstate, nstate+1)
    new () =
        new RegexState({c=Match;out=None;out1=None;lastList=0} (* matching state *), 0)

/// Allocate and initialize State
let state (g:RegexState) (c:NFAState,out:State option,out1:State option) : State=
    g.nstate_plus_plus()
    let s = { lastList=0; c=c; out=out; out1=out1 }
    s

type RewriteState=
    | State_out1 of State
    | State_out of State
/// A partially built NFA without the matching state filled in.
/// Frag.start points at the start state.
/// Frag.out is a list of places that need to be set to the
/// next state for this fragment.
type Frag(start:State, out:RewriteState list)=
    member val start=start
    member val out=out

/// Initialize Frag struct. 
let frag(start:State, out:RewriteState list)=
    let n=new Frag(start=start, out=out)
    n

/// Create singleton list containing just outp. 
let list1 (outp:RewriteState) : RewriteState list= 
    [outp]
/// Patch the list of states at out to point to start.
let patch (list : RewriteState list, s: State) =
    list |> List.iter (fun next->
        match next with
        | State_out s'-> s'.out <- Some s
        | State_out1 s'-> s'.out1 <- Some s
    )
    ()

/// Join the two lists l1 and l2, returning the combination. 
let append(l1,l2)= l1 @ l2

/// Convert postfix regular expression to NFA.
///  Return start state.
let post2nfa (rs:RegexState) (postfix:string):State option=
    let mutable p:string=""
    let stack = new System.Collections.Generic.Stack<Frag>()
    let pop()=
        stack.Pop()
    let push v=
        stack.Push v
    for p in postfix.ToCharArray() do
        match p with
        | '.' -> //catenate 
            let e2 = pop()
            let e1 = pop()
            patch(e1.out, e2.start)
            push(frag(e1.start, e2.out))
        | '|' -> //alternate
            let e2 = pop()
            let e1 = pop()
            let s = state rs (Split, Some(e1.start), Some(e2.start))
            push(frag(s, append(e1.out, e2.out)))
        | '?' -> //zero or one 
            let e =pop()
            let s = state rs (Split, Some(e.start), None)
            push(frag(s, append(e.out, list1(State_out1(s)))))
        | '*' ->
            let e = pop()
            let s = state rs (Split, Some(e.start), None)
            patch(e.out, s)
            push(frag(s, list1(State_out1(s))))
        | '+' -> //one or more
            let e = pop()
            let s = state rs (Split, Some(e.start), None)
            patch(e.out, s)
            push(frag(e.start, list1(State_out1(s))))
        | c -> //default
            let s = state rs (Char c, None, None)
            push(frag(s, list1(State_out(s))))

    let e = pop()
    if (stack.Count>0) then
        None
    else
        patch(e.out, rs.matchstate)
        Some(e.start)
