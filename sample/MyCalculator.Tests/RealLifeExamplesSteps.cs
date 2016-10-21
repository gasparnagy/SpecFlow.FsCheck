using System;
using TechTalk.SpecFlow;

namespace MyCalculator.Tests
{
    [Binding]
    public class RealLifeExamplesSteps
    {
        [Given(@"the user is logged in")]
        public void GivenTheUserIsLoggedIn()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"selected a (.*)")]
        public void GivenSelectedAProductItColorVariations(string product)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"the user adds the product to the basket")]
        public void WhenTheUserAddsTheProductToTheBasket()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"it should be able to choose the color")]
        public void ThenItShouldBeAbleToChooseTheColor()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the user has not logged in yet")]
        public void GivenTheUserHasNotLoggedInYet()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the user tries to access a (.*)")]
        public void WhenTheUserTriesToAccessARestrictedPage(string page)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the user should be redirected to the login page")]
        public void ThenTheUserShouldBeRedirectedToTheLoginPage()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the vehicle is driving with speed (.*)")]
        public void GivenTheVehicleIsDrivingWithSpeedGreaterThanKmH(double speed)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the driver attempts to type in an address")]
        public void WhenTheDriverAttemptsToTypeInAnAddress()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"a warning should be displayed")]
        public void ThenAWarningShouldBeDisplayed()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
