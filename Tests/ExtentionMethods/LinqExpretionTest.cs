using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.ExtentionMethods
{
    public class LinqExpretionTest
    {
        [Theory]
        [InlineData("test")]
        [InlineData(1)]
        [InlineData(true)]
        public void WhereIfValueIsPresentListIsFiltered(object value)
        {
            var list = new List<string>() { "a", "b", "c", "d" };

            var result = list.AsQueryable().WhereIf(x => x == value, value).ToList();

            Assert.NotEqual(list.Count, result.Count);
        }

        [Fact]
        public void WhereIfTrueListIsFiltered()
        {
            var list = new List<string>() { "a", "b", "c", "d" };

            var result = list.AsQueryable().WhereIf(x => x == "a", true).ToList();

            Assert.NotEqual(list.Count, result.Count);
        }

        [Fact]
        public void WhereIfValueIsEmptyListIsNotFiltered()
        {
            var list = new List<string>() { "a", "b", "c", "d" };

            var result = list.AsQueryable().WhereIf(x => x == "a", null).ToList();

            Assert.Equal(list.Count, result.Count);
        }

        [Fact]
        public void WhereIfValueIsFalseListIsNotFiltered()
        {
            var list = new List<string>() { "a", "b", "c", "d" };

            var result = list.AsQueryable().WhereIf(x => x == "a", false).ToList();

            Assert.Equal(list.Count, result.Count);
        }
    }
}
