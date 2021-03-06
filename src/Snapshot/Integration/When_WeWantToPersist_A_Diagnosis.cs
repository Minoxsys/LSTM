﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersist_A_Diagnosis : GivenAPersistenceSpecification<Diagnosis>
    {
        const string DIAGNOSIS_CODE = "CG+; CG-";
        const string DIAGNOSIS_KEYWORD = "Chlamydia";
        const string DIAGNOSIS_DESCRIPTION = "Itching";

        [Test]
        public void It_ShouldSuccessfullyPersist_A_Diagnosis()
        {

            var diagnosis = Specs.CheckProperty(e => e.Description, DIAGNOSIS_DESCRIPTION)
                .CheckProperty(c => c.Code, DIAGNOSIS_CODE)
                .CheckProperty(c => c.Keyword, DIAGNOSIS_KEYWORD)
                .VerifyTheMappings();

            Assert.IsNotNull(diagnosis);
            Assert.IsInstanceOf<Guid>(diagnosis.Id);
            Assert.AreEqual(diagnosis.Description, DIAGNOSIS_DESCRIPTION);
            Assert.AreEqual(diagnosis.Code, DIAGNOSIS_CODE);
            Assert.AreEqual(diagnosis.Keyword, DIAGNOSIS_KEYWORD);

            session.Delete(diagnosis);
        }
    }
}
