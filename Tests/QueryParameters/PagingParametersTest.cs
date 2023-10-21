using kickerapi.QueryParameters;

namespace Tests.QueryParameters
{
    public class PagingParametersTest
    {

        [Fact]
        public void ItSetsMaxPageSizeWhenValueIsTooHigh()
        {
            PagingParameters pagingParameters = new PagingParameters();
            pagingParameters.PageSize = 1000;

            Assert.Equal(50, pagingParameters.PageSize);
        }
    }
}
