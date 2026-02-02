Feature: Navigate Story
  As a user reading an Ink story
  I want to navigate forward and backward through the story
  So that I can explore the narrative at my own pace

  Scenario: Advance to next story content
    Given the story is compiled and can continue
    When I click next
    Then the next lines of story text should be displayed
    And the current choices should be updated
    And the step should be saved in history

  Scenario: Display choices when story pauses
    Given the story has reached a choice point
    Then the available choices should be displayed to the user

  Scenario: Select a choice
    Given the story is displaying choices
    When I select a choice
    Then the story should advance based on the selected choice
    And the next content should be displayed

  Scenario: Go to previous step
    Given the story has multiple steps in history
    And the HasPrevious setting is enabled
    When I click previous
    Then the previous story state should be restored
    And the previous text and choices should be displayed
    And the Lottie and image state should be restored

  Scenario: Previous button unavailable on first step
    Given the story is on the first step
    Then the previous button should not be available

  Scenario: Previous button unavailable when setting disabled
    Given the story has multiple steps in history
    But the HasPrevious setting is disabled
    Then the previous button should not be available

  Scenario: Detect story finish
    Given the story cannot continue
    And there are no choices available
    Then the finish state should be indicated

  Scenario: Process story text with Markdown
    Given the story produces text with Markdown formatting
    When the text is processed
    Then the output should be rendered as HTML

  Scenario: Process story text with line breaks
    Given the story produces text with backslash characters
    When the text is processed
    Then backslashes should be converted to newlines before rendering
