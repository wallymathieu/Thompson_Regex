namespace Tests
open System
open NUnit.Framework

[<TestFixture>]
type RegexToPostfixTests()=
    [<Test>]
    member self.Test()=
        Assert.AreEqual ("abc||", RegexToPostfix.re2post ("a|b|c"))
        Assert.AreEqual ("ab.c|", RegexToPostfix.re2post ("ab|c"))

    [<Test>]
    member self.Test_2()=
        Assert.AreEqual ("ab|", RegexToPostfix.re2post ("(a|b)"))
