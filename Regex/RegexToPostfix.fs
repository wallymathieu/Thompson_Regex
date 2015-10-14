module RegexToPostfix 

open System
open System.Text
open System.Collections.Generic
type re2post_s={
    nalt:int
    natom:int
}

let rec re2post'
    (paren: re2post_s list) 
    (s: re2post_s)
    = function
    | [] -> 
        if paren.Length <> 0 then
            failwith "unmatched!"
        else
            (List.replicate (s.natom-1) '.' ) @ (List.replicate s.nalt '|')
    | c:: t ->
        match c with
        | '('->
            if(s.natom > 1) then
                '.' :: (re2post' paren {s with natom=s.natom-1} t)
            else
                (re2post' (s:: paren) {natom=0; nalt=0} t)
        | '|'->
            if(s.natom = 0) then
                failwith "unmatched!";
            else
                let dots = List.replicate (s.natom-1) '.' 
                dots @ (re2post' paren {nalt=s.nalt+1; natom=0} t)
        | ')'->
            if(s.natom = 0) then
                failwith "unmatched!";
            else
                let ops = (List.replicate (s.natom-1) '.') @ (List.replicate s.nalt '|' )
                match paren with
                | p::paren_rest ->
                    ops @ (re2post' paren_rest {nalt=p.nalt; natom=p.natom+1} t)
                | _ -> failwith "unmatched!"
        | '*'
        | '+'
        | '?'->
            if(s.natom = 0) then
                failwith "unmatched!"
            else
                c::(re2post' paren s t)
        | c' ->
            let mutable natom = s.natom
            let op' =
                if(natom > 1) then
                    natom<-natom-1
                    ['.']
                else
                    []
            natom<-natom+1
            op' @ [c'] @ (re2post' paren {s with natom=natom} t)

let re2post(re:string)=
    let result = re2post' [] {natom=0;nalt=0} ( re.ToCharArray() |> Array.toList )
    new String(result |> List.toArray)
