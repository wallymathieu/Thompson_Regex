module Tests


open Expecto
open Thompson
let assertRegexMatch regex string =
  Expect.equal (Regex.isMatch (regex, string)) true <| sprintf "expected %s to match %s" regex string
let assertNotARegexMatch regex string =
  Expect.equal (Regex.isMatch (regex, string)) false <| sprintf "expected %s to not match %s" regex string
let testCaseRegexMatch regex string = testCase (sprintf "regex match %s %s" regex string) <| fun _ -> assertRegexMatch regex string
let testCaseRegexNotAMatch regex string = testCase (sprintf "regex not a match %s %s" regex string) <| fun _ -> assertNotARegexMatch regex string
[<Tests>]
let tests =
  testList "RegexTests" [

    testCase "A_or_b" <| fun _ ->
      assertRegexMatch "(a|b)" "a"

    testCase "A_followed_by_b" <| fun _ ->
      assertRegexMatch "ab" "ab"

    testCase "Just_A" <| fun _ ->
      assertRegexMatch "a" "a"

    testCase "A_b_zero_or_more" <| fun _ ->
      assertRegexMatch "ab*" "ab"
      assertRegexMatch "ab*" "abbb"
      assertNotARegexMatch "ab*" "ac"
      assertNotARegexMatch "ab*" "aab"

    //from bytecode tests in the c code
    testCaseRegexMatch      "abcdefg"  "abcdefg"
    testCaseRegexNotAMatch  "(a|b)*a"  "ababababab"
    testCaseRegexMatch      "(a|b)*a"  "aaaaaaaaba"
    testCaseRegexNotAMatch  "(a|b)*a"  "aaaaaabac"
    testCaseRegexMatch      "a(b|c)*d" "abccbcccd"
    testCaseRegexNotAMatch  "a(b|c)*d" "abccbcccde"
  ]
[<Tests>]
let tests2 =
  testList "RegexToPostfixTests" [
    testCase "Test" <| fun _ ->
      Expect.equal (RegexToPostfix.re2post ("a|b|c")) "abc||" "should be abc||"
      Expect.equal (RegexToPostfix.re2post ("ab|c")) "ab.c|" "should be ab.c|"
    testCase "Test_2" <| fun _ ->
      Expect.equal (RegexToPostfix.re2post ("(a|b)")) "ab|" "should be ab|"
  ]
