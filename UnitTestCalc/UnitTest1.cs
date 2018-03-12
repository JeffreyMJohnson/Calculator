using System.Collections.Generic;
using Calc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCalc
{
    [TestClass]
    public class CalculatorUnitTests
    {
        [TestMethod]
        public void SingleOperationPassTestGroup()
        {
            // note the test precalcs '2+2' to preload for the single operation.
            Dictionary<string, double> singleOperations = new Dictionary<string, double>()
            {
                { "+234=", 238},
                { "-5 =", -1},
                { "*2=", 8},
                { "/4=", 1}
            };

            foreach (var singleOperation in singleOperations)
            {
                SingleOperationPassTest(singleOperation.Key, singleOperation.Value);
            }
        }

        [TestMethod]
        public void SingleOperationFailTestGroup()
        {
            // note the test precalcs '2+2' to preload for the single operation.
            List<string> singleOperations = new List<string>()
            {
                "*a32+2=",
                "-2",
                "+",
                "="
            };

            foreach (var singleOperation in singleOperations)
            {
                SingleOperationFailTest(singleOperation);
            }
        }

        private void SingleOperationPassTest(string expression, double expected)
        {
            Calculator calc = new Calculator();
            double result = 0;
            bool succeeded = calc.EvaluateExpression("2+2=", out result);

            Assert.IsTrue(succeeded);
            Assert.AreEqual(4, result, .01);

            succeeded = calc.EvaluateExpression(expression, out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual(expected, result, .01);

        }

        private void SingleOperationFailTest(string expression)
        {
            Calculator calc = new Calculator();
            double result = 0;
            bool succeeded = calc.EvaluateExpression("2+2=", out result);

            Assert.IsTrue(succeeded);
            Assert.AreEqual(4, result, .01);

            succeeded = calc.EvaluateExpression(expression, out result);
            Assert.IsFalse(succeeded, expression);
        }
    }

    [TestClass]
    public class ExpressionUnitTests
    {
        private readonly Dictionary<string, double> _validExpressionsLookup = new Dictionary<string, double>()
        {
            { "2 + 2 = ", 4},
            { "43*4=", 172},
            { "-5/100 =", -.05},
            { "7+-6 =", 1},
            { "-5* 5/3 =", -8.333},
            { "-5* 5-15 /3 =", -30}
        };

        private readonly List<string> _invalidExpressionsList = new List<string>()
        {
            "*5+4",
            "1234a",
            "+5",
            "4/556*3+",
            "2+2"
        };

        [TestMethod]
        public void ExpressionValidationPassTestGroup()
        {
            
            foreach (var expression in _validExpressionsLookup)
            {
                ExpressionValidationPass(expression.Key);
            }

        }

        [TestMethod]
        public void ExpressionValidationFailTestGroup()
        {
            foreach (var expression in _invalidExpressionsList)
            {
                ExpressionValidationFail(expression);
            }

        }

        [TestMethod]
        public void ExpressionEvaluationTestGroup()
        {
            foreach (var pair in _validExpressionsLookup)
            {
                ExpressionEvaluation(pair.Key, pair.Value);
            }
        }

        [TestMethod]
        public void OperandValidationPassTestGroup()
        {
            List<string> operands = new List<string>()
            {
                "1234",
                "-35435",
                "1"
            };

            foreach (var operand in operands)
            {
                OperandValidationPass(operand);
            }
            
        }

        [TestMethod]
        public void OperandValidationFailTestGroup()
        {
            List<string> operands = new List<string>()
            {
                "",
                "+9",
                "ad3",
                ".",
                "*"
            };

            foreach (var operand in operands)
            {
                OperandValidationFail(operand);
            }
        }

        [TestMethod]
        public void OperatorValidationPassTestGroup()
        {
            List<string> operators = new List<string>()
            {
                "*",
                "-",
                "/",
                "+",
                "="
            };

            foreach (var op in operators)
            {
                OperatorValidationPass(op);
            }

        }

        [TestMethod]
        public void OperatorValidationFailTestGroup()
        {
            List<string> operators = new List<string>()
            {
                "*9",
                "-7",
                "10",
                "a",
                "+-"
            };

            foreach (var op in operators)
            {
                OperatorValidationFail(op);
            }

        }

        private void ExpressionValidationPass(string expression)
        {
            Expression e = new Calc.Expression(expression);
            Assert.IsTrue(e.IsValid, expression);
        }

        private void ExpressionValidationFail(string expression)
        {
            Expression e = new Calc.Expression(expression);
            Assert.IsFalse(e.IsValid, expression);
        }

        private void ExpressionEvaluation(string expression, double expected)
        {
            Expression e = new Calc.Expression(expression);
            Assert.AreEqual(expected, e.EvaluatedValue, .001);
        }

        private void OperandValidationPass(string operand)
        {
            Assert.IsTrue(Expression.IsValidOperand(operand), operand);
        }

        private void OperandValidationFail(string operand)
        {
            Assert.IsFalse(Expression.IsValidOperand(operand), operand);
        }

        private void OperatorValidationPass(string op)
        {
            Assert.IsTrue(Expression.IsValidOperator(op), op);
        }

        private void OperatorValidationFail(string op)
        {
            Assert.IsFalse(Expression.IsValidOperator(op), op);
        }
    }
}
