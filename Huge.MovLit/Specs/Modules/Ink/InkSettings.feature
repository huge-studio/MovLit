Feature: Ink Module Settings
  As an administrator
  I want to configure the Ink module settings
  So that I can customize the story reader behavior

  Scenario: Load Ink settings
    Given I am on the Ink settings page
    When the settings load
    Then the CenterJustify setting should be displayed
    And the HasPrevious setting should be displayed
    And the ContinueText setting should be displayed

  Scenario: Update Ink settings
    Given I am editing Ink settings
    When I change the settings values
    And I save the settings
    Then the module settings should be persisted
