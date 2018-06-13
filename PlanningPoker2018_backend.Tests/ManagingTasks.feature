Feature: ManagingTasks
	In order to run planning poker session
	As a product owner
	I want to manage tasks in session

@select-to-estimate
Scenario: Select task to estimate
	Given I have a list of tasks
	When I select a task
	Then the task should change status to 'Voting'
