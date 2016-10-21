Feature: RealLifeExamples

Scenario: Should be able to choose color
	Given the user is logged in
	And selected a product with color variations
	When the user adds the product to the basket
	Then it should be able to choose the color

Scenario: Restricted pages require login
	Given the user has not logged in yet
	When the user tries to access a restricted page
	Then the user should be redirected to the login page

Scenario: Do not let the user type while driving
	Given the vehicle is driving with speed greater than 5 km/h
	When the driver attempts to type in an address
	Then a warning should be displayed