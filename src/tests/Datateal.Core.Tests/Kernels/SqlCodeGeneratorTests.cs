using Datateal.Core.Kernels;

namespace Datateal.Core.Tests.Kernels;

public class SqlCodeGeneratorTests
{
    [Fact]
    public void WrapSql_NormalSql_ContainsSqlInOutput()
    {
        var result = SqlCodeGenerator.WrapSql("SELECT 1");

        Assert.Contains("duckdb.execute(\"\"\"SELECT 1\"\"\")", result);
    }

    [Fact]
    public void WrapSql_TripleQuoteInSql_IsEscaped()
    {
        // Input SQL contains a triple-quote: SELECT '"""' AS col
        var sql = "SELECT '\"\"\"' AS col";

        var result = SqlCodeGenerator.WrapSql(sql);

        // The triple-quote must be escaped so it cannot terminate the Python string.
        Assert.Contains("SELECT '\\\"\\\"\\\"' AS col", result);
    }

    [Fact]
    public void WrapSql_DoubleQuoteInSql_IsNotEscaped()
    {
        var result = SqlCodeGenerator.WrapSql("SELECT \"\" AS col");

        Assert.Contains("SELECT \"\" AS col", result);
    }

    [Fact]
    public void WrapSql_SingleQuoteInSql_IsNotEscaped()
    {
        var result = SqlCodeGenerator.WrapSql("SELECT '\"' AS col");

        Assert.Contains("SELECT '\"' AS col", result);
    }

    [Fact]
    public void WrapSql_BackslashInSql_IsEscaped()
    {
        var result = SqlCodeGenerator.WrapSql(@"SELECT '\n' AS col");

        Assert.Contains(@"SELECT '\\n' AS col", result);
    }

    [Fact]
    public void WrapSql_BackslashBeforeTripleQuote_BothAreEscaped()
    {
        // Input: \""" (backslash then triple-quote)
        // Expected after escaping: \\\"\"\" (backslash escaped, then triple-quote escaped)
        var sql = "\\\"\"\"";

        var result = SqlCodeGenerator.WrapSql(sql);

        Assert.Contains("\\\\\\\"\\\"\\\"", result);
    }

    [Fact]
    public void WrapSql_AlwaysContainsPythonBoilerplate()
    {
        var result = SqlCodeGenerator.WrapSql("SELECT 1");

        Assert.Contains("import duckdb", result);
        Assert.Contains("__df = duckdb.execute(", result);
        Assert.Contains("__result", result);
    }
}
