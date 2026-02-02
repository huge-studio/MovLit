Feature: Compile Story
  As the Ink module
  I want to compile the Ink JSON into a runnable story
  So that the user can interact with the narrative

  Scenario: Successfully compile an Ink story
    Given a story entity with valid Ink JSON is loaded
    When the story is compiled
    Then the Ink runtime story should be created
    And external functions should be bound to the story

  Scenario: Compile story with empty Ink JSON
    Given a story entity with empty Ink JSON is loaded
    When the story is compiled
    Then the Ink runtime story should be null

  Scenario: Handle compilation error
    Given a story entity with invalid Ink JSON is loaded
    When the story is compiled
    Then the Ink runtime story should be null
    And an error message should be displayed to the user

  Scenario: Include external function headers in compilation
    Given a story entity is loaded
    When the story is compiled
    Then the Ink headers for external functions should be prepended
    And the headers should include playSound, showImage, showLottie
    And the headers should include navigateUrl, navigatePage
    And the headers should include registerItem, collectItem, hasItem, countItems, totalItems, itemIcon
