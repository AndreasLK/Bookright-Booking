using Domain.Enums;
using Facade.Common.Dtos;
using System.Reflection;

namespace Tests.Domain
{
        public class EnumSyncTests
        {
                [Fact]
                public void GenderEnum_And_GenderDto_MustMatchExactly()
                {
                        // 1. Get all the string names from both Enums
                        var domainNames = Enum.GetNames(typeof(Gender)).OrderBy(n => n).ToArray();
                        var dtoNames = Enum.GetNames(typeof(GenderDto)).OrderBy(n => n).ToArray();

                        // 2. Get all the underlying integer values from both Enums
                        var domainValues = (int[])Enum.GetValues(typeof(Gender));
                        var dtoValues = (int[])Enum.GetValues(typeof(GenderDto));

                        // ASSERTION 1: Do they have the exact same number of items?
                        Assert.True(domainNames.Length == dtoNames.Length,
                            $"Count mismatch! Domain has {domainNames.Length}, DTO has {dtoNames.Length}");

                        // ASSERTION 2: Are the names identical?
                        for (int i = 0; i < domainNames.Length; i++)
                        {
                                Assert.Equal(domainNames[i], dtoNames[i]);
                        }

                        // ASSERTION 3: Do the underlying integer values match perfectly?
                        // (Crucial if you cast them by number later)
                        for (int i = 0; i < domainValues.Length; i++)
                        {
                                Assert.Equal(domainValues[i], dtoValues[i]);
                        }
                }
        }
}
