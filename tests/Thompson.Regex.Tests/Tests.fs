module Tests


open Expecto
open Thompson
let assertTrue a = Expect.equal a true "expected true"
let assertFalse a = Expect.equal a false "expected false"
[<Tests>]
let tests =
  testList "RegexTests" [

    testCase "A_or_b" <| fun _ ->
      assertTrue <| Regex.isMatch ("(a|b)", "a")

    testCase "A_followed_by_b" <| fun _ ->
      assertTrue <| Regex.isMatch ("ab", "ab")

    testCase "Just_A" <| fun _ ->
      assertTrue <| Regex.isMatch ("a", "a")

    testCase "A_b_zero_or_more" <| fun _ ->
      assertTrue <| Regex.isMatch ("ab*", "ab")
      assertTrue <| Regex.isMatch ("ab*", "abbb")
      assertFalse <| Regex.isMatch ("ab*", "ac")
      assertFalse <| Regex.isMatch ("ab*", "aab")
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
