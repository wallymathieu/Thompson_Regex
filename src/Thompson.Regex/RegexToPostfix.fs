module RegexToPostfix

open System
open System.Text

type private re2post_s=struct
    val mutable nalt:int
    val mutable natom:int
end
/// Convert infix regexp re to postfix notation.
/// Insert . as explicit concatenation operator.
/// Cheesy parser, returns string.
let re2post(re:string)=
    let mutable nalt =0
    let mutable natom =0
    let buf = ResizeArray<char>(re.Length)
    let paren = ResizeArray<re2post_s>(100)

    let mutable p = 0
    let mutable nalt = 0
    let mutable natom = 0
    for c in re.ToCharArray () do
        match c with
        | '('->
            if(natom > 1) then
                natom <- natom-1
                buf.Add '.'
            else
                if paren.Count<=p then paren.Add (re2post_s())
                let mutable p' = paren.[p]
                p'.nalt <- nalt
                p'.natom <- natom
                paren.[p] <- p'
                p <- p+1
                nalt <- 0
                natom <- 0
        | '|'->
            if(natom = 0) then
                failwith "unmatched!"
            else
                buf.AddRange (List.replicate (natom-1) '.')
                natom <- 0
                nalt <- nalt+1
        | ')'->
            if(p = 0 || natom = 0) then
                failwith "unmatched!"
            else
                buf.AddRange (List.replicate (natom-1) '.')
                natom<-0
                buf.AddRange (List.replicate nalt '|')
                p <- p-1
                nalt <- paren.[p].nalt
                natom <- paren.[p].natom
                natom <- natom+1
        | '*'
        | '+'
        | '?'->
            if(natom = 0) then
                failwith "unmatched!"
            else
                buf.Add c
        | c' ->
            if(natom > 1) then
                natom<-natom-1
                buf.Add '.'
            buf.Add c'
            natom <- natom+1
        
    
    if p <> 0 then
        failwith "unmatched!"
    else
        buf.AddRange (List.replicate (natom-1) '.')
        buf.AddRange (List.replicate nalt '|')

    //*dst = 0;
        buf.ToArray() |> String