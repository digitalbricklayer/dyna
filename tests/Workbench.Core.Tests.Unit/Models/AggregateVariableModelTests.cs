﻿using System;
using NUnit.Framework;
using Workbench.Core.Models;

namespace Workbench.Core.Tests.Unit.Models
{
    [TestFixture]
    public class AggregateVariableModelTests
    {
        [Test]
        public void InitializeAggregateWithValidNameSetsExpectedNameInVariable()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("x")));
            Assert.That(sut.Name, Is.EqualTo("x"));
        }

        [Test]
        public void InitializeVariableWithEmptyExpressionWoutWhitespace()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("x"), 5, new VariableDomainExpressionModel()));
            Assert.That(sut.DomainExpression.IsEmpty, Is.True);
        }

        [Test]
        public void InitializeVariableWithDomainReferenceRawExpressionWithWhitespace()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("x"), 2, new VariableDomainExpressionModel("   $A    ")));
            Assert.That(sut.DomainExpression.DomainReference.DomainName.Name, Is.EqualTo("A"));
        }

        [Test]
        public void InitializeVariableWithDomainReferenceRawExpressionWoutWhitespace()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("x"), 1, new VariableDomainExpressionModel("$A")));
            Assert.That(sut.DomainExpression.DomainReference.DomainName.Name, Is.EqualTo("A"));
        }

        [Test]
        public void InitializeVariableWithInlineRawExpressionWoutWhitespace()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("x"), 10, new VariableDomainExpressionModel("1..10")));
            Assert.That(sut.DomainExpression.InlineDomain, Is.Not.Null);
        }

        [Test]
        public void ChangeDomainOfAggregatedVariableWithValueInsideAggregateDomain()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("A test"), 10, new VariableDomainExpressionModel("1..10")));
            sut.Resize(10);
            sut.OverrideDomainTo(9, new VariableDomainExpressionModel("1..5"));
            var actualVariable = sut.GetVariableByIndex(9);
            Assert.That(actualVariable.DomainExpression.Text, Is.EqualTo("1..5"));
        }

        [Test]
        public void ChangeDomainOfAggregatedVariableWithValueOutsideAggregateDomain()
        {
            var theModel = new ModelModel();
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("A test"), 2, new VariableDomainExpressionModel("1..10")));
            theModel.AddVariable(sut);
            sut.Resize(10);
            sut.OverrideDomainTo(9, new VariableDomainExpressionModel("1..5"));
            Assert.Throws<ArgumentException>(() => sut.OverrideDomainTo(9, new VariableDomainExpressionModel("8..11")));
        }

        [Test]
        public void ChangeDomainOfAggregatedVariableWithValueOutsideAggregateDomainX()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("A test"), 1, new VariableDomainExpressionModel("1..10")));
            sut.Resize(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.OverrideDomainTo(10, new VariableDomainExpressionModel("8..10")));
        }

        [Test]
        public void ResizeAggregateWithZeroThrowsArgumentOutOfRangeException()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("x")));
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Resize(0));
        }

        [Test]
        public void ResizeAggregateWithTenSetsNewSize()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("x")));
            sut.Resize(10);
            Assert.That(sut.AggregateCount, Is.EqualTo(10));
        }

        [Test]
        public void ResizeAggregateToGreaterSIzeAllVariablesNotNull()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("x")));
            sut.Resize(10);
            Assert.That(sut.Variables, Is.All.InstanceOf<VariableModel>());
        }

        [Test]
        public void ResizeAggregateToSmallerSizeAllVariablesNotNull()
        {
            var sut = new AggregateVariableGraphicModel(new AggregateVariableModel(new ModelModel(), new ModelName("x")));
            sut.Resize(10);
            sut.Resize(5);
            Assert.That(sut.Variables, Is.All.InstanceOf<VariableModel>());
        }
    }
}
