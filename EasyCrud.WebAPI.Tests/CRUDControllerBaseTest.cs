﻿using EasyCrud.Factories;
using EasyCrud.Logging;
using EasyCrud.Models;
using EasyCrud.Tests;
using EasyCrud.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace EasyCrud.WebAPI.Tests
{
    [TestClass]
    public abstract class CrudControllerBaseTest<TRecordExchangeType, TRecordType, TIndexType> : TestBase
        where TRecordType : class , IModel<TIndexType>, new()
        where TRecordExchangeType : class, IModelDataExchange<TRecordType, TIndexType>, new()
    {
        protected RepositoryMockBase<TRecordType, TIndexType> RepositoryMock { get; set; }
        protected CrudController<TRecordExchangeType, TRecordType, TIndexType> Controller { get; set; }

        protected abstract CrudController<TRecordExchangeType, TRecordType, TIndexType> CreateController(IRepositoryFactory factory, ILogger logger);

        protected abstract TRecordExchangeType CreateTestElement(int seed = 0);
        protected abstract void InvalidateTestElement(TRecordExchangeType input);
        protected abstract void UpdateTestElement(TRecordExchangeType input);
        protected abstract void NullUpdateTestElement(TRecordExchangeType input);

        public TRecordExchangeType CreateTestElementWithRepositoryTest<TRepositoryTest>(int seed = 0) where TRepositoryTest : RepositoryBaseTest<TRecordType, TIndexType>, new()
        {
            var repositoryTest = new TRepositoryTest {RepositoriesFactory = RepositoriesFactory};
            var feedback = repositoryTest.CreateTestElement(seed);
            var testElement = new TRecordExchangeType();
            testElement.InitializeFromModel(feedback);
            return testElement;
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            RepositoryMock = RepositoriesFactory.GetRepository<TRecordType, TIndexType>() as RepositoryMockBase<TRecordType, TIndexType>;
            Controller = CreateController(RepositoriesFactory, LoggerMock);
        }

        [TestCleanup]
        public override void CleanUp()
        {
        }

        [TestMethod]
        public async Task Get_Is_Successful()
        {
            var testData = CreateTestElement();
            var postResult = await Controller.Post(testData);

            var getResult = await Controller.Get(postResult.Id);
            getResult.ContentsEqual(testData).Should().BeTrue();
        }

        [TestMethod]
        public async Task Get_With_Invalid_Id_Should_Fail()
        {
            var invalidId = new TRecordType().GetInvalidKey();

            try
            {
                var getResult = await Controller.Get(invalidId);
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType(typeof(HttpResponseException));
                var httpEx = ex as HttpResponseException;
                httpEx.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public async Task Post_Is_Successful()
        {
            var testData = CreateTestElement();

            var initialItemCount = RepositoryMock.StoreList.Count();
            var postResult = await Controller.Post(testData);

            postResult.Should().NotBeNull();
            postResult.Id.Should().NotBeNull();
            RepositoryMock.StoreList.Count.ShouldBeEquivalentTo(initialItemCount + 1);
        }

        [TestMethod]
        public async Task Post_With_Invalid_Fields_Should_Fail()
        {
            var testData = CreateTestElement();
            InvalidateTestElement(testData);

            var initialItemCount = RepositoryMock.StoreList.Count();

            try
            {
                var postResult = await Controller.Post(testData);
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType(typeof(HttpResponseException));
                var httpEx = ex as HttpResponseException;
                httpEx.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            RepositoryMock.StoreList.Count.ShouldBeEquivalentTo(initialItemCount);
        }

        [TestMethod]
        public async Task Post_With_Already_Used_Id_Should_Fail_If_Key_Is_Not_Autogenerated()
        {
            var testData = CreateTestElement();
            var postResult = await Controller.Post(testData);

            testData = CreateTestElement(1);
            var record = new TRecordType();
            testData.UpdateModel(record);
            record.SetKey(postResult.Id);
            testData.InitializeFromModel(record);

            var initialItemCount = RepositoryMock.StoreList.Count();

            if (record.KeyIsAutogenerated() == false)
            {
                try
                {
                    await Controller.Post(testData);
                }
                catch (Exception ex)
                {
                    ex.Should().BeOfType(typeof(HttpResponseException));
                    var httpEx = ex as HttpResponseException;
                    httpEx.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                }

                RepositoryMock.StoreList.Count.ShouldBeEquivalentTo(initialItemCount);
            }
            else
            {
                postResult = await Controller.Post(testData);
                postResult.Id.Should().NotBeNull();
                record.KeyEqual(postResult.Id).Should().BeFalse();
                RepositoryMock.StoreList.Count.ShouldBeEquivalentTo(initialItemCount + 1);
            }
        }

        [TestMethod]
        public async Task Put_Is_Successful()
        {
            var testData = CreateTestElement();
            var postResult = await Controller.Post(testData);
            testData = await Controller.Get(postResult.Id);
            testData.Should().NotBeNull();

            UpdateTestElement(testData);
            await Controller.Put(testData);

            var testDataUpdated = await Controller.Get(postResult.Id);
            testDataUpdated.ContentsEqual(testData).Should().BeTrue();
        }

        [TestMethod]
        public async Task Put_With_Null_Fields_Should_Not_Make_Changes()
        {
            var testData = CreateTestElement();
            var postResult = await Controller.Post(testData);
            testData = await Controller.Get(postResult.Id);
            testData.Should().NotBeNull();

            var testDataNullUpdated = await Controller.Get(postResult.Id);
            NullUpdateTestElement(testDataNullUpdated);

            await Controller.Put(testDataNullUpdated);

            var testDataUpdated = await Controller.Get(postResult.Id);
            testDataUpdated.ContentsEqual(testData).Should().BeTrue();
        }

        [TestMethod]
        public async Task Put_With_Invalid_Fields_Should_Fail()
        {
            var testData = CreateTestElement();
            var postResult = await Controller.Post(testData);
            testData = await Controller.Get(postResult.Id);
            testData.Should().NotBeNull();

            InvalidateTestElement(testData);

            try
            {
                await Controller.Put(testData);
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType(typeof(HttpResponseException));
                var httpEx = ex as HttpResponseException;
                httpEx.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [TestMethod]
        public async Task Put_With_Unused_Key_Should_Fail()
        {
            var testData = CreateTestElement();

            try
            {
                await Controller.Put(testData);
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType(typeof(HttpResponseException));
                var httpEx = ex as HttpResponseException;
                httpEx.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public async Task Delete_Is_Successful()
        {
            var initialItemCount = RepositoryMock.StoreList.Count();

            var testData = CreateTestElement();
            var postResult = await Controller.Post(testData);
            testData = await Controller.Get(postResult.Id);
            testData.Should().NotBeNull();

            await Controller.Delete(postResult.Id);
            RepositoryMock.StoreList.Count.ShouldBeEquivalentTo(initialItemCount);
        }

        protected async Task Post_With_Specific_DataSet_Should_Fail(IEnumerable<TRecordExchangeType> dataSet, TRecordExchangeType failingValue)
        {
            foreach(var i in dataSet)
            {
                var postResult = await Controller.Post(i);
                var record = new TRecordType();
                i.UpdateModel(record);
                record.SetKey(postResult.Id);
                i.InitializeFromModel(record);
            }

            var initialItemCount = RepositoryMock.StoreList.Count();

            var exceptionOccurred = false;
            try
            {
                var postResult = await Controller.Post(failingValue);
            }
            catch (Exception ex)
            {
                exceptionOccurred = true;
                ex.Should().BeOfType(typeof(HttpResponseException));
                var httpEx = ex as HttpResponseException;
                httpEx.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            exceptionOccurred.Should().BeTrue();
            RepositoryMock.StoreList.Count.ShouldBeEquivalentTo(initialItemCount);
        }

        //This function sets failingValue's ID to the last ID generated for the provided dataset
        protected async Task Put_Last_Data_Element_With_Specific_DataSet_Should_Fail(IEnumerable<TRecordExchangeType> dataSet, TRecordExchangeType failingValue)
        {
            TIndexType lastKey = default(TIndexType);
            foreach (var i in dataSet)
            {
                var postResult = await Controller.Post(i);
                var record = new TRecordType();
                i.UpdateModel(record);
                lastKey = postResult.Id;
                record.SetKey(lastKey);
                i.InitializeFromModel(record);
            }

            var failingRecord = new TRecordType();
            failingValue.UpdateModel(failingRecord);
            failingRecord.SetKey(lastKey);
            failingValue.InitializeFromModel(failingRecord);

            var exceptionOccurred = false;
            try
            {
                await Controller.Put(failingValue);
            }
            catch (Exception ex)
            {
                exceptionOccurred = true;
                ex.Should().BeOfType(typeof(HttpResponseException));
                var httpEx = ex as HttpResponseException;
                httpEx.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            exceptionOccurred.Should().BeTrue();
        }
    }
}
