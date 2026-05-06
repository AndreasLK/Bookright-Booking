using Domain.Enums;
using Facade.Common.Dtos;
using Xunit;
using System.Linq;

namespace Tests.Domain
{
        /// <summary>
        /// Validates that the Domain and Facade layers maintain identical Gender definitions.
        /// This is a safeguard for the 'Clean Architecture' boundary between layers.
        /// </summary>
        public class EnumSyncTests
        {
                [Fact]
                public void GenderEnum_And_GenderDto_MustMatchExactly()
                {
                        // 1. Extract and sort names to ensure order-independent comparison
                        var domainNames = Enum.GetNames(enumType: typeof(Gender))
                            .OrderBy(keySelector: name => name)
                            .ToArray();

                        var dtoNames = Enum.GetNames(enumType: typeof(GenderDto))
                            .OrderBy(keySelector: name => name)
                            .ToArray();

                        // ASSERTION 1: Verify counts match
                        Assert.Equal(
                            expected: domainNames.Length,
                            actual: dtoNames.Length);

                        // 2. Pairwise comparison of names and underlying values
                        for (int index = 0; index < domainNames.Length; index++)
                        {
                                var currentName = domainNames[index];

                                // ASSERTION 2: Names must be identical
                                Assert.Equal(
                                    expected: currentName,
                                    actual: dtoNames[index]);

                                // 3. Resolve underlying integer values
                                var domainValue = (int)Enum.Parse(enumType: typeof(Gender), value: currentName);
                                var dtoValue = (int)Enum.Parse(enumType: typeof(GenderDto), value: currentName);

                                // ASSERTION 3: Integer values must match to ensure safe casting
                                Assert.Equal(
                                    expected: domainValue,
                                    actual: dtoValue);
                        }
                }
        }
}
