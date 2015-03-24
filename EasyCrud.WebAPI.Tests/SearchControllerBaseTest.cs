using EasyCrud.Factories;
using EasyCrud.Logging;
using EasyCrud.Models;
using EasyCrud.Tests;
using EasyCrud.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCrud.WebAPI.Tests
{
    public abstract class SearchControllerBaseTest<TSearchQueryType, TResponseType> : TestBase
        where TSearchQueryType : class, ISearchQuery, new()
    {
        protected SearchController<TSearchQueryType, TResponseType> Controller { get; set; }

        protected abstract SearchController<TSearchQueryType, TResponseType> CreateController(IRepositoryFactory factory, ILogger logger);

        protected abstract Task<bool> BuildDataSet(int configurationIndex);
        protected abstract Task<bool> ClearDataSet();
        protected abstract Task<TSearchQueryType> BuildQuery(int configurationIndex);
        protected abstract Task<IEnumerable<TResponseType>> BuildExpectedResult(int configurationIndex);
        protected abstract void AssertResponsesMatch(TResponseType input, TResponseType reference);

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            Controller = CreateController(RepositoriesFactory, LoggerMock);
        }

        [TestCleanup]
        public override void CleanUp()
        {
        }

        public async Task PerformSingleTestRun(int dataSetConfigurationIndex, int queryConfigurationIndex, int resultConfigurationIndex)
        {
            await BuildDataSet(dataSetConfigurationIndex);
            var query = await BuildQuery(queryConfigurationIndex);
            var referenceResponse = await BuildExpectedResult(resultConfigurationIndex);

            var searchResult = await Controller.Get(query);

            var skippedReference = query.NumElementsToSkip();
            searchResult.Skipped.Should().Be(query.NumElementsToSkip());

            var resultsArr = searchResult.Results.ToArray();
            var referenceArr = referenceResponse.ToArray();

            resultsArr.Length.Should().Be(referenceArr.Length);
            for (int i = 0; i < resultsArr.Length; i++)
            {
                var element = resultsArr[i];
                var reference = referenceArr[i];
                AssertResponsesMatch(element, reference);
            }

            await ClearDataSet();
        }
    }
}
