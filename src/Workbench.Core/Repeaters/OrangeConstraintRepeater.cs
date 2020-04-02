﻿using System;
using System.Diagnostics;
using Workbench.Core.Models;
using Workbench.Core.Nodes;
using Workbench.Core.Parsers;
using Workbench.Core.Solvers;

namespace Workbench.Core.Repeaters
{
    /// <summary>
    /// Process a constraint repeater by expanding the expression an 
    /// appropriate number of times.
    /// </summary>
    internal class OrangeConstraintRepeater
    {
        private OrangeConstraintRepeaterContext _context;
        private readonly OrangeModelSolverMap _modelSolverMap;
        private readonly BundleModel _bundle;
        private readonly OrangeValueMapper _valueMapper;
        private readonly ConstraintNetwork _constraintNetwork;
        private readonly ArcBuilder _arcBuilder;

        internal OrangeConstraintRepeater(ConstraintNetwork constraintNetwork, OrangeModelSolverMap modelSolverMap, BundleModel bundle, OrangeValueMapper theValueMapper)
        {
            _constraintNetwork = constraintNetwork;
            _modelSolverMap = modelSolverMap;
            _bundle = bundle;
            _valueMapper = theValueMapper;
            _arcBuilder = new ArcBuilder(_modelSolverMap, _valueMapper);
        }

        internal void Process(OrangeConstraintRepeaterContext context)
        {
            Debug.Assert(context.HasRepeaters);

            _context = context;
            var constraintExpressionParser = new ConstraintExpressionParser();
            var expressionTemplateWithoutExpanderText = StripExpanderFrom(context.Constraint.Expression.Text);
            while (context.Next())
            {
                var expressionText = InsertCounterValuesInto(expressionTemplateWithoutExpanderText);
                var expandedConstraintExpressionResult = constraintExpressionParser.Parse(expressionText);
                ProcessConstraint(expandedConstraintExpressionResult.Root);
            }
        }

        internal OrangeConstraintRepeaterContext CreateContextFrom(ExpressionConstraintModel constraint)
        {
            return new OrangeConstraintRepeaterContext(constraint);
        }

        private void ProcessConstraint(ConstraintExpressionNode constraintExpressionNode)
        {
            var newArcs = _arcBuilder.Build(constraintExpressionNode);
            _constraintNetwork.AddArc(newArcs);
        }

        private string StripExpanderFrom(string expressionText)
        {
            var expanderKeywordPos = expressionText.IndexOf("|", StringComparison.Ordinal);
            var raw = expressionText.Substring(0, expanderKeywordPos);
            return raw.Trim();
        }

        private string InsertCounterValuesInto(string expressionTemplateText)
        {
            var accumulatingTemplateText = expressionTemplateText;
            foreach (var aCounter in _context.Counters)
            {
                accumulatingTemplateText = InsertCounterValueInto(accumulatingTemplateText,
                                                                  aCounter.CounterName,
                                                                  aCounter.CurrentValue);
            }

            return accumulatingTemplateText;
        }

        private string InsertCounterValueInto(string expressionTemplateText, string counterName, int counterValue)
        {
            return expressionTemplateText.Replace(counterName, Convert.ToString(counterValue));
        }
    }
}
