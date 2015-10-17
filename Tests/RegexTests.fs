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

    [<Test>]
    member self.A_b_zero_or_more()=
        Assert.That(Regex.isMatch ("ab*", "ab"), Is.True)
        Assert.That(Regex.isMatch ("ab*", "abbb"), Is.True)
        Assert.That(Regex.isMatch ("ab*", "ac"), Is.False)
        Assert.That(Regex.isMatch ("ab*", "aab"), Is.False)
