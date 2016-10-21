using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace MyCalculator.Tests
{
    [Binding]
    public class AdditionSteps
    {
        private readonly Calculator calculator = new Calculator();
        private int savedResult;
            
        [Given]
        public void Given_I_have_entered_NUMBER_into_the_calculator(int number)
        {
            calculator.Enter(number);
        }

        [Given]
        public void Given_I_have_pressed_add()
        {
            calculator.Add();
        }

        [Given]
        public void Given_I_have_pressed_multiply()
        {
            calculator.Multiply();
        }

        [Given]
        public void Given_saved_the_result()
        {
            savedResult = calculator.Result;
        }

        [When]
        public void When_I_press_add()
        {
            calculator.Add();
        }

        [Then]
        public void Then_the_result_should_be_EXPECTEDRESULT_on_the_screen(int expectedResult)
        {
            Assert.AreEqual(expectedResult, calculator.Result);
        }

        [Then]
        public void Then_the_two_results_should_be_the_same()
        {
            Assert.AreEqual(savedResult, calculator.Result);
        }
    }
}
