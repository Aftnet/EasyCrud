using EasyCrud.Models;
using EasyCrud.Repositories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EasyCrud.Tests
{
    [TestClass]
    public abstract class RepositoryBaseTest<TRecordType, TIndexType> : TestBase
        where TRecordType : class , IModel<TIndexType>
    {
        protected List<TRecordType> TestElementsInDb;
        protected TRecordType TestElementNotInDb;

        public abstract TRecordType CreateTestElement(int seed = 0);
        public abstract void UpdateTestElement(TRecordType input);

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private IRepository<TRecordType, TIndexType> _dataAccess;
        protected IRepository<TRecordType, TIndexType> DataAccess
        {
            get
            {
                if(_dataAccess == null)
                {
                    _dataAccess = RepositoriesFactory.GetRepository<TRecordType, TIndexType>();
                }
                return _dataAccess;
            }
        }

        protected IRepository<TOtherRecordType, TOtherIndexType> GetContextSharingRepository<TOtherRecordType, TOtherIndexType>() where TOtherRecordType : class , IModel<TOtherIndexType>
        {
            return RepositoriesFactory.GetContextSharingRepository<TOtherRecordType, TOtherIndexType, TRecordType, TIndexType>(DataAccess);
        }

        protected TOtherIndexType GenerateRelatedEntityId<TGeneratingRepositoryTest, TOtherRecordType, TOtherIndexType>()
            where TGeneratingRepositoryTest : RepositoryBaseTest<TOtherRecordType, TOtherIndexType>, new()
            where TOtherRecordType : class , IModel<TOtherIndexType>
        {
            var generatingRepositoryTest = new TGeneratingRepositoryTest { RepositoriesFactory = RepositoriesFactory };
            var testElement = generatingRepositoryTest.CreateTestElement();
            var repository = RepositoriesFactory.GetContextSharingRepository<TOtherRecordType, TOtherIndexType, TRecordType, TIndexType>(DataAccess);
            var output = repository.CreateAsync(testElement).Result;
            return output;
        }

        protected virtual void BuildDataSet(int recordsToCreate = 10)
        {
            TestElementsInDb = new List<TRecordType>();

            for (int i = 0; i < recordsToCreate; i++)
            {
                var newElement = CreateTestElement(i);
                var createdElementId = DataAccess.CreateAsync(newElement).Result;
                newElement.SetKey(createdElementId);
                TestElementsInDb.Add(newElement);
            }

            TestElementNotInDb = CreateTestElement(recordsToCreate);
        }

        [TestInitialize]
        public override void Initialize()
        {
            var dataDirPath = AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 0; i < 2; i++)
            {
                dataDirPath = Directory.GetParent(dataDirPath).FullName;
            }
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirPath);
            
            base.Initialize();
            
            DataAccess.ClearData();
            BuildDataSet();
        }

        [TestCleanup]
        public override void CleanUp()
        {
            DataAccess.ClearData();
            DataAccess.Dispose();
        }

        [TestMethod]
        public async Task Entity_Is_Created_Successfully()
        {
            var createdElementId = await DataAccess.CreateAsync(TestElementNotInDb);
            var createdElement = await DataAccess.ReadAsync(createdElementId);
            createdElement.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Entity_Is_Read_Successfully()
        {
            var createdElementId = await DataAccess.CreateAsync(TestElementNotInDb);

            var selectedElement = await DataAccess.ReadAsync(createdElementId);
            selectedElement.ContentsEqual(TestElementNotInDb).Should().BeTrue();
        }

        [TestMethod]
        public async Task Entity_Is_Updated_Successfully()
        {
            var createdElementId = await DataAccess.CreateAsync(TestElementNotInDb);

            var selectedElement = await DataAccess.ReadAsync(createdElementId);
            selectedElement.Should().NotBeNull();
            UpdateTestElement(selectedElement);

            await DataAccess.UpdateAsync(selectedElement);

            var selectedElementUpdated = await DataAccess.ReadAsync(createdElementId);
            selectedElement.ContentsEqual(selectedElementUpdated).Should().BeTrue();
        }

        [TestMethod]
        public async Task Entity_Is_Deleted_Successfully()
        {
            var createdElementId = await DataAccess.CreateAsync(TestElementNotInDb);

            var selectedElement = await DataAccess.ReadAsync(createdElementId);
            selectedElement.Should().NotBeNull();

            await DataAccess.DeleteAsync(createdElementId);

            selectedElement = await DataAccess.ReadAsync(createdElementId);
            selectedElement.Should().BeNull();
        }
    }
}
