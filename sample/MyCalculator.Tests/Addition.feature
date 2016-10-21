Feature: Addition

Addition (in mathematics) is defined as (see http://www.aaamath.com/pro74ax2.htm)
- Identity property: a + 0 = a
- Commutative property: a + b = b + a
- Associative property: (a + b) + c = a + (b + c)
- Distributive property: a * (b + c) = a*b + a*c

In order to see, how the error is reported in a case of failure, uncomment line 8 in Addition.cs

Scenario: ExampleBased - Add two numbers
	Given I have entered 1 into the calculator
	And I have entered 2 into the calculator
	When I press add
	Then the result should be 3 on the screen

@propertyBased
Scenario: PropertyBased - Identity property
	a + 0 == a
	Given I have entered any number into the calculator
	And I have entered 0 into the calculator
	When I press add
	Then the result should be the first number on the screen

@propertyBased
Scenario: PropertyBased - Commutativity property
	(a + b) == b + a
	Given I have entered any number 'a' into the calculator
	And I have entered any number 'b' into the calculator
	When I press add
	Then the result should be the same as b+a on the screen

@propertyBased
Scenario: PropertyBased - Associative property
	(a + b) + c == a + (b + c)
	#note: this scenario was phrased like a script (a typical BDD anti-pattern) for the sake of this demo only
	Given I have entered any number 'a' into the calculator
	And I have entered any number 'b' into the calculator
	And I have pressed add
	And I have entered any number 'c' into the calculator
	And I have pressed add
	And saved the result
	And I have entered any number 'a' into the calculator
	And I have entered any number 'b' into the calculator
	And I have entered any number 'c' into the calculator
	And I have pressed add
	When I press add
	Then the two results should be the same
