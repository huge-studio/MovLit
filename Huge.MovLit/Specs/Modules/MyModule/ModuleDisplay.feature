Feature: MyModule Display
  As a user
  I want to view the custom module content
  So that I can interact with the module functionality

  Scenario: Load module index view
    Given the MyModule is configured on a page
    When the module loads
    Then the index view should be rendered

  Scenario: Load module edit view
    Given the MyModule is configured on a page
    And the user has edit permissions
    When the user navigates to the edit action
    Then the edit view should be rendered

  Scenario: Load module settings view
    Given the MyModule is configured on a page
    And the user has admin permissions
    When the user opens the module settings
    Then the settings view should be rendered
