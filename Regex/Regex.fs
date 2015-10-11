(*
 * Regular expression implementation.
 * Supports only ( | ) * + ?.  No escapes.
 * Compiles to NFA and then simulates NFA
 * using Thompson's algorithm.
 *
 * See also http://swtch.com/~rsc/regexp/ and
 * Thompson, Ken.  Regular Expression Search Algorithm,
 * Communications of the ACM 11(6) (June 1968), pp. 419-422.
 * 
 * Copyright (c) 2007 Russ Cox.
 * Can be distributed under the MIT license, see bottom of file.
 *)
module Regex
  
(*
 * Represents an NFA state plus zero or one or two arrows exiting.
 * if c == Match, no arrows out; matching state.
 * If c == Split, unlabeled arrows to out and out1 (if != NULL).
 * If c < 256, labeled arrow with character c to out.
 *)
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

        static member toInt state=
            let unicode = new System.Text.UnicodeEncoding()

            match state with
            | Char c -> unicode.GetBytes([|c|]).[0] |> int
            | Match -> 256
            | Split -> 257
           
//[<Struct>]
type State={
    c:NFAState
    out:State option
    out1:State option
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

(*State matchstate = { Match };   /* matching state */
*)

(* Allocate and initialize State *)
let state (g:RegexState) (c:NFAState,out:State option,out1:State option) : State=
    g.nstate_plus_plus()
    let s = {lastList=0;c=c;out=out;out1=out1}
    s

type Ptrlist= State option list 

(*
 * A partially built NFA without the matching state filled in.
 * Frag.start points at the start state.
 * Frag.out is a list of places that need to be set to the
 * next state for this fragment.
 *)
(*
type Frag=
    {
        start:State;
        out:Ptrlist
    }
*)
type Frag(start:State, out:Ptrlist)=
    member val start=start
    member val out=out with get,set

(* Initialize Frag struct. *)
let frag(start:State, out:Ptrlist)=
    let n=new Frag(start=start, out=out)
    n

(* Create singleton list containing just outp. *)
let list1 (outp:State option) : Ptrlist= 
    [outp]
(* Patch the list of states at out to point to start. *)
let patch (list : Ptrlist, s: State) : Ptrlist =
    let l = list |> List.length
    [ for i in 1 .. l ->Some s ]

(* Join the two lists l1 and l2, returning the combination. *)
let append(l1:Ptrlist, l2:Ptrlist) :Ptrlist=
    l1 @ l2

(*
 * Convert postfix regular expression to NFA.
 * Return start state.
 *)
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
            e1.out <- patch(e1.out, e2.start)
            push(frag(e1.start, e2.out))
        | '|' -> //alternate
            let e2 = pop()
            let e1 = pop()
            let s = state rs (Split, Some(e1.start), Some(e2.start))
            push(frag(s, append(e1.out, e2.out)))
        | '?' -> //zero or one 
            let e =pop()
            let s = state rs (Split, Some(e.start), None)
            push(frag(s, append(e.out, list1(s.out1))))
        | '*' ->
            let e = pop()
            let s = state rs (Split, Some(e.start), None)
            e.out <- patch(e.out, s)
            push(frag(s, list1(s.out1)))
        | '+' -> //one or more
            let e = pop()
            let s = state rs (Split, Some(e.start), None)
            e.out <- patch(e.out, s)
            push(frag(e.start, list1(s.out1)))
        | c -> //default
            let s = state rs (Char c, None, None)
            push(frag(s, list1(s.out)))

    let e = pop()
    if (stack.Count>0) then
        None
    else
        e.out <- patch(e.out, rs.matchstate)
        Some(e.start)

type List=System.Collections.Generic.List<State>
type ListState(l1:List,l2:List,listid:int)=
    member val l1=l1
    member val l2=l2
    member val listid=listid with get, set
    member self.listid_plus_plus()=
        self.listid <- self.listid+1
    new ()=
        new ListState(new List(), new List(), 0)

(* Add s to l, following unlabeled arrows. *)
let rec addstate (ls:ListState) (l:List,s':State option)=
    // (s == NULL || s->lastlist == listid)
    if Option.isNone s' || ls.listid = s'.Value.lastList  then 
        ()
    else
        let s = s'.Value// TODO:Fix
        // s->lastlist = listid;
        s.lastList <- ls.listid

        if (NFAState.isSplit s.c) then
            // follow unlabeled arrows
            addstate ls (l, s.out)
            addstate ls (l, s.out1)
            // return;
        else 
            // l->s[l->n++] = s;
            l.Add(s)
        ()

(* Compute initial state list *)
let startlist (ls:ListState) (start:State,l:List)=
    l.Clear()
    ls.listid_plus_plus()
    addstate ls (l, Some(start))
    l


(* Check whether state list contains a match. *)
let ismatch (rs:RegexState) (l:List)=
    l.Contains( rs.matchstate ) 

(*
 * Step the NFA from the states in clist
 * past the character c,
 * to create next NFA state set nlist.
 *)
let step (ls:ListState) (clist:List, c:char, nlist:List)=
    ls.listid_plus_plus()
    nlist.Clear()
    for s in clist do
        if s.c.matches c then
            addstate ls (nlist, s.out)

(* Run NFA to determine whether it matches s. *)
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

(*
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the
 * Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute,
 * sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall
 * be included in all copies or substantial portions of the
 * Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY
 * KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE AUTHORS
 * OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *)
