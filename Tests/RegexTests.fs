namespace Tests
open NUnit.Framework

[<TestFixture>]
type RegexTests() = 
    [<Test>]
    member self.A_or_b()=
        Assert.That(Regex.isMatch ("(a|b)", "a"), Is.True)

    [<Test>]
    member self.A_followed_by_b()=
        Assert.That(Regex.isMatch ("ab", "ab"), Is.True)

    [<Test>]
    member self.Just_A()=
        Assert.That(Regex.isMatch ("a", "a"), Is.True)
