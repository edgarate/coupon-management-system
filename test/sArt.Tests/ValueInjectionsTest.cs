﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using NUnit.Framework;
using Omu.ValueInjecter;
using Core.Model;
using Core.Repository;
using Data;
using Infra;
using Infra.Builder;
using Infra.Dto;

namespace sArt.Tests
{
    public class ValueInjectionsTest
    {
        [Test]
        public void EntitiesToIntsTest()
        {
            var s = new Dinner { Meals = new List<Meal> { new Meal { Id = 3 }, new Meal { Id = 7 } } };

            var t = new DinnerInput();

            t.InjectFrom<EntitiesToInts>(s);

            Assert.IsNotNull(t.Meals);
            Assert.AreEqual(2, t.Meals.Count());
            Assert.AreEqual(3, t.Meals.First());
        }

        [Test]
        public void IntsToEntities()
        {
            WindsorRegistrar.RegisterSingleton(typeof(IReadRepo<>), typeof(ReadRepo<>));
            WindsorRegistrar.RegisterSingleton(typeof(IDbContextFactory), typeof(DbContextFactory));
            using (var scope = new TransactionScope())
            {
                var repo = new Repo<Meal>(new DbContextFactory());
                var m1 = new Meal { Name = "a" };
                var m2 = new Meal { Name = "b" };

                repo.Insert(m1);
                repo.Insert(m2);
                repo.Save();

                var s = new DinnerInput { Meals = new List<int> { m1.Id, m2.Id } };
                var t = new Dinner();

                t.InjectFrom<IntsToEntities>(s);

                Assert.IsNotNull(t.Meals);
                Assert.AreEqual(2, t.Meals.Count);
                Assert.AreEqual(m1.Id, t.Meals.First().Id);
            }
        }

        [Test]
        public void NormalToNullables()
        {
            var d = new Dinner { ChefId = 3 };
            var di = new DinnerInput();

            di.InjectFrom<NormalToNullables>(d);

            Assert.AreEqual(3, di.ChefId);
            Assert.AreEqual(null, di.Date);
            Assert.AreEqual(null, di.CountryId);
        }

        [Test]
        public void NullablesToNormal()
        {
            var di = new DinnerInput { ChefId = 3 };
            var d = new Dinner();

            d.InjectFrom<NullablesToNormal>(di);

            Assert.AreEqual(3, d.ChefId);
            Assert.AreEqual(0, d.CountryId);
            Assert.AreEqual(default(DateTime), d.Date);
        }
    }
}