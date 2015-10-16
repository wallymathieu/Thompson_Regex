﻿module Regex
  
open NFA
open RegexToPostfix

type MList<'t> when 't : equality=
    System.Collections.Generic.List<State<'t>>

type ListState(listid:int) =
    member val listid=listid with get, set
    member val lastList = new System.Collections.Generic.Dictionary<int,int>()
    member self.listid_plus_plus()=
        self.listid <- self.listid+1
    new ()=
        new ListState(0)
    static member getLastList v (ls:ListState) =
        if ls.lastList.ContainsKey(v) then
            ()
        else
            ls.lastList.Add(v,0)
        ls.lastList.[v]

    static member isListid v (ls:ListState) =
        ls.listid = (ListState.getLastList v ls) 

    static member setListid v (ls:ListState) =
        ls.lastList.[v] <- ls.listid

/// Run NFA to determine whether it matches s.
let matches (start:State<'t>,s:'t seq):bool=
    /// Check whether state list contains a match.
    let ismatch (l:MList<'t>)=
        l.FindIndex( fun s-> NFAState<'t>.isMatch s.c ) >= 0 

    /// Add s to l, following unlabeled arrows.
    let rec addstate (ls:ListState) (l:MList<'t>) (s:State<'t>)=
        // (s == NULL || s->lastlist == listid)
        if (ListState.isListid s.id ls)  then 
            ()
        else
            // s->lastlist = listid;
            ListState.setListid s.id ls

            if (NFAState<'t>.isSplit s.c) then
                // follow unlabeled arrows
                let addstate = Option.iter(addstate ls l) 
                addstate s.out
                addstate s.out1
                // return;
            else
                // l->s[l->n++] = s;
                l.Add(s)
            ()

    /// Step the NFA from the states in clist
    /// past the character c,
    /// to create next NFA state set nlist.
    let step (ls:ListState) (clist:MList<'t>, c:'t, nlist:MList<'t>)=
        ls.listid_plus_plus()
        nlist.Clear()
        for s in clist do
            if s.c.matches c then
                let addstate = Option.iter(addstate ls nlist) 
                addstate s.out

    /// Compute initial state list
    let startlist (ls:ListState) (start:State<'t>,l:MList<'t>)=
        l.Clear()
        ls.listid_plus_plus()
        addstate ls l start
        l

    let ls = ListState()
    let l1 = new MList<_>()
    let l2 = new MList<_>()
    let mutable clist = startlist ls (start, l1)
    let mutable nlist = l2
    for c in s do
        step ls (clist, c, nlist)
        // swap clist, nlist :
        let t = clist
        clist <- nlist
        nlist <- t
    // 

    ismatch clist

let isMatch (regex, value)=
    let post2format (p:string)=
        let to_m c=
            match OperationType.fromChar c with
            | None -> Value c 
            | Some opt-> Match.Operation(opt)

        p.ToCharArray()
            |> Array.map to_m 
            |> Array.toList
    
    //
    re2post(regex)
        |> post2format
        |> NFA.post2nfa
        |> Option.map (fun start-> matches (start, value))
    