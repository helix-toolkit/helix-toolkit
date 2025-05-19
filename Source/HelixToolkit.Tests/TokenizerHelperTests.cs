using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HelixToolkit.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class TokenizerHelperTests
{
    [Test]
    public void GetNumericListSeparator()
    {
        char separator = TokenizerHelper.GetNumericListSeparator(CultureInfo.InvariantCulture);
        Assert.That(separator, Is.EqualTo(','));
    }

    [Test]
    public void GetNumericListSeparatorFrench()
    {
        char separator = TokenizerHelper.GetNumericListSeparator(CultureInfo.GetCultureInfo("fr-FR"));
        Assert.That(separator, Is.EqualTo(';'));
    }

    [Test, Sequential]
    public void FoundSeparator([Values("a", "a b", "a,b")] string str, [Values(false, false, true)] bool hasSeparator)
    {
        var tokenizer = new TokenizerHelper(str, CultureInfo.InvariantCulture);
        tokenizer.NextToken(false, ',');

        bool result = tokenizer.FoundSeparator;
        Assert.That(result, Is.EqualTo(hasSeparator));
    }

    [Test]
    public void GetCurrentToken()
    {
        var tokenizer = new TokenizerHelper("", CultureInfo.InvariantCulture);
        var token = tokenizer.GetCurrentToken().ToString();
        Assert.That(token, Is.EqualTo(""));
    }

    [Test]
    public void NextToken()
    {
        var tokenizer = new TokenizerHelper("a", CultureInfo.InvariantCulture);
        tokenizer.NextToken();
        var token = tokenizer.GetCurrentToken().ToString();
        Assert.That(token, Is.EqualTo("a"));
    }

    [Test]
    public void NextTokenRequiredThrows()
    {
        var tokenizer = new TokenizerHelper("a", CultureInfo.InvariantCulture);
        tokenizer.NextTokenRequired();
        Assert.Throws<InvalidOperationException>(() => tokenizer.NextTokenRequired());
    }

    [Test, Sequential]
    public void NextTokenRequired([Values("a b c", "a,b,c", "a;b;c")] string str, [Values(' ', ',', ';')] char separator)
    {
        var tokenizer = new TokenizerHelper(str, '\'', separator);
        string a = tokenizer.NextTokenRequired().ToString();
        string b = tokenizer.NextTokenRequired().ToString();
        string c = tokenizer.NextTokenRequired().ToString();

    }

    [Test]
    public void LastTokenRequiredThrows()
    {
        var tokenizer = new TokenizerHelper("a b", CultureInfo.InvariantCulture);
        tokenizer.NextTokenRequired();
        Assert.Throws<InvalidOperationException>(tokenizer.LastTokenRequired);
    }

    [Test]
    public void LastTokenRequired()
    {
        var tokenizer = new TokenizerHelper("a b", CultureInfo.InvariantCulture);
        tokenizer.NextTokenRequired();
        tokenizer.NextTokenRequired();
        tokenizer.LastTokenRequired();
    }
}
