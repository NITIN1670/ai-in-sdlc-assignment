
using System;
using System.Linq;
using InvoiceApp.Domain;
using Xunit;

namespace InvoiceApp.Tests;

public class DomainArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOn_ApiLayer()
    {
        var references = typeof(Invoice).Assembly.GetReferencedAssemblies();

        Assert.DoesNotContain(
            references,
            reference => reference.Name == "InvoiceApp.Api");
    }

    [Fact]
    public void Domain_ShouldNotDependOn_AspNetCore()
    {
        var references = typeof(Invoice).Assembly.GetReferencedAssemblies();

        Assert.DoesNotContain(
            references,
            reference => reference.Name?.StartsWith(
                "Microsoft.AspNetCore",
                StringComparison.Ordinal) == true);
    }
}
