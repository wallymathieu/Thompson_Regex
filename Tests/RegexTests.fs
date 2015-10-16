namespace Tests
open NUnit.Framework

[<TestFixture>]
type RegexTests() = 
    member self._true=Is.EqualTo(Some true)

    [<Test>]
    member self.A_or_b()=
        Assert.That(Regex.isMatch ("(a|b)", "a"), self._true)

    [<Test>]
    member self.A_followed_by_b()=
        Assert.That(Regex.isMatch ("ab", "ab"), self._true)

    [<Test>]
    member self.Just_A()=
        Assert.That(Regex.isMatch ("a", "a"), self._true)
