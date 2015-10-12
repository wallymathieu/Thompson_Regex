module Regex
  
open NFA

type List=System.Collections.Generic.List<State>
type ListState(l1:List,l2:List,listid:int)=
    member val l1=l1
    member val l2=l2
    member val listid=listid with get, set
    member self.listid_plus_plus()=
        self.listid <- self.listid+1
    new ()=
        new ListState(new List(), new List(), 0)

/// Add s to l, following unlabeled arrows.
let rec addstate (ls:ListState) (l:List) (s:State)=
    // (s == NULL || s->lastlist == listid)
    if ls.listid = s.lastList  then 
        ()
    else
        // s->lastlist = listid;
        s.lastList <- ls.listid

        if (NFAState.isSplit s.c) then
            // follow unlabeled arrows
            let addstate = Option.iter(addstate ls l) 
            addstate s.out
            addstate s.out1
            // return;
        else 
            // l->s[l->n++] = s;
            l.Add(s)
        ()

/// Compute initial state list
let startlist (ls:ListState) (start:State,l:List)=
    l.Clear()
    ls.listid_plus_plus()
    addstate ls l start
    l


/// Check whether state list contains a match.
let ismatch (rs:RegexState) (l:List)=
    l.FindIndex( fun s-> NFAState.isMatch s.c ) >= 0 

/// Step the NFA from the states in clist
/// past the character c,
/// to create next NFA state set nlist.
let step (ls:ListState) (clist:List, c:char, nlist:List)=
    ls.listid_plus_plus()
    nlist.Clear()
    for s in clist do
        if s.c.matches c then
            let addstate = Option.iter(addstate ls nlist) 
            addstate s.out


/// Run NFA to determine whether it matches s.
let matches rs ls (start:State,s:string):bool=
    let mutable clist = startlist ls (start, ls.l1)
    let mutable nlist = ls.l2
    for c in s.ToCharArray() do
        step ls (clist, c, nlist)
        // swap clist, nlist :
        let t = clist
        clist <- nlist
        nlist <- t

    ismatch rs clist
