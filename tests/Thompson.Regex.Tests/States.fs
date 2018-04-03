module States
open System
open System.Text

    let rec printnfa (start:NFA.State<char>, depth:int)=
        if depth>100 then
            failwith "!!"
        else
            let sb = new StringBuilder()
            let printSpaces num=
                String.init num (fun _ -> " ")
            let append (v:string)=
                sb.Append(v) |> ignore

            append(printSpaces(depth));
            append(String.Format("(c={0}, id={1} \n", start.c, start.id));
            start.out |> Option.iter (fun out->
                append(printSpaces(depth+1))
                append(",out=\n")
                append(printnfa(out, depth+2))
            )
            start.out1 |> Option.iter (fun out->
                append(printSpaces(depth+1))
                append(",out1:\n")
                append(printnfa(out, depth+2))
            )
            append(printSpaces(depth))
            append(")\n")
            sb.ToString()
