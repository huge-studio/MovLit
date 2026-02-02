Feature: Load Story
  As a user navigating to a story page
  I want the Ink story to load automatically
  So that I can begin reading the interactive fiction

  Scenario: Load an existing story for a module
    Given a story exists for the current module
    When the Ink module loads
    Then the story should be fetched from the StoryService
    And the story should be compiled

  Scenario: Create starter story on first visit
    Given no story exists for the current module
    And the user is logged in
    When the Ink module loads
    Then a starter story should be created with the default title and description
    And the story author should be set to the current user

  Scenario: Handle story fetch failure gracefully
    Given the StoryService returns an error
    When the Ink module loads
    Then an error message should be logged
    And the module should not crash
