﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Web.Common.Configuration;
using Sdl.Web.Common.Models;
using Sdl.Web.Tridion.Tests.Models;

namespace Sdl.Web.Tridion.Tests
{
    [TestClass]
    public class R2ContentProviderTest : ContentProviderTest
    {
        public R2ContentProviderTest()
            : base(new R2Mapping.DefaultContentProviderR2(), () => TestFixture.ParentLocalization)
        {
        }

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            DefaultInitialize(testContext);
        }

        [TestMethod]
        public void GetPageModel_ContextExpressions_Success()
        {
            string testPageUrlPath = TestLocalization.GetAbsoluteUrlPath(TestFixture.ContextExpressionsTestPageRelativeUrlPath);

            PageModel pageModel = TestContentProvider.GetPageModel(testPageUrlPath, TestLocalization, addIncludes: false);

            Assert.IsNotNull(pageModel, "pageModel");
            OutputJson(pageModel);

            RegionModel mainRegion = pageModel.Regions["Main"];
            EntityModel[] entitiesWithExtensionData = mainRegion.Entities.Where(e => e.ExtensionData != null).ToArray();
            EntityModel[] entitiesWithCxInclude = entitiesWithExtensionData.Where(e => e.ExtensionData.ContainsKey("CX.Include")).ToArray();
            EntityModel[] entitiesWithCxExclude = entitiesWithExtensionData.Where(e => e.ExtensionData.ContainsKey("CX.Exclude")).ToArray();

            Assert.AreEqual(8, entitiesWithExtensionData.Length, "entitiesWithExtensionData.Length");
            Assert.AreEqual(6, entitiesWithCxInclude.Length, "entitiesWithCxInclude.Length");
            Assert.AreEqual(4, entitiesWithCxExclude.Length, "entitiesWithCxExclude.Length");
        }

        [TestMethod]
        public void GetPageModel_RetrofitMapping_Success() // See TSI-1757
        {
            Localization testLocalization = TestFixture.ChildLocalization;
            string testPageUrlPath = testLocalization.GetAbsoluteUrlPath(TestFixture.Tsi1757PageRelativeUrlPath);

            PageModel pageModel = TestContentProvider.GetPageModel(testPageUrlPath, testLocalization, addIncludes: false);

            Assert.IsNotNull(pageModel, "pageModel");
            OutputJson(pageModel);

            Tsi1757TestEntity3 testEntity3 = pageModel.Regions["Main"].Entities[0] as Tsi1757TestEntity3;
            Assert.IsNotNull(testEntity3, "testEntity3");

            Assert.AreEqual("This is the textField of TSI-1757 Test Component 3", testEntity3.TextField, "testEntity3.TextField");
            Assert.IsNotNull(testEntity3.CompLinkField, "testEntity3.CompLinkField");
            Assert.AreEqual(2, testEntity3.CompLinkField.Count, "testEntity3.CompLinkField.Count");

            Tsi1757TestEntity1 testEntity1 = testEntity3.CompLinkField[0] as Tsi1757TestEntity1;
            Assert.IsNotNull(testEntity1, "testEntity1");
            Assert.AreEqual("This is the textField of TSI-1757 Test Component 1", testEntity1.TextField, "testEntity1.TextField");
            Assert.AreEqual("This is the embeddedTextField of TSI-1757 Test Component 1", testEntity1.EmbeddedTextField, "testEntity1.EmbeddedTextField");

            Tsi1757TestEntity2 testEntity2 = testEntity3.CompLinkField[1] as Tsi1757TestEntity2;
            Assert.IsNotNull(testEntity2, "testEntity2");
            Assert.AreEqual("This is the textField of TSI-1757 Test Component 2", testEntity2.TextField, "testEntity2.TextField");
        }

    }
}
