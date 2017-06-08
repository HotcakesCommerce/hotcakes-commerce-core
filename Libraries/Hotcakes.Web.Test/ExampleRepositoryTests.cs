#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using Hotcakes.Web.Data;
using Hotcakes.Web.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Web.Test
{
    [TestClass]
    public class ExampleRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            MemoryStrategyFactory.Clear();
        }

        [TestMethod]
        public void CanCreateRepository()
        {
            var repository = ExampleRepository.InstantiateForMemory();

            Assert.IsNotNull(repository, "Repository shouldn't be null");
        }

        [TestMethod]
        public void CanCreateObjectInRepository()
        {
            var repository = ExampleRepository.InstantiateForMemory();

            var o = new ExampleBase
            {
                LastUpdatedUtc = new DateTime(2001, 1, 1),
                Description = "This is an example base",
                IsActive = true
            };

            Assert.IsTrue(repository.Create(o), "Create should be true");
            Assert.AreNotEqual(string.Empty, o.bvin, "Bvin should not be empty");
            Assert.AreEqual(DateTime.UtcNow.Year, o.LastUpdatedUtc.Year, "Last updated date should match current year");
        }

        [TestMethod]
        public void CanFindObjectInRepositoryAfterCreate()
        {
            var repository = ExampleRepository.InstantiateForMemory();

            var o = new ExampleBase
            {
                LastUpdatedUtc = new DateTime(2001, 1, 1),
                Description = "This is an example base",
                IsActive = true
            };

            repository.Create(o);

            var targetId = o.bvin;

            var found = repository.Find(targetId);

            Assert.IsNotNull(found, "Found item should not be null");
            Assert.AreEqual(o.bvin, found.bvin, "Bvin should match");
            Assert.AreEqual(o.Description, found.Description, "Bvin should match");
            Assert.AreEqual(o.IsActive, found.IsActive, "IsActive should match");
            Assert.AreEqual(o.LastUpdatedUtc.Ticks, found.LastUpdatedUtc.Ticks, "Last Updated should match");
            Assert.AreEqual(o.SubObjects.Count, found.SubObjects.Count, "Sub object count should match");
        }

        [TestMethod]
        public void CanCreateSubItemsDuringCreate()
        {
            var repository = ExampleRepository.InstantiateForMemory();

            var o = new ExampleBase {Description = "Test Base"};
            o.SubObjects.Add(new ExampleSubObject {Name = "Test Sub A"});
            o.SubObjects.Add(new ExampleSubObject {Name = "Test Sub B"});
            o.SubObjects.Add(new ExampleSubObject {Name = "Test Sub C"});

            var result = repository.Create(o);
            Assert.IsTrue(result, "Create should return true");

            Assert.AreEqual(3, o.SubObjects.Count, "Sub object count should be three");
            foreach (var sub in o.SubObjects)
            {
                Assert.IsTrue(sub.Id > 0, "Sub object " + sub.Name + " should have and ID > 0");
                Assert.IsTrue(sub.BaseId == o.bvin, "Sub object " + sub.Name + " should have received base bvin");
                Assert.IsTrue(sub.SortOrder > 0, "Sub object " + sub.Name + " should have sort order > 0");
            }
        }

        [TestMethod]
        public void CanMergeSubItems()
        {
            var repository = ExampleRepository.InstantiateForMemory();

            // Create Basic Sample
            var o = new ExampleBase {Description = "Test Base"};
            o.SubObjects.Add(new ExampleSubObject {Name = "Test Sub A"});
            o.SubObjects.Add(new ExampleSubObject {Name = "Test Sub B"});
            o.SubObjects.Add(new ExampleSubObject {Name = "Test Sub C"});
            var result = repository.Create(o);

            var existing = repository.Find(o.bvin);
            Assert.AreEqual(o.SubObjects.Count, existing.SubObjects.Count, "Sub object count should be equal");
            Assert.AreEqual(o.SubObjects[0].Name, existing.SubObjects[0].Name, "First item name should be equal");

            existing.SubObjects.Add(new ExampleSubObject {Name = "New Sub A"}); // index 3, then 2
            existing.SubObjects.Add(new ExampleSubObject {Name = "New Sub B"}); // index 4, then 3
            existing.SubObjects[0].Name = "Updated Sub A";
            existing.SubObjects.RemoveAt(2);

            Assert.IsTrue(repository.Update(existing), "update should be true");

            var target = repository.Find(o.bvin);
            Assert.IsNotNull(target, "target should not be null");
            Assert.AreEqual(4, target.SubObjects.Count, "Sub object count should be four after merge");

            Assert.AreEqual("Updated Sub A", target.SubObjects[0].Name, "First sub name didn't match");
            Assert.AreEqual("Test Sub B", target.SubObjects[1].Name, "Second name didn't match");
            Assert.AreEqual("New Sub A", target.SubObjects[2].Name, "Third name didn't match");
            Assert.AreEqual("New Sub B", target.SubObjects[3].Name, "Fourth Name didn't match");
        }

        [TestMethod]
        public void CanDeleteSubsOnDelete()
        {
            var repository = ExampleRepository.InstantiateForMemory();

            // Create Basic Sample
            var o = new ExampleBase {Description = "Test Base"};
            o.SubObjects.Add(new ExampleSubObject {Name = "Test Sub A"});
            o.SubObjects.Add(new ExampleSubObject {Name = "Test Sub B"});
            o.SubObjects.Add(new ExampleSubObject {Name = "Test Sub C"});
            var result = repository.Create(o);

            var existing = repository.Find(o.bvin);
            Assert.AreEqual(o.SubObjects.Count, existing.SubObjects.Count, "Sub object count should be equal");
            Assert.AreEqual(o.SubObjects[0].Name, existing.SubObjects[0].Name, "First item name should be equal");

            Assert.IsTrue(repository.Delete(o.bvin));

            var target = repository.Find(o.bvin);
            Assert.IsNull(target, "target should not null after delete");

            var subs = repository.PeakIntoSubObjects(o.bvin);
            Assert.IsNotNull(subs, "Sub list should NOT be null but should be empty.");
            Assert.AreEqual(0, subs.Count, "Sub list should be empty.");
        }
    }
}