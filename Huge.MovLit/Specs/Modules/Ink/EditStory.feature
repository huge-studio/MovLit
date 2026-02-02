Feature: Edit Story
  As a story author
  I want to edit the Ink story content
  So that I can create and update interactive fiction

  Scenario: Load story for editing
    Given I am on the story edit page
    And a story exists for the current module
    When the edit page loads
    Then the story content should be loaded into the code editor
    And the story tags should be pre-selected

  Scenario: Save story with valid Ink
    Given I am editing a story
    And the Ink content is valid and compiles successfully
    When I click save
    Then the story should be saved via the StoryService
    And I should be navigated back to the return URL
    And a success message should be displayed

  Scenario: Save story with compilation error
    Given I am editing a story
    And the Ink content has a syntax error
    When I click save
    Then the story should not be saved
    And a compilation error message should be displayed with adjusted line numbers

  Scenario: Save story with empty content
    Given I am editing a story
    And the Ink content is blank
    When I click save
    Then a compilation failure message should be displayed

  Scenario: Update story tags
    Given I am editing a story
    When I select tags "adventure" and "mystery"
    And I click save
    Then the story tags should be updated to include "adventure" and "mystery"

  Scenario: Clear story tags
    Given I am editing a story with existing tags
    When I deselect all tags
    And I click save
    Then the story tags should be an empty list

  Scenario: Select cover art
    Given I am editing a story
    When I select a cover art file
    Then the story CoverArtUrl should be set to the file URL

  Scenario: Cancel editing
    Given I am editing a story
    When I click cancel
    Then I should be navigated back to the return URL
    And no changes should be saved

  Scenario: Handle missing story on edit page
    Given no story exists for the current module
    When I try to save
    Then a warning message should be displayed instructing to visit the Ink module first
