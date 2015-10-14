module RegexToPostfix 

open System
open System.Text
open System.Collections.Generic
[<Struct>]
type re2post_s=
    struct
        val mutable nalt:int
        val mutable natom:int
    end


let re2post(re:string)=
    let mutable nalt =0
    let mutable natom =0
    let buf=new StringBuilder();
    let paren = new List<re2post_s>(100)//, *p;

    let mutable p = 0
    let mutable nalt = 0
    let mutable natom = 0
    for c in re.ToCharArray () do
        match c with
        | '('->
            if(natom > 1) then
                natom <- natom-1
                buf.Append('.')|> ignore
            else
                if paren.Count<=p then
                    paren.Add(new re2post_s())
                else
                    ()
                let mutable p' =paren.[p]
                p'.nalt <- nalt
                p'.natom <- natom
                paren.[p] <- p'
                p<-p+1;
                nalt <- 0;
                natom <- 0;
        | '|'->
            if(natom = 0) then
                failwith "!";
            else
                buf.Append( List.replicate (natom-1) '.' |> List.toArray ) |> ignore
                natom<-0
                nalt<-nalt+1;
        | ')'->
            if(p = 0 || natom = 0) then
                failwith "!";
            else
                buf.Append( List.replicate (natom-1) '.'  |> List.toArray) |> ignore
                natom<-0
                buf.Append( List.replicate nalt '|'  |> List.toArray) |> ignore
                p<-p-1
                nalt <- paren.[p].nalt
                natom <- paren.[p].natom
                natom<-natom+1;
        | '*'
        | '+'
        | '?'->
            if(natom = 0) then
                failwith "!"
            else
                buf.Append(c)|> ignore
        | c' ->
            if(natom > 1) then
                natom<-natom-1
                buf.Append( '.') |> ignore
            else
                ()
            buf.Append( c') |> ignore
            natom<-natom+1
        
    
    if p <> 0 then
        None
    else
        buf.Append( List.replicate (natom-1) '.'  |> List.toArray) |> ignore
        buf.Append( List.replicate nalt '|' |> List.toArray) |> ignore

    //*dst = 0;
        Some(buf.ToString())
